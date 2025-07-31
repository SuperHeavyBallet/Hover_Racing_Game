using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static SceneStartup;

/// <summary>
/// This Script takes the components set in the Main Menu and builds the ship from that list, or falls back to default build on direct scene loading
/// </summary>

public class Ship_Constructor : MonoBehaviour
{

    Ship_Passport shipPassport;
    Ship_Movement SCRIPT_ShipMovement;

    Dictionary<ComponentSlotType, ComponentName> componentSlots = new();
    
    GameObject chosenFrame;
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

   

   

    void Awake()
    {
        SCRIPT_ShipMovement = this.GetComponent<Ship_Movement>();
        
    }

    private void Start()
    {
        CheckInstance();
        PlaceFrame();
       PlaceComponent();
    }


    void CheckInstance()
    {
        
        shipPassport = Ship_Passport.Instance;
 
        if (shipPassport != null)
        {
            if (shipPassport.GetShipLoadout() != null)
            {
                componentSlots = shipPassport.GetShipLoadout();
            }
            else
            {
                componentSlots = shipPassport.CreateDefaultLoadout();
            }
        }
        else
        {
            Debug.LogWarning("Ship Passport not found or loadout not received. Using default backup loadout.");
            componentSlots = shipPassport.CreateDefaultLoadout();
        }
    }

    /*
    Dictionary<ComponentSlotType, ComponentName> CreateDefaultLoadout()
    {
        return new Dictionary<ComponentSlotType, ComponentName>
    {
        { ComponentSlotType.Frame, ComponentName.mediumFrame },
        { ComponentSlotType.FrontLeft, ComponentName.jetEngine },
        { ComponentSlotType.FrontRight, ComponentName.jetEngine },
        { ComponentSlotType.BackLeft, ComponentName.jetEngine },
        { ComponentSlotType.BackRight,ComponentName.jetEngine },
        { ComponentSlotType.BackLeft1, ComponentName.empty },
        { ComponentSlotType.BackRight1, ComponentName.empty },
        { ComponentSlotType.ExtraFront, ComponentName.boostGulp },
        { ComponentSlotType.ExtraLeft, ComponentName.jetEngine },
        { ComponentSlotType.ExtraRight, ComponentName.jetEngine }
    };
    }*/
    /*
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
    }*/
    /*
    public GameObject GetFrameReference()
    {
        if (chosenFrame != null) return chosenFrame;
        else return null;
    }

    void SetFrameReference(GameObject frame)
    {
        chosenFrame = frame;
    }*/

    
    void PlaceFrame()
    {
        Transform position = null;
        GameObject newComponent = null;

        foreach (var pair in componentSlots)
        {
            if (pair.Key == ComponentSlotType.Frame)
            {  
                position = framePosition;

                switch (pair.Value)
                {
                    case ComponentName.lightFrame:
                        newComponent = shipPassport.lightFrame;  break;
                    case ComponentName.mediumFrame:
                        newComponent = shipPassport.mediumFrame; break;
                    case ComponentName.heavyFrame: 
                        newComponent = shipPassport.heavyFrame; break;
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

                    //SetFrameReference(chosenComponent);
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

            newComponent = shipPassport.GetPrefab(pair.Value);
            
    
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
