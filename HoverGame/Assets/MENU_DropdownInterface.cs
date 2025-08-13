using System.Collections.Generic;
using UnityEngine;

public class MENU_DropdownInterface : MonoBehaviour
{

    private MenuConstructorController controller;
    [SerializeField] private ShipComponents_DropdownGenerator dropdownGenerator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        controller = GetComponent<MenuConstructorController>();
        dropdownGenerator = GetComponent<ShipComponents_DropdownGenerator>();
    }

    void UpdateIndividualDropdown(ComponentSlotPosition slotPosition ,int val)
    {
        dropdownGenerator.Update_IndividualDropdownOptions(slotPosition,val);
    }

    public void Update_ALL_DropdownOptions(Dictionary<ComponentSlotPosition, SlotState> currentSlotComponents)
    {
        dropdownGenerator.Update_ALL_DropdownOptions(currentSlotComponents);
    }

    public void UpdateComponentSlot_FRAME(int val)
    {
        if (controller.GET_SupressUIStatus() == true) return;
        Debug.Log("GIT UPDATE PUSH: " + " : " + val);
        //controller.UPDATE_Frame(ComponentSlotPosition.Frame, val);

        string frameID = controller.GET_ComponentKeys(ComponentCategory.Frame, val);
        controller.SET_DYNAMIC_Frame_At_Position(frameID);
        UpdateIndividualDropdown(ComponentSlotPosition.Frame, val);



    }
    public void UpdateComponentSlot_FL(int val)
    {
        if (controller.GET_SupressUIStatus() == true) return;
        ChooseAndSendComponent(val, ComponentCategory.Engine, ComponentSlotPosition.FrontLeft);
        UpdateIndividualDropdown(ComponentSlotPosition.FrontLeft, val);
    }

    public void UpdateComponentSlot_FR(int val)
    {
        if (controller.GET_SupressUIStatus() == true) return;
        ChooseAndSendComponent(val, ComponentCategory.Engine, ComponentSlotPosition.FrontRight);
        UpdateIndividualDropdown(ComponentSlotPosition.FrontRight, val);
    }

    public void UpdateComponentSlot_BL(int val)
    {
        if(controller.GET_SupressUIStatus() == true) return;
        ChooseAndSendComponent(val, ComponentCategory.Engine, ComponentSlotPosition.BackLeft);
        UpdateIndividualDropdown(ComponentSlotPosition.BackLeft, val);
    }

    public void UpdateComponentSlot_BR(int val)
    {
        if (controller.GET_SupressUIStatus() == true) return;
        ChooseAndSendComponent(val, ComponentCategory.Engine, ComponentSlotPosition.BackRight);
        UpdateIndividualDropdown(ComponentSlotPosition.BackRight, val);
    }

    public void UpdateComponentSlot_BL1(int val)
    {
        if (controller.GET_SupressUIStatus() == true) return;
        ChooseAndSendComponent(val, ComponentCategory.Engine, ComponentSlotPosition.BackLeft1);
        UpdateIndividualDropdown(ComponentSlotPosition.BackLeft1, val);
    }
    public void UpdateComponentSlot_BR1(int val)
    {
        if (controller.GET_SupressUIStatus() == true) return;
        ChooseAndSendComponent(val, ComponentCategory.Engine, ComponentSlotPosition.BackRight1);
        UpdateIndividualDropdown(ComponentSlotPosition.BackRight1, val);
    }

    public void UpdateComponentSlot_ExtraTop(int val)
    {
        if (controller.GET_SupressUIStatus() == true) return;
        ChooseAndSendComponent(val, ComponentCategory.ExtraTop, ComponentSlotPosition.ExtraTop);
        UpdateIndividualDropdown(ComponentSlotPosition.ExtraTop, val);
    }

    public void UpdateComponentSlot_ExtraLeft(int val)
    {
        if (controller.GET_SupressUIStatus() == true) return;
        ChooseAndSendComponent(val, ComponentCategory.Extra, ComponentSlotPosition.ExtraLeft);
        UpdateIndividualDropdown(ComponentSlotPosition.ExtraLeft, val);
    }

    public void UpdateComponentSlot_ExtraRight(int val)
    {
        if (controller.GET_SupressUIStatus() == true) return;
        ChooseAndSendComponent(val, ComponentCategory.Extra, ComponentSlotPosition.ExtraRight);
        UpdateIndividualDropdown(ComponentSlotPosition.ExtraRight, val);
    }

    void ChooseAndSendComponent(int val, ComponentCategory componentCategory, ComponentSlotPosition componentSlotPosition)
    {
        controller.SET_DYNAMIC_Component_At_Position(
            componentSlotPosition, 
            controller.GET_ComponentKeys(componentCategory, val)
            );  
    }

}
