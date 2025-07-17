using TMPro;
using UnityEngine;

public class CurrencyTracker : MonoBehaviour
{

    public TextMeshProUGUI currencyCountText;

    public int currencyCount;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        

    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("PickUp"))
        {

            Debug.Log("PICKUP");

            PickUp_Controller pickUp_Controller = other.gameObject.GetComponent<PickUp_Controller>();

            if(pickUp_Controller != null )
            {
                // Prevent multiple triggers
                if (pickUp_Controller.hasBeenPickedUp)
                    return;
                pickUp_Controller.hasBeenPickedUp = true;

                string pickUpType = pickUp_Controller.GetPickUpType();
                
                if ( pickUpType == "currency")
                {
                    
                    UpdateCurrencyCount(1);
                }
            }

           Destroy(other.gameObject);
        }
    }

    void UpdateCurrencyCount(int amountToAdd)
    {
        currencyCount += amountToAdd;
        currencyCountText.text = currencyCount.ToString();
    }
}
