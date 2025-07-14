using UnityEngine;

[CreateAssetMenu(fileName = "ShipComponent", menuName = "Scriptable Objects/ShipComponent")]
public class ShipComponent : ScriptableObject
{

    public ComponentType componentType = ComponentType.engine;

    public enum ComponentType
    {
        engine,
        jetEngine,
        aireon

    }

    public ComponentWeight componentWeight = ComponentWeight.medium;

    public enum ComponentWeight
    {
        light,
        medium,
        heavy
    }
    
}
