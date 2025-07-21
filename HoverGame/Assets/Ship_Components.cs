using TMPro;
using UnityEngine;

public class Ship_Components : MonoBehaviour
{

    // POTENTIALLY REMOVE THIS ENTIRE SCRIPT

    private GameObject COMPONENT_ShipFrame;
    private GameObject COMPONENT_FrameEngine;

    private GameObject COMPONENT_FrontRight;
    private GameObject COMPONENT_FrontLeft;
    private GameObject COMPONENT_BackRight;
    private GameObject COMPONENT_BackLeft;

    private Transform POSITION_FrontRight;
    private Transform POSITION_FrontLeft;
    private Transform POSITION_BackRight;
    private Transform POSITION_BackLeft;

    private Transform POSITION_FrameEngine;

    public Ship_Movement SCRIPT_shipMovement;

    public ShipClass SCRIPT_shipClass;

    public int totalWeight;












    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    

       
    }

   

    void SpawnFrameEngine()
    {
        GameObject newFrameEngine = Instantiate(COMPONENT_FrameEngine, POSITION_FrameEngine);
        newFrameEngine.gameObject.name = "FRAME_ENGINE";

        EngineController ecf = newFrameEngine.GetComponent<EngineController>();

        if (ecf == null)
        {
            Debug.LogWarning("EngineController missing on LIGHT engine!");
        }
        else
        {
            Debug.Log("Engine Controller is valid");
            SCRIPT_shipMovement.RegisterEngineFireListener(ecf);
        }
        
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SpawnComponent(GameObject component, Transform position)
    {
        GameObject newComponent = Instantiate(component, position);

        EngineController ec = newComponent.GetComponent<EngineController>();
        SCRIPT_shipMovement.RegisterEngineFireListener(ec);
        Debug.Log("Spawning component: " + component.name + " at " + position.name);


        string componentWeight = newComponent.GetComponent<ComponentController>().GetComponentWeight();


        switch(componentWeight)
        {
            case "light":
                totalWeight += 20;
                break;
            case "medium":
                totalWeight += 40;
                break;
            case "heavy":
                totalWeight += 60;
                break;
                default:
                break;
        }


    }
}
