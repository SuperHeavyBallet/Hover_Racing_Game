using System.Collections.Generic;
using UnityEngine;

public class ShipStatsCalculator : MonoBehaviour
{
    public float BASE_TopSpeed { get; private set; }
    public float BASE_MovementForce { get; private set; }
    public float BASE_RotationSpeed { get; private set; }
    public float BASE_BoostFuelConsumptionRate { get; private set; }
    public float BASE_NormalFuelConsumptionRate { get; private set; }

    private int shipWeight;
    private int maxBoostFuel;
    private int currentBoostFuel;
    private int maxNormalFuel;
    private int currentNormalFuel;

    public int STAT_ManualBoostAmount { get; private set; }

    [SerializeField] private ComponentCatalogue componentCatalogue;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CalculatePerformance(Dictionary<ComponentSlotPosition, SlotState> components)
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
            /// POTENTIAL ROUTE FOR CALUCULAR PER COMPONENT, NOT HARD STRINGS
            ComponentDefinition componentDefinition = componentCatalogue.GetById(pair.Value.selectedId);


            totalWeight += componentDefinition.weight;
            normalFuel += componentDefinition.normalfuelDelta;
            boostFuel += componentDefinition.boostFuelDelta;
            rawPower += componentDefinition.powerDelta;
            rawSpeed += componentDefinition.topSpeedDelta;
            normalFuelConsumptionRate += componentDefinition.normalFuelConsumptionDelta;
            boostFuelConsumptionRate += componentDefinition.boostFuelConsumptionDelta;

            if(componentDefinition.category == ComponentCategory.Engine) {  engineCount++; }
        }

        float weightFactor = Mathf.Max(totalWeight, 1);

        BASE_TopSpeed = Mathf.Max(10f, rawSpeed - weightFactor * 0.1f);
        BASE_MovementForce = Mathf.Max(10f, rawPower - weightFactor * 0.05f);

        float minWeight = 100f;
        float maxWeight = 1000f;
        float t = Mathf.InverseLerp(maxWeight, minWeight, weightFactor); // Note the reverse
        BASE_RotationSpeed = Mathf.Lerp(30f, 150f, t); // Heavy = 30, Light = 150

        BASE_BoostFuelConsumptionRate = Mathf.Max(0, 0.25f + boostFuelConsumptionRate);
        BASE_NormalFuelConsumptionRate = Mathf.Max(0.25f + normalFuelConsumptionRate);

        shipWeight = Mathf.Max(0, totalWeight);

        maxBoostFuel = Mathf.Max(0, boostFuel);
        currentBoostFuel = Mathf.Max(0, boostFuel);
        maxNormalFuel = Mathf.Max(0, normalFuel);
        currentNormalFuel = Mathf.Max(0, normalFuel);
        STAT_ManualBoostAmount = Mathf.Max(0, rawPower);




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

    public float GetShipWeight()
    {
        return shipWeight;
    }
}
