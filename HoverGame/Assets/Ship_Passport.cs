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
    public Dictionary<ComponentSlotPosition, ComponentName> componentSlots  = new();
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

    Dictionary<ComponentName, GameObject> frameComponentOptions = new();
    Dictionary<ComponentName, GameObject> engineComponentOptions = new();
    Dictionary<ComponentName, GameObject> extraComponentOptions = new();
    Dictionary<ComponentName, GameObject> extraTopComponentOptions = new();


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
        DefineComponentOptions();
    }

    public GameObject GetShipFrame()
    {
        foreach (var component in componentPrefabs)
        {
            GameObject frame;

            switch (component.Key)
            {
                case ComponentName.lightFrame:
                    frame = component.Value;
                    break;
                    case ComponentName.mediumFrame:
                    frame = component.Value;
                    break;
                case ComponentName.heavyFrame:
                    frame = component.Value;
                    break;
                    default:
                    frame = null;

                break;
            }
            return frame;
        }

        return null;

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


    public void SetShipLoadout(Dictionary<ComponentSlotPosition, ComponentName> shipLoadout )
    {
        receivedShipLoadout = true;

        componentSlots.Clear();

        componentSlots = shipLoadout;

        
    }

  
    public bool CheckBoostGulpPresent()
    {
        foreach(var component in componentSlots)
        {
            if(component.Value == ComponentName.boostGulp)
            {
                return true;
            }
        }

        return false;
    }

    public Dictionary<ComponentSlotPosition, ComponentName> GetShipLoadout()
    {
        var result = new Dictionary<ComponentSlotPosition, ComponentName>();
        bool hasExtraSlots = false;

        if (componentSlots.Count > 0)
        {
            Debug.Log("Ship Prepared");
            foreach (var pair in componentSlots)
            {
                if (pair.Key == ComponentSlotPosition.Frame)
                {
                    if (pair.Value == ComponentName.heavyFrame)
                    {
                        hasExtraSlots = true;
                    }
                }

                if (pair.Key == ComponentSlotPosition.BackLeft1 || pair.Key == ComponentSlotPosition.BackRight1)
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
            Debug.Log("No Ship Prepared, making default");
            var defaultLoadout = CreateDefaultLoadout();
            componentSlots = defaultLoadout;
            return defaultLoadout;
        }

        



    }



    public Dictionary<ComponentSlotPosition, ComponentName> CreateDefaultLoadout()
    {
        var defaultLoadout = new Dictionary<ComponentSlotPosition, ComponentName>
    {
        { ComponentSlotPosition.Frame, ComponentName.mediumFrame },
        { ComponentSlotPosition.FrontLeft, ComponentName.jetEngine },
        { ComponentSlotPosition.FrontRight, ComponentName.jetEngine },
        { ComponentSlotPosition.BackLeft, ComponentName.jetEngine },
        { ComponentSlotPosition.BackRight, ComponentName.jetEngine },
        { ComponentSlotPosition.BackLeft1, ComponentName.empty },
        { ComponentSlotPosition.BackRight1, ComponentName.empty },
        { ComponentSlotPosition.ExtraTop, ComponentName.boostGulp },
        { ComponentSlotPosition.ExtraLeft, ComponentName.empty },
        { ComponentSlotPosition.ExtraRight, ComponentName.empty }
    };

        return defaultLoadout;
    }

    public ComponentName GetWeaponType()
    {
        foreach (var slot in componentSlots)
        {
            if(slot.Key == ComponentSlotPosition.ExtraTop)
            {
           
                return slot.Value;
            }
        }

        return ComponentName.empty;
    }

    void DefineComponentOptions()
    {
        frameComponentOptions = new Dictionary<ComponentName, GameObject>
        {
             { ComponentName.lightFrame, lightFrame},
             { ComponentName.mediumFrame, mediumFrame},
             { ComponentName.heavyFrame, heavyFrame},
             { ComponentName.empty , null }
        };

        engineComponentOptions = new Dictionary<ComponentName, GameObject>
        {
             { ComponentName.engine, engine },
             { ComponentName.jetEngine, jetEngine},
             { ComponentName.empty , null }
        };

        extraTopComponentOptions = new Dictionary<ComponentName, GameObject>
        {
             { ComponentName.boostGulp, boostGulp },
             { ComponentName.machineGun, machineGun},
             { ComponentName.missile, missile},
             { ComponentName.empty , null }
        };

        extraComponentOptions = new Dictionary<ComponentName, GameObject>
        {
             { ComponentName.fuelTank, fuelTank },
             { ComponentName.aireon, aireon},
             { ComponentName.empty , null }
        };
    }

    public Dictionary<ComponentName, GameObject> GetFrameComponentOptions()
    {
        return frameComponentOptions;
    }

    public Dictionary<ComponentName, GameObject> GetEngineComponentOptions()
    {
        return engineComponentOptions;
    }

    public Dictionary<ComponentName, GameObject> GetExtraTopComponentOptions()
    {
        return extraTopComponentOptions;
    }

    public Dictionary<ComponentName, GameObject> GetExtraComponentOptions()
    {
        return extraComponentOptions;
    }


}

