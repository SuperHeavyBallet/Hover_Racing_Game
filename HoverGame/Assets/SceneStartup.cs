using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// This Script is where the ship components are decided upon in the main menu
/// </summary>
/// 
public class SceneStartup : MonoBehaviour
{

    //public static SceneStartup Instance { get; private set; }

    [SerializeField]
    private string selectedVehicleClass = "light";
    public Vector3 spawnPosition;
    public float gameDifficulty;

    public ShipClass shipClass;

    public TextMeshProUGUI frameText;
    public TextMeshProUGUI fLText;
    public TextMeshProUGUI fRText;
    public TextMeshProUGUI bLText;
    public TextMeshProUGUI bRText;
    public TextMeshProUGUI bL1Text;
    public TextMeshProUGUI bR1Text;
    public TextMeshProUGUI extraLeftText;
    public TextMeshProUGUI extraRightText;

    public GameObject engine;
    public GameObject jetEngine;
    public GameObject aireon;
    public GameObject lightFrame;
    public GameObject mediumFrame;
    public GameObject heavyFrame;
    public GameObject fuelTank;


    public Transform framePosition;
    public Transform fLPosition;
    public Transform fRPosition;
    public Transform bLPosition;
    public Transform bRPosition;
    public Transform bL1Position;
    public Transform bR1Position;
    public Transform extraLeftPosition;
    public Transform extraRightPosition;

    Ship_Passport shipPassport;

    
    public enum ComponentSlotType
    {
        Frame,
        FrontLeft,
        FrontRight,
        BackLeft,
        BackRight,
        BackLeft1,
        BackRight1,
        ExtraLeft,
        ExtraRight
    }

    public Dictionary<ComponentSlotType, ComponentSlot> componentSlots = new();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {

        
        /*
        // Singleton enforcement
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Keep across scene loads
        */

        InitializeComponentSlots();

        GenerateShip();

        DisplayComponentMeshes();

        
    }

    private void Start()
    {
        shipPassport = GameObject.Find("ShipPassport").GetComponent<Ship_Passport>();
        
    }

    public void GenerateShip()
    {
        SetComponentSlot(ComponentSlotType.Frame, "medium");
        SetComponentSlot(ComponentSlotType.FrontLeft, "engine");
        SetComponentSlot(ComponentSlotType.FrontRight, "engine");
        SetComponentSlot(ComponentSlotType.BackLeft, "engine");
        SetComponentSlot(ComponentSlotType.BackRight, "engine");
        SetComponentSlot(ComponentSlotType.BackLeft1, "empty");
        SetComponentSlot(ComponentSlotType.BackRight1, "empty");
        SetComponentSlot(ComponentSlotType.ExtraLeft, "fuelTank");
        SetComponentSlot(ComponentSlotType.ExtraRight, "fuelTank");
    }

    void DisplayComponentMeshes()
    {
        foreach (var pair in componentSlots)
        {
            string selectedKey = pair.Value.selectedComponentKey;

            if (string.IsNullOrEmpty(selectedKey) || selectedKey == "empty") continue;

            if (pair.Value.components.TryGetValue(selectedKey, out GameObject prefab))
            {
                GameObject newComponent = Instantiate(prefab, pair.Value.position);
                newComponent.gameObject.SetActive(true);
            }
        }
    }

    public void UpdateComponent_Frame(string newComponentName)
    {

        if (componentSlots.TryGetValue(ComponentSlotType.Frame, out var frameSlot))
        {

            if (frameSlot.selectedComponentKey != newComponentName)
            {
                foreach (Transform child in frameSlot.position)
                {
                    Destroy(child.gameObject);
                }

                SetComponentSlot(ComponentSlotType.Frame, newComponentName);

                DisplayComponentMeshes();
            }

        }




    }

    string GetComponentName(int val)
    {
        string replacementComponent = "empty";

        switch (val)
        {
            case 0:
                replacementComponent = "engine";
                break;
            case 1:
                replacementComponent = "jetEngine";
                break;
            case 2:
                replacementComponent = "aireon";
                break;
            case 3: replacementComponent = "fuelTank";
                break;
            default:
                replacementComponent = "empty";
                break;
        }

        return replacementComponent;
    }

    void UpdateComponent(ComponentSlotType slotType, string replacementComponent)
    {
        if (componentSlots.TryGetValue(slotType, out var slotPosition))
        {
            if (slotPosition.selectedComponentKey != replacementComponent)
            {
                foreach (Transform child in slotPosition.position)
                {
                    Destroy(child.gameObject);
                }

                SetComponentSlot(slotType, replacementComponent);
                DisplayComponentMeshes();
            }
        }
    }
    public void UpdateComponentSlot_FL(int val)
    {
        string replacementComponent = GetComponentName(val);

        UpdateComponent(ComponentSlotType.FrontLeft, replacementComponent);
    }

    public void UpdateComponentSlot_FR(int val)
    {
        string replacementComponent = GetComponentName(val);

        UpdateComponent(ComponentSlotType.FrontRight, replacementComponent);
    }

    public void UpdateComponentSlot_BL(int val)
    {
        string replacementComponent = GetComponentName(val);

        UpdateComponent(ComponentSlotType.BackLeft, replacementComponent);
    }

    public void UpdateComponentSlot_BR(int val)
    {
        string replacementComponent = GetComponentName(val);

        UpdateComponent(ComponentSlotType.BackRight, replacementComponent);
    }

