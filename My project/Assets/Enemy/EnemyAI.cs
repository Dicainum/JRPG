using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private EnemyStats enemyStats;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Transform target;
    [SerializeField] private LayerMask Ground, Player;

    [Header("Patroling")]
    [SerializeField] private Transform[] waypoints;
    private int currentWaypointIndex = 0;
    private int patrolDirection = 1;
    private bool isWaiting = false;
    private Coroutine waitCoroutine;

    [Header("Attacking")]
    [SerializeField] private float timeBetweenAttacks;
    private bool alreadyAttacked;

    [Header("Ranges")]
    [SerializeField] private float sightRange, attackRange;
    [SerializeField] private bool playerInSightRange, playerInAttackRange;

    [Header("Chasing")]
    [SerializeField] private GameObject alertIcon;
    private float lastTimePlayerSeen;

    [Header("AttackOptions")]
    private bool isStunned;
    private bool lostEnemyInAggro = false;
    private bool rememberEnemy = false;
    private bool isPreparingAttack;
    private float attackPrepareStartTime;
    private Coroutine aggroTimer;
    private bool attackCorutineStarted = false;

    private void Awake()
    {
        target = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (isStunned)
        {
            agent.SetDestination(transform.position);
            return;
        }

        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, Player);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, Player);

        bool inFOV = playerInSightRange && PlayerInFOV();
        if (inFOV)
        {
            if(aggroTimer != null)
            {
                StopCoroutine(aggroTimer);
            }
            lostEnemyInAggro = false;
            rememberEnemy = true;
        }
        else
        {
            if (!lostEnemyInAggro && rememberEnemy)
            {
                aggroTimer = StartCoroutine(AggroMemory());
            }
        }

            bool canSeePlayer = inFOV || lostEnemyInAggro;

        if (!canSeePlayer && !playerInAttackRange)
        {
            Patroling();
        }

        if (canSeePlayer && !playerInAttackRange)
        {
            ChaseTarget();
            isPreparingAttack = false;
        }

        if (canSeePlayer && playerInAttackRange)
        {
            if (!isPreparingAttack && !alreadyAttacked && !attackCorutineStarted)
            {
                Debug.Log("Attack delay started");
                isPreparingAttack = true;
                StartCoroutine(AttackDelay());
            }
        }
    }

    private IEnumerator AggroMemory()
    {
        lostEnemyInAggro = true;
        yield return new WaitForSeconds (enemyStats.visionMemoryTime);
        lostEnemyInAggro = false;
        rememberEnemy = false;
    }

    private IEnumerator AttackDelay()
    {
        attackCorutineStarted = true;
        yield return new WaitForSeconds(enemyStats.reactionDelay);
        AttackTarget();
        attackCorutineStarted = false;
    }

    private bool PlayerInFOV()
    {
        if (target == null) return false;

        Vector3 origin = transform.position + Vector3.up * enemyStats.eyeHeight + transform.forward * enemyStats.forwardOffset;
        Vector3 dirToPlayer = (target.position - origin).normalized;

        float angle = Vector3.Angle(transform.forward, dirToPlayer);
        if (angle > enemyStats.fovAngle / 2f)
            return false;

        if (Physics.Raycast(origin, dirToPlayer, out RaycastHit hit, sightRange))
        {
            Debug.DrawRay(origin, dirToPlayer * sightRange, Color.red);
            if (hit.transform.root == target.root)
                return true;
        }

        return false;
    }

    private void Patroling()
    {
        if (waypoints.Length == 0) return;
        if (isWaiting) return;

        agent.speed = enemyStats.patrolingSpeed;
        Transform targetPoint = waypoints[currentWaypointIndex];
        agent.SetDestination(targetPoint.position);

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            if (!isWaiting)
                waitCoroutine = StartCoroutine(PatrolPause());
        }
    }

    private IEnumerator PatrolPause()
    {
        isWaiting = true;

        yield return new WaitForSeconds(enemyStats.patrolPauseTime);

        isWaiting = false;

        currentWaypointIndex += patrolDirection;

        if (currentWaypointIndex >= waypoints.Length)
        {
            patrolDirection = -1;
            currentWaypointIndex = waypoints.Length - 2;
        }
        else if (currentWaypointIndex < 0)
        {
            patrolDirection = 1;
            currentWaypointIndex = 1;
        }
    }

    private void ChaseTarget()
    {
        if (alertIcon) alertIcon.SetActive(true);
        agent.SetDestination(target.position);
        agent.speed = enemyStats.chasingSpeed;
    }

    private void AttackTarget()
    {
        Debug.Log("Player attacked");
        agent.SetDestination(transform.position);
        transform.LookAt(target);

        if (!alreadyAttacked)
        {
            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }
    public void OnAttacked()
    {
        if (isStunned) return;

        Debug.Log($"{name} stunned by player attack");
        isStunned = true;
        agent.isStopped = true;

        if (alertIcon) alertIcon.SetActive(false);

        alreadyAttacked = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        if (target != null && playerInSightRange)
        {
            Gizmos.color = Color.blue;
            Vector3 origin = transform.position + Vector3.up * enemyStats.eyeHeight + transform.forward * enemyStats.forwardOffset;
            Vector3 leftDir = Quaternion.Euler(0, -enemyStats.fovAngle / 2f, 0) * transform.forward;
            Vector3 rightDir = Quaternion.Euler(0, enemyStats.fovAngle / 2f, 0) * transform.forward;
            Gizmos.DrawRay(origin, leftDir * sightRange);
            Gizmos.DrawRay(origin, rightDir * sightRange);
        }
    }
}