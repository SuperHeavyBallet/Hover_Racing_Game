using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[System.Serializable]

public enum ComponentSlotPosition
{
    Frame,
    FrontLeft,
    FrontRight,
    BackLeft,
    BackRight,
    BackLeft1,
    BackRight1,
    ExtraTop,
    ExtraLeft,
    ExtraRight
}

public enum ComponentName
{
    Empty,
    Light_Frame,
    Medium_Frame,
    Heavy_Frame,
    Engine,
    Jet_Engine,
    Aireon,
    Fuel_Tank,
    Boost_Gulp,
    Machine_Gun,
    Missile

}
public class ComponentSlot 
{
    //public TextMeshProUGUI label;
    public Transform position;
    public Dictionary<ComponentName, GameObject> components; // "engine" => prefab

    public ComponentName selectedComponentKey;
}
