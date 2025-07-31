using UnityEngine;

public class BoostFuelController : MonoBehaviour
{
    UI_Controller UI_Router;

    public int totalBoostFuelCapacity { get; private set; } = 0;
    public int currentBoostFuelAmount { get; private set; }

    private void Awake()
    {
        UI_Router = GameObject.Find("PLAYER_UI").GetComponent<UI_Controller>();
    }



    public void SubtractBoostFuelCount(int newFuelCount)
    {
        currentBoostFuelAmount -= newFuelCount;
        // UpdateFuelCountDisplay();
    }

    public void AddBoostFuelCount(int newFuelCount)
    {
        currentBoostFuelAmount += newFuelCount;
        //UpdateFuelCountDisplay();
    }

    public void UpdateBoostFuelCountDisplay(float newBoostFuelAmount)
    {
        int convertedAmount = Mathf.RoundToInt(newBoostFuelAmount);
        UI_Router.UpdateBoostFuelDisplay(convertedAmount);
    }

    public void SetTotalBoostFuelCapacity(int newTotalBoostFuelCapacity)
    {
        totalBoostFuelCapacity = newTotalBoostFuelCapacity;
    }
}
