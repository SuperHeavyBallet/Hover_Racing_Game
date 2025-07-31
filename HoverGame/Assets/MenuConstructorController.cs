using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuConstructorController : MonoBehaviour
{

    [Header("Text UI")]
    public TextMeshProUGUI frameText;
    public TextMeshProUGUI fLText;
    public TextMeshProUGUI fRText;
    public TextMeshProUGUI bLText;
    public TextMeshProUGUI bRText;
    public TextMeshProUGUI bL1Text;
    public TextMeshProUGUI bR1Text;
    public TextMeshProUGUI extraFrontText;
    public TextMeshProUGUI extraLeftText;
    public TextMeshProUGUI extraRightText;


    [Header("Object Positions")]
    public Transform framePosition;
    public Transform fLPosition;
    public Transform fRPosition;
    public Transform bLPosition;
    public Transform bRPosition;
    public Transform bL1Position;
    public Transform bR1Position;
    public Transform extraFrontPosition;
    public Transform extraLeftPosition;
    public Transform extraRightPosition;

    

    [Header("Dropdown Menus to Enable/Disable for Heavy Frames")]
    public GameObject backLeft1_Dropdown;
    public GameObject backRight1_Dropdown;
    public GameObject backLeft1Label;
    public GameObject backRight1Label;

    Ship_Passport shipPassport;

    public Dictionary<ComponentSlotType, ComponentSlot> componentSlots = new();
    public Dictionary<ComponentSlotType, ComponentName> persistentShipLoadout = new();

    void Awake()
    {
        shipPassport = GameObject.Find("ShipPassport").GetComponent<Ship_Passport>();
        CheckExistingShip();
    }

    public void CheckExistingShip()
    {
        persistentShipLoadout = shipPassport.GetShipLoadout();

        if (persistentShipLoadout != null)
        {
            GenerateExistingShip();
        }
        else
        {
            InitializeComponentSlots();
            GenerateNewShip();
        }

        DisplayComponentMeshes();
    }

    

    void InitializeComponentSlots()
    {
        SetComponentSlots(ComponentSlotType.Frame, frameText, framePosition);
        SetComponentSlots(ComponentSlotType.FrontLeft, fLText, fLPosition);
        SetComponentSlots(ComponentSlotType.FrontRight, fRText, fRPosition);
        SetComponentSlots(ComponentSlotType.BackLeft, bLText, bLPosition);
        SetComponentSlots(ComponentSlotType.BackRight, bRText, bRPosition);
        SetComponentSlots(ComponentSlotType.BackLeft1, bL1Text, bL1Position);
        SetComponentSlots(ComponentSlotType.BackRight1, bR1Text, bR1Position);
        SetComponentSlots(ComponentSlotType.ExtraFront, extraFrontText, extraFrontPosition);
        SetComponentSlots(ComponentSlotType.ExtraLeft, extraLeftText, extraLeftPosition);
        SetComponentSlots(ComponentSlotType.ExtraRight, extraRightText, extraRightPosition);
    }

    public void GenerateExistingShip()
    {
        InitializeComponentSlots();

        foreach (var component in persistentShipLoadout)
        {
            if (component.Key == ComponentSlotType.Frame)
            {
                UpdateComponent_Frame(component.Value);
            }
            else
            {
                UpdateComponent(component.Key, component.Value);
            }
        }
    }

    public void GenerateNewShip()
    {
        SetComponentSlot(ComponentSlotType.Frame, ComponentName.mediumFrame);
        SetComponentSlot(ComponentSlotType.FrontLeft, ComponentName.engine);
        SetComponentSlot(ComponentSlotType.FrontRight, ComponentName.engine);
        SetComponentSlot(ComponentSlotType.BackLeft, ComponentName.engine);
        SetComponentSlot(ComponentSlotType.BackRight, ComponentName.engine);
        SetComponentSlot(ComponentSlotType.BackLeft1, ComponentName.empty);
        SetComponentSlot(ComponentSlotType.BackRight1, ComponentName.empty);
        SetComponentSlot(ComponentSlotType.ExtraFront, ComponentName.boostGulp);
        SetComponentSlot(ComponentSlotType.ExtraLeft, ComponentName.fuelTank);
        SetComponentSlot(ComponentSlotType.ExtraRight, ComponentName.fuelTank);
    }

    void DisplayComponentMeshes()
    {
        foreach (var pair in componentSlots)
        {
            CleanupExcessMeshes(pair.Value.position);
            InstantiateSelectedComponent(pair.Value);
        }
    }

    void InstantiateSelectedComponent(ComponentSlot slot)
    {
        var selectedKey = slot.selectedComponentKey;
        if (selectedKey == ComponentName.empty) return;

        if (slot.components.TryGetValue(selectedKey, out var prefab))
        {
            GameObject newComp = Instantiate(prefab, slot.position);
            newComp.SetActive(true);
        }
    }

   

    public void CheckHeavyFrame()
    {
        bool isHeavyFrame = false;

        foreach (var pair in componentSlots)
        {
            if (pair.Key == ComponentSlotType.Frame)
            {
                if (pair.Value.selectedComponentKey == ComponentName.heavyFrame)
                {
                    isHeavyFrame = true;
                    break;
                }
            }
        }

        SetOptionsForExtraSlots(isHeavyFrame);
    }

    ComponentName GetComponentName(int val)
    {
        ComponentName replacementComponent = ComponentName.empty;

        switch (val)
        {
            case 0: replacementComponent = ComponentName.engine; break;
            case 1: replacementComponent = ComponentName.jetEngine; break;
            case 2: replacementComponent = ComponentName.aireon; break;
            case 3: replacementComponent = ComponentName.fuelTank; break;
            case 4: replacementComponent = ComponentName.boostGulp; break;
            case 5: replacementComponent = ComponentName.machineGun; break;
            case 6: replacementComponent = ComponentName.missile; break;
            default: replacementComponent = ComponentName.empty; break;
        }

        return replacementComponent;
    }

    void SetComponentSlot(ComponentSlotType slot, ComponentName componentKey)
    {
        if (!componentSlots.TryGetValue(slot, out var slotData)) return;

        if (slotData.label != null)
            slotData.label.text = componentKey.ToString();
        slotData.selectedComponentKey = componentKey;

        foreach (var kvp in slotData.components)
        {
            if (kvp.Value != null)
            {
                kvp.Value.SetActive(kvp.Key == componentKey);
            }
        }

    }

    void UpdateComponent_Frame(ComponentName newComponentName)
    {
        if (componentSlots.TryGetValue(ComponentSlotType.Frame, out var frameSlot))
        {
            if (frameSlot.selectedComponentKey != newComponentName)
            {
                CleanupExcessMeshes(frameSlot.position);

                SetComponentSlot(ComponentSlotType.Frame, newComponentName);

                DisplayComponentMeshes();
            }
        }

        CheckHeavyFrame();
    }

    ComponentName GetFrameType(int val)
    {
        ComponentName replacementComponent = ComponentName.empty;

        switch (val)
        {
            case 0: replacementComponent = ComponentName.lightFrame; break;
            case 1: replacementComponent = ComponentName.mediumFrame; break;
            case 2: replacementComponent = ComponentName.heavyFrame; break;
            default: replacementComponent = ComponentName.empty; break;
        }

        return replacementComponent;
    }

    void UpdateComponent(ComponentSlotType slotType, ComponentName replacementComponent)
    {
        if (componentSlots.TryGetValue(slotType, out var slotPosition))
        {
            if (slotPosition.selectedComponentKey != replacementComponent)
            {

                CleanupExcessMeshes(slotPosition.position);

                

                SetComponentSlot(slotType, replacementComponent);
                DisplayComponentMeshes();

                if (slotType == ComponentSlotType.Frame)
                {
                    CheckHeavyFrame();
                }
            }
        }

    }

    void CleanupExcessMeshes(Transform slotPosition)
    {
        foreach (Transform child in slotPosition)
        {
            Destroy(child.gameObject);
        }
    }

    public void UpdateComponentSlot(ComponentSlotType slotType, int val)
    {
        ComponentName replacementComponent = slotType switch
        {
            ComponentSlotType.Frame => GetFrameType(val),
            _ => GetComponentName(val),
        };

        UpdateComponent(slotType, replacementComponent);
    }
    // Currently these need to be explicit and repeated for dropdow access in unity, will probably be changed later, so might not be worth trying to fix atm
    #region
    public void UpdateComponentSlot_FRAME(int val) => UpdateComponentSlot(ComponentSlotType.Frame, val);

    public void UpdateComponentSlot_FL(int val) => UpdateComponentSlot(ComponentSlotType.FrontLeft, val);

    public void UpdateComponentSlot_FR(int val) => UpdateComponentSlot(ComponentSlotType.FrontRight, val);

    public void UpdateComponentSlot_BL(int val) => UpdateComponentSlot(ComponentSlotType.BackLeft, val);

    public void UpdateComponentSlot_BR(int val) => UpdateComponentSlot(ComponentSlotType.BackRight, val);

    public void UpdateComponentSlot_BL1(int val) => UpdateComponentSlot(ComponentSlotType.BackLeft1, val);

    public void UpdateComponentSlot_BR1(int val) => UpdateComponentSlot(ComponentSlotType.BackRight1, val);

    public void UpdateComponentSlot_ExtraFront(int val) => UpdateComponentSlot(ComponentSlotType.ExtraFront, val);

    public void UpdateComponentSlot_ExtraLeft(int val) => UpdateComponentSlot(ComponentSlotType.ExtraLeft, val);

    public void UpdateComponentSlot_ExtraRight(int val) => UpdateComponentSlot(ComponentSlotType.ExtraRight, val);
    #endregion




    void SetComponentSlots(ComponentSlotType slotType, TextMeshProUGUI slotLabel, Transform slotPosition)
    {
        componentSlots[slotType] = new ComponentSlot
        {
            label = slotLabel,
            position = slotPosition,
            components = new Dictionary<ComponentName, GameObject>
            {
                { ComponentName.lightFrame, shipPassport.GetPrefab(ComponentName.lightFrame) },
                { ComponentName.mediumFrame, shipPassport.GetPrefab(ComponentName.mediumFrame) },
                { ComponentName.heavyFrame , shipPassport.GetPrefab(ComponentName.heavyFrame) },
                 { ComponentName.engine, shipPassport.GetPrefab(ComponentName.engine) },
                { ComponentName.jetEngine,  shipPassport.GetPrefab(ComponentName.jetEngine) },
                { ComponentName.aireon , shipPassport.GetPrefab(ComponentName.aireon)},
                { ComponentName.fuelTank, shipPassport.GetPrefab(ComponentName.fuelTank) },
                { ComponentName.boostGulp, shipPassport.GetPrefab(ComponentName.boostGulp)  },
                { ComponentName.machineGun, shipPassport.GetPrefab(ComponentName.machineGun)  },
                { ComponentName.missile, shipPassport.GetPrefab(ComponentName.missile)  },
                { ComponentName.empty , null }
            }
        };

        if(slotType == ComponentSlotType.Frame )
        {
            CheckHeavyFrame();
        }
    }

    void SetOptionsForExtraSlots(bool value)
    {
        backLeft1_Dropdown.gameObject.SetActive(value);
        backLeft1Label.gameObject.SetActive(value);
        bL1Text.gameObject.SetActive(value);
        bR1Text.gameObject.SetActive(value);
        backRight1_Dropdown.gameObject.SetActive(value);
        backRight1Label.gameObject.SetActive(value);
    }


    public void SetShipLoadout()
    {
        var shipLoadout = new Dictionary<ComponentSlotType, ComponentName>();

        foreach (var pair in componentSlots)
        {
            shipLoadout[pair.Key] = pair.Value.selectedComponentKey;
        }

        shipPassport.SetShipLoadout(shipLoadout);
    }


   




}
