using UnityEngine;

public class WizardStaff : MonoBehaviour
{
    public int damage = 10;
    public float range = 100f;

    private int leftMouseButton = 0, rightMouseButton = 1;

    public Camera cam;

    public GameObject impactParticles;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(leftMouseButton))
        {
            //Debug.Log("Left mouse button was pressed!");
            Primary();
        }
    }

    void Primary()
    {
        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, range))
        {
            //Debug.Log(hit.transform.name);
            if (hit.transform.tag == "MeleeEnemy")
            {
                MeleeAI target = hit.transform.GetComponent<MeleeAI>();
                target.TakeDamage(damage);
                Debug.Log("Melee Enemy is hit");
            }
            if (hit.transform.tag == "RangeEnemy")
            {
                rangedAI target = hit.transform.GetComponent<rangedAI>();
                target.TakeDamage(damage);
                Debug.Log("Ranged Enemy is hit");
            }

            GameObject impact = Instantiate(impactParticles, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(impact, 1f);
        }
    }
}
