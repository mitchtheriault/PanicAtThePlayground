using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class NerdKidController : MonoBehaviour
{
    public enum NerdState { Idle, Flee, Stunned, Respawn }
    private NerdState currentState;

    private NavMeshAgent agent;
    public List<Transform> wanderPoints; // Points for wandering
    private int currentDestination;

    public float detectionRadius = 5f; // Radius for detecting Mischief Mike
    public float stunnedDuration = 3f; // Time spent stunned
    private float stunnedTimer;

    private Transform player; // Reference to Mischief Mike
    private Animator animator;

    public float walkingSpeed = 1.0f;
    public float runningSpeed = 3.0f;

    public AudioSource punchSound;



    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        player = GameObject.FindWithTag("Player").transform;

        SetState(NerdState.Idle); // Start in the Idle state
    }

    private void Awake()
    {
        // Find all GameObjects with the "wanderpoint" tag and add their transforms to the list.
        GameObject[] points = GameObject.FindGameObjectsWithTag("WanderPoint");
        wanderPoints = new List<Transform>();

        foreach (GameObject point in points)
        {
            wanderPoints.Add(point.transform);
        }

        Debug.Log($"Found {wanderPoints.Count} wander points.");
    }

    void Update()
    {
        switch (currentState)
        {
            case NerdState.Idle:
                HandleIdleState();
                break;

            case NerdState.Flee:
                HandleFleeState();
                break;

            case NerdState.Stunned:
                HandleStunnedState();
                break;

            case NerdState.Respawn:
                HandleRespawnState();
                break;
        }

        UpdateAnimation(); // Keep animations in sync with movement
    }

    void SetState(NerdState newState)
    {
        currentState = newState;

        switch (newState)
        {
            case NerdState.Idle:
                SetNewDestination();
                agent.speed = walkingSpeed; // Normal walking speed
                break;

            case NerdState.Flee:
                agent.speed = runningSpeed; // Faster fleeing speed
                MoveToClosestFleePoint();
                break;

            case NerdState.Stunned:
                agent.isStopped = true; // Stop movement
                stunnedTimer = stunnedDuration;
                break;

            case NerdState.Respawn:
                Respawn();
                break;
        }
    }

    void UpdateAnimation()
    {
        animator.SetBool("IsWalking", agent.velocity.magnitude > 0.1f);
    }

    // Idle State Logic
    void HandleIdleState()
    {
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            SetNewDestination();
        }

        // Check if Mischief Mike is nearby
        if (Vector3.Distance(transform.position, player.position) < detectionRadius)
        {
            SetState(NerdState.Flee);
        }
    }

    void SetNewDestination()
    {
        currentDestination = Random.Range(0, wanderPoints.Count);
        agent.SetDestination(wanderPoints[currentDestination].position);
    }

    // Flee State Logic
    void HandleFleeState()
    {
        if (agent.remainingDistance < 0.5f)
        {
            SetState(NerdState.Idle); // Stop fleeing after reaching a point
        }
    }

    void MoveToClosestFleePoint()
    {
        Transform closestPoint = null;
        float closestDistance = Mathf.Infinity;

        foreach (Transform point in wanderPoints)
        {
            float distance = Vector3.Distance(transform.position, point.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestPoint = point;
            }
        }

        if (closestPoint != null)
        {
            agent.SetDestination(closestPoint.position);
        }
    }

    // Stunned State Logic
    void HandleStunnedState()
    {
        stunnedTimer -= Time.deltaTime;
        if (stunnedTimer <= 0)
        {
            SetState(NerdState.Respawn);
        }
    }

    public void GetStunned()
    {
        SetState(NerdState.Stunned);
        animator.SetTrigger("Hit");
    }

    // Respawn State Logic
    void HandleRespawnState()
    {
        SetNewDestination(); // Choose a random point
        SetState(NerdState.Idle);
    }

    void Respawn()
    {
        int spawnIndex = Random.Range(0, wanderPoints.Count);
        transform.position = wanderPoints[spawnIndex].position;
        agent.isStopped = false;
    }

    // Collision Handling
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerAttack"))
        {
            ReactToHit();
        }
    }

    public void ReactToHit()
    {
        animator.SetTrigger("Hit");
        // Optional: Add knockback force
        Rigidbody rb = GetComponentInChildren<Rigidbody>();
        if (rb != null)
        {
            Vector3 knockback = (transform.position - player.transform.position).normalized * 5f;
            rb.AddForce(knockback, ForceMode.Impulse);
        }

        GameManager.instance.AddScore(1);
        punchSound.Play();
    }


}