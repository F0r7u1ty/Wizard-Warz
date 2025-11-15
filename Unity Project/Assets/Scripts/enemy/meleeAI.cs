using UnityEngine;
using UnityEngine.AI;

public class MeleeAI : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;

    public LayerMask whatIsGround, whatIsPlayer;

    public int health;

    // Patrolling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    // Attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;
    public int attackDamage = 4; // Damage to deal on hit

    // Melee Attack Specifics
    public float jumpForce = 5f; // How high/fast the enemy jumps
    public float jumpDuration = 1.7f; // How long the jump takes
    private bool isJumping = false;
    private bool damageDealtThisJump = false; // Prevents multiple hits per jump
    // States
    public float sightRange, AttackRange;
    public bool playerInSightRange, playerInAttackRange;

    private void Awake()
    {
        player = GameObject.Find("Player").transform;
        // Get the NavMeshAgent component if it's not set in the Inspector
        if (agent == null)
        {
            agent = GetComponent<NavMeshAgent>();
        }
    }

    private void Update()
    {
        // Prevent movement logic if the enemy is currently executing the jump
        if (isJumping) return;

        // Check for sight and attack range
        // AttackRange for a melee enemy should typically be smaller than for ranged.
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, AttackRange, whatIsPlayer);

        if (!playerInSightRange && !playerInAttackRange) Patroling();
        if (playerInSightRange && !playerInAttackRange) ChasePlayer();
        // A crucial difference: for a jump attack, you might want to only jump
        // when in attack range AND when the NavMeshAgent is stopped.
        if (playerInSightRange && playerInAttackRange) AttackPlayer();
    }

    private void FixedUpdate()
    {
        // Check for hit ONLY during the jump phase
        if (isJumping && !damageDealtThisJump)
        {
            CheckForHit();
        }
    }

    private void SearchWalkPoint()
    {
        // calc random point in range
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        // Check if the random point is on the ground
        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
        {
            walkPointSet = true;
        }
    }

    private void Patroling()
    {
        if (!walkPointSet) SearchWalkPoint();
        if (walkPointSet) agent.SetDestination(walkPoint);

        Vector3 distanceToWalk = transform.position - walkPoint;

        // walkpoint reached
        if (distanceToWalk.magnitude < 1f)
        {
            walkPointSet = false;
        }
    }

    private void ChasePlayer()
    {
        agent.SetDestination(player.position);
    }

    private void AttackPlayer()
    {
        // Make sure enemy doesn't move during the wind-up before the jump
        agent.SetDestination(transform.position);

        // Look at the player before jumping
        transform.LookAt(player);

        if (!alreadyAttacked)
        {
            // Start the jump attack logic
            // use the Rigidbody to apply force for a physics-based jump.
            // Ensure melee guys have Rigidbody component

            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                //Prepare for jump
                isJumping = true;
                agent.enabled = false; // Disable NavMeshAgent while using Rigidbody
                damageDealtThisJump = false;
                //Apply the jump force towards the player
                Vector3 directionToPlayer = (player.position - transform.position).normalized;

                // Combine a forward/horizontal force with an upward force
                Vector3 jumpVelocity = directionToPlayer * (jumpForce / 2) + Vector3.up * jumpForce;

                // Clear any residual velocity before applying the new force
                rb.linearVelocity = Vector3.zero;
                rb.AddForce(jumpVelocity, ForceMode.Impulse);

                //Mark as attacked and schedule the reset
                alreadyAttacked = true;
                Invoke(nameof(EndJump), jumpDuration); // Schedule the end of the jump phase
                Invoke(nameof(ResetAttack), timeBetweenAttacks); // Schedule the attack cooldown
            }
            else
            {
                Debug.LogError("MeleeEnemyAI needs a Rigidbody component for the jump attack!");
                // If no Rigidbody, still reset attack to prevent loop
                alreadyAttacked = true;
                Invoke(nameof(ResetAttack), timeBetweenAttacks);
            }
        }
    }


    // New method to restore NavMeshAgent control after the jump
    private void EndJump()
    {
        isJumping = false;
        // Re-enable the NavMeshAgent to resume normal movement/chase
        if (agent != null)
        {
            agent.enabled = true;
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0) Invoke(nameof(DestroyEnemy), 0.5f);
    }

    private void DestroyEnemy()
    {
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, AttackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }

    private void CheckForHit()
    {
        // sphere checks hit instead of colliders, more consistent
        Collider[] hitPlayers = Physics.OverlapSphere(transform.position, 1.5f, whatIsPlayer);

        // Adjust 1.5f to be the size of attack "hitbox" radius during the jump.

        foreach (Collider playerCollider in hitPlayers)
        {
            // Check to ensure we only hit the player, although LayerMask helps here
            if (playerCollider.CompareTag("Player"))
            {
                if (isJumping && !damageDealtThisJump)
                {
                    // Damage Logic
                    GameData.playerHealth -= attackDamage;
                    Debug.Log($"MeleeAI hit player from sphere! Damage dealt: {attackDamage}.");

                    // Set flag to prevent further hits until the next jump
                    damageDealtThisJump = true;

                    // You may want to stop checking after the hit:
                    return;
                }
            }
        }
    }
}