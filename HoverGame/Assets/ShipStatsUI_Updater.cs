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
    public float ShipTopSpeed;
    public float ShipPower;
    public float ShipControl;
    void Awake()
    {
       
    }

 
    public void UpdateShipStats(Dictionary<ComponentSlotPosition, SlotState> componentSlotPositions)
    {
        var shipLoadout = new Dictionary<ComponentSlotPosition, ComponentName>();

        foreach (var pair in componentSlotPositions)
        {
            shipLoadout[pair.Key] = IdToEnum(pair.Value.selectedId);
        }

        SCRIPT_ShipStatsCalculator.CalculatePerformance(shipLoadout);
        ShipTopSpeed = SCRIPT_ShipStatsCalculator.GetShipTopSpeed();
        ShipPower = SCRIPT_ShipStatsCalculator.GetShipPower();
        ShipControl = SCRIPT_ShipStatsCalculator.GetShipControl();

        DISPLAY_ShipTopSpeed.GetComponent<TextMeshProUGUI>().text = "TOP SPEED: " + ShipTopSpeed.ToString();
        DISPLAY_ShipPower.GetComponent<TextMeshProUGUI>().text = "POWER: " + ShipPower.ToString();
        DISPLAY_ShipControl.GetComponent<TextMeshProUGUI>().text = "CONTROL: " + ShipControl.ToString();
    }

    static string EnumToId(ComponentName n) => n.ToString();
    static ComponentName IdToEnum(string id) => Enum.TryParse(id, out ComponentName e) ? e : ComponentName.EMPTY;

}
