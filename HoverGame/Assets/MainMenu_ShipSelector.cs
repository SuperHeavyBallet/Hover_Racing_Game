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
}
