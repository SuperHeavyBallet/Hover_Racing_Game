using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneStartup : MonoBehaviour
{

    public static SceneStartup Instance { get; private set; }

    [SerializeField]
    private string selectedVehicleClass = "light";
    public Vector3 spawnPosition;
    public float gameDifficulty;

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
        return selectedVehicleClass;
    }


}
