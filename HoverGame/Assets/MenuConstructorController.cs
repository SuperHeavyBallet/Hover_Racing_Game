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
    public Dictionary<ComponentSlotPosition, SlotState> receivedShipLoadout = new();

    ComponentDefinition currentFrame;

    List <Transform> engineSlotPositions = new List<Transform>();
    List <Transform> extraSlotPositions = new List<Transform>();
    Transform extraTopSlotPosition;

    

    [Header("Dictionaries")]
    Dictionary<string, GameObject> OPTIONS_FrameComponents = new();
    Dictionary<string, GameObject> OPTIONS_EngineComponents = new();
    Dictionary<string, GameObject> OPTIONS_ExtraComponents = new();
    Dictionary<string, GameObject> OPTIONS_ExtraTopComponents = new();

    [Header("Lists")]
    List<string> componentsList = new List<string>();
    List<string> frameKeys = new();
    List<string> engineComponentKeys = new();
    List<string> extraTopComponentKeys = new();
    List<string> extraComponentKeys = new();


    [Header("References")]

    private ComponentCatalogue_Wrapper _componentCatalogue_Wrapper;

    //public ComponentCatalogue SCRIPT_ComponentCatalogue; // assign the asset in the inspector
    ShipComponentsList_Controller SCRIPT_shipComponentsList_Controller;
    MeshDisplayController SCRIPT_MeshDisplayController;
    ShipStatsUI_Updater SCRIPT_ShipStatsUI_Updater;
    ShipComponents_DropdownGenerator SCRIPT_ShipComponentsDropdownGenerator;
    MENU_DropdownInterface SCRIPT_Menu_DropdownInterface;

    Menu_FramePlacer SCRIPT_FramePlacer;

    Frame_Layout currentFrameLayout;
    GameObject currentFrameInstance;

    

    public bool currentFrameIsHeavy = false;
    bool _suppressUI;

    string LOCAL_EMPTY_ID;
    string LOCAL_FRAME_LIGHT_ID;
    string LOCAL_FRAME_MEDIUM_ID;
    string LOCAL_FRAME_HEAVY_ID;
    #endregion

    // START PROCEDURE /////////////////////

    #region START PROCEDURE
    void Awake()
    {

        _suppressUI = true;
        SETUP_SET_LocalIDs();
        SETUP_GET_SCRIPT_References();
        //SCRIPT_ComponentCatalogue.EnsureBuilt();
        _componentCatalogue_Wrapper.EnsureBuilt();
        SETUP_GET_LoadoutFrom_ShipPassport();

        if (receivedShipLoadout != null)
        {
            CREATE_ShipModel();
        }

        
        _suppressUI = false;
    }
    
    public void CREATE_ShipModel()
    {
        if (receivedShipLoadout.TryGetValue(ComponentSlotPosition.Frame, out var frameSlot))
        {
            SET_INITIAL_Frame_At_Position(frameSlot.selectedId);
        }

        foreach (var kvp in receivedShipLoadout)
        {
            if (kvp.Key == ComponentSlotPosition.Frame) continue;
            SET_INITIAL_Component_At_Position(kvp.Key, kvp.Value.selectedId);
        }

        SCRIPT_Menu_DropdownInterface.Update_ALL_DropdownOptions(componentSlotPositions);
        UPDATE_UIElements(); // do once here

    }
    #endregion

    // UPDATE FUNCTIONS //////////////////////////////////////////////////////

    #region UPDATE FUNCTIONS

     void UPDATE_AllSlotMeshes_AfterFrameChange()
    {
        foreach(var kvp in componentSlotPositions)
        {
            var slotPos = kvp.Key;
            if (slotPos == ComponentSlotPosition.Frame) continue;

            var slotState = kvp.Value;
            if(slotState?.position == null) continue;

            SCRIPT_MeshDisplayController.CleanupExcessMeshesInSlot(slotState.position);

            var selectedID = slotState.selectedId;
            if(string.IsNullOrEmpty(selectedID) || selectedID == LOCAL_EMPTY_ID)
            {
                // Keep it empty visually
                continue;
            }

            var def = _componentCatalogue_Wrapper?.Get_ComponentBy_ID(selectedID);
            if(def?.prefab == null) continue;

            SCRIPT_MeshDisplayController?.InstantiatePrefabAtPosition(def.prefab, slotState.position);
        }
    }
    void UPDATE_UIElements()
    {
        UPDATE_CREATE_NewComponentsList();
        SCRIPT_shipComponentsList_Controller.ExposeComponentsAsList(componentSlotPositions);
        SCRIPT_ShipStatsUI_Updater.UpdateShipStats(componentSlotPositions);
       
    }

    void UPDATE_CREATE_NewComponentsList()
    {
        _componentCatalogue_Wrapper.EnsureBuilt();
        componentsList.Clear();

        foreach (var kvp in componentSlotPositions)
        {
            string id = kvp.Value.selectedId;
            var def = _componentCatalogue_Wrapper.Get_ComponentBy_ID(id);
            if (def == null)
            {
                continue;
            }
            componentsList.Add(def.displayName);
        }
    }

    void UPDATE_FrameLayout()
    {
        currentFrameLayout = currentFrameInstance.GetComponent<Frame_Layout>();

        if (currentFrameLayout != null)
        {
            UPDATE_SlotPositions(engineSlotPositions, currentFrameLayout.GetEngineSlots());
            UPDATE_SlotPositions(extraSlotPositions, currentFrameLayout.GetExtraSlots());
            UPDATE_ExtraTopSlotPosition(currentFrameLayout);
        }
    }

    void UPDATE_Component(ComponentSlotPosition slotPosition, string newComponentId)
    {
        if (componentSlotPositions.TryGetValue(slotPosition, out var slot))
        {
            slot.selectedId = newComponentId;

        }
    }

    void UPDATE_SlotPositions(List<Transform> slotPositions, Transform[] slots)
    {
        slotPositions.Clear();

        foreach (var slot in slots)
        {
            slotPositions.Add(slot);
        }
    }

    void UPDATE_ExtraTopSlotPosition(Frame_Layout frameLayout)
    {
        extraTopSlotPosition = frameLayout.GetExtraTopSlot();
    }

    void UPDATE_ADD_OptionsTo_KeyList(Dictionary<string, GameObject> options, List<string> keyList)
    {
        foreach (var option in options)
        {
            keyList.Add(option.Key);
        }
    }

    public void UPDATE_Frame(ComponentSlotPosition slotPosition, int val)
    {
        if (slotPosition != ComponentSlotPosition.Frame) return; // Bail out if it's not a frame change
       // Debug.Log("Succesful in UPDATE FRAME: " +  slotPosition + " : " + val);
        string replacementComponent = GET_FrameType(val);
        SET_DYNAMIC_Frame_At_Position(replacementComponent);
    }

    #endregion

    // GET FUNCTIONS //////////////////////////////////////////////////////

    #region GET FUNCTIONS


    public bool GET_SupressUIStatus()
    {
        return _suppressUI;
    }

    public void GET_HeavyFrameStatus()
    {
        currentFrameIsHeavy = currentFrame.hasExtraBackEngineSlots;
    }

    string GET_FrameType(int val)
    {
        string replacementComponent;

        switch (val)
        {
            case 0: replacementComponent = LOCAL_FRAME_LIGHT_ID; break;
            case 1: replacementComponent = LOCAL_FRAME_MEDIUM_ID; break;
            case 2: replacementComponent = LOCAL_FRAME_HEAVY_ID; break;
            default: replacementComponent = LOCAL_EMPTY_ID; break;
        }

        return replacementComponent;
    }

    public string GET_ComponentKeys(ComponentCategory componentCategory, int val)
    {
        List<string> keys;

        switch (componentCategory)
        {
            case ComponentCategory.Frame:
                keys = frameKeys; break;
            case ComponentCategory.Engine:
                keys = engineComponentKeys; break;
            case ComponentCategory.ExtraTop:
                keys = extraTopComponentKeys; break;
            case ComponentCategory.Extra:
                keys = extraComponentKeys; break;
            default: keys = engineComponentKeys; break;

        }
        return keys[val];
    }

    #endregion

    // REFRESH FUNCTIONS //////////////////////////////////////////////////

   

    // SET FUNCTIONS //////////////////////////////////////////////////////

    #region SET FUNCTIONS
    void SET_INITIAL_Frame_At_Position(string newComponentId)
    {

        currentFrame = SET_FrameType(newComponentId);
        currentFrameInstance = SCRIPT_MeshDisplayController.InstantiatePrefabAtPosition(currentFrame.prefab, framePosition);
        UPDATE_FrameLayout();

        SET_FrameSlotPositionsAndContents();

        UPDATE_Component(ComponentSlotPosition.Frame, newComponentId);

        GET_HeavyFrameStatus();
        SET_OptionsForExtraSlots(currentFrameIsHeavy);

        SETUP_CREATE_DropDownReferences();
        UPDATE_UIElements();

    }

    public void SET_DYNAMIC_Frame_At_Position(string newComponentId)
    {
        if (currentFrameInstance != null)
            Destroy(currentFrameInstance);

        currentFrame = SET_FrameType(newComponentId);
        currentFrameInstance = SCRIPT_MeshDisplayController.InstantiatePrefabAtPosition(currentFrame.prefab, framePosition);
        UPDATE_FrameLayout();

        SET_FrameSlotPositionsAndContents();

        UPDATE_Component(ComponentSlotPosition.Frame, newComponentId);

        GET_HeavyFrameStatus();
        SET_OptionsForExtraSlots(currentFrameIsHeavy);
        UPDATE_AllSlotMeshes_AfterFrameChange();
        UPDATE_UIElements();
    }


    ComponentDefinition SET_FrameType(string frameId)
    {
        ComponentDefinition chosenFrame;

        chosenFrame = _componentCatalogue_Wrapper.Get_ComponentBy_ID(frameId);

        return chosenFrame;
    }

    void SET_FrameSlotPositionsAndContents()
    {
        if (currentFrameLayout != null)
        {
            SET_ComponentOptions();

            SET_Frame_PositionAndContents();
            SET_Front_Engine_PositionsAndContents();
            SET_Back_Engine_PositionsAndContents();
            SET_Back_Extra_PositionsAndContents();
            SET_Extra_Top_PositionsAndContents();
            SET_Extra_PositionsAndContents();
        }
    }

    void SET_INITIAL_Component_At_Position(ComponentSlotPosition slotPosition, string requestedComponentId)
    {

        if (slotPosition == ComponentSlotPosition.Frame) return;

        if (!componentSlotPositions.TryGetValue(slotPosition, out var slotState) || slotState == null)
        {
            Debug.LogWarning($"SET_INITIAL_Component_At_Position: SlotState missing for {slotPosition}");
            return;
        }

        // Guard: slot transform must exist
        var slotTransform = slotState.position;
        if (slotTransform == null)
        {
            Debug.LogWarning($"SET_INITIAL_Component_At_Position: Slot Transform is null for {slotPosition}");
            return;
        }

        // If requested is empty or not allowed for this slot, downgrade to LOCAL_EMPTY_ID
        string finalId = requestedComponentId;

        // Not selected or explicitly empty?
        if (string.IsNullOrEmpty(finalId) || finalId == LOCAL_EMPTY_ID)
        {
            finalId = LOCAL_EMPTY_ID;
        }
        else
        {
            // Respect allowed options for this slot (from SET_FrameSlot_Definitions)
            var options = slotState.optionsById;
            if (options == null || !options.ContainsKey(finalId))
            {
                Debug.Log($"SET_INITIAL_Component_At_Position: Requested '{requestedComponentId}' not allowed in {slotPosition}. Using EMPTY.");
                finalId = LOCAL_EMPTY_ID;
            }
        }

        // If EMPTY, just clear any existing meshes in the slot and update the id
        if (finalId == LOCAL_EMPTY_ID)
        {
            SCRIPT_MeshDisplayController?.CleanupExcessMeshesInSlot(slotTransform);
            UPDATE_Component(slotPosition, LOCAL_EMPTY_ID);
            return; // nothing to instantiate
        }

        // Lookup definition safely
        var def = _componentCatalogue_Wrapper?.Get_ComponentBy_ID(finalId);
        if (def == null)
        {
            Debug.LogWarning($"SET_INITIAL_Component_At_Position: Unknown component id '{finalId}' in slot {slotPosition}. Using EMPTY.");
            SCRIPT_MeshDisplayController?.CleanupExcessMeshesInSlot(slotTransform);
            UPDATE_Component(slotPosition, LOCAL_EMPTY_ID);
            return;
        }

        // Instantiate if a prefab exists; otherwise treat as empty visually but keep the id if that’s your design
        if (def.prefab != null)
        {
            SCRIPT_MeshDisplayController?.CleanupExcessMeshesInSlot(slotTransform);
            _ = SCRIPT_MeshDisplayController?.InstantiatePrefabAtPosition(def.prefab, slotTransform);
        }
        else
        {
            // No prefab: clear visuals
            SCRIPT_MeshDisplayController?.CleanupExcessMeshesInSlot(slotTransform);
        }

        UPDATE_Component(slotPosition, def.id);

    }
    public void SET_DYNAMIC_Component_At_Position(ComponentSlotPosition slotPosition, string replacementComponentId)
    {

        if (slotPosition == ComponentSlotPosition.Frame) return;

        if (!componentSlotPositions.TryGetValue(slotPosition, out var slot)) return;

        string existingComponent = slot.selectedId;

      

            if (componentSlotPositions.TryGetValue(slotPosition, out var position))
            {
                string currentComponentId = position.selectedId;
                SCRIPT_MeshDisplayController.CleanupExcessMeshesInSlot(position.position);
                ComponentDefinition componentDef = _componentCatalogue_Wrapper.Get_ComponentBy_ID(replacementComponentId);
                GameObject prefab = componentDef.prefab;

                GameObject newComponent = SCRIPT_MeshDisplayController.InstantiatePrefabAtPosition(prefab, position.position);
            }

            UPDATE_Component(slotPosition, replacementComponentId);

            UPDATE_UIElements();
        


    }

    void SET_FrameSlot_Definitions(ComponentSlotPosition componentSlotPosition, Transform position, IReadOnlyList<ComponentDefinition> allowedOptions)
    {
        var possibleOptions = new Dictionary<string, ComponentDefinition>(allowedOptions.Count);
        foreach (var definition in allowedOptions)
        {
            possibleOptions[definition.id] = definition;
        }

        // Preferred: wantedId from saved loadout
        receivedShipLoadout.TryGetValue(componentSlotPosition, out var wantedId);

        // Secondary: previousId if we’re rebuilding this slot (e.g., after changing frame)
        componentSlotPositions.TryGetValue(componentSlotPosition, out var existing);

        string selected;
        var previousId = existing?.selectedId;

        if (!string.IsNullOrEmpty(wantedId.selectedId) && possibleOptions.ContainsKey(wantedId.selectedId))
        {
            selected = wantedId.selectedId;
        }
        else if (!string.IsNullOrEmpty(previousId) && possibleOptions.ContainsKey(previousId))
        {
            selected = previousId;
        }
        else
        {
            selected = LOCAL_EMPTY_ID;
        }

        componentSlotPositions[componentSlotPosition] = new SlotState
        {
            position = position,
            optionsById = possibleOptions,
            selectedId = selected
        };

    }

    void SET_OptionsForExtraSlots(bool value)
    {
        backLeft1_Dropdown.gameObject.SetActive(value);
        backLeft1Label.gameObject.SetActive(value);
        backRight1_Dropdown.gameObject.SetActive(value);
        backRight1Label.gameObject.SetActive(value);
    }

    public void SEND_ShipLoadout()
    {
        var shipLoadout = new Dictionary<ComponentSlotPosition, string>();

        foreach (var pair in componentSlotPositions)
        {
            shipLoadout[pair.Key] = pair.Value.selectedId;
        }

        SHIP_PASSPORT.SetShipLoadout(componentSlotPositions);
    }




    void SET_ComponentOptions()
    {
        frameKeys.Clear();
        engineComponentKeys.Clear();
        extraTopComponentKeys.Clear();
        extraComponentKeys.Clear();

        OPTIONS_FrameComponents = CONVERT_ToPrefabDict(_componentCatalogue_Wrapper.Get_ComponentBy_Category(ComponentCategory.Frame));
        OPTIONS_EngineComponents = CONVERT_ToPrefabDict(_componentCatalogue_Wrapper.Get_ComponentBy_Category(ComponentCategory.Engine));
        OPTIONS_ExtraTopComponents = CONVERT_ToPrefabDict(_componentCatalogue_Wrapper.Get_ComponentBy_Category(ComponentCategory.ExtraTop));
        OPTIONS_ExtraComponents = CONVERT_ToPrefabDict(_componentCatalogue_Wrapper.Get_ComponentBy_Category(ComponentCategory.Extra));

        UPDATE_ADD_OptionsTo_KeyList(OPTIONS_FrameComponents, frameKeys);
        UPDATE_ADD_OptionsTo_KeyList(OPTIONS_EngineComponents, engineComponentKeys);
        UPDATE_ADD_OptionsTo_KeyList(OPTIONS_ExtraTopComponents, extraTopComponentKeys);
        UPDATE_ADD_OptionsTo_KeyList(OPTIONS_ExtraComponents, extraComponentKeys);
    }



    void SET_Frame_PositionAndContents()
    {
        SET_FrameSlot_Definitions(ComponentSlotPosition.Frame, framePosition, _componentCatalogue_Wrapper.Get_ComponentBy_Category(ComponentCategory.Frame));
    }
    void SET_Front_Engine_PositionsAndContents()
    {
        var engines = _componentCatalogue_Wrapper.Get_ComponentBy_Category(ComponentCategory.Engine);
        SET_FrameSlot_Definitions(ComponentSlotPosition.FrontLeft, currentFrameLayout.GetFrontLeftPosition(), engines);
        SET_FrameSlot_Definitions(ComponentSlotPosition.FrontRight, currentFrameLayout.GetFrontRightPosition(), engines);
    }

    void SET_Back_Engine_PositionsAndContents()
    {
        var engines = _componentCatalogue_Wrapper.Get_ComponentBy_Category(ComponentCategory.Engine);
        SET_FrameSlot_Definitions(ComponentSlotPosition.BackLeft, currentFrameLayout.GetBackLeftPosition(), engines);
        SET_FrameSlot_Definitions(ComponentSlotPosition.BackRight, currentFrameLayout.GetBackRightPosition(), engines);
    }

    void SET_Back_Extra_PositionsAndContents()
    {
        if (currentFrame.hasExtraBackEngineSlots)
        {
            var engines = _componentCatalogue_Wrapper.Get_ComponentBy_Category(ComponentCategory.Engine);
            SET_FrameSlot_Definitions(ComponentSlotPosition.BackLeft1, currentFrameLayout.GetBackLeft1Position(), engines);
            SET_FrameSlot_Definitions(ComponentSlotPosition.BackRight1, currentFrameLayout.GetBackRight1Position(), engines);
        }
    }

    void SET_Extra_Top_PositionsAndContents()
    {
        SET_FrameSlot_Definitions(ComponentSlotPosition.ExtraTop, currentFrameLayout.GetExtraTopPosition(), _componentCatalogue_Wrapper.Get_ComponentBy_Category(ComponentCategory.ExtraTop));
    }

    void SET_Extra_PositionsAndContents()
    {
        var extras = _componentCatalogue_Wrapper.Get_ComponentBy_Category(ComponentCategory.Extra);
        SET_FrameSlot_Definitions(ComponentSlotPosition.ExtraLeft, currentFrameLayout.GetExtraLeftPosition(), extras);
        SET_FrameSlot_Definitions(ComponentSlotPosition.ExtraRight, currentFrameLayout.GetExtraRightPosition(), extras);
    }
    #endregion

    // SETUP FUNCTIONS //////////////////////////////////////////////////////

    #region SetupFunctions
    void SETUP_GET_SCRIPT_References()
    {  
        _componentCatalogue_Wrapper = this.GetComponent<ComponentCatalogue_Wrapper>();

        SCRIPT_shipComponentsList_Controller = this.GetComponent<ShipComponentsList_Controller>();
        SCRIPT_MeshDisplayController = this.GetComponent<MeshDisplayController>();
        SCRIPT_ShipStatsUI_Updater = this.GetComponent<ShipStatsUI_Updater>();
        SCRIPT_ShipComponentsDropdownGenerator = this.GetComponent<ShipComponents_DropdownGenerator>();
        SCRIPT_Menu_DropdownInterface = this.GetComponent<MENU_DropdownInterface>();
        

    }
    void SETUP_CREATE_DropDownReferences()
    {
        SCRIPT_ShipComponentsDropdownGenerator.CreateDropdownOptions(componentSlotPositions);
    }

    void SETUP_GET_LoadoutFrom_ShipPassport()
    {
        SHIP_PASSPORT = GameObject.Find("ShipPassport").GetComponent<Ship_Passport>();
        receivedShipLoadout = SHIP_PASSPORT.GetShipLoadout();
    }

    void SETUP_SET_LocalIDs()
    {
        LOCAL_EMPTY_ID = _componentCatalogue_Wrapper.GET_EmptyComponentID_AsString();
        LOCAL_FRAME_LIGHT_ID = _componentCatalogue_Wrapper.GET_FrameID_AsString("LIGHT");
        LOCAL_FRAME_MEDIUM_ID = _componentCatalogue_Wrapper.GET_FrameID_AsString("MEDIUM");
        LOCAL_FRAME_HEAVY_ID = _componentCatalogue_Wrapper.GET_FrameID_AsString("HEAVY");
    }
    #endregion

    #region
    // CONVERTER FUNCTIONS //////////////////////////////////////////////////////////

    Dictionary<string, GameObject> CONVERT_ToPrefabDict(IReadOnlyList<ComponentDefinition> allowedOptions)
    {
        var dictionary = new Dictionary<string, GameObject>();

        foreach (var componentDefinition in allowedOptions)
        {
            dictionary[componentDefinition.id] = componentDefinition.prefab;
        }

        dictionary[LOCAL_EMPTY_ID] = null; // if you want the empty option
        return dictionary;
    }
    #endregion
}

