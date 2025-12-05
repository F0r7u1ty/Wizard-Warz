using UnityEngine;

public class StarBullet : MonoBehaviour
{
    public float speed = 25f;
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Fire(Vector3 direction)
    {
        rb.linearVelocity = direction.normalized * speed;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "MeleeEnemy")
        {
            MeleeAI target = collision.transform.GetComponent<MeleeAI>();
            if (target != null) target.TakeDamage(20);
        }
        if (collision.transform.tag == "RangeEnemy")
        {
            rangedAI target = collision.transform.GetComponent<rangedAI>();
            if (target != null) target.TakeDamage(20);
        }
        Destroy(gameObject);
    }
}
