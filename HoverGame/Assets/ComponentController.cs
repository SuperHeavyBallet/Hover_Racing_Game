using UnityEngine;

public class ComponentController : MonoBehaviour
{

    public ShipComponent componentScipt;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       string componentType = componentScipt.componentType.ToString();
      
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public string GetComponentName()
    {
        return componentScipt.componentType.ToString();
    }

    public string GetComponentWeight()
    {
        return componentScipt.componentWeight.ToString();
    }
}
