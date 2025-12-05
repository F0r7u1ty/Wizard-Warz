using UnityEngine;
using UnityEngine.AI;

public class rangedAI : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;

    public LayerMask whatIsGround, whatIsPlayer;

    public int health;
    private bool isDead = false;
    //patrol
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    //Attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;
    public GameObject projectile;
    public Transform firePoint;

    //States
    public float sightRange, AttackRange;
    public bool playerInSightRange, playerInAttackRange;

    private void Awake()
    {
        player = GameObject.Find("Player").transform;
    }

    private void Update()
    {
        //check for sight and attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, AttackRange, whatIsPlayer);

        if (!playerInSightRange && !playerInAttackRange) Patroling();
        if (playerInSightRange && !playerInAttackRange) ChasePlayer();
        if (playerInSightRange && playerInAttackRange) AttackPlayer();
    }

    private void SearchWalkPoint()
    {
        //calc random point in range
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);
    
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

        //walkpoint reached
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
            agent.SetDestination(transform.position);
            transform.LookAt(player);

            if (!alreadyAttacked)
            {
                Vector3 spawnPos = (firePoint != null) ? firePoint.position : transform.position;
                // Aim at player (optionally aim at player's chest or head: add Vector3.up * 1.2f)
                Vector3 targetPos = player.position + Vector3.up * 1.0f; // tweak vertical aim if needed
                Vector3 aimDir = (targetPos - spawnPos).normalized;

                // spawn with rotation so forward points along aimDir
                Quaternion spawnRot = Quaternion.LookRotation(aimDir, Vector3.up);

                Rigidbody rb = Instantiate(projectile, spawnPos, spawnRot).GetComponent<Rigidbody>();

                // Use velocity for consistent shot instead of AddForce (you can still use AddForce if you prefer)
                float shotSpeed = 40f;//32f;
                rb.linearVelocity = aimDir * shotSpeed;
                // small upward arc if you want:
                rb.linearVelocity += Vector3.up * 4f; // or tweak as needed
                                                      // rotate aimDir by a few degrees to the right
                float degreesCorrection = 3f;
                //Quaternion correction = Quaternion.Euler(0f, degreesCorrection, 0f);
                Quaternion correction = Quaternion.Euler(-2f, degreesCorrection, 0f);

                Vector3 correctedAim = correction * aimDir;
                rb.linearVelocity = correctedAim * shotSpeed;

                alreadyAttacked = true;
                Invoke(nameof(ResetAttack), timeBetweenAttacks);
            }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0 && !isDead)
        {
            isDead = true;
            GetComponent<Collider>().enabled = false;
            GameData.numEnemies--;
            Invoke(nameof(DestroyEnemy), 0.5f);
        }
        /*health -= damage;
        if (health <= 0)
        {
            Invoke(nameof(DestroyEnemy), 0.5f);
            GameData.numEnemies--;
        } */
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
}