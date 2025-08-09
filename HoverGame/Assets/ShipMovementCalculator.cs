using System.Collections.Generic;
using UnityEngine;

public class ShipMovementCalculator : MonoBehaviour
{
    public float BASE_TopSpeed;
    public float BASE_MovementForce;
    public float BASE_RotationSpeed;
    public float BASE_NormaFuelConsumptionRate;
    public float BASE_BoostFuelConsumptionRate;

    public float currentNormalFuel = 500f;
    public float maxNormalFuel = 500f;
    public float currentBoostFuel = 100f;
    public float maxBoostFuel = 100f;

    public int surgeBoostMultiplier = 1;

    public int STAT_ManualBoostAmount;
    public float STAT_RotationSpeed;

    public float shipWeight;

    FuelController fuelController;
    BoostFuelController boostFuelController;
    UI_Controller UI_Router;

    Ship_Movement shipMovement;

    float sideBoostAmount = 10f;

    int boostZoneAmount = 50;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        fuelController = GetComponent<FuelController>();
        boostFuelController = GetComponent<BoostFuelController>();
        shipMovement = GetComponent<Ship_Movement>();
        UI_Router = GameObject.Find("PLAYER_UI").GetComponent<UI_Controller>();
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
                case ComponentName.Light_Frame:
                    totalWeight += 50;
                    boostFuel += 30;
                    normalFuel += 200;
                    break;
                case ComponentName.Medium_Frame:
                    totalWeight += 70;
                    boostFuel += 10;
                    normalFuel += 100;
                    break;
                case ComponentName.Heavy_Frame:
                    totalWeight += 100;
                    break;

                case ComponentName.Engine:
                    totalWeight += 100;
                    rawPower += 15;
                    rawSpeed += 30;
                    engineCount += 1;
                    break;
                case ComponentName.Jet_Engine:
                    totalWeight += 70;
                    rawPower += 20;
                    rawSpeed += 25;
                    jetEngineCount += 1;
                    break;

                case ComponentName.Fuel_Tank:
                    totalWeight += 10;
                    boostFuel += 50;
                    normalFuel += 200;
                    fuelTankCount += 1;
                    break;
                case ComponentName.Aireon:
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

        fuelController.SetTotalFuelCapacity(normalFuel);

        string calculatedTopSpeed = $"Top Speed: {BASE_TopSpeed:F1}";
        UI_Router.DEBUG_UpdateTopSpeedDisplay(calculatedTopSpeed);

        string calculatedTopPower = $"Top Power: {BASE_MovementForce:F1}";
        UI_Router.DEBUG_UpdateTopPowerDisplay(calculatedTopPower);

        string calculatedTopWeight = $"Weight: {totalWeight}";
        UI_Router.DEBUG_UpdateTopWeightDisplay(calculatedTopWeight);


    }


    public float CalculateCurrentTopSpeed(bool isManualBoosting, bool isTrackBoosting, bool isLimiting, bool isSurgeBoosting)
    {

        int workingTopSpeed = 0;

        if (isManualBoosting)
        {
            workingTopSpeed += STAT_ManualBoostAmount;
        }

        if (isTrackBoosting)
        {
            workingTopSpeed += boostZoneAmount;
        }

        if (isLimiting)
        {
            workingTopSpeed -= 50;
        }

        if(isSurgeBoosting)
        {
            surgeBoostMultiplier = 4 ;
         
            
        }
        else
        {
            surgeBoostMultiplier = 1;
        }

        workingTopSpeed *= surgeBoostMultiplier;

        return BASE_TopSpeed + workingTopSpeed;
    }

    public float CalculateCurrentMovementForce(bool isManualBoosting, bool isTrackBoosting, bool isLimiting, bool isSurgeBoosting)
    {
        int workingMovementForce = 0;
        if (isManualBoosting)
        {
            workingMovementForce += STAT_ManualBoostAmount;
        }
        if (isTrackBoosting)
        {
            workingMovementForce += boostZoneAmount;
        }

        if (isLimiting)
        {
            workingMovementForce -= 50;
        }

        if (isSurgeBoosting)
        {
            workingMovementForce += 200;

        }

        return BASE_MovementForce + workingMovementForce;
    }

    public float CalculateCurrentRotationSpeed(bool isManualBoosting, bool isTrackBoosting, bool isLimiting)
    {
        int workingRotationSpeed = 0;

        if (isLimiting)
        {
            workingRotationSpeed += 100;
        }

        return BASE_RotationSpeed + workingRotationSpeed;
    }

    public float CalculateCurrentSideBoostAmount()
    {
        float weightFactor = Mathf.Clamp01(shipWeight / 500f);
        float agilityFactor = 1f - weightFactor;

        float topSpeed = shipMovement.CURRENT_TopSpeed;
        if (topSpeed <= 0.01f) topSpeed = 0.01f; // Prevent division by zero

        float speedLerp = Mathf.Lerp(sideBoostAmount, sideBoostAmount * 4f, shipMovement.forwardSpeed / topSpeed);
        float weightedBoost = speedLerp * Mathf.Lerp(0.5f, 1.5f, agilityFactor);

        return weightedBoost;

    }
}
