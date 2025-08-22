using UnityEngine;


public enum ComponentCategory
{
    Frame,
    Engine,
    Extra,
    ExtraTop,
    Empty
}


[CreateAssetMenu(fileName = "Component", menuName = "Scriptable/Component Definition")]
public class ComponentDefinition : ScriptableObject
{

    [Header("Identity")]
    public string id;
    public string displayName;

    [Header("Type & Prefab")]
    public ComponentCategory category;
    public GameObject prefab;

    [Header("UI / Balancing")]

    public Sprite icon;
    public int cost;
    public int weight;

    public int topSpeedDelta;
    public int powerDelta;
    public int controlDelta;

    public int normalfuelDelta;
    public int boostFuelDelta;

    public int normalFuelConsumptionDelta;
    public int boostFuelConsumptionDelta;


    [Header("Frame Specific Extras")]

    public bool hasExtraBackEngineSlots;
}
