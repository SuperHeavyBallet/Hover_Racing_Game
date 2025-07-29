using UnityEngine;

public class FuelController : MonoBehaviour
{

    UI_Controller UI_Router;

    public int totalFuelCapacity { get; private set; } = 0;
    public int currentFuelAmount { get; private set; }

    private void Awake()
    {
        UI_Router = GameObject.Find("PLAYER_UI").GetComponent<UI_Controller>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SubtractFuelCount(int newFuelCount)
    {
        currentFuelAmount -= newFuelCount;
        UpdateFuelCountDisplay();
    }

    public void AddFuelCount(int newFuelCount)
    {
        currentFuelAmount += newFuelCount;
        UpdateFuelCountDisplay();
    }

    void UpdateFuelCountDisplay()
    {
        UI_Router.UpdateFuelDisplay(currentFuelAmount);
    }
}
