// How to make ALL kinds of GUNS with just ONE script! (Unity3d tutorial)
// https://www.youtube.com/watch?v=bqNW08Tac0Y

using UnityEngine;
using TMPro;
using Unity.Burst.CompilerServices;

public class GunScript : MonoBehaviour
{
    //Gun stats
    public int damage;
    public float timeBetweenShooting, spread, range, reloadTime, timeBetweenShots;
    public int magazineSize, bulletsPerTap;
    public bool allowButtonHold;
    int bulletsLeft, bulletsShot;

    //bools 
    bool shooting, readyToShoot, reloading;

    //Reference
    public Camera fpsCam;
    public Transform attackPoint;
    public RaycastHit rayHit;
    public LayerMask whatIsEnemy;
    public AudioSource source;

    //Graphics
    public GameObject muzzleFlash, bulletHoleGraphic;
    public CameraShake camShake;
    public float camShakeMagnitude, camShakeDuration;
    public TextMeshProUGUI text;

    private void Awake()
    {
        bulletsLeft = magazineSize;
        readyToShoot = true;
    }
    private void Update()
    {
        MyInput();

        //SetText
        text.SetText(bulletsLeft + " / " + magazineSize);
    }
    private void MyInput()
    {
        if (allowButtonHold) shooting = Input.GetKey(KeyCode.Mouse0);
        else shooting = Input.GetKeyDown(KeyCode.Mouse0);

        if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && !reloading) Reload();

        //Shoot
        if (readyToShoot && shooting && !reloading && bulletsLeft > 0)
        {
            bulletsShot = bulletsPerTap;
            Shoot();
        }
    }
    private void Shoot()
    {
        readyToShoot = false;

        //Spread
        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);

        //Calculate Direction with Spread
        Vector3 direction = fpsCam.transform.forward + new Vector3(x, y, 0);

        //RayCast
        bool hitSomething = Physics.Raycast(fpsCam.transform.position, direction, out rayHit, range, whatIsEnemy);
        if (hitSomething)
        {

            if (rayHit.collider.CompareTag("Enemy"))
                rayHit.collider.GetComponent<AIGeneral>().TakeDamage(damage);
        }

        //ShakeCamera
        StartCoroutine(camShake.Shake(camShakeDuration, camShakeMagnitude));

        if (hitSomething)
        {
            //Graphics
            Instantiate(bulletHoleGraphic, rayHit.point + (0.01f * rayHit.normal), Quaternion.LookRotation(-1 * rayHit.normal, rayHit.transform.up));
        }



        // Instantiate the muzzle flash with the adjusted rotation and position
        GameObject flashInstance = Instantiate(muzzleFlash, GameObject.Find("AttackPoint").transform);
        flashInstance.transform.SetParent(GameObject.Find("AttackPoint").transform, false);
        flashInstance.transform.localPosition = new Vector3(0f,0f,0.06f);
        flashInstance.transform.localRotation = Quaternion.Euler(0f, -90f, 0f);

        Destroy(flashInstance, timeBetweenShooting / 2f);


        bulletsLeft--;
        bulletsShot--;

        Invoke("ResetShot", timeBetweenShooting);

        if (bulletsShot > 0 && bulletsLeft > 0)
            Invoke("Shoot", timeBetweenShots);

        source.Play();
    }
    private void ResetShot()
    {
        readyToShoot = true;
        Destroy(GameObject.Find("Toon Muzzleflash 1 Variant"));
    }
    private void Reload()
    {
        reloading = true;
        Invoke("ReloadFinished", reloadTime);
    }
    private void ReloadFinished()
    {
        bulletsLeft = magazineSize;
        reloading = false;
    }
}