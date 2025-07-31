using UnityEngine;

public class TriggerZoneController : MonoBehaviour
{
    Ship_Movement ship_Movement;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ship_Movement = GetComponent<Ship_Movement>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.CompareTag("BoostZone") && !ship_Movement.enteredBoostZone)
        {

            ship_Movement.ActivateBoostZone(true);

            
        }
        else if (other.gameObject.CompareTag("PitStopZone") && !ship_Movement.inPitStop)
        {
            ship_Movement.ActivatePitStop(true);
            
        }
        else if (other.gameObject.CompareTag("PickUp"))
        {
            other.gameObject.SetActive(false);
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("BoostZone"))
        {
            ship_Movement.ExitedBoostZone();
        }
        else if (other.gameObject.CompareTag("PitStopZone") && ship_Movement.inPitStop)
        {
            ship_Movement.ActivatePitStop(false);
        }
    }
}
