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

    [Header("Light Ship Components")]
    public GameObject LIGHT_COMPONENT_ShipFrame;
    public GameObject LIGHT_COMPONENT_FrameEngine;

    public GameObject LIGHT_COMPONENT_FrontRight;
    public GameObject LIGHT_COMPONENT_FrontLeft;
    public GameObject LIGHT_COMPONENT_BackRight;
    public GameObject LIGHT_COMPONENT_BackLeft;

    public Transform LIGHT_POSITION_FrontRight;
    public Transform LIGHT_POSITION_FrontLeft;
    public Transform LIGHT_POSITION_BackRight;
    public Transform LIGHT_POSITION_BackLeft;

    public Transform LIGHT_POSITION_FrameEngine;

    [Header("Medium Ship Components")]
    public GameObject MED_COMPONENT_ShipFrame;
    public GameObject MED_COMPONENT_FrameEngine;

    public GameObject MED_COMPONENT_FrontRight;
    public GameObject MED_COMPONENT_FrontLeft;
    public GameObject MED_COMPONENT_BackRight;
    public GameObject MED_COMPONENT_BackLeft;

    public Transform MED_POSITION_FrontRight;
    public Transform MED_POSITION_FrontLeft;
    public Transform MED_POSITION_BackRight;
    public Transform MED_POSITION_BackLeft;

    public Transform MED_POSITION_FrameEngine;

    [Header("Heavy Ship Components")]
    public GameObject HEAVY_COMPONENT_ShipFrame;
    public GameObject HEAVY_COMPONENT_FrameEngine;

    public GameObject HEAVY_COMPONENT_FrontRight;
    public GameObject HEAVY_COMPONENT_FrontLeft;
    public GameObject HEAVY_COMPONENT_BackRight;
    public GameObject HEAVY_COMPONENT_BackLeft;

    public Transform HEAVY_POSITION_FrontRight;
    public Transform HEAVY_POSITION_FrontLeft;
    public Transform HEAVY_POSITION_BackRight;
    public Transform HEAVY_POSITION_BackLeft;

    public Transform HEAVY_POSITION_FrameEngine;

    public string vehicleClass;


    public TextMeshProUGUI shipClassText;





    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       // SelectShip();

       // SpawnComponent(COMPONENT_BackLeft, POSITION_BackLeft);
        //SpawnComponent(COMPONENT_BackRight, POSITION_BackRight);
       // SpawnComponent(COMPONENT_FrontLeft, POSITION_FrontLeft);
       // SpawnComponent(COMPONENT_FrontRight, POSITION_FrontRight);

       // SpawnFrameEngine();

       
    }

    void SelectShip()
    {
        /*
        SceneStartup sceneStartup = SceneStartup.Instance;

        if (sceneStartup != null)
        {
            vehicleClass = sceneStartup.GetVehicleClass();
        }
        else
        {
            vehicleClass = "light";
        }

        shipClassText.text = vehicleClass;  

        if (vehicleClass == "light")
        {
            COMPONENT_FrameEngine = LIGHT_COMPONENT_FrameEngine;
            COMPONENT_FrontLeft = LIGHT_COMPONENT_FrontLeft;
            COMPONENT_FrontRight = LIGHT_COMPONENT_FrontRight;
            COMPONENT_BackLeft = LIGHT_COMPONENT_BackLeft;
            COMPONENT_BackRight = LIGHT_COMPONENT_BackRight;

            POSITION_FrameEngine = LIGHT_POSITION_FrameEngine;
            POSITION_FrontLeft = LIGHT_POSITION_FrontLeft;
            POSITION_FrontRight = LIGHT_POSITION_FrontRight;
            POSITION_BackLeft = LIGHT_POSITION_BackLeft;
            POSITION_BackRight = LIGHT_POSITION_BackRight;
        }
        else if (vehicleClass == "medium")
        {
            COMPONENT_FrameEngine = MED_COMPONENT_FrameEngine;
            COMPONENT_FrontLeft = MED_COMPONENT_FrontLeft;
            COMPONENT_FrontRight= MED_COMPONENT_FrontRight;
            COMPONENT_BackLeft= MED_COMPONENT_BackLeft;
            COMPONENT_BackRight= MED_COMPONENT_BackRight;

            POSITION_FrameEngine= MED_POSITION_FrameEngine;
            POSITION_FrontLeft= MED_POSITION_FrontLeft;
            POSITION_FrontRight= MED_POSITION_FrontRight;
            POSITION_BackLeft= MED_POSITION_BackLeft;
            POSITION_BackRight= MED_POSITION_BackRight;
        }
        else if (vehicleClass == "heavy")
        {
            COMPONENT_FrameEngine = HEAVY_COMPONENT_FrameEngine;
            COMPONENT_FrontLeft= HEAVY_COMPONENT_FrontLeft;
            COMPONENT_FrontRight = HEAVY_COMPONENT_FrontRight;
            COMPONENT_BackLeft = HEAVY_COMPONENT_BackLeft;
            COMPONENT_BackRight = HEAVY_COMPONENT_BackRight;

            POSITION_FrameEngine = HEAVY_POSITION_FrameEngine;
            POSITION_FrontLeft = HEAVY_POSITION_FrontLeft;
            POSITION_FrontRight = HEAVY_POSITION_FrontRight;
            POSITION_BackLeft = HEAVY_POSITION_BackLeft;
            POSITION_BackRight = HEAVY_POSITION_BackRight;
        }
        else
        {
            Debug.Log("Error");
            COMPONENT_FrameEngine = MED_COMPONENT_FrameEngine;
            COMPONENT_FrontLeft = MED_COMPONENT_FrontLeft;
            COMPONENT_FrontRight = MED_COMPONENT_FrontRight;
            COMPONENT_BackLeft = MED_COMPONENT_BackLeft;
            COMPONENT_BackRight = MED_COMPONENT_BackRight;

            POSITION_FrameEngine = MED_POSITION_FrameEngine;
            POSITION_FrontLeft = MED_POSITION_FrontLeft;
            POSITION_FrontRight = MED_POSITION_FrontRight;
            POSITION_BackLeft = MED_POSITION_BackLeft;
            POSITION_BackRight = MED_POSITION_BackRight;

        }*/
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
