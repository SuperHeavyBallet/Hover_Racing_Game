using System.Collections;
using UnityEngine;

public class ShipWeaponRouter : MonoBehaviour
{

    Ship_Passport shipPassport;
    string weaponName;

    public GameObject missilePrefab;
    public Transform weaponFirePosition;

    public bool fireIsHeldDown = false;

    public GameObject bulletImpact;
    float delayBetweenShots = 0.05f;
    Coroutine FireOneShot;

    public LineRenderer shipSight;


    Audio_Manager audioManager;
    public LayerMask hitLayers;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        shipPassport = Ship_Passport.Instance;
        weaponName = shipPassport.GetWeaponType();

        audioManager = GameObject.Find("AudioManager").GetComponent<Audio_Manager>();


    }

    // Update is called once per frame
    void Update()
    {
        UpdateSightLine();

        if (fireIsHeldDown && weaponName == "MACHINE_GUN")
        {
            FireMachineGun();
        }
    }

    public void ShipFireWeapon()
    {
       

        if(weaponName == "MISSILE_LAUNCHER")
        {

            if(!fireIsHeldDown)
            {
                FireMissile();
            }
           
            
        }
        else if(weaponName == "MACHINE_GUN")
        {
            FireMachineGun();
        }

        fireIsHeldDown = true;
    }

    public void ShipCeaseFireWeapon()
    {
        fireIsHeldDown = false;
    }

    void FireMissile()
    {
        GameObject newMissile = Instantiate(missilePrefab, weaponFirePosition.position, weaponFirePosition.transform.rotation);
    }

    void FireMachineGun()
    {
        if (FireOneShot == null)
        {
            audioManager.PlayOneBulletFire();
            CheckLineOfFire();
            FireOneShot = StartCoroutine(CooldownBetweenShots());
        }
        
    }

    void CheckLineOfFire()
    {
        float gunRange = 250f;

        RaycastHit hit;

        if(Physics.Raycast(weaponFirePosition.transform.position, weaponFirePosition.transform.forward, out hit, gunRange, hitLayers))
        {
            
            GameObject newBulletImpact = Instantiate(bulletImpact, hit.point, Quaternion.LookRotation(hit.normal));
        }
    }

    IEnumerator CooldownBetweenShots()
    {
        yield return new WaitForSeconds(delayBetweenShots);
        
        StopCoroutine(FireOneShot);
        FireOneShot = null;

    }

    void UpdateSightLine()
    {
        float sightRange = 250f;

        Vector3 startPos = weaponFirePosition.position;
        Vector3 endPos = startPos + weaponFirePosition.forward * sightRange;

        shipSight.SetPosition(0, startPos); // Start point
        shipSight.SetPosition(1, endPos);   // End point
    }
}
