using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UIElements;

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

    public Dictionary<ComponentSlotPosition, SlotState> componentSlotPositions = new();
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

    const string EMPTY_ID = "EMPTY";
    #endregion

    void Awake()
    {
        Get_SCRIPT_References();
        SCRIPT_ComponentCatalogue.EnsureBuilt();
        CheckExistingShip();
        Create_DropDownReferences();
    }

    // IMPORTANT FUNCTIONS
    public void CheckExistingShip()
    {
        Get_LoadoutFrom_ShipPassport();

        if (receivedShipLoadout != null)
        {
            
            GenerateShipModel();
        }
    }
    
    public void GenerateShipModel()
    {
        // Good To this point
        // 1) Frame first (explicit)
        if (receivedShipLoadout.TryGetValue(ComponentSlotPosition.Frame, out var frameId))
        {
            Place_Frame(frameId); // creates all slots in componentSlotPositions
        }
        else
        {
            Debug.LogWarning("Loadout missing Frame; using default.");
            // optionally: Place_Frame("FRAME_MEDIUM");
        }

        // 2) Then apply the rest
        foreach (var kvp in receivedShipLoadout)
        {
            if (kvp.Key == ComponentSlotPosition.Frame) continue;
            Set_Component_At_Position(kvp.Key, kvp.Value);
        }
 
    }

    void Place_Frame(string newComponentId)
    {
        //Set, Create and Update for new frame
        currentFrame = DecideFrame(newComponentId);
        currentFrameInstance = SCRIPT_MeshDisplayController.InstantiatePrefabAtPosition(currentFrame.prefab, framePosition);
        UpdateFrameLayout();

        // Define the positions and contents of each component slot per current frame
        SET_FrameSlotPositionsAndContents();

        UpdateComponents(ComponentSlotPosition.Frame, newComponentId);

        CheckIfHeavyFrame();
        SetOptionsForExtraSlots(currentFrameIsHeavy);

        // Maybe Good To this point - Potential Issue with Updating Ship Stats, Wait until Components added to check if it is actually displaying
        UpdateUIElements();
    }

    ComponentDefinition DecideFrame(string frameId)
    {
        ComponentDefinition chosenFrame;

        chosenFrame = SCRIPT_ComponentCatalogue.GetById(frameId);

        return chosenFrame;
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

    void UpdateComponents(ComponentSlotPosition slotPosition, string newComponentId)
    {
        if (componentSlotPositions.TryGetValue(slotPosition, out var slot))
        {
            slot.selectedId = newComponentId;
        }    
    }

    void Set_Component_At_Position(ComponentSlotPosition slotPosition, string replacementComponentId)
    {

        if(slotPosition == ComponentSlotPosition.Frame) return;

        if (!componentSlotPositions.TryGetValue(slotPosition, out var slot)) return;

       

        Debug.Log("CHECK EMPTY: " + replacementComponentId);


        if (componentSlotPositions.TryGetValue(slotPosition, out var position))
        {
            string currentComponentId = position.selectedId;
           

            Debug.Log("HERE: " + replacementComponentId);

            
            ComponentDefinition componentDef = SCRIPT_ComponentCatalogue.GetById(replacementComponentId);
            GameObject prefab = componentDef.prefab;

            GameObject newCom = Instantiate(prefab, framePosition);

            //SCRIPT_MeshDisplayController.InstantiatePrefabAtPosition(prefab, position.position);

            UpdateComponents(slotPosition, replacementComponentId);
                
        }

        UpdateUIElements();
 
    }

    

    



    void Set_FrameSlot_Definitions(ComponentSlotPosition componentSlotPosition, Transform position, IReadOnlyList<ComponentDefinition> allowedOptions)
    {
        var possibleOptions = new Dictionary<string, ComponentDefinition>(allowedOptions.Count);
        foreach(var definition in allowedOptions)
        {
            possibleOptions[definition.id] = definition;
        }

        // Preferred: wantedId from saved loadout
        receivedShipLoadout.TryGetValue(componentSlotPosition, out var wantedId);

        // Secondary: previousId if we’re rebuilding this slot (e.g., after changing frame)
        componentSlotPositions.TryGetValue(componentSlotPosition, out var existing);

        string selected;
        var previousId = existing?.selectedId;

        if (!string.IsNullOrEmpty(wantedId) && possibleOptions.ContainsKey(wantedId))
        {
            selected = wantedId;
        }
        else if(!string.IsNullOrEmpty(previousId) && possibleOptions.ContainsKey(previousId))
        {
            selected = previousId;
        }
        else
        {
            selected = EMPTY_ID;
        }

        componentSlotPositions[componentSlotPosition] = new SlotState
        {
           position = position,
           optionsById = possibleOptions,
           selectedId = selected
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

    public void CheckIfHeavyFrame()
    {
        currentFrameIsHeavy = currentFrame.hasExtraBackEngineSlots;
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
            default: replacementComponent = EMPTY_ID; break;
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
            default: replacementComponent = EMPTY_ID; break;
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
           Place_Frame(replacementComponent);
        }
    }

    public void UpdateComponentSlot_FRAME(int val) => UpdateComponentSlot(ComponentSlotPosition.Frame, val);
    /*
    public void UpdateComponentSlot_FL(int val)
    {

        string chosenComponent = engineComponentKeys[val];
        Set_Component_At_Position(ComponentSlotPosition.FrontLeft, chosenComponent);
    }

    public void UpdateComponentSlot_FR(int val)
    {
        string chosenComponent = engineComponentKeys[val];
        Set_Component_At_Position(ComponentSlotPosition.FrontRight, chosenComponent);
    }

    public void UpdateComponentSlot_BL(int val)
    {
        string chosenComponent = engineComponentKeys[val];
        Set_Component_At_Position(ComponentSlotPosition.BackLeft, chosenComponent);
    }

    public void UpdateComponentSlot_BR(int val)
    {
        string chosenComponent = engineComponentKeys[val];
        Set_Component_At_Position(ComponentSlotPosition.BackRight, chosenComponent);
    }

    public void UpdateComponentSlot_BL1(int val)
    {
        string chosenComponent = engineComponentKeys[val];
        Set_Component_At_Position(ComponentSlotPosition.BackLeft1, chosenComponent);
    }
    public void UpdateComponentSlot_BR1(int val)
    {
        string chosenComponent = engineComponentKeys[val];
        Set_Component_At_Position(ComponentSlotPosition.BackRight1, chosenComponent);
    }

    public void UpdateComponentSlot_ExtraFront(int val)
    {
        string chosenComponent = engineComponentKeys[val];
        Set_Component_At_Position(ComponentSlotPosition.ExtraTop, chosenComponent);
    }

    public void UpdateComponentSlot_ExtraLeft(int val)
    {
        string chosenComponent = engineComponentKeys[val];
        Set_Component_At_Position(ComponentSlotPosition.ExtraLeft, chosenComponent);
    }

    public void UpdateComponentSlot_ExtraRight(int val)
    {
        string chosenComponent = engineComponentKeys[val];
        Set_Component_At_Position(ComponentSlotPosition.ExtraRight, chosenComponent);
    }

    */



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
            shipLoadout[pair.Key] = pair.Value.selectedId;
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
        BuildComponentsList();
        SCRIPT_shipComponentsList_Controller.ExposeComponentsAsList(componentsList, componentSlotPositions);
        SCRIPT_ShipStatsUI_Updater.UpdateShipStats(componentSlotPositions);
    }

    void BuildComponentsList()
    {
        // Make sure lookups exist
        // (Do this once in Awake ideally, but having it here is a safe guard)
        SCRIPT_ComponentCatalogue.EnsureBuilt();

        componentsList.Clear();

        foreach (var kvp in componentSlotPositions)
        {
            string id = kvp.Value.selectedId;



            var def = SCRIPT_ComponentCatalogue.GetById(id);
            if (def == null)
            {
                continue;
            }

      
            componentsList.Add(def.displayName);
        }
    }

    Dictionary<string, GameObject> ToPrefabDict(IReadOnlyList<ComponentDefinition> defs)
    {
        var d = new Dictionary<string, GameObject>();
        foreach (var def in defs) d[def.id] = def.prefab;
        d[EMPTY_ID] = null; // if you want the empty option
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
        Set_FrameSlot_Definitions(ComponentSlotPosition.Frame, framePosition, SCRIPT_ComponentCatalogue.GetByCategory(ComponentCategory.Frame));
    }
    void Set_Front_Engine_PositionsAndContents()
    {
        var engines = SCRIPT_ComponentCatalogue.GetByCategory(ComponentCategory.Engine);
        Set_FrameSlot_Definitions(ComponentSlotPosition.FrontLeft, currentFrameLayout.GetFrontLeftPosition(), engines);
        Set_FrameSlot_Definitions(ComponentSlotPosition.FrontRight, currentFrameLayout.GetFrontRightPosition(), engines);
    }

    void Set_Back_Engine_PositionsAndContents()
    {
        var engines = SCRIPT_ComponentCatalogue.GetByCategory(ComponentCategory.Engine);
        Set_FrameSlot_Definitions(ComponentSlotPosition.BackLeft, currentFrameLayout.GetBackLeftPosition(), engines);
        Set_FrameSlot_Definitions(ComponentSlotPosition.BackRight, currentFrameLayout.GetBackRightPosition(), engines);
    }

    void Set_Back_Extra_PositionsAndContents()
    {
        if (currentFrame.hasExtraBackEngineSlots)
        {
            var engines = SCRIPT_ComponentCatalogue.GetByCategory(ComponentCategory.Engine);
            Set_FrameSlot_Definitions(ComponentSlotPosition.BackLeft1, currentFrameLayout.GetBackLeft1Position(), engines);
            Set_FrameSlot_Definitions(ComponentSlotPosition.BackRight1, currentFrameLayout.GetBackRight1Position(), engines);
        }
    }

    void Set_Extra_Top_PositionsAndContents()
    {
        Set_FrameSlot_Definitions(ComponentSlotPosition.ExtraTop, currentFrameLayout.GetExtraTopPosition(), SCRIPT_ComponentCatalogue.GetByCategory(ComponentCategory.ExtraTop));
    }

    void Set_Extra_PositionsAndContents()
    {
        var extras = SCRIPT_ComponentCatalogue.GetByCategory(ComponentCategory.Extra);
        Set_FrameSlot_Definitions(ComponentSlotPosition.ExtraLeft, currentFrameLayout.GetExtraLeftPosition(), extras);
        Set_FrameSlot_Definitions(ComponentSlotPosition.ExtraRight, currentFrameLayout.GetExtraRightPosition(), extras);
    }
}

