using UnityEngine;
using System;
using System.Collections.Generic;
using TMPro;
public class ShipStatsUI_Updater : MonoBehaviour
{
    public ShipStatsCalculator SCRIPT_ShipStatsCalculator;

    public GameObject DISPLAY_ShipTopSpeed;
    public GameObject DISPLAY_ShipPower;
    public GameObject DISPLAY_ShipControl;
    public GameObject DISPLAY_ShipWeight;
    private float shipTopSpeed;
    private float shipPower;
    private float shipControl;
    private float shipWeight;

    void Awake()
    {
       
    }

 
    public void UpdateShipStats(Dictionary<ComponentSlotPosition, SlotState> componentSlotPositions)
    {
       /* var shipLoadout = new Dictionary<ComponentSlotPosition, ComponentName>();

        foreach (var pair in componentSlotPositions)
        {
            shipLoadout[pair.Key] = IdToEnum(pair.Value.selectedId);
        }*/

        SCRIPT_ShipStatsCalculator.CalculatePerformance(componentSlotPositions);
        shipTopSpeed = SCRIPT_ShipStatsCalculator.GetShipTopSpeed();
        shipPower = SCRIPT_ShipStatsCalculator.GetShipPower();
        shipControl = SCRIPT_ShipStatsCalculator.GetShipControl();
        shipWeight = SCRIPT_ShipStatsCalculator.GetShipWeight();

        DISPLAY_ShipTopSpeed.GetComponent<TextMeshProUGUI>().text = "TOP SPEED: " + shipTopSpeed.ToString();
        DISPLAY_ShipPower.GetComponent<TextMeshProUGUI>().text = "POWER: " + shipPower.ToString();
        DISPLAY_ShipControl.GetComponent<TextMeshProUGUI>().text = "CONTROL: " + shipControl.ToString();
        DISPLAY_ShipWeight.GetComponent<TextMeshProUGUI>().text = "WEIGHT: " + shipWeight.ToString();
    }

    static string EnumToId(ComponentName n) => n.ToString();
    static ComponentName IdToEnum(string id) => Enum.TryParse(id, out ComponentName e) ? e : ComponentName.EMPTY;

}
