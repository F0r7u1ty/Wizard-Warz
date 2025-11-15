using UnityEngine;

public class WizardWand : MonoBehaviour
{

    public GameObject bullet;

    public float shootForce, upwardForce;

    public float timeBetweenShooting, spread, timeBetweenShots;

    bool shooting, readyToShoot;

    public Camera cam;
    public Transform attackPoint;

    public bool allowInvoke = true;

    private void Awake()
    {
        readyToShoot = true;
    }

    private void Update()
    {
        MyInput();
    }


    private void MyInput()
    {
        shooting = Input.GetKeyDown(KeyCode.Mouse0);

        if (readyToShoot && shooting && GameData.ExhaustPlayerMana(10))
        {
            Primary();
        }
    }

    private void Primary()
    {
        readyToShoot = false;

        Ray ray = cam.ViewportPointToRay(new Vector3(.5f, .5f, 0));
        RaycastHit hit;

        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.GetPoint(75);
        }

        Vector3 directionWithoutSpread = targetPoint - attackPoint.position;

        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);

        Vector3 directionWithSpread = directionWithoutSpread + new Vector3(x, y, 0);

        GameObject currentBullet = Instantiate(bullet, attackPoint.position, Quaternion.identity);

        currentBullet.transform.forward = directionWithSpread.normalized;

        currentBullet.GetComponent<Rigidbody>().AddForce(directionWithSpread.normalized * shootForce, ForceMode.Impulse);
        currentBullet.GetComponent<Rigidbody>().AddForce(cam.transform.up * upwardForce, ForceMode.Impulse);

        Destroy(currentBullet,3f); // will be removed in future

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