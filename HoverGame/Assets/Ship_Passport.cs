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
    public Dictionary<ComponentSlotType, string> componentSlots { get; private set; } = new(); 


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

        Debug.Log("PASSPORT");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ReceiveShipLoadout(Dictionary<ComponentSlotType, string> shipLoadout )
    {
        componentSlots.Clear();

        componentSlots = shipLoadout;

        Debug.Log("SHIP PASSPORT");

        foreach(var slot in componentSlots)
        {
            Debug.Log("PASSPORT: " + slot);
        }
        
    }

    public Dictionary<ComponentSlotType, string> GetShipLoadout()
    {
        var result = new Dictionary<ComponentSlotType, string>();

        foreach (var pair in componentSlots)
        {
            result[pair.Key] = pair.Value;


        }



        return result;

    }
}
