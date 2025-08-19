using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.ComponentModel;
using NUnit.Framework;
public class ShipComponentsList_Controller : MonoBehaviour
{

    public GameObject[] UI_ComponenentListElements;
    List<SlotState> newComponentsList = new List<SlotState>();
    [SerializeField] private ComponentCatalogue componentCatalogue;
    

    


    public void ExposeComponentsAsList(Dictionary<ComponentSlotPosition, SlotState> componentSlotPositions)
    {
        
        newComponentsList.Clear();

        foreach (var componentSlot in componentSlotPositions)
        {

            newComponentsList.Add(componentSlot.Value);
        }

        ClearElementContents();
        BuildNewUIList(newComponentsList);
    }

    void BuildNewUIList(List<SlotState> componentsList)
    {
        if (componentCatalogue == null)
        {
            Debug.LogError("ShipComponentsList_Controller: componentCatalogue is not assigned.");
            return;
        }

        List<string> filledSlots = new List<string>();
        List<string> emptySlots = new List<string>();


        foreach (var componentSlot in componentsList)
        {
            // 1) No ID -> treat as empty
            if (string.IsNullOrEmpty(componentSlot.selectedId))
            {
                emptySlots.Add("EMPTY");
                continue;
            }
            // 2) Lookup safely
            var def = componentCatalogue.GetById(componentSlot.selectedId);

            // 3) Unknown ID or no display name -> treat as empty
            if (def == null || string.IsNullOrEmpty(def.displayName))
            {
                emptySlots.Add("EMPTY");
            }
            else
            {
                filledSlots.Add(def.displayName);
            }
        }

        // Filled first, then empties
        var finalSlots = new List<string>(filledSlots.Count + emptySlots.Count);
        finalSlots.AddRange(filledSlots);
        finalSlots.AddRange(emptySlots);

        // Don’t assume you have the same number of UI elements as slots
        int count = Mathf.Min(finalSlots.Count, UI_ComponenentListElements?.Length ?? 0);


        for (int i = 0; i < count; i++)
        {
            var go = UI_ComponenentListElements[i];
            if (go == null) { Debug.LogWarning($"UI element {i} is null."); continue; }

            var elementText = go.GetComponent<TextMeshProUGUI>();
            if (elementText == null) { Debug.LogWarning($"No TextMeshProUGUI on element {i}."); continue; }

            var name = finalSlots[i];
            elementText.text = (name == "EMPTY" || name == "Empty") ? "" : name;
        }

        // If you have *more* UI elements than items, blank the rest
        for (int i = count; i < (UI_ComponenentListElements?.Length ?? 0); i++)
        {
            var elementText = UI_ComponenentListElements[i]?.GetComponent<TextMeshProUGUI>();
            if (elementText != null) elementText.text = "";
        }


    }

 


    void ClearElementContents()
    {
        if (UI_ComponenentListElements == null) return;

        foreach (var textElement in UI_ComponenentListElements)
        {
            var tmp = textElement?.GetComponent<TextMeshProUGUI>();
            if (tmp != null) tmp.text = "";
        }
    }
}
