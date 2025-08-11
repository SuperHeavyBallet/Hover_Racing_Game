using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum ComponentSlotPosition { Frame, FrontLeft, FrontRight, BackLeft, BackRight, BackLeft1, BackRight1, ExtraTop, ExtraLeft, ExtraRight }


[System.Serializable]


public enum ComponentName
{
    EMPTY,
    LIGHT_FRAME,
    MEDIUM_FRAME,
    HEAVY_FRAME,
    ENGINE,
    JET_ENGINE,
    AIREON,
    FUEL_TANK,
    BOOST_GULP,
    MACHINE_GUN,
    MISSILE_LAUNCHER

}
public class ComponentSlot 
{

    public Transform position;
    public ComponentSlotPosition slotPosition;
    public ComponentCategory acceptsCategory;
    public string selectedComponentId;

    public Dictionary<string, GameObject> components; // "engine" => prefab

    //public ComponentName selectedComponentKey;
}

[Serializable]
public class SlotState
{
    public Transform position;
    // Prefer definitions so you can access prefab + displayName, etc.
    public Dictionary<string, ComponentDefinition> optionsById;
    public string selectedId; // "EMPTY" or a real id
}
