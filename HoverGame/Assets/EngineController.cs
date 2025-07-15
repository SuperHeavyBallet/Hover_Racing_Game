using UnityEngine;

public class EngineController : MonoBehaviour, IEngineFireListener
{

    public GameObject exhaustFlame;
    public GameObject boostFlame;
    public bool isFiring;
    public GameObject ship;
    public Ship_Movement shipMovement;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        exhaustFlame.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if(shipMovement.isFiring)

        {
            exhaustFlame.SetActive(true);
        }
        else
        {
            exhaustFlame.SetActive(false);
        }*/
    }

    void HandleMovementChanged(bool isMoving)
    {



    }

    void OnDestroy()
    {
       
    }

    public void OnShipEngineFiring(bool isFiring)
    {
        if(isFiring)
        {
            exhaustFlame.SetActive(true);
        }
        else
        {
            exhaustFlame.SetActive(false);
        }

       
    }

    public void OnShipBoostFiring(bool isFiring)
    {
        if (isFiring)
        {
            boostFlame.SetActive(true);
        }
        else
        {
            boostFlame.SetActive(false);
        }
    }
}