    public void UpdateComponentSlot_BL1(int val)
    {
        string replacementComponent = GetComponentName(val);

        UpdateComponent(ComponentSlotType.BackLeft1, replacementComponent);
    }

    public void UpdateComponentSlot_BR1(int val)
    {
        string replacementComponent = GetComponentName(val);

        UpdateComponent(ComponentSlotType.BackRight1, replacementComponent);
    }

    public void UpdateComponentSlot_ExtraLeft(int val)
    {
        string replacementComponent = GetComponentName(val);

        UpdateComponent(ComponentSlotType.ExtraLeft, replacementComponent);
    }

    public void UpdateComponentSlot_ExtraRight(int val)
    {
        string replacementComponent = GetComponentName(val);

        UpdateComponent(ComponentSlotType.ExtraRight, replacementComponent);
    }

    void InitializeComponentSlots()
    {
        componentSlots[ComponentSlotType.Frame] = new ComponentSlot
        {
            label = frameText,
            position = framePosition,
            components = new Dictionary<string, GameObject>
            {
                { "light", lightFrame },
                { "medium", mediumFrame },
                { "heavy" , heavyFrame },
                { "empty", aireon }
            }
        };

        componentSlots[ComponentSlotType.FrontLeft] = new ComponentSlot
        {
            label = fLText,
            position = fLPosition,
            components = new Dictionary<string, GameObject>
            {
                { "engine", engine },
                { "jetEngine", jetEngine },
                { "aireon" , aireon },
                { "empty", aireon }
            }
        };

        componentSlots[ComponentSlotType.FrontRight] = new ComponentSlot
        {
            label = fRText,
            position = fRPosition,
            components = new Dictionary<string, GameObject>
            {
                { "engine", engine },
                { "jetEngine", jetEngine },
                { "aireon" , aireon },
                { "empty", aireon }
            }
        };

        componentSlots[ComponentSlotType.BackLeft] = new ComponentSlot
        {
            label = bLText,
            position = bLPosition,
            components = new Dictionary<string, GameObject>
            {
                { "engine", engine },
                { "jetEngine", jetEngine },
                { "aireon" , aireon },
                { "empty", aireon }
            }
        };

        componentSlots[ComponentSlotType.BackRight] = new ComponentSlot
        {
            label = bRText,
            position = bRPosition,
            components = new Dictionary<string, GameObject>
            {
                { "engine", engine },
                { "jetEngine", jetEngine },
                { "aireon" , aireon },
                { "empty", aireon }
            }
        };

        componentSlots[ComponentSlotType.BackLeft1] = new ComponentSlot
        {
            label = bL1Text,
            position = bL1Position,
            components = new Dictionary<string, GameObject>
            {
                { "engine", engine },
                { "jetEngine", jetEngine },
                { "aireon" , aireon },
                { "empty", aireon }
            }
        };

        componentSlots[ComponentSlotType.BackRight1] = new ComponentSlot
        {
            label = bR1Text,
            position = bR1Position,
            components = new Dictionary<string, GameObject>
            {
                { "engine", engine },
                { "jetEngine", jetEngine },
                { "aireon" , aireon },
                { "empty", aireon }
            }
        };

        componentSlots[ComponentSlotType.ExtraLeft] = new ComponentSlot
        {
            label = extraLeftText,
            position = extraLeftPosition,
            components = new Dictionary<string, GameObject>
            {
                { "fuelTank", fuelTank },
                { "engine", engine },
                { "jetEngine", jetEngine },
                { "empty", aireon }
            }
        };

        componentSlots[ComponentSlotType.ExtraRight] = new ComponentSlot
        {
            label = extraRightText,
            position = extraRightPosition,
            components = new Dictionary<string, GameObject>
            {
                { "fuelTank", fuelTank },
                { "engine", engine },
                { "jetEngine", jetEngine },
                { "empty", aireon }
            }
        };

    }




    void SetComponentSlot(ComponentSlotType slot, string componentKey)
    {
        if (!componentSlots.TryGetValue(slot, out var slotData)) return;

        slotData.label.text = componentKey;
        slotData.selectedComponentKey = componentKey;

        foreach (var kvp in slotData.components)
        {
            kvp.Value.SetActive(kvp.Key == componentKey);
        }

    }

    /*
    // Update is called once per frame
    void Update()
    {

    }

    public void SetVehicleClass(string vehicleClass)
    {

        selectedVehicleClass = vehicleClass;
        //Debug.Log("Scene Startup: " + vehicleClass);
    }

    public string GetVehicleClass()
    {
        //Debug.Log("Get Vehicle: " + selectedVehicleClass);
        return "light";
    }

    public Dictionary<ComponentSlotType, string> GetShipLoadout()
    {
        var result = new Dictionary<ComponentSlotType, string>();

        foreach (var pair in componentSlots)
        {
            result[pair.Key] = pair.Value.selectedComponentKey;

            
        }



        return result;

    }

    */

    public void SetShipLoadout()
    {
        var shipLoadout = new Dictionary<ComponentSlotType, string>();

        foreach (var pair in componentSlots)
        {
            shipLoadout[pair.Key] = pair.Value.selectedComponentKey;
        }

        shipPassport.ReceiveShipLoadout(shipLoadout);

        
    }



}
