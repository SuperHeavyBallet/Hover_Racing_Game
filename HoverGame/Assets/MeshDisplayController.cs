using System.Collections.Generic;
using UnityEngine;

public class MeshDisplayController : MonoBehaviour
{
    

    public GameObject InstantiatePrefabAtPosition(GameObject prefab, Transform position)
    {
        Debug.Log("In Instantiate with: " + prefab.name + ", " + position);
       CleanupExcessMeshes (position);

        GameObject newObject = Instantiate(prefab, position);
        newObject.SetActive(true);

        return newObject;

    }

    void CleanupExcessMeshes(Transform position)
    {
        foreach (Transform child in position)
        {
            Destroy(child.gameObject);
        }
    }
}
