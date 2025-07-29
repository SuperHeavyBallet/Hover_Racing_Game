using TMPro;
using UnityEngine;

public class UI_Controller : MonoBehaviour
{
    public GameObject boostText;
    public GameObject megaBoostText;
    public GameObject boostGulpText;

    public TextMeshProUGUI speedDisplay;

    public TextMeshProUGUI currentBoostFuelDisplay;

    public TextMeshProUGUI FPSDisplay;


    public TextMeshProUGUI currentFuelDisplay;

    


    // Debugging UI
    [Header("DEBUGGING UI")]
    public TextMeshProUGUI topSpeedText;
    public TextMeshProUGUI topPowerText;
    public TextMeshProUGUI topWeightText;


    public void ShowBoostText() => boostText.SetActive(true);
    public void HideBoostText() => boostText.SetActive(false);

    public void ShowMegaBoostText() => megaBoostText.SetActive(true);
    public void HideMegaBoostText() => megaBoostText.SetActive(false);

    public void ShowBoostGulpText() => boostGulpText.SetActive(true);
    public void HideBoostGulpText() => boostGulpText.SetActive(false);


    public void UpdateSpeedDisplay(string newSpeed) => speedDisplay.text = newSpeed;

    public void UpdateBoostFuelDisplay(string newBoostFuelAmount) => currentBoostFuelDisplay.text = newBoostFuelAmount;

    public void UpdateFuelDisplay(int newFuelAmount) => currentFuelDisplay.text = newFuelAmount.ToString();

    // Debugging UI Functions

    public void DEBUG_UpdateTopSpeedDisplay(string topSpeed) => topSpeedText.text = topSpeed; 
    public void DEBUG_UpdateTopPowerDisplay(string topPower) => topPowerText.text = topPower;
    public void DEBUG_UpdateTopWeightDisplay(string topWeight) => topWeightText.text = topWeight;
}
