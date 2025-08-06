using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static SceneStartup;

/// <summary>
/// This Script takes the components set in the Main Menu and builds the ship from that list in level, or falls back to default build on direct scene loading
/// </summary>

public class Ship_Constructor : MonoBehaviour
{

    Ship_Passport shipPassport;
    Ship_Movement SCRIPT_ShipMovement;

    Dictionary<ComponentSlotPosition, ComponentName> componentSlots = new();

    // Positions to place components around the frame
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
        CheckShipPassportExists();
        DecideFrame();
        PlaceComponent();
    }


    void CheckShipPassportExists()
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


    void DecideFrame()
    {
        Transform position = null;
        GameObject newComponent = null;

        foreach (var pair in componentSlots)
        {
            if (pair.Key == ComponentSlotPosition.Frame)
            {
                position = framePosition;

                switch (pair.Value)
                {
                    case ComponentName.lightFrame:
                        newComponent = shipPassport.lightFrame; break;
                    case ComponentName.mediumFrame:
                        newComponent = shipPassport.mediumFrame; break;
                    case ComponentName.heavyFrame:
                        newComponent = shipPassport.heavyFrame; break;
                    default:
                        Debug.LogWarning("No Frame Type Assigned");
                        break;
                }
            }
        }

        PlaceFrame(position, newComponent);

    }

    void PlaceFrame(Transform position, GameObject newComponent)
    {

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
                extraFrontPosition = frameLayout.GetExtraTopPosition();
                extraLeftPosition = frameLayout.GetExtraLeftPosition();
                extraRightPosition = frameLayout.GetExtraRightPosition();
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
                case ComponentSlotPosition.FrontLeft: 
                    position = frontLeftPosition; break;
                case ComponentSlotPosition.FrontRight: 
                    position = frontRightPosition; break;
                case ComponentSlotPosition.BackLeft: 
                    position = backLeftPosition; break;
                case ComponentSlotPosition.BackRight: 
                    position = backRightPosition; break;
                case ComponentSlotPosition.BackLeft1: 
                    position = backLeft1Position; break;
                case ComponentSlotPosition.BackRight1: 
                    position = backRight1Position; break;
                case ComponentSlotPosition.ExtraTop:
                    position = extraFrontPosition; break;
                case ComponentSlotPosition.ExtraLeft:
                    position = extraLeftPosition; break;
                case ComponentSlotPosition.ExtraRight:
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
