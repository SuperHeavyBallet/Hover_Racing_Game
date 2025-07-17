using JetBrains.Annotations;
using UnityEngine;

public class PickUp_Controller : MonoBehaviour
{


    public PickupTypeChoice SELECTED_PickupType = PickupTypeChoice.currency;

    public enum PickupTypeChoice
    {
        currency,
        other
    }

    public bool hasBeenPickedUp = false;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public string GetPickUpType()
    {
      
            return SELECTED_PickupType.ToString();
      
        
    }
}
