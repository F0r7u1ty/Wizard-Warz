using UnityEngine;

public class ProjectileDestruction : MonoBehaviour
{
    public float lifetime = 2f;
    private bool canCollide = false; // New flag to control collision

    void Start()
    {
        // 1. Set the total lifetime for cleanup
        Destroy(gameObject, lifetime);

        // 2. Wait a fraction of a second before allowing collision detection
        Invoke(nameof(EnableCollision), 0.02f);
    }

    private void EnableCollision()
    {
        // Set the flag to true after a very short delay (e.g., 0.05 seconds)
        canCollide = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Only destroy the projectile if the canCollide flag is true
        if (canCollide)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                GameData.playerHealth -= 2;
            }
            // You can also add a check here to ensure it's not the enemy itself, 
            // but the time delay should handle that.
            Destroy(gameObject);
        }
    }
}