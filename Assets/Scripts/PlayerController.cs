using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(PlayerInput), typeof(EntityVitals))]
public class PlayerController : MonoBehaviour, IMovementController
{
    private PlayerInput playerInput;
    private Rigidbody2D rb;

    private Quaternion rotationDestination;

    private float movementSpeed = 5f;
    private Vector2 currentInputDirection;

    private EntityVitals entityVitals;

    public event Action OnMove;
    public event Action OnStop;
    public event Action OnBeginDodge;
    public event Action OnEndDodge;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody2D>();
        entityVitals = GetComponent<EntityVitals>();
    }

    private void Start()
    {
        entityVitals.MaxHealth = 100;
        entityVitals.CurrentHealth = 100;
        entityVitals.OnDeath += () => Destroy(gameObject);

        playerInput.actions["Move"].performed += ctx => Move(ctx.ReadValue<Vector2>());
        playerInput.actions["Look"].performed += SetLook;
        playerInput.actions["Dodge"].performed += Dodge;

        playerInput.actions["Move"].canceled += _ => Stop();
    }

    private void OnEnable()
    {
        playerInput.actions["Move"].Enable();
        playerInput.actions["Look"].Enable();
    }

    private void OnDisable()
    {
        playerInput.actions["Move"].Disable();
        playerInput.actions["Look"].Disable();
    }

    private void Update()
    {
        Rotate();
    }

    private void Rotate() {
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rotationDestination, 100f * Time.deltaTime);
    }

    private void Move(Vector2 inputDirection)
    {
        OnMove?.Invoke();
        currentInputDirection = inputDirection;
        rb.velocity = currentInputDirection * movementSpeed;
    }

    private void Dodge(InputAction.CallbackContext context)
    {
        OnBeginDodge?.Invoke();
        IEnumerator ResetVelocity(float time)
        {
            yield return new WaitForSeconds(time);
            Move(currentInputDirection);
            OnEndDodge?.Invoke();
        }

        if (currentInputDirection == Vector2.zero)
        {
            // Dodge where the player is looking if they are not moving.
            rb.AddForce(transform.up * movementSpeed * 3, ForceMode2D.Impulse);
        }

        rb.AddForce(currentInputDirection * movementSpeed * 3, ForceMode2D.Impulse);

        StartCoroutine(ResetVelocity(0.2f));
    }

    /// <summary>
    /// Sets the destination for the player to look at the mouse cursor.
    /// </summary>
    private void SetLook(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(input);
        rotationDestination = Quaternion.LookRotation(Vector3.forward, worldPosition - transform.position);
    }

    private void Stop()
    {
        OnStop?.Invoke();
        rb.velocity = Vector2.zero;
        currentInputDirection = Vector2.zero;
    }
}
