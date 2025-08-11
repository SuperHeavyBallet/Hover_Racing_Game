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

    [Header("References")]
    public ComponentCatalogue componentCatalogue; // assign the asset in the inspector

    // The ship build, by IDs:
    public Dictionary<ComponentSlotPosition, string> componentSlots  = new();

    public GameObject mediumFrame;
    
    public bool receivedShipLoadout = false;

    public Dictionary<ComponentName, GameObject> componentPrefabs;


    public GameObject engine;
    public GameObject jetEngine;
    public GameObject aireon;
    public GameObject lightFrame;
    
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
       // DefineComponentOptions();
    }
    
    public GameObject GetShipFrame()
    {
        GameObject chosenFrame = mediumFrame;

        if(chosenFrame != null)
        {
            return chosenFrame;
        }

        return null;

    }
    
    void InitialisePrefabReferences()
    {
        componentPrefabs.Add(ComponentName.LIGHT_FRAME, lightFrame);
        componentPrefabs.Add(ComponentName.MEDIUM_FRAME, mediumFrame);
        componentPrefabs.Add(ComponentName.HEAVY_FRAME , heavyFrame);
        componentPrefabs.Add(ComponentName.ENGINE, engine);
        componentPrefabs.Add(ComponentName.JET_ENGINE, jetEngine);
        componentPrefabs.Add(ComponentName.AIREON, aireon);
        componentPrefabs.Add(ComponentName.FUEL_TANK, fuelTank);
        componentPrefabs.Add(ComponentName.BOOST_GULP, boostGulp);
        componentPrefabs.Add(ComponentName.MACHINE_GUN, machineGun);
        componentPrefabs.Add(ComponentName.MISSILE_LAUNCHER, missile);
    }
    /*
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
    }*/


    public void SetShipLoadout(Dictionary<ComponentSlotPosition, string> shipLoadout )
    {
        componentSlots = shipLoadout ?? new();

        /*
        receivedShipLoadout = true;

        componentSlots.Clear();

        componentSlots = shipLoadout;*/


    }

  
    public bool CheckBoostGulpPresent()
    {
        foreach(var component in componentSlots)
        {
            if(component.Value == "BOOST_GULP")
            {
                return true;
            }
        }

        return false;
    }
  

    public Dictionary<ComponentSlotPosition, string> GetShipLoadout()
    {
        if (componentSlots.Count > 0) return new(componentSlots);


        // Default loadout now also by IDs (use actual IDs from your catalogue)
        var def = new Dictionary<ComponentSlotPosition, string>
        {
            { ComponentSlotPosition.Frame,     "FRAME_MEDIUM" },
            { ComponentSlotPosition.FrontLeft, "ENGINE_JET"   },
            { ComponentSlotPosition.FrontRight,"ENGINE_JET"   },
            { ComponentSlotPosition.BackLeft,  "ENGINE_JET"   },
            { ComponentSlotPosition.BackRight, "ENGINE_JET"   },
            { ComponentSlotPosition.BackLeft1, "EMPTY"        },
            { ComponentSlotPosition.BackRight1,"EMPTY"        },
            { ComponentSlotPosition.ExtraTop,  "BOOST_GULP"   },
            { ComponentSlotPosition.ExtraLeft, "FUEL_TANK"         },
            { ComponentSlotPosition.ExtraRight,"FUEL_TANK"         },
        };
        componentSlots = def;
        return def;
    }

    /*
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

    */

    public Dictionary<ComponentSlotPosition, string> CreateDefaultLoadout()
    {
        var defaultLoadout = new Dictionary<ComponentSlotPosition, string>
    {
            { ComponentSlotPosition.Frame,     "FRAME_MEDIUM" },
            { ComponentSlotPosition.FrontLeft, "ENGINE_JET"   },
            { ComponentSlotPosition.FrontRight,"ENGINE_JET"   },
            { ComponentSlotPosition.BackLeft,  "ENGINE_JET"   },
            { ComponentSlotPosition.BackRight, "ENGINE_JET"   },
            { ComponentSlotPosition.BackLeft1, "EMPTY"        },
            { ComponentSlotPosition.BackRight1,"EMPTY"        },
            { ComponentSlotPosition.ExtraTop,  "BOOST_GULP"   },
            { ComponentSlotPosition.ExtraLeft, "FUEL_TANK"         },
            { ComponentSlotPosition.ExtraRight,"FUEL_TANK"        },
    };

        return defaultLoadout;
    }

    

    public string GetWeaponType()
    {


        string chosenWeapon = "BOOST_GULP";

        if (chosenWeapon != null)
        {
            return chosenWeapon;
        }

        return "EMPTY";
    }

    /*
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

*/
}

