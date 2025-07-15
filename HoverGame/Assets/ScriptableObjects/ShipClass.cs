using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "ShipClass", menuName = "Scriptable Objects/ShipClass")]
public class ShipClass : ScriptableObject
{
    public shipWeightClass SELECTED_shipWeightClass = shipWeightClass.medium;
    public enum shipWeightClass
    {
        light,
        medium,
        heavy
    }

    
    
}
