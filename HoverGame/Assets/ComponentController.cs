using UnityEngine;

public class ComponentController : MonoBehaviour
{

    public ShipComponent componentScipt;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       string componentType = componentScipt.componentType.ToString();
       Debug.Log("BRRR " + componentType);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
