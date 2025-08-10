using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Analytics;

public class MenuConstructorController : MonoBehaviour
{

    #region ASSIGNMENTS
    [Header("Object Positions")]
    public Transform framePosition;
  
    [Header("Dropdown Menus to Enable/Disable for Heavy Frames")]
    public GameObject backLeft1_Dropdown;
    public GameObject backRight1_Dropdown;
    public GameObject backLeft1Label;
    public GameObject backRight1Label;

    Ship_Passport SHIP_PASSPORT;

    public Dictionary<ComponentSlotPosition, ComponentSlot> componentSlotPositions = new();
    public Dictionary<ComponentSlotPosition, string> receivedShipLoadout = new();

    ComponentDefinition currentFrame;

    List <Transform> engineSlotPositions = new List<Transform>();
    List <Transform> extraSlotPositions = new List<Transform>();
    Transform extraTopSlotPosition;

    //public GameObject DISPLAY_LightFrame;
   // public GameObject DISPLAY_MediumFrame;
   // public GameObject DISPLAY_HeavyFrame;


    public bool currentFrameIsLight = false;
    public bool currentFrameIsHeavy = false;

    Dictionary<string, GameObject> frameComponentOptions = new();
    Dictionary<string, GameObject> engineComponentOptions = new();
    Dictionary<string, GameObject> extraComponentOptions = new();
    Dictionary<string, GameObject> extraTopComponentOptions = new();

    List<string> componentsList = new List<string>();


    List<string> frameKeys = new();
    
    List<string> engineComponentKeys = new();

    List<string> extraTopComponentKeys = new();

    List<string> extraComponentKeys = new();

    [Header("References")]
    public ComponentCatalogue SCRIPT_ComponentCatalogue; // assign the asset in the inspector
    ShipComponentsList_Controller SCRIPT_shipComponentsList_Controller;
    MeshDisplayController SCRIPT_MeshDisplayController;
    ShipStatsUI_Updater SCRIPT_ShipStatsUI_Updater;
    ShipComponents_DropdownGenerator SCRIPT_ShipComponentsDropdownGenerator;

    Frame_Layout currentFrameLayout;
    GameObject currentFrameInstance;
    #endregion

    void Awake()
    {
        Get_SCRIPT_References();
        CheckExistingShip();
        Create_DropDownReferences();
    }

    // IMPORTANT FUNCTIONS
    public void CheckExistingShip()
    {
        Get_LoadoutFrom_ShipPassport();

        if (receivedShipLoadout != null)
        {
            // Good To this point
            GenerateShipModel();
        }
    }
    
    public void GenerateShipModel()
    {
        // Good To this point
        foreach (var kvp in receivedShipLoadout)
        {
            if (kvp.Key == ComponentSlotPosition.Frame)
            {
                Place_Frame(kvp.Value);
            }
            else
            {
               Set_ComponentPosition(kvp.Key, kvp.Value);
            }
        }
    }

    void Place_Frame(string newComponentId)
    {
        
        

        //Set, Create and Update for new frame
        currentFrame = DecideFrame(newComponentId);
        InstantiatePrefabAtPosition(currentFrame.prefab, framePosition);
        UpdateFrameLayout();

   

        // Authorise the positions and contents of each component slot per current frame
        SET_FrameSlotPositionsAndContents();


        // Good To this point
        UpdateComponents(ComponentSlotPosition.Frame, newComponentId);

        CheckIfLightFrame();
        CheckIfHeavyFrame();
        SetOptionsForExtraSlots(currentFrameIsHeavy);
        
        UpdateUIElements();
    }

    ComponentDefinition DecideFrame(string frameId)
    {
        ComponentDefinition chosenFrame;

        chosenFrame = SCRIPT_ComponentCatalogue.GetById(frameId);

        return chosenFrame;
    }

    void InstantiatePrefabAtPosition(GameObject prefab, Transform position)
    {
       currentFrameInstance = Instantiate(prefab, position);
    }

    void UpdateFrameLayout()
    {
        currentFrameLayout = currentFrameInstance.GetComponent<Frame_Layout>();

        if (currentFrameLayout != null)
        {
            UpdateSlotPositions(engineSlotPositions, currentFrameLayout.GetEngineSlots());
            UpdateSlotPositions(extraSlotPositions, currentFrameLayout.GetExtraSlots());
            UpdateExtraTopSlotPosition(currentFrameLayout);
        }
    }

    void SET_FrameSlotPositionsAndContents()
    {
        if (currentFrameLayout != null)
        {
            DefineComponentOptions();

            Set_Frame_PositionAndContents();
            Set_Front_Engine_PositionsAndContents();
            Set_Back_Engine_PositionsAndContents();
            Set_Back_Extra_PositionsAndContents();
            Set_Extra_Top_PositionsAndContents();
            Set_Extra_PositionsAndContents();
        }
    }

    void UpdateComponents(ComponentSlotPosition slotType, string componentId)
    {

        /// SEARCH FOR THIS, AT THIS POINT
        /// 
        Debug.Log("ID: " + componentId);
        if (!componentSlotPositions.TryGetValue(slotType, out var slot))
        {
            Debug.Log("Update Component NOT OK");
            return;
        }

        Debug.Log("Update Component YES OK");
        slot.selectedComponentId = componentId;
    }



