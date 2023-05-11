using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class WeaponController : MonoBehaviour
{
    private PlayerInput playerInput;
    private IEnumerator currentAttackCountdown;

    private Animator animator;
    private AudioSource audioSource;

    private Collider2D weaponHitbox;

    [SerializeField]
    private PlayerController playerController;

    [SerializeField]
    private float attackResetTime = 0.15f;

    private void Awake()
    {
        playerInput = transform.parent.GetComponent<PlayerInput>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        weaponHitbox = GetComponent<Collider2D>();

        weaponHitbox.enabled = false;
    }

    private void Start()
    {
        playerInput.actions["Attack"].performed += Attack;
        playerInput.actions["ChargeAttack"].performed += ChargedAttack;
    }

    private void Attack(InputAction.CallbackContext context)
    {
        animator.SetBool("isAttack", true);

        if (currentAttackCountdown != null)
        {
            StopCoroutine(currentAttackCountdown);
        }

        IEnumerator CountdownToAttackEnd()
        {
            yield return new WaitForSeconds(attackResetTime);
            animator.SetBool("isAttack", false);
        }

        currentAttackCountdown = CountdownToAttackEnd();
        StartCoroutine(currentAttackCountdown);
    }

    private void ChargedAttack(InputAction.CallbackContext context)
    {
        animator.SetTrigger("isChargedAttack");
    }

    public void EnablePlayerMovement()
    {
        playerController.enabled = true;
    }

    public void DisablePlayerMovement()
    {
        playerController.enabled = false;
    }

    public void EnableWeaponHitbox()
    {
        weaponHitbox.enabled = true;  
    }

    public void DisableWeaponHitbox()
    {
        weaponHitbox.enabled = false;
    }
}
