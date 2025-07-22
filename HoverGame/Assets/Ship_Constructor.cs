using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static SceneStartup;

/// <summary>
/// This Script takes the components set in the Main Menu and builds the ship from that list
/// </summary>

public class Ship_Constructor : MonoBehaviour
{
    SceneStartup sceneStartup;
    Ship_Passport shipPassport;

    Dictionary<ComponentSlotType, ComponentName> componentSlots = new();
    Dictionary<ComponentSlotType, ComponentName> BACKUP_componentSlots = new();

    public Transform framePosition;

    Transform frontLeftPosition;
    Transform frontRightPosition;
    Transform backLeftPosition;
    Transform backRightPosition;
    Transform extraFrontPosition;
    Transform extraLeftPosition;
    Transform extraRightPosition;

    Transform backLeft1Position;
    Transform backRight1Position;

    Ship_Movement SCRIPT_ShipMovement;

    public GameObject lightFramePrefab;
    public GameObject mediumFramePrefab;
    public GameObject heavyFramePrefab;
    public GameObject enginePrefab;
    public GameObject jetEnginePrefab;
    public GameObject aireonPrefab;
    public GameObject fuelTankPrefab;
    public GameObject boostGulpPrefab;

    void Awake()
    {
        SCRIPT_ShipMovement = this.GetComponent<Ship_Movement>();
        CheckInstance();
        PlaceFrame();
        PlaceComponent();
    }


    void CheckInstance()
    {
        //sceneStartup = SceneStartup.Instance;
        shipPassport = Ship_Passport.Instance;

        if (shipPassport != null)
        {
            componentSlots = shipPassport.GetShipLoadout();
        }
        else
        {
            Debug.LogWarning("Ship Passport not found or loadout not received. Using default backup loadout.");

            componentSlots = CreateDefaultLoadout();

        
        }
    }

    Dictionary<ComponentSlotType, ComponentName> CreateDefaultLoadout()
    {
        return new Dictionary<ComponentSlotType, ComponentName>
    {
        { ComponentSlotType.Frame, ComponentName.mediumFrame },
        { ComponentSlotType.FrontLeft, ComponentName.engine },
        { ComponentSlotType.FrontRight, ComponentName.engine },
        { ComponentSlotType.BackLeft, ComponentName.engine },
        { ComponentSlotType.BackRight,ComponentName.engine },
        { ComponentSlotType.BackLeft1, ComponentName.empty },
        { ComponentSlotType.BackRight1, ComponentName.empty },
        { ComponentSlotType.ExtraFront, ComponentName.boostGulp },
        { ComponentSlotType.ExtraLeft, ComponentName.fuelTank },
        { ComponentSlotType.ExtraRight, ComponentName.fuelTank }
    };
    }

    public List<ComponentName> GetShipLoadout()
    {
        bool hasExtraSlots = false;

        List<ComponentName> componentList = new List<ComponentName>();

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
                if(hasExtraSlots)
                {
                    componentList.Add(pair.Value);
                }
            }
            else
            {
                componentList.Add(pair.Value);
            }

        }
        return componentList;
    }


    void PlaceFrame()
    {
        Transform position = null;
        GameObject newComponent = null;

        foreach (var pair in componentSlots)
        {
            if(pair.Key == ComponentSlotType.Frame)
            {
                position = framePosition;

                switch (pair.Value)
                {
                    case ComponentName.lightFrame:
                        newComponent = lightFramePrefab;  break;
                    case ComponentName.mediumFrame:
                        newComponent = mediumFramePrefab; break;
                    case ComponentName.heavyFrame: 
                        newComponent = heavyFramePrefab; break;
                    default:
                        Debug.LogWarning("No Frame Type Assigned");
                        break;
                }

                if (newComponent != null && position != null)
                {
                    GameObject chosenComponent = Instantiate(newComponent, position);
                    chosenComponent.gameObject.SetActive(true);

                    Frame_Layout frameLayout = chosenComponent.GetComponent<Frame_Layout>();

                    if(frameLayout != null)
                    {
                        frontLeftPosition = frameLayout.GetFrontLeftPosition();
                        frontRightPosition = frameLayout.GetFrontRightPosition();
                        backLeftPosition = frameLayout.GetBackLeftPosition();
                        backRightPosition = frameLayout.GetBackRightPosition();
                        backLeft1Position = frameLayout.GetBackLeft1Position();
                        backRight1Position = frameLayout.GetBackRight1Position();
                        extraFrontPosition = frameLayout.GetExtraFrontPosition();
                        extraLeftPosition = frameLayout.GetExtraLeftPosition();
                        extraRightPosition = frameLayout.GetExtraRightPosition();
                    }
                }
                break;
            }
        }
    }

    void PlaceComponent()
    { 
        foreach(var pair in componentSlots)
        {
            Transform position = null;
            GameObject newComponent = null;

            switch (pair.Key)
            {
                case ComponentSlotType.FrontLeft: 
                    position = frontLeftPosition; break;
                case ComponentSlotType.FrontRight: 
                    position = frontRightPosition; break;
                case ComponentSlotType.BackLeft: 
                    position = backLeftPosition; break;
                case ComponentSlotType.BackRight: 
                    position = backRightPosition; break;
                case ComponentSlotType.BackLeft1: 
                    position = backLeft1Position; break;
                case ComponentSlotType.BackRight1: 
                    position = backRight1Position; break;
                case ComponentSlotType.ExtraFront:
                    position = extraFrontPosition; break;
                case ComponentSlotType.ExtraLeft:
                    position = extraLeftPosition; break;
                case ComponentSlotType.ExtraRight:
                    position = extraRightPosition; break;
                default:
                    Debug.LogWarning("No Slot Assigned");
                    break;
            }
            
            switch (pair.Value)
            {
                case ComponentName.engine:
                    newComponent = enginePrefab; break;
                case ComponentName.jetEngine:
                    newComponent = jetEnginePrefab; break;
                case ComponentName.aireon:
                    newComponent = aireonPrefab; break;
                case ComponentName.fuelTank:
                    newComponent = fuelTankPrefab; break;
                case ComponentName.boostGulp:
                    newComponent = boostGulpPrefab; break;
                default:
                    Debug.LogWarning("No Component Value Assigned");
                    break;
            }

            if (newComponent != null && position != null)
            {
                GameObject chosenComponent = Instantiate(newComponent, position);
                chosenComponent.gameObject.SetActive(true);

                EngineController engineController = chosenComponent.GetComponent<EngineController>();
                if(engineController != null )
                {
                    SCRIPT_ShipMovement.RegisterEngineFireListener(engineController);
                }
            }
            else
{
    Debug.LogWarning($"Component {pair.Value} not placed. Position is {(position == null ? "null" : "OK")}");
}
        }
    }

  
}
