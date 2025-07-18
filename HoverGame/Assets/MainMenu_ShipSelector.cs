using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu_ShipSelector : MonoBehaviour
{

    public SceneStartup sceneStartup;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sceneStartup = SceneStartup.Instance;


    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadScene()
    {
        SceneManager.LoadScene("Level1");
    }

    public void ChooseShip(string shipClass)
    {
        switch(shipClass)
        {
            case "light":
                sceneStartup.SetVehicleClass(shipClass);
                break;
            case "medium":
                sceneStartup.SetVehicleClass(shipClass);
                break;
            case "heavy":
                sceneStartup.SetVehicleClass(shipClass);
                break;
            default:
                Debug.Log("Error");
                break;
        }
    }

    public void ChooseComponentSlotFrame(string component)
    {
        //sceneStartup.SetComponentSlotFrame(component);
    }

    public void ChooseComponentFL(int val)
    {
        string chosenComponent = "engine";

        if(val == 0)
        {
            chosenComponent = "engine";
        }
        else if(val == 1)
        {
            chosenComponent = "jetEngine";
        }
        //sceneStartup.SetComponentSlotFL(chosenComponent);
    }

    public void ChooseComponentFR(int val)
    {
        string chosenComponent = "engine";

        if (val == 0)
        {
            chosenComponent = "engine";
        }
        else if (val == 1)
        {
            chosenComponent = "jetEngine";
        }
        //sceneStartup.SetComponentSlotFR(chosenComponent);
    }

    public void ChooseComponentBL(int val)
    {
        string chosenComponent = "engine";

        if (val == 0)
        {
            chosenComponent = "engine";
        }
        else if (val == 1)
        {
            chosenComponent = "jetEngine";
        }
        //sceneStartup.SetComponentSlotBL(chosenComponent);
    }

    public void ChooseComponentBR(int val)
    {
        string chosenComponent = "engine";

        if (val == 0)
        {
            chosenComponent = "engine";
        }
        else if (val == 1)
        {
            chosenComponent = "jetEngine";
        }
       // sceneStartup.SetComponentSlotBR(chosenComponent);
    }

    public void ChooseComponentBL1(int val)
    {
        string chosenComponent = "engine";

        if (val == 0)
        {
            chosenComponent = "engine";
        }
        else if (val == 1)
        {
            chosenComponent = "jetEngine";
        }
        //sceneStartup.SetComponentSlotBL1(chosenComponent);
    }

    public void ChooseComponentBR1(int val)
    {
        string chosenComponent = "engine";

        if (val == 0)
        {
            chosenComponent = "engine";
        }
        else if (val == 1)
        {
            chosenComponent = "jetEngine";
        }
       // sceneStartup.SetComponentSlotBR1(chosenComponent);
    }
}
