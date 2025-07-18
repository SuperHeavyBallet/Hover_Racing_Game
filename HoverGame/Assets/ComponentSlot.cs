using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[System.Serializable]
public class ComponentSlot 
{
    public TextMeshProUGUI label;
    public Transform position;
    public Dictionary<string, GameObject> components; // "engine" => prefab

    public string selectedComponentKey;  // <-- You need this
}
