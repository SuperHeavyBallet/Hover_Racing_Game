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

    Dictionary<ComponentSlotPosition, SlotState> componentSlots = new();

    //public GameObject TEST_LightFame;
    //public GameObject TEST_MedFame;
    //public GameObject TEST_HevFame;

    // Positions to place components around the frame
    [SerializeField] private Transform framePosition;
    [SerializeField] private Transform frontLeftPosition;
    [SerializeField] private Transform frontRightPosition;
    [SerializeField] private Transform backLeftPosition;
    [SerializeField] private Transform backRightPosition;
    [SerializeField] private Transform extraFrontPosition;
    Transform extraLeftPosition;
    [SerializeField] private Transform extraRightPosition;

    [SerializeField] private Transform backLeft1Position;
    [SerializeField] private Transform backRight1Position;

    [SerializeField] private ComponentCatalogue componentCatalogue;

    void Awake()
    {
        SCRIPT_ShipMovement = this.GetComponent<Ship_Movement>(); 
        
    }

    private void Start()
    {
        if (!componentCatalogue)
        {
            Debug.LogError("ComponentCatalogue not assigned on Ship_Constructor.");
            return;
        }
        componentCatalogue.EnsureBuilt();

        CheckShipPassportExists();


        DecideFrame();
        PlaceComponents();
        
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
 
        }

        foreach (var slot in componentSlots)
        {
            Debug.Log(slot.Key +" ; "+ slot.Value);
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

                var def = componentCatalogue.GetById(pair.Value.selectedId);

                if (def != null && def.prefab != null)
                {
                    newComponent = def.prefab;
                }
                else
                {
                    Debug.LogWarning($"Frame id '{pair.Value.selectedId}' not found or has no prefab assigned.");
                    newComponent = null;
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
    
    void PlaceComponents()
    { 

        foreach(var pair in componentSlots)
        {

            

            if (pair.Key == ComponentSlotPosition.Frame) continue; // Don't place frame

            var def = componentCatalogue.GetById(pair.Value.selectedId);
            if (def == null)
            {
                Debug.LogWarning($"Unknown component id '{pair.Value.selectedId}' in slot {pair.Key}");
                continue;
            }

            var prefab = def.prefab;
            if (!prefab)
            {
                Debug.LogWarning($"Component '{pair.Value.selectedId}' has no prefab assigned.");
                continue;
            }

            Transform position = null;
            GameObject newComponent = null;

            int localScaleX = 1;

            switch (pair.Key)
            {
                case ComponentSlotPosition.FrontLeft:
                    localScaleX = -1;
                    position = frontLeftPosition; break;
                case ComponentSlotPosition.FrontRight:
                    localScaleX = 1;
                    position = frontRightPosition; break;
                case ComponentSlotPosition.BackLeft:
                    localScaleX = -1;
                    position = backLeftPosition; break;
                case ComponentSlotPosition.BackRight:
                    localScaleX = 1;
                    position = backRightPosition; break;
                case ComponentSlotPosition.BackLeft1:
                    localScaleX = -1;
                    position = backLeft1Position; break;
                case ComponentSlotPosition.BackRight1:
                    localScaleX = 1;
                    position = backRight1Position; break;
                case ComponentSlotPosition.ExtraTop:
                    localScaleX = 1;
                    position = extraFrontPosition; break;
                case ComponentSlotPosition.ExtraLeft:
                    localScaleX = -1;
                    position = extraLeftPosition; break;
                case ComponentSlotPosition.ExtraRight:
                    localScaleX = 1;
                    position = extraRightPosition; break;
                default:
                    localScaleX = 1;
                    Debug.LogWarning("No Slot Assigned: " + pair.Key + " : " + pair.Value);
                    break;
            }

            newComponent = componentCatalogue.GetById(pair.Value.selectedId).prefab;
            
    
            if (newComponent != null && position != null)
            {
                GameObject chosenComponent = Instantiate(newComponent, position);
                chosenComponent.gameObject.SetActive(true);

                EngineController engineController = chosenComponent.GetComponent<EngineController>();
                if(engineController != null )
                {
                    SCRIPT_ShipMovement.RegisterEngineFireListener(engineController, localScaleX);
                }
            }
            else
            {
                Debug.LogWarning($"Component {pair.Value} not placed. Position is {(position == null ? "null" : "OK")}");
            }
        }
    }

  
}
