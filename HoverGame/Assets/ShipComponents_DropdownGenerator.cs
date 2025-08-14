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

    [SerializeField] private ComponentCatalogue catalogueAsset;



    Ship_Passport SHIP_PASSPORT;

    List<ComponentDefinition> unlockedComponents = new List<ComponentDefinition>();


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
        catalogueAsset.EnsureBuilt();

        unlockedComponents = FetchUnlockedList();

        SetDropdownOptions(frameDropdown, AssembleDropDown(ComponentCategory.Frame), 1);

        SetDropdownOptions(frontLeftDropdown, AssembleDropDown(ComponentCategory.Engine), AssembleDropDown(ComponentCategory.Engine).Count - 1);
        SetDropdownOptions(frontRightDropdown, AssembleDropDown(ComponentCategory.Engine), AssembleDropDown(ComponentCategory.Engine).Count - 1);
        SetDropdownOptions(backLeftDropdown, AssembleDropDown(ComponentCategory.Engine), AssembleDropDown(ComponentCategory.Engine).Count - 1);
        SetDropdownOptions(backRightDropdown, AssembleDropDown(ComponentCategory.Engine), AssembleDropDown(ComponentCategory.Engine).Count - 1);
        SetDropdownOptions(backLeft1Dropdown, AssembleDropDown(ComponentCategory.Engine), AssembleDropDown(ComponentCategory.Engine).Count - 1);
        SetDropdownOptions(backRight1Dropdown, AssembleDropDown(ComponentCategory.Engine), AssembleDropDown(ComponentCategory.Engine).Count - 1);
        SetDropdownOptions(extraTopDropdown, AssembleDropDown(ComponentCategory.ExtraTop), AssembleDropDown(ComponentCategory.Engine).Count - 1);
        SetDropdownOptions(extraLeftDropdown, AssembleDropDown(ComponentCategory.Extra), AssembleDropDown(ComponentCategory.Extra).Count - 1);
        SetDropdownOptions(extraRightDropdown, AssembleDropDown(ComponentCategory.Extra), AssembleDropDown(ComponentCategory.Extra).Count - 1);
    }

    void SetDropdownOptions(TMP_Dropdown dropdown, List<string> options, int defaultIndex = 0)
    {
        dropdown.ClearOptions();
        dropdown.AddOptions(options);



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
        string componentName = catalogueAsset.GetById(selectedId).displayName;
        int index = dropdown.options.FindIndex(option => option.text == componentName);
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
        List<string> dropdownOptions = new();

        foreach (ComponentDefinition componentDefinition in catalogueAsset.GetByCategory(componentCategory))
        {
            bool componentUnlocked = false;
            string componentName = componentDefinition.displayName;

            foreach (ComponentDefinition component in unlockedComponents)
            {
                Debug.Log("CHECK: " + componentName + " : " + component.displayName);

                if (component.displayName == componentName)
                {
                    componentUnlocked = true;
                }
            }


            if (!dropdownOptions.Contains(componentName) && componentUnlocked)
            {
                dropdownOptions.Add(componentName);
            }
            else
            {
                Debug.Log("NOT UNLOCKED! : " + componentName);
            }
        }

        if(componentCategory != ComponentCategory.Frame)
        {
            dropdownOptions.Add("Empty");
        }

        return dropdownOptions;

    }

}
