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

    public shipFrameType SELECTED_ShipFrameType = shipFrameType.medium;

    public enum shipFrameType
    {
        light,
        medium,
        heavy
    }

    public GameObject frame;

    public GameObject frameEngine;

    public GameObject frontLeftComponent;
    public GameObject frontRightComponent;
    public GameObject backLeftComponent;
    public GameObject backRightComponent;

    public GameObject backLeft1Component;
    public GameObject backRight1Component;



}
