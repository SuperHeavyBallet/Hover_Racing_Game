using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneStartup : MonoBehaviour
{

    public static SceneStartup Instance { get; private set; }

    [SerializeField]
    private string selectedVehicleClass = "light";
    public Vector3 spawnPosition;
    public float gameDifficulty;

    public ShipClass shipClass;

    public TextMeshProUGUI frameText;
    public TextMeshProUGUI fLText;
    public TextMeshProUGUI fRText;
    public TextMeshProUGUI bLText;
    public TextMeshProUGUI bRText;
    public TextMeshProUGUI bL1Text;
    public TextMeshProUGUI bR1Text;

    public GameObject engine;
    public GameObject jetEngine;
    public GameObject lightFrame;
    public GameObject mediumFrame;
    public GameObject heavyFrame;

    GameObject frameComponent;
    GameObject fLComponent;
    GameObject fRComponent;
    GameObject bLComponent;
    GameObject bRComponent;
    GameObject bL1Component;
    GameObject bR1Component;

    public Transform framePosition;
    public Transform fLPosition;
    public Transform fRPosition;
    public Transform bLPosition;
    public Transform bRPosition;
    public Transform bL1Position;
    public Transform bR1Position;

    public GameObject frame_Display_Light;
    public GameObject frame_Display_Medium;
    public GameObject frame_Display_Heavy;

    public GameObject fL_Component_Engine;
    public GameObject fL_Component_JetEngine;
    public GameObject fL_Component_Aireon;

    public GameObject fR_Component_Engine;
    public GameObject fR_Component_JetEngine;
    public GameObject fR_Component_Aireon;

    public GameObject bL_Component_Engine;
    public GameObject bL_Component_JetEngine;
    public GameObject bL_Component_Aireon;

    public GameObject bR_Component_Engine;
    public GameObject bR_Component_JetEngine;
    public GameObject bR_Component_Aireon;

    public GameObject bL1_Component_Engine;
    public GameObject bL1_Component_JetEngine;
    public GameObject bL1_Component_Aireon;

    public GameObject bR1_Component_Engine;
    public GameObject bR1_Component_JetEngine;
    public GameObject bR1_Component_Aireon;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        // Singleton enforcement
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Keep across scene loads

        StartWithDisplayNotActive();
    }


    void StartWithDisplayNotActive()
    {
        frame_Display_Light.gameObject.SetActive(false);
        frame_Display_Medium.gameObject.SetActive(false);
        frame_Display_Heavy.gameObject.SetActive(false);

        fL_Component_Engine.gameObject.SetActive(false);
        fL_Component_JetEngine.gameObject.SetActive(false);
        fL_Component_Aireon.gameObject.SetActive(false);

        fR_Component_Engine.gameObject.SetActive(false);
        fR_Component_JetEngine.gameObject.SetActive(false);
        fR_Component_Aireon.gameObject.SetActive(false);

        bL_Component_Engine.gameObject.SetActive(false);
        bL_Component_JetEngine.gameObject.SetActive(false);
        bL_Component_Aireon.gameObject.SetActive(false);

        bR_Component_Engine.gameObject.SetActive(false);
        bR_Component_JetEngine.gameObject.SetActive(false);
        bR_Component_Aireon.gameObject.SetActive(false);


        bL1_Component_Engine.gameObject.SetActive(false);
        bL1_Component_JetEngine.gameObject.SetActive(false);
        bL1_Component_Aireon.gameObject.SetActive(false);

        bR1_Component_Engine.gameObject.SetActive(false);
        bR1_Component_JetEngine.gameObject.SetActive(false);
        bR1_Component_Aireon.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetVehicleClass(string vehicleClass)
    {

        selectedVehicleClass = vehicleClass;
        Debug.Log("Scene Startup: " + vehicleClass);
    }

    public string GetVehicleClass()
    {
        Debug.Log("Get Vehicle: " + selectedVehicleClass);
        return shipClass.SELECTED_shipWeightClass.ToString();
    }

    public void SetComponentSlotFrame(string component)
    {
        frameText.text = component;

        switch (component)
        {
            case "light":
                frameComponent = lightFrame;
                frame_Display_Light.gameObject.SetActive(true);
                frame_Display_Medium.gameObject.SetActive(false);
                frame_Display_Heavy.gameObject.SetActive(false);
                break;
            case "medium":
                frameComponent = mediumFrame;
                frame_Display_Light.gameObject.SetActive(false);
                frame_Display_Medium.gameObject.SetActive(true);
                frame_Display_Heavy.gameObject.SetActive(false);
                break;
            case "heavy":
                frameComponent = heavyFrame;
                frame_Display_Light.gameObject.SetActive(false);
                frame_Display_Medium.gameObject.SetActive(false);
                frame_Display_Heavy.gameObject.SetActive(true);
                break;
        }

        

    }

    public void SetComponentSlotFL(string component)
    {
        fLText.text = component;

        switch (component )
        {
            case "engine":
                fLComponent = engine;
                fL_Component_Engine.gameObject.SetActive(true);
                fL_Component_JetEngine.gameObject.SetActive(false);
                fL_Component_Aireon.gameObject.SetActive(false);
                break;
            case "jetEngine":
                fLComponent = jetEngine;
                fL_Component_Engine.gameObject.SetActive(false);
                fL_Component_JetEngine.gameObject.SetActive(true);
                fL_Component_Aireon.gameObject.SetActive(false);
                break;
                default:
                fLComponent = engine;
                break;
        }

       
    }

    public void SetComponentSlotFR(string component)
    {
        fRText.text = component;
        switch (component)
        {
            case "engine":
                fRComponent = engine;
                fR_Component_Engine.gameObject.SetActive(true);
                fR_Component_JetEngine.gameObject.SetActive(false);
                fR_Component_Aireon.gameObject.SetActive(false);
                break;
            case "jetEngine":
                fRComponent = jetEngine;
                fR_Component_Engine.gameObject.SetActive(false);
                fR_Component_JetEngine.gameObject.SetActive(true);
                fR_Component_Aireon.gameObject.SetActive(false);
                break;
            default:
                fRComponent = engine;
                break;
        }

    }

    public void SetComponentSlotBL(string component)
    {
        bLText.text = component;
        switch (component)
        {
            case "engine":
                bLComponent = engine;
                bL_Component_Engine.gameObject.SetActive(true);
                bL_Component_JetEngine.gameObject.SetActive(false);
                bL_Component_Aireon.gameObject.SetActive(false);
                break;
            case "jetEngine":
                bLComponent = jetEngine;
                bL_Component_Engine.gameObject.SetActive(false);
                bL_Component_JetEngine.gameObject.SetActive(true);
                bL_Component_Aireon.gameObject.SetActive(false);
                break;
            default:
                bLComponent = engine;
                break;
        }

      
    }

    public void SetComponentSlotBR(string component)
    {
        bRText.text = component;
        switch (component)
        {
            case "engine":
                bRComponent = engine;
                bR_Component_Engine.gameObject.SetActive(true);
                bR_Component_JetEngine.gameObject.SetActive(false);
                bR_Component_Aireon.gameObject.SetActive(false);
                break;
            case "jetEngine":
                bRComponent = jetEngine;
                bR_Component_Engine.gameObject.SetActive(false);
                bR_Component_JetEngine.gameObject.SetActive(true);
                bR_Component_Aireon.gameObject.SetActive(false);
                break;
            default:
                bRComponent = engine;
                break;
        }

    }

    public void SetComponentSlotBL1(string component)
    {
        bL1Text.text = component;
        switch (component)
        {
            case "engine":
                bL1Component = engine;
                bL1_Component_Engine.gameObject.SetActive(true);
                bL1_Component_JetEngine.gameObject.SetActive(false);
                bL1_Component_Aireon.gameObject.SetActive(false);
                break;
            case "jetEngine":
                bL1Component = jetEngine;
                bL1_Component_Engine.gameObject.SetActive(false);
                bL1_Component_JetEngine.gameObject.SetActive(true);
                bL1_Component_Aireon.gameObject.SetActive(false);
                break;
            default:
                bL1Component = engine;
                break;
        }


    }

    public void SetComponentSlotBR1(string component)
    {
        bR1Text.text = component;
        switch (component)
        {
            case "engine":
                bR1Component = engine;
                bR1_Component_Engine.gameObject.SetActive(true);
                bR1_Component_JetEngine.gameObject.SetActive(false);
                bR1_Component_Aireon.gameObject.SetActive(false);
                break;
            case "jetEngine":
                bL1Component = jetEngine;
                bR1_Component_Engine.gameObject.SetActive(false);
                bR1_Component_JetEngine.gameObject.SetActive(true);
                bR1_Component_Aireon.gameObject.SetActive(false);
                break;
            default:
                bL1Component = engine;
                break;
        }
   

    }

    void SetComponent(string component, string slot)
    {
      
    }


}
