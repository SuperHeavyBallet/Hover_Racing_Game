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

    [Header("Component Catalogue")]
    public ComponentCatalogue componentCatalogue; // assign the asset in the inspector

    [Header("Unlocked Components")] // Assign here what the player will have access too
    public ComponentDefinition[] allUnlockedComponents;

    private List<string> unlockedComponent_IDs = new List<string>();

    // The ship build, by IDs:
    public Dictionary<ComponentSlotPosition, SlotState> componentSlots  = new();
    List<ComponentDefinition> allComponentsList = new List<ComponentDefinition>();
   /*List<string> idOfUnlockedComponents = new List<string> {
        "FRAME_LIGHT",
        "FRAME_MEDIUM",
        "FRAME_HEAVY",
        "ENGINE_DIESEL",
        "ENGINE_JET",
        "FUEL_TANK",
        "BOOST_GULP",
        "MACHINE_GUN"
    };*/

    public GameObject mediumFrame;
    
    List<ComponentDefinition> UnlockedComponents = new();
    //public List<ComponentDefinition> allComponents = new();

    public Dictionary<ComponentName, GameObject> componentPrefabs;


    Dictionary<ComponentSlotPosition, SlotState> defaultLoadout = new Dictionary<ComponentSlotPosition, SlotState>
        {
            { ComponentSlotPosition.Frame, new SlotState{ position = null, optionsById = null, selectedId = "FRAME_MEDIUM" } },

            { ComponentSlotPosition.FrontLeft, new SlotState{ position = null, optionsById = null, selectedId = "ENGINE_JET"  }   },
            { ComponentSlotPosition.FrontRight,new SlotState{ position = null, optionsById = null, selectedId = "ENGINE_JET"  }    },
            { ComponentSlotPosition.BackLeft,  new SlotState{ position = null, optionsById = null, selectedId = "ENGINE_JET"  }   },
            { ComponentSlotPosition.BackRight, new SlotState{ position = null, optionsById = null, selectedId = "ENGINE_JET"  }   },
            { ComponentSlotPosition.BackLeft1, new SlotState{ position = null, optionsById = null, selectedId = "EMPTY"  }         },
            { ComponentSlotPosition.BackRight1, new SlotState{ position = null, optionsById = null, selectedId = "EMPTY"  }         },
            { ComponentSlotPosition.ExtraTop, new SlotState{ position = null, optionsById = null, selectedId = "BOOST_GULP" } },
            { ComponentSlotPosition.ExtraLeft,  new SlotState{ position = null, optionsById = null, selectedId = "EMPTY"  }},
            { ComponentSlotPosition.ExtraRight, new SlotState{ position = null, optionsById = null, selectedId = "EMPTY"  }}
        };

 



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

        BuildUnlockedList();

        BuildLocalCatalogue();

    }
    
    public GameObject GetShipFrame()
    {
        GameObject chosenFrame = mediumFrame;

        if(chosenFrame != null)
        {
            return chosenFrame;
        }
        else
        {
            return mediumFrame;
        }

    }

    void BuildUnlockedList()
    {
        foreach(ComponentDefinition component in allUnlockedComponents)
        {
            unlockedComponent_IDs.Add(component.id);
        }
    }

    void BuildLocalCatalogue()
    {
        allComponentsList.Clear();
        allComponentsList = componentCatalogue.GetListOfAllComponents();

        UpdatePlayerComponentUnlockList();
    }

    void UpdatePlayerComponentUnlockList()
    {
        foreach(var component in allComponentsList)
        {
            if(unlockedComponent_IDs.Contains(component.id))
            {
               UnlockedComponents.Add(component);
            }
        }

        foreach(var component in UnlockedComponents)
        {
           Debug.Log("UNLOCKED: " + component);
        }
    }


    public List<ComponentDefinition> GetUnlockedComponents()
    {
        return UnlockedComponents;
    }
    

    public void SetShipLoadout(Dictionary<ComponentSlotPosition, SlotState> shipLoadout )
    {
        componentSlots = shipLoadout;
    }

  
    public bool CheckBoostGulpPresent()
    {
        foreach(var component in componentSlots)
        {
            if(component.Value.selectedId == "BOOST_GULP")
            {
                return true;
            }
        }

        return false;
    }

    public Dictionary<ComponentSlotPosition, SlotState> GetShipLoadout()
    {
        if (componentSlots.Count > 0)
        {
            return new(componentSlots);
        }
        else
        {
            componentSlots = defaultLoadout;
            return defaultLoadout;
        }
        
    }

    public Dictionary<ComponentSlotPosition, SlotState> CreateDefaultLoadout()
    {
        return defaultLoadout;
    }

    public string GetWeaponType()
    {
        foreach (var component in componentSlots)
        {
            if(component.Key == ComponentSlotPosition.ExtraTop)
            {
                if(component.Value.selectedId == "MACHINE_GUN" || component.Value.selectedId == "MISSILE_LAUNCHER")
                {
                    return component.Value.selectedId;
                }
            }
        }

        return "EMPTY";
    }

}

