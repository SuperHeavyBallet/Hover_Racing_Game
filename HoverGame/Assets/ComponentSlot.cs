using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[System.Serializable]

public enum ComponentSlotType
{
    Frame,
    FrontLeft,
    FrontRight,
    BackLeft,
    BackRight,
    BackLeft1,
    BackRight1,
    ExtraFront,
    ExtraLeft,
    ExtraRight
}

public enum ComponentName
{
    empty,
    lightFrame,
    mediumFrame,
    heavyFrame,
    engine,
    jetEngine,
    aireon,
    fuelTank,
    boostGulp,
    machineGun,
    missile

}
public class ComponentSlot 
{
    public TextMeshProUGUI label;
    public Transform position;
    public Dictionary<ComponentName, GameObject> components; // "engine" => prefab

    public ComponentName selectedComponentKey;
}
