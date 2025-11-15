using UnityEngine;
using UnityEngine.AI;

public class EnemyAnimatorController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private EnemyStats enemyStats;

    private float walkToRunThreshold;

    private void Awake()
    {
        if (!animator) animator = GetComponentInChildren<Animator>();
        if (!agent) agent = GetComponent<NavMeshAgent>();

        walkToRunThreshold = (enemyStats.patrolingSpeed + enemyStats.chasingSpeed) * 0.5f;
    }

    private void Update()
    {
        float speed = agent.velocity.magnitude;

        animator.SetFloat("Speed", speed);
        animator.SetBool("IsRunning", speed > walkToRunThreshold);
    }
}
