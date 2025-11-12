using UnityEngine;
using System.Collections; // Still needed for the beam's duration coroutine

public class WizardStaff : MonoBehaviour
{
    public int primaryDamage = 4;
    public float range = 100f;

    // Fire rate logic is often unnecessary with GetMouseButtonDown,
    // but kept here for potential future use or if you want delayed next shot.
    public float primaryFireRate = 0.2f;
    private float nextFireTime = 0f;

    // The LineRenderer component attached to the staff GameObject
    public LineRenderer lineRenderer;
    // The duration the beam will be visible (e.g., a quick flash)
    public float beamDuration = 0.05f;

    private int leftMouseButton = 0;

    public Camera cam;

    public GameObject impactParticles;

    // Removed: private Coroutine drawBeamCoroutine; (No longer needed without hold-down fire issues)

    void Awake()
    {
        // Get the LineRenderer component if it wasn't assigned in the inspector
        if (lineRenderer == null)
        {
            lineRenderer = GetComponent<LineRenderer>();
        }
        // Ensure the beam is off at the start of the game
        lineRenderer.enabled = false;
    }


    void Update()
    {
        // *** REVERTED: Use GetMouseButtonDown for single-shot (tap-to-fire) ***
        if (Input.GetMouseButtonDown(leftMouseButton) && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + primaryFireRate;
            Primary();
        }
    }

    void Primary()
    {
        if (GameData.ExhaustPlayerMana(9))
        {
            RaycastHit hit;
            // The raycast is the hitscan logic (instant hit)
            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, range))
            {
                // Apply damage (Hitscan Logic)
                if (hit.transform.tag == "MeleeEnemy")
                {
                    MeleeAI target = hit.transform.GetComponent<MeleeAI>();
                    if (target != null) target.TakeDamage(primaryDamage);
                }
                if (hit.transform.tag == "RangeEnemy")
                {
                    rangedAI target = hit.transform.GetComponent<rangedAI>();
                    if (target != null) target.TakeDamage(primaryDamage);
                }

                // Start the Coroutine to draw the beam
                StartCoroutine(DrawBeam(hit.point));

                // Instantiate the impact particles at the hit location
                if (impactParticles != null)
                {
                    GameObject impact = Instantiate(impactParticles, hit.point, Quaternion.LookRotation(hit.normal));
                    Destroy(impact, 1f);
                }
            }
            else
            {
                // If the raycast doesn't hit anything within range, 
                // draw the beam to the max range limit
                Vector3 endPoint = cam.transform.position + cam.transform.forward * range;
                StartCoroutine(DrawBeam(endPoint));
            }
        }
    }

    // Coroutine to manage the visual beam flash 
    IEnumerator DrawBeam(Vector3 endPoint)
    {
        // 1. Set the beam's start point (the camera/staff muzzle) and end point (the hit location)
        lineRenderer.SetPosition(0, cam.transform.position); // Start point
        lineRenderer.SetPosition(1, endPoint); // End point

        // 2. Turn the Line Renderer on (makes the beam visible)
        lineRenderer.enabled = true;

        // 3. Wait for a short flash duration
        yield return new WaitForSeconds(beamDuration);

        // 4. Turn the Line Renderer off (beam disappears)
        lineRenderer.enabled = false;
    }
}