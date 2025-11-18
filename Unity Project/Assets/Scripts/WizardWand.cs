using UnityEngine;

public class WizardWand : MonoBehaviour
{
    public GameObject bullet;

    public float shootForce, upwardForce;

    public float timeBetweenShooting, spread, timeBetweenShots;

    bool shooting, readyToShoot;

    public Camera cam;

    public bool allowInvoke = true;

    // The Transform we will use as the bullet's origin
    public Transform firePoint;

    // --- NEW VARIABLES FOR HEAT-SEEKING & DAMAGE ---
    [Header("Heat Seek and Damage")]
    public LayerMask enemyLayer;      // Assign this in the Inspector to the 'Enemy' layer
    public float heatSeekRange = 70f; // The radius for finding enemies
    public float damage = 10f;       // Damage the bullet will inflict
    // ------------------------------------------------

    private void Awake()
    {
        if (firePoint == null)
        {
            Debug.LogError("FirePoint Transform is not assigned!");
        }
        readyToShoot = true;
    }

    private void Update()
    {
        MyInput();
    }


    private void MyInput()
    {
        // Use Mouse0 (left click) for the primary attack
        shooting = Input.GetKeyDown(KeyCode.Mouse0);

        // Ensure we are ready to shoot and can afford the mana cost (Assuming GameData.ExhaustPlayerMana exists)
        if (readyToShoot && shooting && GameData.ExhaustPlayerMana(10))
        {
            Primary();
        }
    }

    private void Primary()
    {
        readyToShoot = false;

        // 1. Determine the target point and check for heat-seek target
        Vector3 targetPoint;
        Transform heatSeekTarget = null;

        // Check for enemies within the heatSeekRange around the FirePoint
        Collider[] hitColliders = Physics.OverlapSphere(firePoint.position, heatSeekRange, enemyLayer);

        if (hitColliders.Length > 0)
        {
            // **Enemy Found: Set the heat-seek target.**
            // Simple heat-seeking: target the first enemy found
            heatSeekTarget = hitColliders[0].transform;
            targetPoint = heatSeekTarget.position;
        }
        else
        {
            // **No Enemy Found: Aim straight ahead relative to the camera.**
            // This replaces the raycast. The bullet will be fired toward the center
            // of the screen, 75 units away, if no heat-seek target is found.
            const float defaultShootDistance = 75f;
            targetPoint = cam.transform.position + cam.transform.forward * defaultShootDistance;
        }


        // 2. Calculate the base direction from the firePoint to the target
        // The bullet will be aimed slightly towards the targetPoint, giving the projectile
        // a head start on its path.
        Vector3 directionWithoutSpread = targetPoint - firePoint.position;

        // 3. Apply spread
        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);
        Vector3 directionWithSpread = directionWithoutSpread + new Vector3(x, y, 0);

        // 4. Instantiate the bullet at the firePoint
        GameObject currentBullet = Instantiate(bullet, firePoint.position, Quaternion.identity);

        // 5. Pass data to the bullet script
        WandBullet bulletScript = currentBullet.GetComponent<WandBullet>();
        if (bulletScript != null)
        {
            // Pass the detected target (can be null) and the damage value
            bulletScript.SetBullet(heatSeekTarget, damage);
        }

        // 6. Set the bullet's initial forward direction and apply force
        currentBullet.transform.forward = directionWithSpread.normalized;

        currentBullet.GetComponent<Rigidbody>().AddForce(directionWithSpread.normalized * shootForce, ForceMode.Impulse);
        currentBullet.GetComponent<Rigidbody>().AddForce(cam.transform.up * upwardForce, ForceMode.Impulse);

        Destroy(currentBullet, 3f); // will be removed in future

        // 7. Reset for the next shot
        if (allowInvoke)
        {
            Invoke("ResetPrimaryShot", timeBetweenShooting);
            allowInvoke = false;
        }
    }

    private void ResetPrimaryShot()
    {
        readyToShoot = true;
        allowInvoke = true;
    }
}