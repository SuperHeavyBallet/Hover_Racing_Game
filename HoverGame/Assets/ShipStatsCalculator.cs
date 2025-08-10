using System.Collections.Generic;
using UnityEngine;

public class ShipStatsCalculator : MonoBehaviour
{
    public float BASE_TopSpeed { get; private set; }
    public float BASE_MovementForce { get; private set; }
    public float BASE_RotationSpeed { get; private set; }
    public float BASE_BoostFuelConsumptionRate { get; private set; }
    public float BASE_NormaFuelConsumptionRate { get; private set; }

    private int shipWeight;
    private int maxBoostFuel;
    private int currentBoostFuel;
    private int maxNormalFuel;
    private int currentNormalFuel;

    public int STAT_ManualBoostAmount { get; private set; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CalculatePerformance(Dictionary<ComponentSlotPosition, ComponentName> components)
    {
        int rawSpeed = 0;
        int rawPower = 0;
        int totalWeight = 0;
        int normalFuel = 500;
        int boostFuel = 100;
        float normalFuelConsumptionRate = 0;
        float boostFuelConsumptionRate = 0;

        float engineCount = 0;
        float jetEngineCount = 0;
        float fuelTankCount = 0;
        float aireonCount = 0;

        foreach (var pair in components)
        {
            switch (pair.Value)
            {
                case ComponentName.LIGHT_FRAME:
                    totalWeight += 50;
                    boostFuel += 30;
                    normalFuel += 200;
                    break;
                case ComponentName.MEDIUM_FRAME:
                    totalWeight += 70;
                    boostFuel += 10;
                    normalFuel += 100;
                    break;
                case ComponentName.HEAVY_FRAME:
                    totalWeight += 100;
                    break;

                case ComponentName.ENGINE:
                    totalWeight += 100;
                    rawPower += 15;
                    rawSpeed += 30;
                    engineCount += 1;
                    break;
                case ComponentName.JET_ENGINE:
                    totalWeight += 70;
                    rawPower += 20;
                    rawSpeed += 25;
                    jetEngineCount += 1;
                    break;

                case ComponentName.FUEL_TANK:
                    totalWeight += 10;
                    boostFuel += 50;
                    normalFuel += 200;
                    fuelTankCount += 1;
                    break;
                case ComponentName.AIREON:
                    totalWeight -= 10;
                    rawSpeed += 10;
                    boostFuelConsumptionRate -= 0.25f;
                    normalFuelConsumptionRate -= 0.25f;
                    aireonCount += 1;
                    break;
                default:
                    break;
            }
        }

        float weightFactor = Mathf.Max(totalWeight, 1);

        BASE_TopSpeed = Mathf.Max(10f, rawSpeed - weightFactor * 0.1f);
        BASE_MovementForce = Mathf.Max(10f, rawPower - weightFactor * 0.05f);

        float minWeight = 100f;
        float maxWeight = 1000f;
        float t = Mathf.InverseLerp(maxWeight, minWeight, weightFactor); // Note the reverse
        BASE_RotationSpeed = Mathf.Lerp(30f, 150f, t); // Heavy = 30, Light = 150

        BASE_BoostFuelConsumptionRate = 0.25f + boostFuelConsumptionRate;
        BASE_NormaFuelConsumptionRate = 0.25f + normalFuelConsumptionRate;

        shipWeight = Mathf.Max(0, totalWeight);

        maxBoostFuel = boostFuel;
        currentBoostFuel = boostFuel;
        maxNormalFuel = normalFuel;
        currentNormalFuel = normalFuel;
        STAT_ManualBoostAmount = rawPower;



    }

  

    public float GetShipTopSpeed()
    {
        return BASE_TopSpeed;
    }

    public float GetShipPower()
    {
        return BASE_MovementForce;
    }

    public float GetShipControl()
    {
        return BASE_RotationSpeed;
    }
}
