using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class ShipComponents_DropdownGenerator : MonoBehaviour
{

    [SerializeField] private TMP_Dropdown frameDropdown;
    [SerializeField] private TMP_Dropdown frontLeftDropdown;
    [SerializeField] private TMP_Dropdown frontRightDropdown;
    [SerializeField] private TMP_Dropdown backLeftDropdown;
    [SerializeField] private TMP_Dropdown backRightDropdown;
    [SerializeField] private TMP_Dropdown backLeft1Dropdown;
    [SerializeField] private TMP_Dropdown backRight1Dropdown;
    [SerializeField] private TMP_Dropdown extraTopDropdown;
    [SerializeField] private TMP_Dropdown extraLeftDropdown;
    [SerializeField] private TMP_Dropdown extraRightDropdown;

    [SerializeField] private ComponentCatalogue componentCatalogue;



    Ship_Passport SHIP_PASSPORT;

    List<ComponentDefinition> unlockedComponents = new List<ComponentDefinition>();
    HashSet<string> unlockedDisplayNames = new(); // <- fast membership check


    private void Start()
    {
        
    }

    List<ComponentDefinition> FetchUnlockedList()
    {
        SHIP_PASSPORT = GameObject.Find("ShipPassport").GetComponent<Ship_Passport>();
        return SHIP_PASSPORT.GetUnlockedComponents();
    }

    public void CreateDropdownOptions(Dictionary<ComponentSlotPosition, SlotState> existingSlot)
    {
        if (componentCatalogue == null)
        {
            Debug.LogError("DropdownGenerator: componentCatalogue not assigned.");
            return;
        }

        componentCatalogue.EnsureBuilt();

        // reset caches so we don't double-add between calls
        unlockedComponents.Clear();
        unlockedDisplayNames.Clear();

        unlockedComponents = FetchUnlockedList();

        foreach (var def in unlockedComponents)
        {
            // we choose displayName as the canonical text used in dropdowns
            if (!string.IsNullOrEmpty(def.displayName))
                unlockedDisplayNames.Add(def.displayName);
        }

        // Precompute option lists once
        var frameOptions = AssembleDropDown(ComponentCategory.Frame);
        var engineOptions = AssembleDropDown(ComponentCategory.Engine);
        var extraTopOpts = AssembleDropDown(ComponentCategory.ExtraTop);
        var extraOpts = AssembleDropDown(ComponentCategory.Extra);




        // Set options; pick sensible defaults (e.g., 0 or last = "Empty")
    SetDropdownOptions(frameDropdown,     frameOptions,  Mathf.Min(1, frameOptions.Count - 1));

    SetDropdownOptions(frontLeftDropdown, engineOptions, Mathf.Min(1, engineOptions.Count - 1));              // "Empty" last
    SetDropdownOptions(frontRightDropdown,engineOptions, Mathf.Min(1, engineOptions.Count - 1));
    SetDropdownOptions(backLeftDropdown,  engineOptions, Mathf.Min(1, engineOptions.Count - 1));
    SetDropdownOptions(backRightDropdown, engineOptions, Mathf.Min(1, engineOptions.Count - 1));
    SetDropdownOptions(backLeft1Dropdown, engineOptions, Mathf.Min(1, engineOptions.Count - 1));
    SetDropdownOptions(backRight1Dropdown,engineOptions, Mathf.Min(1, engineOptions.Count - 1));

    SetDropdownOptions(extraTopDropdown,  extraTopOpts, Mathf.Min(1, extraTopOpts.Count - 1));
    SetDropdownOptions(extraLeftDropdown, extraOpts, Mathf.Min(1, extraOpts.Count - 1));
    SetDropdownOptions(extraRightDropdown,extraOpts, Mathf.Min(1, extraOpts.Count - 1));
    }

    void SetDropdownOptions(TMP_Dropdown dropdown, List<string> options, int defaultIndex = 0)
    {
        if (dropdown == null)
            return;

        dropdown.ClearOptions();
        // Add once, not inside a loop
        dropdown.AddOptions(options);

        // clamp and set default index
        if (options.Count == 0)
        {
            dropdown.SetValueWithoutNotify(0);
        }
        else
        {
            defaultIndex = Mathf.Clamp(defaultIndex, 0, options.Count - 1);
            dropdown.SetValueWithoutNotify(defaultIndex);
        }

        dropdown.RefreshShownValue();
    }

    public void Update_ALL_DropdownOptions(Dictionary<ComponentSlotPosition, SlotState> currentSlotComponents)
    {

        foreach(var slot in currentSlotComponents)
        {
            TMP_Dropdown currentDropdown = null;

            switch (slot.Key)
            {
                case ComponentSlotPosition.Frame:
                    currentDropdown = frameDropdown; break;
                case ComponentSlotPosition.FrontLeft:
                    currentDropdown = frontLeftDropdown; break;
                case ComponentSlotPosition.FrontRight:
                    currentDropdown = frontRightDropdown; break;
                case ComponentSlotPosition.BackLeft:
                    currentDropdown = backLeftDropdown; break;
                case ComponentSlotPosition.BackRight:
                    currentDropdown = backRightDropdown; break;
                case ComponentSlotPosition.BackLeft1:
                    currentDropdown = backLeft1Dropdown; break;
                case ComponentSlotPosition.BackRight1:
                    currentDropdown = backRight1Dropdown; break;
                case ComponentSlotPosition.ExtraTop:
                    currentDropdown = extraTopDropdown; break;
                case ComponentSlotPosition.ExtraLeft:
                    currentDropdown = extraLeftDropdown; break;
                case ComponentSlotPosition.ExtraRight:
                    currentDropdown = extraRightDropdown; break;
                default:
                    Debug.LogError("No Valid Dropdown Found for: " + slot.Key); break;

            }
            
            if(currentDropdown != null)
            {
                Update_DropdownCurrentOption(slot.Value.selectedId, currentDropdown);
            }
            
        }
    }

    void Update_DropdownCurrentOption(string selectedId, TMP_Dropdown dropdown)
    {

        if (dropdown == null) return;

        if (componentCatalogue == null)
        {
            Debug.LogError("ShipComponents_DropdownGenerator: componentCatalogue is not assigned.");
            return;
        }

        // Map ids -> display names, with a robust “Empty” fallback
        string emptyId = componentCatalogue.GET_EmptyComponentID_AsString(); // your canonical empty id
        string targetName;

        bool isEmpty =
            string.IsNullOrEmpty(selectedId) ||
            selectedId.Equals(emptyId, System.StringComparison.OrdinalIgnoreCase) ||
            selectedId.Equals("EMPTY", System.StringComparison.OrdinalIgnoreCase) ||
            selectedId.Equals("Empty", System.StringComparison.OrdinalIgnoreCase);

        if (isEmpty)
        {
            targetName = "Empty"; // this is the label you add in AssembleDropDown
        }
        else
        {
            var def = componentCatalogue.GetById(selectedId);
            if (def == null)
            {
                Debug.LogWarning($"Dropdown: unknown component id '{selectedId}'. Selecting 'Empty'.");
                targetName = "Empty";
            }
            else
            {
                targetName = def.displayName;
            }
        }

        int index = dropdown.options.FindIndex(o => o.text == targetName);
        if (index < 0)
        {
            // If “Empty” or the targetName isn’t present, fall back to 0 to avoid another nullref
            index = 0;
        }

        dropdown.SetValueWithoutNotify(index);
        dropdown.RefreshShownValue();


    }

    public void Update_IndividualDropdownOptions( ComponentSlotPosition slotPosition ,int selectedInt)
    {
        TMP_Dropdown dropdown;

        switch (slotPosition)
        { 
            case ComponentSlotPosition.Frame:
                dropdown = frameDropdown; break;
            case ComponentSlotPosition.FrontLeft:
                dropdown = frontLeftDropdown; break;
                case ComponentSlotPosition.FrontRight:
                    dropdown = frontRightDropdown; break;
            case ComponentSlotPosition.BackLeft:
                dropdown = backLeftDropdown; break;
                case ComponentSlotPosition.BackRight:
                    dropdown = backRightDropdown; break;
            case ComponentSlotPosition.BackLeft1:
                dropdown = backLeft1Dropdown; break;
            case ComponentSlotPosition.BackRight1:
                dropdown = backRight1Dropdown; break;
                case ComponentSlotPosition.ExtraTop:
                    dropdown = extraTopDropdown; break;
                case ComponentSlotPosition.ExtraLeft:
                    dropdown = extraLeftDropdown; break;
                case ComponentSlotPosition.ExtraRight:
                    dropdown = extraRightDropdown; break;
            default: Debug.LogError("No Dropdown found for: " + slotPosition); dropdown = null; break;
        }


        if(dropdown != null)
        {
            dropdown.SetValueWithoutNotify(selectedInt);
            dropdown.RefreshShownValue();
        }
        
    }


    List<string> AssembleDropDown(ComponentCategory componentCategory)
    {
        var dropdownOptions = new List<string>();

        // Pull all components in this category
        foreach (var def in componentCatalogue.GetByCategory(componentCategory))
        {
            var name = def.displayName;
            if (string.IsNullOrEmpty(name))
                continue;

            // Only include if unlocked; we’re comparing DISPLAY NAMES to DISPLAY NAMES now
            if (unlockedDisplayNames.Contains(name) && !dropdownOptions.Contains(name))
            {
                dropdownOptions.Add(name);
            }
        }

        // Non-frame slots get a robust "Empty" choice
        if (componentCategory != ComponentCategory.Frame)
            dropdownOptions.Add("Empty");

        return dropdownOptions;

    }

}
