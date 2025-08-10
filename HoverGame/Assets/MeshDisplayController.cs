using System.Collections.Generic;
using UnityEngine;

public class MeshDisplayController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DisplayComponentMeshes(Dictionary<ComponentSlotPosition, ComponentSlot> componentSlotPositions)
    {


        foreach (var pair in componentSlotPositions)
        {
            if (pair.Key == ComponentSlotPosition.Frame) continue;

            var slot = pair.Value;
            var id = slot.selectedComponentId;

            if (string.IsNullOrEmpty(id))
            {
               // Debug.LogWarning($"[{slot.slotPosition}] No selectedComponentId set.");
                continue;
            }

            if (!slot.components.TryGetValue(id, out GameObject prefab) || prefab == null)
            {
               // Debug.LogWarning($"[{slot.slotPosition}] No prefab found for id '{id}'.");
                continue;
            }

            //Debug.Log($"[{slot.slotPosition}] Using '{prefab.name}'");


            CleanupExcessMeshes(slot.position);
            InstantiateSelectedComponent(slot, slot.position);
        }
    }
    void InstantiateSelectedComponent(ComponentSlot slot, Transform slotPosition)
    {
        var id = slot.selectedComponentId;
        if (string.IsNullOrEmpty(id) || id == "Empty") return;

        if (slot.components.TryGetValue(id, out var prefab))
        {
            var gameObject = Instantiate(prefab, slotPosition);
            gameObject.SetActive(true);
        }


    }
    void CleanupExcessMeshes(Transform slotPosition)
    {
        foreach (Transform child in slotPosition)
        {
            Destroy(child.gameObject);
        }
    }
}
