using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using static SceneStartup;
using static Ship_Passport;

/// <summary>
/// This script Carries the loadout of the ship across scenes
/// </summary>

public class Ship_Passport : MonoBehaviour
{
    public static Ship_Passport Instance {  get; private set; }
    public Dictionary<ComponentSlotType, ComponentName> componentSlots  = new();
    public bool receivedShipLoadout = false;


    public enum AllPosibleComponents 
    {
        engine,
        jetEngine,
        aireon,
        fuelTank,
        boostGulp
    }

    public Dictionary<AllPosibleComponents, bool> playerUnlockedComponents = new();


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

        InitialiseUnlockedComponents();
        CheckUnlockedComponents();
        playerUnlockedComponents[AllPosibleComponents.boostGulp] = true;
        ReCheckUnlocks();
    }

    void CheckUnlockedComponents()
    {
        foreach(var component in playerUnlockedComponents)
        {
         

            if(component.Value == true)
            {
                Debug.Log(component.Key + " is unlocked");
            }
            else
            {
                Debug.Log(component.Key + " is locked");
                
            }
        }

        


    }

    void ReCheckUnlocks()
    {
        foreach (var component in playerUnlockedComponents)
        {
            if (component.Value == true)
            {
                Debug.Log(component.Key + " is unlocked");
            }
            else
            {
                Debug.Log(component.Key + " is locked");
            }
        }
    }

    void InitialiseUnlockedComponents()
    {
        playerUnlockedComponents.Add(AllPosibleComponents.engine, true);
        playerUnlockedComponents.Add(AllPosibleComponents.jetEngine, true);
        playerUnlockedComponents.Add(AllPosibleComponents.aireon, true);
        playerUnlockedComponents.Add(AllPosibleComponents.fuelTank, true);
        playerUnlockedComponents.Add(AllPosibleComponents.boostGulp, false);
    }

    private void Start()
    {
        if (!receivedShipLoadout)
        {
            Debug.Log("NO LOADOUT RECEIVED!");

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

        
    }

    public Dictionary<ComponentSlotType, ComponentName> GetShipLoadout()
    {
        var result = new Dictionary<ComponentSlotType, ComponentName>();
        bool hasExtraSlots = false;

        if (componentSlots.Count > 0)
        {
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

        else
        {
            var defaultLoadout = CreateDefaultLoadout();
            componentSlots = defaultLoadout;
            return defaultLoadout;
        }

        



    }

    Dictionary<ComponentSlotType, ComponentName> CreateDefaultLoadout()
    {
        var defaultLoadout = new Dictionary<ComponentSlotType, ComponentName>
    {
        { ComponentSlotType.Frame, ComponentName.mediumFrame },
        { ComponentSlotType.FrontLeft, ComponentName.engine },
        { ComponentSlotType.FrontRight, ComponentName.engine },
        { ComponentSlotType.BackLeft, ComponentName.engine },
        { ComponentSlotType.BackRight, ComponentName.engine },
        { ComponentSlotType.BackLeft1, ComponentName.empty },
        { ComponentSlotType.BackRight1, ComponentName.empty },
        { ComponentSlotType.ExtraFront, ComponentName.boostGulp },
        { ComponentSlotType.ExtraLeft, ComponentName.fuelTank },
        { ComponentSlotType.ExtraRight, ComponentName.fuelTank }
    };

        return defaultLoadout;
    }

}
