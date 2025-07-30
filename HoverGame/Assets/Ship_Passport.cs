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

    public Dictionary<ComponentName, GameObject> componentPrefabs;


    public GameObject engine;
    public GameObject jetEngine;
    public GameObject aireon;
    public GameObject lightFrame;
    public GameObject mediumFrame;
    public GameObject heavyFrame;
    public GameObject fuelTank;
    public GameObject boostGulp;
    public GameObject machineGun;
    public GameObject missile;




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

        componentPrefabs = new Dictionary<ComponentName, GameObject>();
        InitialisePrefabReferences();
    }

    void InitialisePrefabReferences()
    {
        componentPrefabs.Add(ComponentName.lightFrame, lightFrame);
        componentPrefabs.Add(ComponentName.mediumFrame, mediumFrame);
        componentPrefabs.Add(ComponentName.heavyFrame, heavyFrame);
        componentPrefabs.Add(ComponentName.engine, engine);
        componentPrefabs.Add(ComponentName.jetEngine, jetEngine);
        componentPrefabs.Add(ComponentName.aireon, aireon);
        componentPrefabs.Add(ComponentName.fuelTank, fuelTank);
        componentPrefabs.Add(ComponentName.boostGulp, boostGulp);
        componentPrefabs.Add(ComponentName.machineGun, machineGun);
        componentPrefabs.Add(ComponentName.missile, missile);
    }

    public GameObject GetPrefab(ComponentName componentName)
    {
        if (componentPrefabs.TryGetValue(componentName, out var prefab))
        {
            return prefab;
        }
        else
        {
            Debug.LogWarning($"No prefab found for component: {componentName}");
            return null;
        }
    }


    

    private void Start()
    {
        if (!receivedShipLoadout)
        {
            Debug.Log("NO LOADOUT RECEIVED!");
        }

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
        { ComponentSlotType.ExtraFront, ComponentName.missile },
        { ComponentSlotType.ExtraLeft, ComponentName.fuelTank },
        { ComponentSlotType.ExtraRight, ComponentName.fuelTank }
    };

        return defaultLoadout;
    }

    public ComponentName GetWeaponType()
    {
        foreach (var slot in componentSlots)
        {
            if(slot.Key == ComponentSlotType.ExtraFront)
            {
           
                return slot.Value;
            }
        }

        return ComponentName.empty;
    }

}

