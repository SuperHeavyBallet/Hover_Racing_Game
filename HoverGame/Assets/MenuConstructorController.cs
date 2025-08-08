using System.Collections;
using System.Collections.Generic;
//using System.ComponentModel;
using TMPro;
//using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
//using UnityEngine.SceneManagement;

public class MenuConstructorController : MonoBehaviour
{

 


    [Header("Object Positions")]
    public Transform framePosition;
  

    [Header("Dropdown Menus to Enable/Disable for Heavy Frames")]
    public GameObject backLeft1_Dropdown;
    public GameObject backRight1_Dropdown;
    public GameObject backLeft1Label;
    public GameObject backRight1Label;

    

    Ship_Passport SHIP_PASSPORT;

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
        ComponentName.Light_Frame,
        ComponentName.Medium_Frame,
        ComponentName.Heavy_Frame
    };

    List<string> engineOptions = new List<string>
    {
        "Engine",
        "Jet Engine",
        "Empty"
    };
    List<ComponentName> engineComponentKeys = new List<ComponentName>
    {
        ComponentName.Engine,
        ComponentName.Jet_Engine,
        ComponentName.Empty
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
        ComponentName.Boost_Gulp,
        ComponentName.Machine_Gun,
        ComponentName.Missile,
        ComponentName.Empty
    };

    List<string> extraOptions = new List<string>
    {
        "Aireon",
        "FuelTank",
        "Empty"
    };
    List<ComponentName> extraComponentKeys = new List<ComponentName>
    {
        ComponentName.Aireon,
        ComponentName.Fuel_Tank,
        ComponentName.Empty
    };

    public GameObject UI_ComponentList_Holder;
    public GameObject PREFAB_UI_ComponentList_Element;

    public GameObject[] UI_ComponenentListElements;
    ShipStatsCalculator SCRIPT_ShipStatsCalculator;
    public GameObject DISPLAY_ShipTopSpeed;
    public GameObject DISPLAY_ShipPower;
    public GameObject DISPLAY_ShipControl;
    public float ShipTopSpeed;
    public float ShipPower;
    public float ShipControl;

    void Awake()
    {
        SCRIPT_ShipStatsCalculator = this.GetComponentInChildren<ShipStatsCalculator>();

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

    void UpdateShipStats()
    {
        var shipLoadout = new Dictionary<ComponentSlotPosition, ComponentName>();

        foreach (var pair in componentSlotPositions)
        {
            shipLoadout[pair.Key] = pair.Value.selectedComponentKey;
        }

        SCRIPT_ShipStatsCalculator.CalculatePerformance(shipLoadout);
        ShipTopSpeed = SCRIPT_ShipStatsCalculator.GetShipTopSpeed();
        ShipPower = SCRIPT_ShipStatsCalculator.GetShipPower();
        ShipControl = SCRIPT_ShipStatsCalculator.GetShipControl();

        DISPLAY_ShipTopSpeed.GetComponent<TextMeshProUGUI>().text = "TOP SPEED: " + ShipTopSpeed.ToString();
        DISPLAY_ShipPower.GetComponent<TextMeshProUGUI>().text = "POWER: " + ShipPower.ToString();
        DISPLAY_ShipControl.GetComponent<TextMeshProUGUI>().text = "CONTROL: " + ShipControl.ToString();
    }

    public void CheckExistingShip()
    {
        SHIP_PASSPORT = GameObject.Find("ShipPassport").GetComponent<Ship_Passport>();
        receivedShipLoadout = SHIP_PASSPORT.GetShipLoadout();

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
        //UpdateComponentDisplayText(ComponentSlotPosition.Frame, newComponentName);
        UpdateComponentPositions();
        CheckIfLightFrame();
        CheckIfHeavyFrame();
        SetOptionsForExtraSlots(currentFrameIsHeavy);
        InitializeComponentSlotDefinitions();

        UpdateUIElements();
    }

    GameObject SetFrameObject(ComponentName frameName)
    {
        GameObject chosenFrame;

        switch (frameName)
        {
            case ComponentName.Light_Frame:
                chosenFrame = DISPLAY_LightFrame; break;
            case ComponentName.Medium_Frame:
                chosenFrame = DISPLAY_MediumFrame; break;
            case ComponentName.Heavy_Frame:
                chosenFrame = DISPLAY_HeavyFrame; break;
            default:
                Debug.LogError("No Frame Found");
                chosenFrame = DISPLAY_MediumFrame; break;
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
                
            }
        }
        UpdateUIElements();

    }

    void UpdateUIElements()
    {
        DisplayComponentMeshes();
        ExposeComponentsAsList();
        UpdateShipStats();
    }


    void InitializeComponentSlotDefinitions()
    {
        Frame_Layout frameLayout = currentFrame.GetComponent<Frame_Layout>();
        if(frameLayout != null)
        {
            DefineComponentOptions();

            InitializeComponentSlot(ComponentSlotPosition.Frame, framePosition, frameComponentOptions);

            if (currentFrameIsLight)
            {
                InitializeComponentSlot(ComponentSlotPosition.FrontLeft, frameLayout.GetEngineSlot(0), engineComponentOptions);
                InitializeComponentSlot(ComponentSlotPosition.FrontRight, frameLayout.GetFrontRightPosition(), extraComponentOptions);

            }
            else
            {
                InitializeComponentSlot(ComponentSlotPosition.FrontLeft,  frameLayout.GetFrontLeftPosition(), engineComponentOptions);
                InitializeComponentSlot(ComponentSlotPosition.FrontRight,  frameLayout.GetFrontRightPosition(), engineComponentOptions);
            }
            
            
            InitializeComponentSlot(ComponentSlotPosition.BackLeft,  frameLayout.GetBackLeftPosition(), engineComponentOptions);
            InitializeComponentSlot(ComponentSlotPosition.BackRight,  frameLayout.GetBackRightPosition(), engineComponentOptions);

            if (currentFrameIsHeavy)
            {
                InitializeComponentSlot(ComponentSlotPosition.BackLeft1,  frameLayout.GetBackLeft1Position(), engineComponentOptions);
                InitializeComponentSlot(ComponentSlotPosition.BackRight1,  frameLayout.GetBackRight1Position(), engineComponentOptions);
            }

            InitializeComponentSlot(ComponentSlotPosition.ExtraTop, frameLayout.GetExtraTopPosition(), extraTopComponentOptions);
            InitializeComponentSlot(ComponentSlotPosition.ExtraLeft,  frameLayout.GetExtraLeftPosition(), extraComponentOptions);
            InitializeComponentSlot(ComponentSlotPosition.ExtraRight,  frameLayout.GetExtraRightPosition(), extraComponentOptions);

        }   
    }


    void InitializeComponentSlot(ComponentSlotPosition componentSlotPosition, Transform slotTransform, Dictionary<ComponentName, GameObject> slotComponents)
    {
        ComponentName previouslySelectedComponent = ComponentName.Empty;

        if (componentSlotPositions.TryGetValue(componentSlotPosition, out var existingSlot))
        {
            previouslySelectedComponent = existingSlot.selectedComponentKey;
        }

        componentSlotPositions[componentSlotPosition] = new ComponentSlot
        {
            //label = slotLabel,
            position = slotTransform,
            components = slotComponents,
            selectedComponentKey = previouslySelectedComponent // Preserve old selection
        };
    }

    void DefineComponentOptions()
    {
        frameComponentOptions = SHIP_PASSPORT.GetFrameComponentOptions();
        engineComponentOptions = SHIP_PASSPORT.GetEngineComponentOptions();
        extraTopComponentOptions = SHIP_PASSPORT.GetExtraTopComponentOptions();
        extraComponentOptions = SHIP_PASSPORT.GetExtraComponentOptions();
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
        if (selectedKey == ComponentName.Empty) return;
       

        if (slot.components.TryGetValue(selectedKey, out var prefab))
        {
            if (selectedKey != ComponentName.Light_Frame || selectedKey != ComponentName.Medium_Frame || selectedKey != ComponentName.Heavy_Frame)
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
            currentFrameIsLight = (frame.selectedComponentKey == ComponentName.Light_Frame);
        }

    }

   

    public void CheckIfHeavyFrame()
    {
        currentFrameIsHeavy = false;

        if(componentSlotPositions.TryGetValue(ComponentSlotPosition.Frame, out var frame))
        {
            currentFrameIsHeavy = (frame.selectedComponentKey == ComponentName.Heavy_Frame);
        }

        
    }

    ComponentName GetComponentName(int val)
    {
        ComponentName replacementComponent = ComponentName.Empty;

        switch (val)
        {
            case 0: replacementComponent = ComponentName.Engine; break;
            case 1: replacementComponent = ComponentName.Jet_Engine; break;
            case 2: replacementComponent = ComponentName.Aireon; break;
            case 3: replacementComponent = ComponentName.Fuel_Tank; break;
            case 4: replacementComponent = ComponentName.Boost_Gulp; break;
            case 5: replacementComponent = ComponentName.Machine_Gun; break;
            case 6: replacementComponent = ComponentName.Missile; break;
            default: replacementComponent = ComponentName.Empty; break;
        }

        return replacementComponent;
    }

    void UpdateComponents(ComponentSlotPosition slotType, ComponentName componentName)
    {
        if (!componentSlotPositions.TryGetValue(slotType, out var slotData)) return;

        slotData.selectedComponentKey = componentName;
    }


    ComponentName GetFrameType(int val)
    {
        ComponentName replacementComponent = ComponentName.Empty;

        switch (val)
        {
            case 0: replacementComponent = ComponentName.Light_Frame; break;
            case 1: replacementComponent = ComponentName.Medium_Frame; break;
            case 2: replacementComponent = ComponentName.Heavy_Frame; break;
            default: replacementComponent = ComponentName.Empty; break;
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

        SHIP_PASSPORT.SetShipLoadout(shipLoadout);
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
        componentsList.Clear(); // <--- Important
        foreach (var pair in componentSlotPositions)
        {
            string componentName = pair.Value.selectedComponentKey.ToString();
            componentsList.Add(componentName);
        }


        foreach (GameObject textElement in UI_ComponenentListElements)
        {
            TextMeshProUGUI elementText = textElement.GetComponent<TextMeshProUGUI>();
            elementText.text = "";

        }

        BuildNewUIList();
    }

    void BuildNewUIList()
    {

        string[] componentsListArray = componentsList.ToArray();

        List<string> populatedComponents = new List<string>();
        List<string> emptyComponents = new List<string>();


        foreach (var component in componentsList)
        {
            if (component == "Empty")
            {
                emptyComponents.Add(component);
            }
            else
            {
                populatedComponents.Add(component);
            }
        }

        List<string> completedList = new List<string>();

        foreach(string component in populatedComponents)
        {
            completedList.Add(component);
        }

        foreach (string component in emptyComponents)
        {
            completedList.Add(component);
        }

        componentsListArray = completedList.ToArray();


        for(int i = 0;i < componentsListArray.Length;i++)
        {
            TextMeshProUGUI elementText = UI_ComponenentListElements[i].GetComponent<TextMeshProUGUI>();
            if (elementText != null)
            {
                if (componentsListArray[i] == "Empty")
                {
                    elementText.text = "";

                }
                else
                {
                    elementText.text = componentsListArray[i];

                }
                
            }
            else
            {
                Debug.LogWarning("No Text given for the Component List UI Element");
            }
        }

    }


}
