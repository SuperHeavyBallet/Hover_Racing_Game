using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class ShipComponents_DropdownGenerator : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

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

    public ComponentCatalogue catalogueAsset;

    public void CreateDropdownOptions()
    {
        catalogueAsset.EnsureBuilt();

        SetDropdownOptions(frameDropdown, AssembleDropDown(ComponentCategory.Frame), 1);

        SetDropdownOptions(frontLeftDropdown, AssembleDropDown(ComponentCategory.Engine), 1);
        SetDropdownOptions(frontRightDropdown, AssembleDropDown(ComponentCategory.Engine), 1);
        SetDropdownOptions(backLeftDropdown, AssembleDropDown(ComponentCategory.Engine), 1);
        SetDropdownOptions(backRightDropdown, AssembleDropDown(ComponentCategory.Engine), 1);
        SetDropdownOptions(backLeft1Dropdown, AssembleDropDown(ComponentCategory.Engine), AssembleDropDown(ComponentCategory.Engine).Count - 1);
        SetDropdownOptions(backRight1Dropdown, AssembleDropDown(ComponentCategory.Engine), AssembleDropDown(ComponentCategory.Engine).Count - 1);
        SetDropdownOptions(extraTopDropdown, AssembleDropDown(ComponentCategory.ExtraTop), 0);
        SetDropdownOptions(extraLeftDropdown, AssembleDropDown(ComponentCategory.Extra), AssembleDropDown(ComponentCategory.Extra).Count - 1);
        SetDropdownOptions(extraRightDropdown, AssembleDropDown(ComponentCategory.Extra), AssembleDropDown(ComponentCategory.Extra).Count - 1);
    }

    void SetDropdownOptions(TMP_Dropdown dropdown, List<string> options, int defaultIndex = 0)
    {
        dropdown.ClearOptions();
        dropdown.AddOptions(options);

        dropdown.value = Mathf.Clamp(defaultIndex, 0, options.Count - 1);
        dropdown.RefreshShownValue();
    }


    List<string> AssembleDropDown(ComponentCategory componentCategory)
    {
        

        List<string> options = new();

        foreach (ComponentDefinition componentDefinition in catalogueAsset.GetByCategory(componentCategory))
        {
            string name = componentDefinition.displayName;

            if (!options.Contains(name))
            {
                options.Add(name);
            }

        }

        return options;

    }

}
