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

        if (components == null)
        {
            Debug.LogError($"{nameof(ShipStatsCalculator)}.{nameof(CalculatePerformance)}: components dict is null.");
            return;
        }

        if (componentCatalogue == null)
        {
            Debug.LogError($"{nameof(ShipStatsCalculator)}: componentCatalogue is null; aborting calc.");
            return;
        }

        int rawSpeed = 0;
        int rawPower = 0;
        int totalWeight = 0;
        int normalFuel = 500;
        int boostFuel = 100;
        float normalFuelConsumptionRate = 0f;
        float boostFuelConsumptionRate = 0f;

        float engineCount = 0f;

        foreach (var pair in components)
        {
            var slotState = pair.Value;

            // No slot / no selection => skip
            if (slotState == null)
            {
                Debug.LogWarning($"Slot {pair.Key} has null SlotState.");
                continue;
            }
            if (string.IsNullOrEmpty(slotState.selectedId))
            {
                // Treat as empty slot
                continue;
            }

            var def = componentCatalogue.GetById(slotState.selectedId);
            if (def == null)
            {
                Debug.LogWarning($"Unknown component id '{slotState.selectedId}' in slot {pair.Key}.");
                continue;
            }

            totalWeight += def.weight;
            normalFuel += def.normalfuelDelta;
            boostFuel += def.boostFuelDelta;
            rawPower += def.powerDelta;
            rawSpeed += def.topSpeedDelta;
            normalFuelConsumptionRate += def.normalFuelConsumptionDelta;
            boostFuelConsumptionRate += def.boostFuelConsumptionDelta;

            if (def.category == ComponentCategory.Engine) engineCount++;
        }

        // Avoid divide-by-zero and keep ranges sane
        float weightFactor = Mathf.Max(totalWeight, 1);

        BASE_TopSpeed = Mathf.Max(10f, rawSpeed - weightFactor * 0.1f);
        BASE_MovementForce = Mathf.Max(10f, rawPower - weightFactor * 0.05f);

        // Heavier => slower rotation
        const float minWeight = 100f;
        const float maxWeight = 1000f;
        float t = Mathf.InverseLerp(maxWeight, minWeight, weightFactor);
        BASE_RotationSpeed = Mathf.Lerp(30f, 150f, t);

        BASE_BoostFuelConsumptionRate = Mathf.Max(0f, 0.25f + boostFuelConsumptionRate);
        BASE_NormalFuelConsumptionRate = Mathf.Max(0f, 0.25f + normalFuelConsumptionRate);

        shipWeight = Mathf.Max(0, totalWeight);

        maxBoostFuel = Mathf.Max(0, boostFuel);
        currentBoostFuel = maxBoostFuel;
        maxNormalFuel = Mathf.Max(0, normalFuel);
        currentNormalFuel = maxNormalFuel;

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
