using TMPro;
using UnityEngine;

public class Ship_Constructor : MonoBehaviour
{

    public ShipClass BASE_ShipStatSheet;

    GameObject frame;
    public Transform framePosition;

    GameObject frameEngine;

    GameObject frontLeftComponent;
    GameObject frontRightComponent;
    GameObject backRightComponent;
    GameObject backLeftComponent;

    GameObject backRight1Component;
    GameObject backLeft1Component;

    Transform frameEnginePosition;

    Transform frontLeftPosition;
    Transform frontRightPosition;
    Transform backLeftPosition;
    Transform backRightPosition;

    Transform backLeft1Position;
    Transform backRight1Position;

    public string vehicleClass;

    public TextMeshProUGUI shipClassText;
    Ship_Movement SCRIPT_ShipMovement;

    string frameSize;

    SceneStartup sceneStartup;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        sceneStartup = SceneStartup.Instance;
        sceneStartup.GetShipLoadouut();

        SCRIPT_ShipMovement = this.GetComponent<Ship_Movement>();

        SelectShip();
        SpawnFrame();
        SpawnFrameEngine();
        SpawnComponents();
    }



    void SelectShip()
    {
        SceneStartup sceneStartup = SceneStartup.Instance;

        if(sceneStartup != null )
        {
            vehicleClass = sceneStartup.GetVehicleClass();
        }
        else
        {
           vehicleClass = BASE_ShipStatSheet.SELECTED_shipWeightClass.ToString();
        }

        shipClassText.text = vehicleClass;
    }


    void SpawnFrame()
    {
        frame = Instantiate(BASE_ShipStatSheet.frame, framePosition);

        Frame_Layout frameLayout = frame.GetComponent<Frame_Layout>();

        if(frameLayout != null )
        {
            frontLeftPosition = frameLayout.GetFrontLeftPosition();
            frontRightPosition = frameLayout.GetFrontRightPosition();
            backLeftPosition = frameLayout.GetBackLeftPosition();
            backRightPosition = frameLayout.GetBackRightPosition();
            frameEnginePosition = frameLayout.GetFrameEnginePosition();

            string frameSize = frameLayout.GetFrameSize();
            if(frameSize == "heavy")
            {
                AddExtraEngins(frameLayout);
            }
        }
    }

    void AddExtraEngins(Frame_Layout frameLayout)
    {
        backLeft1Position = frameLayout.GetBackLeft1Position();
        backRight1Position = frameLayout.GetBackRight1Position();

        SpawnExtraComponents();
    }

    void SpawnFrameEngine()
    {
        frameEngine = Instantiate(BASE_ShipStatSheet.frameEngine, frameEnginePosition);

        EngineController en_controller = frameEngine.GetComponent<EngineController>();

        if( en_controller != null )
        {
            SCRIPT_ShipMovement.RegisterEngineFireListener(en_controller);
        }
    }

    void SpawnComponents()
    {
        frontLeftComponent = Instantiate(BASE_ShipStatSheet.frontLeftComponent, frontLeftPosition);
        frontRightComponent = Instantiate(BASE_ShipStatSheet.frontRightComponent, frontRightPosition);
        backLeftComponent = Instantiate(BASE_ShipStatSheet.backLeftComponent, backLeftPosition);
        backRightComponent = Instantiate(BASE_ShipStatSheet.backRightComponent, backRightPosition);

        EngineController fl_controller = frontLeftComponent.GetComponent<EngineController>();
        EngineController fr_controller = frontRightComponent.GetComponent<EngineController>();
        EngineController bl_controller = backLeftComponent.GetComponent<EngineController>();
        EngineController br_controller = backRightComponent.GetComponent<EngineController>();

        if (fl_controller != null)
        {
            SCRIPT_ShipMovement.RegisterEngineFireListener(fl_controller);
        }

        if (fr_controller != null)
        {
            SCRIPT_ShipMovement.RegisterEngineFireListener(fr_controller);
        }

        if (bl_controller != null)
        {
            SCRIPT_ShipMovement.RegisterEngineFireListener(bl_controller);
        }

        if (br_controller != null)
        {
            SCRIPT_ShipMovement.RegisterEngineFireListener(br_controller);
        }




    }

    void SpawnExtraComponents()
    {
        backLeft1Component = Instantiate(BASE_ShipStatSheet.backLeft1Component, backLeft1Position);
        backRight1Component = Instantiate(BASE_ShipStatSheet.backRight1Component, backRight1Position);

        EngineController bl1_controller = backLeft1Component.GetComponent<EngineController>();
        EngineController br1_controller = backRight1Component.GetComponent<EngineController>();

        if (bl1_controller != null)
        {
            SCRIPT_ShipMovement.RegisterEngineFireListener(bl1_controller);
        }

        if (br1_controller != null)
        {
            SCRIPT_ShipMovement.RegisterEngineFireListener(br1_controller);
        }
    }
}
