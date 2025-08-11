using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.ComponentModel;
public class ShipComponentsList_Controller : MonoBehaviour
{

    public GameObject[] UI_ComponenentListElements;
    List<string> newComponentsList = new List<string>();



    public void ExposeComponentsAsList(List<string> componentsList, Dictionary<ComponentSlotPosition, SlotState> componentSlotPositions)
    {
        //componentsList.Clear(); // <--- Important
        newComponentsList.Clear();

        foreach (var component in componentsList)
        {

            newComponentsList.Add(component);
        }

        ClearElementContents();
        BuildNewUIList(componentsList);
    }

    void BuildNewUIList(List<string> componentsList)
    {

        string[] componentsListArray = componentsList.ToArray();

        List<string> populatedComponents = new List<string>();
        List<string> emptyComponents = new List<string>();


        foreach (var component in componentsList)
        {
            if (component == "EMPTY")
            {
                emptyComponents.Add(component);
            }
            else
            {
                populatedComponents.Add(component);
            }
        }

        List<string> completedList = new List<string>();

        foreach (string component in populatedComponents)
        {
            completedList.Add(component);
        }

        foreach (string component in emptyComponents)
        {
            completedList.Add(component);
        }

        componentsListArray = completedList.ToArray();


        for (int i = 0; i < componentsListArray.Length; i++)
        {
            TextMeshProUGUI elementText = UI_ComponenentListElements[i].GetComponent<TextMeshProUGUI>();
            if (elementText != null)
            {
                if (componentsListArray[i] == "EMPTY")
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
