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
        List<string> filledSlots = new List<string>();
        List<string> emptySlots = new List<string>();


        foreach (var componentSlot in componentsList)
        {
          
            
            string displayName = componentCatalogue.GetById(componentSlot.selectedId).displayName;

          

            if (displayName == "EMPTY" || displayName == "Empty")
            {
                emptySlots.Add(displayName);
            }
            else
            {
                filledSlots.Add(displayName);
            }
        }

       List<string> finalSLots = new List<string>();
        finalSLots.AddRange(filledSlots);
        finalSLots.AddRange(emptySlots);

        string[] componentsListArray = finalSLots.ToArray();

        for (int i = 0; i < componentsListArray.Length; i++)
        {
            TextMeshProUGUI elementText = UI_ComponenentListElements[i].GetComponent<TextMeshProUGUI>();
            if (elementText != null)
            {
                if (componentsListArray[i] == "EMPTY" || componentsListArray[i] == "Empty")
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

 


    void ClearElementContents()
    {
        foreach (GameObject textElement in UI_ComponenentListElements)
        {
            TextMeshProUGUI elementText = textElement.GetComponent<TextMeshProUGUI>();
            elementText.text = "";

        }
    }
}
