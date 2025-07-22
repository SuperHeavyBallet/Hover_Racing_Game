using System.Collections.Generic;
using UnityEngine;
using static SceneStartup;
using static Ship_Passport;

/// <summary>
/// This script Carries the loadout of the ship across scenes
/// </summary>

public class Ship_Passport : MonoBehaviour
{
    public static Ship_Passport Instance {  get; private set; }
    public Dictionary<ComponentSlotType, ComponentName> componentSlots { get; private set; } = new();
    public bool receivedShipLoadout = false;   


// Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (!receivedShipLoadout)
        {
           

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ReceiveShipLoadout(Dictionary<ComponentSlotType, ComponentName> shipLoadout )
    {
        receivedShipLoadout = true;

        componentSlots.Clear();

        componentSlots = shipLoadout;

        Debug.Log("SHIP PASSPORT");

        foreach(var slot in componentSlots)
        {
            Debug.Log("PASSPORT: " + slot);
        }
        
    }

    public Dictionary<ComponentSlotType, ComponentName> GetShipLoadout()
    {
        var result = new Dictionary<ComponentSlotType, ComponentName>();
        bool hasExtraSlots = false;

        foreach (var pair in componentSlots)
        {
            if (pair.Key == ComponentSlotType.Frame)
            {
                if (pair.Value == ComponentName.heavyFrame)
                {
                    hasExtraSlots = true;
                }
            }

            if (pair.Key == ComponentSlotType.BackLeft1 || pair.Key == ComponentSlotType.BackRight1)
            {
                if (hasExtraSlots)
                {
                    result[pair.Key] = pair.Value;
                }
            }
            else
            {
                result[pair.Key] = pair.Value;
            }

            


        }


        return result;

    }

}
