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
            frame = component.Value;
            /*
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
            }*/
            return frame;
        }

        return null;

    }

    void InitialisePrefabReferences()
    {
        componentPrefabs.Add(ComponentName.Light_Frame, lightFrame);
        componentPrefabs.Add(ComponentName.Medium_Frame, mediumFrame);
        componentPrefabs.Add(ComponentName.Heavy_Frame, heavyFrame);
        componentPrefabs.Add(ComponentName.Engine, engine);
        componentPrefabs.Add(ComponentName.Jet_Engine, jetEngine);
        componentPrefabs.Add(ComponentName.Aireon, aireon);
        componentPrefabs.Add(ComponentName.Fuel_Tank, fuelTank);
        componentPrefabs.Add(ComponentName.Boost_Gulp, boostGulp);
        componentPrefabs.Add(ComponentName.Machine_Gun, machineGun);
        componentPrefabs.Add(ComponentName.Missile, missile);
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
            if(component.Value == ComponentName.Boost_Gulp)
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
                    if (pair.Value == ComponentName.Heavy_Frame)
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
        { ComponentSlotPosition.Frame, ComponentName.Medium_Frame },
        { ComponentSlotPosition.FrontLeft, ComponentName.Jet_Engine },
        { ComponentSlotPosition.FrontRight, ComponentName.Jet_Engine },
        { ComponentSlotPosition.BackLeft, ComponentName.Jet_Engine },
        { ComponentSlotPosition.BackRight, ComponentName.Jet_Engine },
        { ComponentSlotPosition.BackLeft1, ComponentName.Empty },
        { ComponentSlotPosition.BackRight1, ComponentName.Empty },
        { ComponentSlotPosition.ExtraTop, ComponentName.Boost_Gulp },
        { ComponentSlotPosition.ExtraLeft, ComponentName.Empty },
        { ComponentSlotPosition.ExtraRight, ComponentName.Empty }
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

        return ComponentName.Empty;
    }

    void DefineComponentOptions()
    {
        frameComponentOptions = new Dictionary<ComponentName, GameObject>
        {
             { ComponentName.Light_Frame, lightFrame},
             { ComponentName.Medium_Frame, mediumFrame},
             { ComponentName.Heavy_Frame, heavyFrame},
             { ComponentName.Empty , null }
        };

        engineComponentOptions = new Dictionary<ComponentName, GameObject>
        {
             { ComponentName.Engine, engine },
             { ComponentName.Jet_Engine, jetEngine},
             { ComponentName.Empty , null }
        };

        extraTopComponentOptions = new Dictionary<ComponentName, GameObject>
        {
             { ComponentName.Boost_Gulp, boostGulp },
             { ComponentName.Machine_Gun, machineGun},
             { ComponentName.Missile, missile},
             { ComponentName.Empty , null }
        };

        extraComponentOptions = new Dictionary<ComponentName, GameObject>
        {
             { ComponentName.Fuel_Tank, fuelTank },
             { ComponentName.Aireon, aireon},
             { ComponentName.Empty , null }
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