    void Set_ComponentPosition(ComponentSlotPosition slotPosition, string replacementComponentId)
    {
        if (componentSlotPositions.TryGetValue(slotPosition, out var position))
        {
            var newId = replacementComponentId;
            if (position.selectedComponentId != newId)
            {
                UpdateComponents(slotPosition, newId);      
            }
        }
        UpdateUIElements();
    }

    

    



    void SetFrameSlot(ComponentSlotPosition pos, Transform t, Dictionary<string, GameObject> slotComponents)
    {
        string previousId = null;
        if(componentSlotPositions.TryGetValue(pos, out var existing))
        {
            previousId = existing.selectedComponentId; 
        }

        componentSlotPositions[pos] = new ComponentSlot()
        {
            position = t,
            components = slotComponents,
            selectedComponentId = previousId ?? "EMPTY"
        };

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


    public void CheckIfLightFrame()
    {
        currentFrameIsLight = false;

        if (componentSlotPositions.TryGetValue(ComponentSlotPosition.Frame, out var frame))
        {
            currentFrameIsLight = IdToEnum(frame.selectedComponentId) == ComponentName.LIGHT_FRAME;
        }

    }

   

    public void CheckIfHeavyFrame()
    {
        currentFrameIsHeavy = false;

        if(componentSlotPositions.TryGetValue(ComponentSlotPosition.Frame, out var frame))
        {
            currentFrameIsHeavy = IdToEnum(frame.selectedComponentId) == ComponentName.HEAVY_FRAME;
        }

        
    }

    string GetComponentName(int val)
    {
        string replacementComponent;

        switch (val)
        {
            case 0: replacementComponent = "ENGINE"; break;
            case 1: replacementComponent = "JET_ENGINE"; break;
            case 2: replacementComponent = "AIREON"; break;
            case 3: replacementComponent = "FUEL_TANK"; break;
            case 4: replacementComponent = "BOOST_GULP"; break;
            case 5: replacementComponent = "MACHINE_GUN"; break;
            case 6: replacementComponent = "MISSILE_LAUNCHER"; break;
            default: replacementComponent = "EMPTY"; break;
        }

        return replacementComponent;
    }

   

    
    string GetFrameType(int val)
    {
        string replacementComponent;

        switch (val)
        {
            case 0: replacementComponent = "FRAME_LIGHT"; break;
            case 1: replacementComponent = "FRAME_MEDIUM"; break;
            case 2: replacementComponent = "FRAME_HEAVY"; break;
            default: replacementComponent = "EMPTY"; break;
        }

        return replacementComponent;
    }

  

  

    public void UpdateComponentSlot(ComponentSlotPosition slotPosition, int val)
    {
        string replacementComponent = slotPosition switch
        {
            ComponentSlotPosition.Frame => GetFrameType(val),
            _ => GetComponentName(val),
        };

        if(slotPosition == ComponentSlotPosition.Frame)
        {
           // Place_Frame(replacementComponent);
        }
    }

    public void UpdateComponentSlot_FRAME(int val) => UpdateComponentSlot(ComponentSlotPosition.Frame, val);

    public void UpdateComponentSlot_FL(int val)
    {

        string chosenComponent = engineComponentKeys[val];
        Set_ComponentPosition(ComponentSlotPosition.FrontLeft, chosenComponent);
    }

    public void UpdateComponentSlot_FR(int val)
    {
        string chosenComponent = engineComponentKeys[val];
        Set_ComponentPosition(ComponentSlotPosition.FrontRight, chosenComponent);
    }

    public void UpdateComponentSlot_BL(int val)
    {
        string chosenComponent = engineComponentKeys[val];
        Set_ComponentPosition(ComponentSlotPosition.BackLeft, chosenComponent);
    }

    public void UpdateComponentSlot_BR(int val)
    {
        string chosenComponent = engineComponentKeys[val];
        Set_ComponentPosition(ComponentSlotPosition.BackRight, chosenComponent);
    }

    public void UpdateComponentSlot_BL1(int val)
    {
        string chosenComponent = engineComponentKeys[val];
        Set_ComponentPosition(ComponentSlotPosition.BackLeft1, chosenComponent);
    }
    public void UpdateComponentSlot_BR1(int val)
    {
        string chosenComponent = engineComponentKeys[val];
        Set_ComponentPosition(ComponentSlotPosition.BackRight1, chosenComponent);
    }

    public void UpdateComponentSlot_ExtraFront(int val)
    {
        string chosenComponent = engineComponentKeys[val];
        Set_ComponentPosition(ComponentSlotPosition.ExtraTop, chosenComponent);
    }

    public void UpdateComponentSlot_ExtraLeft(int val)
    {
        string chosenComponent = engineComponentKeys[val];
        Set_ComponentPosition(ComponentSlotPosition.ExtraLeft, chosenComponent);
    }

    public void UpdateComponentSlot_ExtraRight(int val)
    {
        string chosenComponent = engineComponentKeys[val];
        Set_ComponentPosition(ComponentSlotPosition.ExtraRight, chosenComponent);
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
        var shipLoadout = new Dictionary<ComponentSlotPosition, string>();

        foreach (var pair in componentSlotPositions)
        {
            shipLoadout[pair.Key] = pair.Value.selectedComponentId;
        }

        SHIP_PASSPORT.SetShipLoadout(shipLoadout);
    }

    static string EnumToId(ComponentName n) => n.ToString();
    static ComponentName IdToEnum(string id) => Enum.TryParse(id, out ComponentName e) ? e : ComponentName.EMPTY;

    

    

    Dictionary<string, GameObject> ToIdDict(Dictionary<ComponentName, GameObject> source)
    {
        var d = new Dictionary<string, GameObject>();
        foreach(var kv in source) d[kv.Key.ToString()] = kv.Value;
        return d;
    }


    //////////////////// SETUPS ETC - OUT OF SIGHT OUT OF MIND
    void Get_SCRIPT_References()
    {
        
        SCRIPT_shipComponentsList_Controller = this.GetComponent<ShipComponentsList_Controller>();
        SCRIPT_MeshDisplayController = this.GetComponent<MeshDisplayController>();
        SCRIPT_ShipStatsUI_Updater = this.GetComponent<ShipStatsUI_Updater>();
        SCRIPT_ShipComponentsDropdownGenerator = this.GetComponent<ShipComponents_DropdownGenerator>();
    }
    void Create_DropDownReferences()
    {
        SCRIPT_ShipComponentsDropdownGenerator.CreateDropdownOptions();
    }

    void Get_LoadoutFrom_ShipPassport()
    {
        SHIP_PASSPORT = GameObject.Find("ShipPassport").GetComponent<Ship_Passport>();
        receivedShipLoadout = SHIP_PASSPORT.GetShipLoadout();
    }

    void UpdateUIElements()
    {
        SCRIPT_MeshDisplayController.DisplayComponentMeshes(componentSlotPositions);
        SCRIPT_shipComponentsList_Controller.ExposeComponentsAsList(componentsList, componentSlotPositions);
        SCRIPT_ShipStatsUI_Updater.UpdateShipStats(componentSlotPositions);
    }

    Dictionary<string, GameObject> ToPrefabDict(IReadOnlyList<ComponentDefinition> defs)
    {
        var d = new Dictionary<string, GameObject>();
        foreach (var def in defs) d[def.id] = def.prefab;
        d["Empty"] = null; // if you want the empty option
        return d;
    }


    void DefineComponentOptions()
    {
        frameComponentOptions = ToPrefabDict(SCRIPT_ComponentCatalogue.GetByCategory(ComponentCategory.Frame));
        engineComponentOptions = ToPrefabDict(SCRIPT_ComponentCatalogue.GetByCategory(ComponentCategory.Engine));
        extraTopComponentOptions = ToPrefabDict(SCRIPT_ComponentCatalogue.GetByCategory(ComponentCategory.ExtraTop));
        extraComponentOptions = ToPrefabDict(SCRIPT_ComponentCatalogue.GetByCategory(ComponentCategory.Extra));
    }

    void Set_Frame_PositionAndContents()
    {
        SetFrameSlot(ComponentSlotPosition.Frame, framePosition, frameComponentOptions);
    }
    void Set_Front_Engine_PositionsAndContents()
    {
        SetFrameSlot(ComponentSlotPosition.FrontLeft, currentFrameLayout.GetFrontLeftPosition(), engineComponentOptions);
        SetFrameSlot(ComponentSlotPosition.FrontRight, currentFrameLayout.GetFrontRightPosition(), engineComponentOptions);
    }

    void Set_Back_Engine_PositionsAndContents()
    {
        SetFrameSlot(ComponentSlotPosition.BackLeft, currentFrameLayout.GetBackLeftPosition(), engineComponentOptions);
        SetFrameSlot(ComponentSlotPosition.BackRight, currentFrameLayout.GetBackRightPosition(), engineComponentOptions);
    }

    void Set_Back_Extra_PositionsAndContents()
    {
        if (currentFrame.hasExtraBackEngineSlots)
        {
            SetFrameSlot(ComponentSlotPosition.BackLeft1, currentFrameLayout.GetBackLeft1Position(), engineComponentOptions);
            SetFrameSlot(ComponentSlotPosition.BackRight1, currentFrameLayout.GetBackRight1Position(), engineComponentOptions);
        }
    }

    void Set_Extra_Top_PositionsAndContents()
    {
        SetFrameSlot(ComponentSlotPosition.ExtraTop, currentFrameLayout.GetExtraTopPosition(), extraTopComponentOptions);
    }

    void Set_Extra_PositionsAndContents()
    {
        SetFrameSlot(ComponentSlotPosition.ExtraLeft, currentFrameLayout.GetExtraLeftPosition(), extraComponentOptions);
        SetFrameSlot(ComponentSlotPosition.ExtraRight, currentFrameLayout.GetExtraRightPosition(), extraComponentOptions);
    }
}

