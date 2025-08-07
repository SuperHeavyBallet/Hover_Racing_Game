using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuConstructorController : MonoBehaviour
{

    [Header("Text UI")]
    public TextMeshProUGUI TEXT_FRAME;
    public TextMeshProUGUI TEXT_FRONTLEFT;
    public TextMeshProUGUI TEXT_FRONTRIGHT;
    public TextMeshProUGUI TEXT_BACKLEFT;
    public TextMeshProUGUI TEXT_BACKRIGHT;
    public TextMeshProUGUI TEXT_BACKLEFT_1;
    public TextMeshProUGUI TEXT_BACKRIGHT_1;
    public TextMeshProUGUI TEXT_EXTRATOP;
    public TextMeshProUGUI TEXT_EXTRALEFT;
    public TextMeshProUGUI TEXT_EXTRARIGHT;


    [Header("Object Positions")]
    public Transform framePosition;
  

    [Header("Dropdown Menus to Enable/Disable for Heavy Frames")]
    public GameObject backLeft1_Dropdown;
    public GameObject backRight1_Dropdown;
    public GameObject backLeft1Label;
    public GameObject backRight1Label;

    

    Ship_Passport shipPassport;

    public Dictionary<ComponentSlotPosition, ComponentSlot> componentSlotPositions = new();
    public Dictionary<ComponentSlotPosition, ComponentName> receivedShipLoadout = new();

    GameObject currentFrame;

    List <Transform> engineSlotPositions = new List<Transform>();
    List <Transform> extraSlotPositions = new List<Transform>();
    Transform extraTopSlotPosition;

    public GameObject DISPLAY_LightFrame;
    public GameObject DISPLAY_MediumFrame;
    public GameObject DISPLAY_HeavyFrame;


    public bool currentFrameIsLight = false;
    public bool currentFrameIsHeavy = false;

    Dictionary<ComponentName, GameObject> frameComponentOptions = new();
    Dictionary<ComponentName, GameObject> engineComponentOptions = new();
    Dictionary<ComponentName, GameObject> extraComponentOptions = new();
    Dictionary<ComponentName, GameObject> extraTopComponentOptions = new();

    public TMP_Dropdown frameDropdown;
    public TMP_Dropdown frontLeftDropdown;
    public TMP_Dropdown frontRightDropdown;
    public TMP_Dropdown backLeftDropdown;
    public TMP_Dropdown backRightDropdown;
    public TMP_Dropdown backLeft1Dropdown;
    public TMP_Dropdown backRight1Dropdown;
    public TMP_Dropdown extraTopDropdown;
    public TMP_Dropdown extraLeftDropdown;
    public TMP_Dropdown extraRightDropdown;

    List<string> componentsList = new List<string>();

    List<string> frameOptions = new List<string>
    {
        "Light",
        "Medium",
        "Heavy"
    };
    List<ComponentName> frameKeys = new List<ComponentName>
    {
        ComponentName.lightFrame,
        ComponentName.mediumFrame,
        ComponentName.heavyFrame
    };

    List<string> engineOptions = new List<string>
    {
        "Engine",
        "Jet Engine",
        "Empty"
    };
    List<ComponentName> engineComponentKeys = new List<ComponentName>
    {
        ComponentName.engine,
        ComponentName.jetEngine,
        ComponentName.empty
    };

    List<string> extraTopOptions = new List<string>
    {
        "Boost Gulp",
        "Machine Gun",
        "Missile Launcher",
        "Empty"
    };
    List<ComponentName> extraTopComponentKeys = new List<ComponentName>
    {
        ComponentName.boostGulp,
        ComponentName.machineGun,
        ComponentName.missile,
        ComponentName.empty
    };

    List<string> extraOptions = new List<string>
    {
        "Aireon",
        "FuelTank",
        "Empty"
    };
    List<ComponentName> extraComponentKeys = new List<ComponentName>
    {
        ComponentName.aireon,
        ComponentName.fuelTank,
        ComponentName.empty
    };

    public GameObject UI_ComponentList_Holder;
    public GameObject PREFAB_UI_ComponentList_Element;


    void Awake()
    {
        CheckExistingShip();
        SetDropdownOptions(frameDropdown, frameOptions, 1);
        SetDropdownOptions(frontLeftDropdown, engineOptions, 1);
        SetDropdownOptions(frontRightDropdown, engineOptions, 1);
        SetDropdownOptions(backLeftDropdown, engineOptions, 1);
        SetDropdownOptions(backRightDropdown, engineOptions, 1);
        SetDropdownOptions(backLeft1Dropdown, engineOptions, engineOptions.Count - 1);
        SetDropdownOptions(backRight1Dropdown, engineOptions, engineOptions.Count -1);
        SetDropdownOptions(extraTopDropdown, extraTopOptions, 0);
        SetDropdownOptions(extraLeftDropdown, extraOptions, extraOptions.Count - 1);
        SetDropdownOptions(extraRightDropdown, extraOptions, extraOptions.Count - 1);
    }

    public void CheckExistingShip()
    {
        shipPassport = GameObject.Find("ShipPassport").GetComponent<Ship_Passport>();
        receivedShipLoadout = shipPassport.GetShipLoadout();

        if (receivedShipLoadout != null)
        {
            GenerateShipModel();
        }
    }

    public void GenerateShipModel()
    {
        foreach (var kvp in receivedShipLoadout)
        {
            if (kvp.Key == ComponentSlotPosition.Frame)
            {
                Place_Frame(kvp.Value);
            }
            else
            {
                Place_Component(kvp.Key, kvp.Value);
            }
        }

    }

    void Place_Frame(ComponentName newComponentName)
    {
        DisableAllFrames();
        currentFrame = SetFrameObject(newComponentName);
        EnableSingleFrame(currentFrame);
        UpdateComponents(ComponentSlotPosition.Frame, newComponentName);
        UpdateComponentDisplayText(ComponentSlotPosition.Frame, newComponentName);
        UpdateComponentPositions();
        CheckIfLightFrame();
        CheckIfHeavyFrame();
        SetOptionsForExtraSlots(currentFrameIsHeavy);
        InitializeComponentSlotDefinitions();

        DisplayComponentMeshes();
    }

    GameObject SetFrameObject(ComponentName frameName)
    {
        GameObject chosenFrame;

        switch (frameName)
        {
            case ComponentName.lightFrame:
                chosenFrame = DISPLAY_LightFrame; break;
            case ComponentName.mediumFrame:
                chosenFrame = DISPLAY_MediumFrame; break;
            case ComponentName.heavyFrame:
                chosenFrame = DISPLAY_HeavyFrame; break;
            default:
                Debug.LogError("No Frame Found");
                chosenFrame = shipPassport.mediumFrame; break;
        }

        return chosenFrame;
    }

    void Place_Component(ComponentSlotPosition slotPosition, ComponentName replacementComponent)
    {
        if (componentSlotPositions.TryGetValue(slotPosition, out var position))
        {
            if (position.selectedComponentKey != replacementComponent)
            {
                UpdateComponents(slotPosition, replacementComponent);
                UpdateComponentDisplayText(slotPosition, replacementComponent);
                DisplayComponentMeshes();
            }
        }

        ExposeComponentsAsList();

    }


    void InitializeComponentSlotDefinitions()
    {
        Frame_Layout frameLayout = currentFrame.GetComponent<Frame_Layout>();
        if(frameLayout != null)
        {
            DefineComponentOptions();

            InitializeComponentSlot(ComponentSlotPosition.Frame, TEXT_FRAME, framePosition, frameComponentOptions);

            if (currentFrameIsLight)
            {
                InitializeComponentSlot(ComponentSlotPosition.FrontLeft, TEXT_FRONTLEFT, frameLayout.GetEngineSlot(0), engineComponentOptions);
                InitializeComponentSlot(ComponentSlotPosition.FrontRight, TEXT_FRONTRIGHT, frameLayout.GetFrontRightPosition(), extraComponentOptions);

            }
            else
            {
                InitializeComponentSlot(ComponentSlotPosition.FrontLeft, TEXT_FRONTLEFT, frameLayout.GetFrontLeftPosition(), engineComponentOptions);
                InitializeComponentSlot(ComponentSlotPosition.FrontRight, TEXT_FRONTRIGHT, frameLayout.GetFrontRightPosition(), engineComponentOptions);
            }
            
            
            InitializeComponentSlot(ComponentSlotPosition.BackLeft, TEXT_BACKLEFT, frameLayout.GetBackLeftPosition(), engineComponentOptions);
            InitializeComponentSlot(ComponentSlotPosition.BackRight, TEXT_BACKRIGHT, frameLayout.GetBackRightPosition(), engineComponentOptions);

            if (currentFrameIsHeavy)
            {
                InitializeComponentSlot(ComponentSlotPosition.BackLeft1, TEXT_BACKLEFT_1, frameLayout.GetBackLeft1Position(), engineComponentOptions);
                InitializeComponentSlot(ComponentSlotPosition.BackRight1, TEXT_BACKRIGHT_1, frameLayout.GetBackRight1Position(), engineComponentOptions);
            }

            InitializeComponentSlot(ComponentSlotPosition.ExtraTop, TEXT_EXTRATOP, frameLayout.GetExtraTopPosition(), extraTopComponentOptions);
            InitializeComponentSlot(ComponentSlotPosition.ExtraLeft, TEXT_EXTRALEFT, frameLayout.GetExtraLeftPosition(), extraComponentOptions);
            InitializeComponentSlot(ComponentSlotPosition.ExtraRight, TEXT_EXTRARIGHT, frameLayout.GetExtraRightPosition(), extraComponentOptions);

        }   
    }


    void InitializeComponentSlot(ComponentSlotPosition componentSlotPosition, TextMeshProUGUI slotLabel, Transform slotTransform, Dictionary<ComponentName, GameObject> slotComponents)
    {
        ComponentName previouslySelectedComponent = ComponentName.empty;

        if (componentSlotPositions.TryGetValue(componentSlotPosition, out var existingSlot))
        {
            previouslySelectedComponent = existingSlot.selectedComponentKey;
        }

        componentSlotPositions[componentSlotPosition] = new ComponentSlot
        {
            label = slotLabel,
            position = slotTransform,
            components = slotComponents,
            selectedComponentKey = previouslySelectedComponent // Preserve old selection
        };
    }

    void DefineComponentOptions()
    {
        frameComponentOptions = shipPassport.GetFrameComponentOptions();
        engineComponentOptions = shipPassport.GetEngineComponentOptions();
        extraTopComponentOptions = shipPassport.GetExtraTopComponentOptions();
        extraComponentOptions = shipPassport.GetExtraComponentOptions();
    }


    void UpdateComponentPositions()
    {
        Frame_Layout frameLayout = currentFrame.GetComponent<Frame_Layout>();

        if (frameLayout != null)
        {
            UpdateSlotPositions(engineSlotPositions, frameLayout.GetEngineSlots());
            UpdateSlotPositions(extraSlotPositions, frameLayout.GetExtraSlots());
            UpdateExtraTopSlotPosition(frameLayout);
        }
        else
        {
            Debug.LogWarning("No Valid Frame Positions Found");
        }
    }

    void UpdateSlotPositions(List<Transform> slotPositions, Transform[] slots)
    {
        slotPositions.Clear();
        slotPositions.AddRange(slots);
    }

    void UpdateExtraTopSlotPosition(Frame_Layout frameLayout)
    {
        extraTopSlotPosition = frameLayout.GetExtraTopSlot();
    }









    void EnableSingleFrame(GameObject frame)
    {
        frame.SetActive(true);
    }
    void DisableAllFrames()
    {
        DISPLAY_LightFrame.gameObject.SetActive(false);
        DISPLAY_MediumFrame.gameObject.SetActive(false);
        DISPLAY_HeavyFrame.gameObject.SetActive(false);
    }

    void DisplayComponentMeshes()
    {
        foreach (var pair in componentSlotPositions)
        {
            if(pair.Key != ComponentSlotPosition.Frame)
            {
                
                 CleanupExcessMeshes(pair.Value.position);
                 InstantiateSelectedComponent(pair.Value, pair.Value.position.transform);
               
                
                
            }  
        }
    }

    void InstantiateSelectedComponent(ComponentSlot slot, Transform slotPosition)
    {
        var selectedKey = slot.selectedComponentKey;
        if (selectedKey == ComponentName.empty) return;

        if (slot.components.TryGetValue(selectedKey, out var prefab))
        {
            if (selectedKey != ComponentName.lightFrame || selectedKey != ComponentName.mediumFrame || selectedKey != ComponentName.heavyFrame)
            {
                GameObject newComp = Instantiate(prefab, slotPosition);
              
                newComp.SetActive(true);
            }
        }
    }

    public void CheckIfLightFrame()
    {
        currentFrameIsLight = false;

        if (componentSlotPositions.TryGetValue(ComponentSlotPosition.Frame, out var frame))
        {
            currentFrameIsLight = (frame.selectedComponentKey == ComponentName.lightFrame);
        }

    }

   

    public void CheckIfHeavyFrame()
    {
        currentFrameIsHeavy = false;

        if(componentSlotPositions.TryGetValue(ComponentSlotPosition.Frame, out var frame))
        {
            currentFrameIsHeavy = (frame.selectedComponentKey == ComponentName.heavyFrame);
        }

        
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

    void UpdateComponents(ComponentSlotPosition slotType, ComponentName componentName)
    {
        if (!componentSlotPositions.TryGetValue(slotType, out var slotData)) return;

        slotData.selectedComponentKey = componentName;
    }

    void UpdateComponentDisplayText(ComponentSlotPosition slotType, ComponentName componentName)
    {
        if (!componentSlotPositions.TryGetValue(slotType, out var slotData)) return;

        if (slotData.label != null)
        {
            slotData.label.text = componentName.ToString();
        }
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

  

    void CleanupExcessMeshes(Transform slotPosition)
    {
        foreach (Transform child in slotPosition)
        {
            Destroy(child.gameObject);
        }
    }

    public void UpdateComponentSlot(ComponentSlotPosition slotPosition, int val)
    {
        ComponentName replacementComponent = slotPosition switch
        {
            ComponentSlotPosition.Frame => GetFrameType(val),
            _ => GetComponentName(val),
        };

        if(slotPosition == ComponentSlotPosition.Frame)
        {
            Place_Frame(replacementComponent);
        }
        
        //Place_Component(slotPosition, replacementComponent);
        

       
    }

    public void UpdateComponentSlot_FRAME(int val) => UpdateComponentSlot(ComponentSlotPosition.Frame, val);

    public void UpdateComponentSlot_FL(int val)
    {
        ComponentName chosenComponent = engineComponentKeys[val];
        Place_Component(ComponentSlotPosition.FrontLeft, chosenComponent);
    }

    public void UpdateComponentSlot_FR(int val)
    {
        ComponentName chosenComponent = engineComponentKeys[val];
        Place_Component(ComponentSlotPosition.FrontRight, chosenComponent);
    }

    public void UpdateComponentSlot_BL(int val)
    {
        ComponentName chosenComponent = engineComponentKeys[val];
        Place_Component(ComponentSlotPosition.BackLeft, chosenComponent);
    }

    public void UpdateComponentSlot_BR(int val)
    {
        ComponentName chosenComponent = engineComponentKeys[val];
        Place_Component(ComponentSlotPosition.BackRight, chosenComponent);
    }

    public void UpdateComponentSlot_BL1(int val)
    {
        ComponentName chosenComponent = engineComponentKeys[val];
        Place_Component(ComponentSlotPosition.BackLeft1, chosenComponent);
    }
    public void UpdateComponentSlot_BR1(int val)
    {
        ComponentName chosenComponent = engineComponentKeys[val];
        Place_Component(ComponentSlotPosition.BackRight1, chosenComponent);
    }

    public void UpdateComponentSlot_ExtraFront(int val)
    {
        ComponentName chosenComponent = extraTopComponentKeys[val];
        Place_Component(ComponentSlotPosition.ExtraTop, chosenComponent);
    }

    public void UpdateComponentSlot_ExtraLeft(int val)
    {
        ComponentName chosenComponent = extraComponentKeys[val];
        Place_Component(ComponentSlotPosition.ExtraLeft, chosenComponent);
    }

    public void UpdateComponentSlot_ExtraRight(int val)
    {
        ComponentName chosenComponent = extraComponentKeys[val];
        Place_Component(ComponentSlotPosition.ExtraRight, chosenComponent);
    }





    void SetOptionsForExtraSlots(bool value)
    {
        backLeft1_Dropdown.gameObject.SetActive(value);
        backLeft1Label.gameObject.SetActive(value);
        TEXT_BACKLEFT_1.gameObject.SetActive(value);
        TEXT_BACKRIGHT_1.gameObject.SetActive(value);
        backRight1_Dropdown.gameObject.SetActive(value);
        backRight1Label.gameObject.SetActive(value);
    }


    public void SetShipLoadout()
    {
        var shipLoadout = new Dictionary<ComponentSlotPosition, ComponentName>();

        foreach (var pair in componentSlotPositions)
        {
            shipLoadout[pair.Key] = pair.Value.selectedComponentKey;
        }

        shipPassport.SetShipLoadout(shipLoadout);
    }

    void SetDropdownOptions(TMP_Dropdown dropdown, List<string> options, int defaultIndex = 0)
    {
        dropdown.ClearOptions();
        dropdown.AddOptions(options);

        dropdown.value = Mathf.Clamp(defaultIndex, 0, options.Count - 1);
        dropdown.RefreshShownValue();
    }


    void ExposeComponentsAsList()
    {
        
        foreach(var pair in componentSlotPositions)
        {
            string componentName = pair.Value.selectedComponentKey.ToString();
            componentsList.Add(componentName);
            
        }

        BuildNewUIList();
    }

    void BuildNewUIList()
    {
        foreach (Transform child in UI_ComponentList_Holder.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (string component in componentsList)
        {
            GameObject newComponentListElement = Instantiate(PREFAB_UI_ComponentList_Element, UI_ComponentList_Holder.transform);
            TextMeshProUGUI listElementText = newComponentListElement.GetComponent<TextMeshProUGUI>();
            if(listElementText != null )
            {
                listElementText.text = component;
            }
            else
            {
                Debug.LogWarning("componentTextPrefab does not have a TextMeshProUGUI component attached.");
            }
        }
        
    }


}
