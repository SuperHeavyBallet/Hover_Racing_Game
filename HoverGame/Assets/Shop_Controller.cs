using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Shop_Controller : MonoBehaviour
{
    [SerializeField] private ComponentCatalogue componentCatalogue;

    private List<ComponentDefinition> allComponents = new();

   [SerializeField] private GameObject ShopCatalogueHolder;
    [SerializeField] private GameObject ComponentListingPrefab;
    [SerializeField] private GameObject ComponentRowPrefab;

    private GameObject[] componentRows;
    private int numberOfComponents;
    private int numberOfRows;

    public GameObject[] rowSlots;
    public List<GameObject> readyComponentSlots;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetAllComponents();
        BuildRows();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void BuildRows()
    {
        numberOfComponents = allComponents.Count;
        List<GameObject> listOfAllPanels = new List<GameObject>();
        numberOfRows = Mathf.CeilToInt(numberOfComponents / 6f);

        for (int i = 0; i < numberOfRows; i++)
        {
            if (i >= rowSlots.Length)
            {
                Debug.LogWarning($"Row index {i} is out of bounds of rowSlots (length {rowSlots.Length})");
                break; 
            }

            GameObject newRow = Instantiate(ComponentRowPrefab, rowSlots[i].transform);

            ComponentRowController newRowController = newRow.GetComponent<ComponentRowController>();
            if(newRowController != null )
            {
                GameObject[] newRowComponentPanels = newRowController.GetComponentPanels();
                foreach (GameObject panel in newRowComponentPanels)
                {
                    listOfAllPanels.Add(panel);
                }
            }
        }

        GameObject[] arrayOfAllPanels = listOfAllPanels.ToArray();
        ComponentDefinition[] arrayOfAllComponents = allComponents.ToArray();

        for (int i = 0;i < arrayOfAllComponents.Length;i++)
        {
            ComponentPanelController newPanelController = arrayOfAllPanels[i].GetComponent<ComponentPanelController>();

            if(newPanelController != null )
            {
                ComponentDefinition component = arrayOfAllComponents[i];
                newPanelController.BuildComponent(component.icon, component.displayName, component.cost);
            }
        }
    }
    void BuildComponentListing()
    {
    }

    void BuildShop()
    {
        foreach(var component in allComponents)
        {
            GameObject newComponentListing = Instantiate(ComponentListingPrefab, ShopCatalogueHolder.transform);
            newComponentListing.name = component.displayName;
            ComponentPanelController newComponentController = newComponentListing.GetComponent<ComponentPanelController>();

            if(newComponentController != null)
            {
                newComponentController.BuildComponent(component.icon, component.displayName, component.cost);
            }

        }
    }

    void GetAllComponents()
    {
        allComponents.Clear();
        allComponents = componentCatalogue.GetListOfAllComponents();

    }

    public void LoadBuilder(string sceneName)
    {
        SceneManager.LoadSceneAsync(sceneName);
    }
}
