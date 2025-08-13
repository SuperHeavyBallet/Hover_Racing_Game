using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MeshDisplayController : MonoBehaviour
{
    

    public GameObject InstantiatePrefabAtPosition(GameObject prefab, Transform position)
    {
        
        

        if (prefab == null || position == null)
        {

            return null;
        }

        GameObject newObject = Instantiate(prefab, position.position, position.rotation, position);
        newObject.transform.localPosition = Vector3.zero;
        newObject.transform.localRotation = Quaternion.identity;
        newObject.transform.localScale = Vector3.one;

        newObject.SetActive(true);

        

        return newObject;

    }

    public void CleanupExcessMeshesInSlot(Transform position)
    {
        #if UNITY_EDITOR
                // If the inspector is showing something under 'position', move selection away first
                if (Selection.activeTransform != null && Selection.activeTransform.IsChildOf(position))
                {
                    Selection.activeTransform = position; // or: Selection.activeObject = null;
                }
        #endif

                for (int i = position.childCount - 1; i >= 0; i--)
                {
                    var child = position.GetChild(i);

                    // In play mode, Destroy is correct; in edit mode you likely want DestroyImmediate.
        #if UNITY_EDITOR
                    if (!Application.isPlaying)
                        Object.DestroyImmediate(child.gameObject);
                    else
        #endif
                        Object.Destroy(child.gameObject);
                }

        /*
        foreach (Transform child in position)
        {
            Destroy(child.gameObject);
        }*/
    }
}
