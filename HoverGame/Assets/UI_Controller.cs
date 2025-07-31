using TMPro;
using UnityEngine;

public class UI_Controller : MonoBehaviour
{
    public GameObject boostText;
    public GameObject megaBoostText;
    public GameObject boostGulpText;
    public GameObject pitStopText;

    public TextMeshProUGUI speedDisplay;

    public TextMeshProUGUI currentBoostFuelDisplay;

    public TextMeshProUGUI FPSDisplay;


    public TextMeshProUGUI currentFuelDisplay;




    private void Start()
    {
      
    }


    // Debugging UI
    [Header("DEBUGGING UI")]
    public TextMeshProUGUI topSpeedText;
    public TextMeshProUGUI topPowerText;
    public TextMeshProUGUI topWeightText;


    public void ShowBoostText(bool conditional) => boostText.SetActive(conditional);


    public void ShowMegaBoostText(bool conditional) => megaBoostText.SetActive(conditional);


    public void ShowBoostGulpText(bool conditional) => boostGulpText.SetActive(conditional);


    public void ShowPitStopText(bool conditional) => pitStopText.SetActive(conditional);



    public void UpdateSpeedDisplay(int newSpeed) => speedDisplay.text = newSpeed.ToString();

  

    public void UpdateFuelDisplay(int newFuelAmount) => currentFuelDisplay.text = newFuelAmount.ToString();
    public void UpdateBoostFuelDisplay(int newBoostFuelAmount) => currentBoostFuelDisplay.text = newBoostFuelAmount.ToString();

    // Debugging UI Functions

    public void DEBUG_UpdateTopSpeedDisplay(string topSpeed) => topSpeedText.text = topSpeed; 
    public void DEBUG_UpdateTopPowerDisplay(string topPower) => topPowerText.text = topPower;
    public void DEBUG_UpdateTopWeightDisplay(string topWeight) => topWeightText.text = topWeight;
}
