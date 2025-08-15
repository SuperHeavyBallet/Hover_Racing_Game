using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Shop_Controller : MonoBehaviour
{
    [SerializeField] private ComponentCatalogue componentCatalogue;

    public List<ComponentDefinition> allComponents = new();

   [SerializeField] private GameObject ShopCatalogueHolder;
    [SerializeField] private GameObject ComponentListingPrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetAllComponents();
        BuildShop();
    }

    // Update is called once per frame
    void Update()
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
