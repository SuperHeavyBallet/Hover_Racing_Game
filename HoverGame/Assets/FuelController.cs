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



    public void SubtractFuelCount(int newFuelCount)
    {
        currentFuelAmount -= newFuelCount;
       // UpdateFuelCountDisplay();
    }

    public void AddFuelCount(int newFuelCount)
    {
        currentFuelAmount += newFuelCount;
        //UpdateFuelCountDisplay();
    }

    public void UpdateFuelCountDisplay(float newFuelAmount)
    {
        int convertedAmount = Mathf.RoundToInt(newFuelAmount);
        UI_Router.UpdateFuelDisplay(convertedAmount);
    }

    public void SetTotalFuelCapacity(int newTotalFuelCapacity)
    {
        totalFuelCapacity = newTotalFuelCapacity;
    }
}
