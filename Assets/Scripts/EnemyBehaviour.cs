using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

[RequireComponent(typeof(EntityVitals))]
public class EnemyBehaviour : MonoBehaviour, IMovementController
{
    enum EnemyState
    {
        Chase, // Make a beeline for the player
        ChargeUp, // Pause for 2 seconds and begin spinning saws
        Rush, // Rush towards the player
        Retreat // Retreat backwards
    }

    private NavMeshAgent agent;

    [SerializeField]
    private Transform fillBar;

    [SerializeField]
    private Transform sawBlade;

    private Transform player;

    private Animator weaponAnimator;

    private EnemyState state;
    private IEnumerator stateCoroutine;
    private float retreatAngle;
    private float speed;

    private EntityVitals entityVitals;

    private AudioSource audioSource;
    private AudioPlayer audioPlayer;

    public event Action OnMove;
    public event Action OnStop;
    public event Action OnBeginDodge;
    public event Action OnEndDodge;

    // Start is called before the first frame update
    void Start()
    {
        entityVitals = GetComponent<EntityVitals>();
        entityVitals.MaxHealth = 100;
        entityVitals.CurrentHealth = 100;
        entityVitals.OnDeath += () => Destroy(gameObject);
        entityVitals.OnTakeDamage += () => {
            fillBar.localScale = new Vector3(1, entityVitals.HealthPercentage, 1);
            fillBar.localPosition = new Vector3(0, entityVitals.CurrentHealth / (2 * entityVitals.MaxHealth) - 0.5f, 0);

            weaponAnimator.SetBool("isAttack", false);
            state = EnemyState.Retreat;
        };

        agent = GetComponent<NavMeshAgent>();

        agent.updateRotation = false;  // prevent the enemy from rotating when moving.
        agent.updateUpAxis = false;  // prevent the enemy from tilting when moving.

        speed = agent.speed;

        player = GameObject.Find("Player").transform;

        weaponAnimator = sawBlade.GetComponent<Animator>();

        audioSource = GetComponent<AudioSource>();
        audioPlayer = GetComponent<AudioPlayer>();

    }

    void Update() {
        // When the enemy is close to the player, pause for 2 seconds, begin spinning saws and rush towards the player
        // Then, quickly retreat backwards

        switch (state)
        {
            case EnemyState.Chase:
                ProcChase();
                break;

            case EnemyState.ChargeUp:
                ProcChargeUp();
                break;

            case EnemyState.Rush:
                ProcRush();
                break;

            case EnemyState.Retreat:
                ProcRetreat();
                break;
        }
    }

    IEnumerator WaitThenDo(float secondsToWait, Action callback = null)
    {
        yield return new WaitForSeconds(secondsToWait);
        callback?.Invoke();
    }

    private void ProcChase()
    {
        agent.SetDestination(player.position);
        weaponAnimator.SetBool("isAttack", false);

        if (agent.path.corners.Length == 2)
        {
            float distance = Vector3.Distance(transform.position, player.position);
            if (distance < 5)
            {
                state = EnemyState.Retreat;
            }
            else if (distance < 8)
            {
                // if straight line of sight to player, charge up
                state = EnemyState.ChargeUp;
            }
        }
    }

    private void ProcChargeUp()
    {
        agent.isStopped = true;

        if (stateCoroutine == null)
        {
            weaponAnimator.SetBool("isAttack", true);
            stateCoroutine = WaitThenDo(2, () =>
            {
                agent.SetDestination(player.position);

                state = EnemyState.Rush;


                stateCoroutine = null;
                agent.isStopped = false;
            });

            StartCoroutine(stateCoroutine);
        }

        if (agent.path.corners.Length != 2)
        {
            state = EnemyState.Chase;
            agent.speed = speed;
            StopCoroutine(stateCoroutine);
            stateCoroutine = null;

        }
    }

    private void ProcRush()
    {
        agent.speed = speed * 2;
        agent.SetDestination(player.position);

        if (stateCoroutine == null)
        {
            stateCoroutine = WaitThenDo(3, () =>
            {
                state = EnemyState.Retreat;
                stateCoroutine = null;
                agent.speed = speed;
            });
            StartCoroutine(stateCoroutine);
        }

        if (agent.path.corners.Length != 2)
        {
            state = EnemyState.Chase;
            agent.speed = speed;
            StopCoroutine(stateCoroutine);
            stateCoroutine = null;

        }
        else if (Vector3.Distance(transform.position, player.position) <= 1.5f)
        {
            state = EnemyState.Retreat;
            StopCoroutine(stateCoroutine);
            stateCoroutine = null;
        }
    }

    private void ProcRetreat()
    {
        if (stateCoroutine == null)
        {
            SetRetreatAngle();
            weaponAnimator.SetBool("isAttack", false);
            stateCoroutine = WaitThenDo(2, () =>
            {
                state = EnemyState.ChargeUp;
                stateCoroutine = null;
                retreatAngle = 0;
            });
            StartCoroutine(stateCoroutine);
        }

        Vector3 direction = Quaternion.AngleAxis(retreatAngle, Vector3.forward) * (transform.position - player.position);

        agent.SetDestination(transform.position + direction.normalized * 5);

        if (Vector3.Distance(transform.position, player.position) > 8)
        {
            StopCoroutine(stateCoroutine);
            stateCoroutine = null;
            state = EnemyState.Chase;
            agent.speed = speed;
            retreatAngle = 0;
        }
    }

    private void SetRetreatAngle()
    {
        // check left and right and choose the one with the most space
        Vector2 directionToPlayer = (player.position - transform.position).normalized * 5;

        bool leftTerminated = NavMesh.Raycast(transform.position, transform.position + Quaternion.AngleAxis(-45, Vector3.forward) * directionToPlayer, out NavMeshHit leftHit, NavMesh.AllAreas);

        bool rightTerminated = NavMesh.Raycast(transform.position, transform.position + Quaternion.AngleAxis(45, Vector3.forward) * directionToPlayer, out NavMeshHit rightHit, NavMesh.AllAreas);

        if (leftTerminated && rightTerminated)
        {
            retreatAngle = leftHit.distance > rightHit.distance ? -45 : 45;
        }
        else if (leftTerminated)
        {
            retreatAngle = -45;
        }
        else if (rightTerminated)
        {
            retreatAngle = 45;
        }
        else
        {
            retreatAngle = Random.Range(0, 2) == 0 ? -45 : 45;
        }
    }
}
