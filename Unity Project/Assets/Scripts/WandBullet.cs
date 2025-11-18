using UnityEngine;

public class WandBullet : MonoBehaviour
{
    private Transform target;
    private float damageAmount;
    private Rigidbody rb;

    [Header("Seeking Properties")]
    public float seekTorque = 2f;      // How aggressively the bullet turns toward the target
    public float constantSpeed = 15f;  // Maintain speed while seeking
    public LayerMask enemyLayer;       // Must match the layer used in WizardWand

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        // Ensure the bullet has a Rigidbody component
        if (rb == null)
        {
            Debug.LogError("WizardBullet requires a Rigidbody component on the bullet prefab.");
        }
    }

    // Called by WizardWand right after instantiation
    public void SetBullet(Transform newTarget, float damage)
    {
        target = newTarget;
        damageAmount = damage;
    }

    void FixedUpdate()
    {
        // Only seek if a target was found when the bullet was fired
        if (target != null)
        {
            // Calculate direction to the target
            Vector3 directionToTarget = (target.position - transform.position).normalized;

            // Calculate the rotation needed to face the target
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);

            // Smoothly rotate towards the target
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, Time.fixedDeltaTime * seekTorque));

            // Maintain speed by updating the velocity in the new forward direction
            rb.linearVelocity = transform.forward * constantSpeed;
        }
        // If target is null, the bullet moves with its initial force applied in Primary()
    }

    void OnTriggerEnter(Collider other)
    {
        // 1. Check if the collided object is on the designated enemy layer
        if ((enemyLayer.value & (1 << other.gameObject.layer)) > 0)
        {
            // 2. Damage logic matching the WizardStaff tag system
            if (other.transform.tag == "MeleeEnemy")
            {
                // Attempt to get the MeleeAI script for damage
                MeleeAI targetAI = other.transform.GetComponent<MeleeAI>();
                if (targetAI != null)
                {
                    targetAI.TakeDamage((int)damageAmount);
                }
                Destroy(gameObject); // Destroy bullet after hitting enemy
                return;
            }
            else if (other.transform.tag == "RangeEnemy")
            {
                // Attempt to get the rangedAI script for damage
                rangedAI targetAI = other.transform.GetComponent<rangedAI>();
                if (targetAI != null)
                {
                    targetAI.TakeDamage((int)damageAmount);
                }
                Destroy(gameObject); // Destroy bullet after hitting enemy
                return;
            }

            // If it hits an enemy but the tag is wrong/missing, destroy the bullet anyway
            Destroy(gameObject);
        }

        // Optional: Destroy the bullet if it hits something else (e.g., walls)
        // If you only want it destroyed by enemies, remove this 'else' block.
        else
        {
            Destroy(gameObject);
        }
    }
}