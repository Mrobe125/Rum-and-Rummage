using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAIController : MonoBehaviour
{
    public enum AIState { Patrol, Chase, Attack, Search }

    [Header("Stats")]
    public EnemyStats stats;


    [Header("Configuration")]
    public AIState currentState = AIState.Patrol;
    public bool canPatrol = true; // False for Charging Beasts
    public float detectionRange = 15f;
    public float attackRange = 2f;
    public float searchDuration = 4f;

    [Header("References")]
    public Transform[] patrolPoints;
    protected NavMeshAgent agent;
    protected Transform player;

    private int patrolIndex;
    private Vector3 lastKnownPosition;
    private float searchTimer;

    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (stats != null)
        {
            canPatrol = stats.canPatrol;
            detectionRange = stats.detectionRange;
            attackRange = stats.attackRange;
            searchDuration = stats.searchDuration;
            agent.speed = stats.moveSpeed;
        }

        currentState = canPatrol ? AIState.Patrol : AIState.Search;
    }

    protected virtual void Update()
    {
        if (player == null) return;

        switch (currentState)
        {
            case AIState.Patrol: HandlePatrol(); break;
            case AIState.Chase: HandleChase(); break;
            case AIState.Attack: HandleAttack(); break;
            case AIState.Search: HandleSearch(); break;
        }
    }

    protected virtual void HandlePatrol()
    {
        if (CanSeePlayer()) { currentState = AIState.Chase; return; }
        if (patrolPoints.Length == 0) return;

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            agent.SetDestination(patrolPoints[patrolIndex].position);
            patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
        }
    }

    protected virtual void HandleChase()
    {
        if (!CanSeePlayer())
        {
            lastKnownPosition = player.position;
            searchTimer = searchDuration;
            agent.SetDestination(lastKnownPosition);
            currentState = AIState.Search;
            return;
        }

        float dist = Vector3.Distance(transform.position, player.position);
        if (dist <= attackRange)
        {
            agent.ResetPath();
            currentState = AIState.Attack;
            return;
        }

        agent.SetDestination(player.position);
    }

    protected virtual void HandleAttack()
    {
        // Override in subclasses: Melee swing, Ranged projectile + LoS, or Beast charge dash
        float dist = Vector3.Distance(transform.position, player.position);
        if (dist > attackRange * 1.15f)
        {
            currentState = AIState.Chase;
        }
    }

    protected virtual void HandleSearch()
    {
        if (CanSeePlayer()) { currentState = AIState.Chase; return; }

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            searchTimer -= Time.deltaTime;
            if (searchTimer <= 0f)
            {
                currentState = canPatrol ? AIState.Patrol : AIState.Search;
            }
        }
    }

    protected bool CanSeePlayer()
    {
        float dist = Vector3.Distance(transform.position, player.position);
        if (dist > detectionRange) return false;

        Vector3 origin = transform.position + Vector3.up;
        Vector3 dir = (player.position + Vector3.up - origin).normalized;
        return !Physics.Raycast(origin, dir, dist, LayerMask.GetMask("Environment"));
    }
}