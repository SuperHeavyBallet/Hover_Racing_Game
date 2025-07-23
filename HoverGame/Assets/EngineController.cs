using UnityEngine;

public class EngineController : MonoBehaviour, IEngineFireListener
{

    public GameObject exhaustFlame;
    public GameObject boostFlame;
    public bool isFiring;
    public GameObject ship;
    public Ship_Movement shipMovement;

    public GameObject nozzle;
    public Transform nozzlePivot;

    float minYaw = -20f;
    float maxYaw = 20f;
    private float currentYaw = 0f;
    float returnSpeed = 150f;

    bool isReceivingInput = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        exhaustFlame.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
        if(nozzle != null)
        {
            if (!isReceivingInput)
            {
                currentYaw = Mathf.MoveTowards(currentYaw, 0f, returnSpeed * Time.deltaTime);
                nozzle.transform.localRotation = Quaternion.Euler(0f, currentYaw, 0f);
            }

            // Reset flag each frame
            isReceivingInput = false;
        }
        


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

    
    public void OnShipRotateNozzle(float turnAmount)
    {
        isReceivingInput = true;
        currentYaw = Mathf.Clamp(currentYaw + turnAmount, minYaw, maxYaw);

        // Set rotation relative to initial rotation
        nozzle.transform.localRotation = Quaternion.Euler(0f, currentYaw, 0f);
    }
}
