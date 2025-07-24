using UnityEngine;
using System.Collections;
public class PlayerPositionSpawner : MonoBehaviour
{
    public GameObject player;
    public Transform spawnPosition;

    bool hasSpawnedCorrectPosition = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //StartCoroutine(PlacePlayerAfterDelay());


    }

    // Update is called once per frame
    void Update()
    {
       

            
       
    }

    void LateUpdate()
    {
        //Debug.Log("Player position in LateUpdate: " + transform.position);
    }

    IEnumerator PlacePlayerAfterDelay()
    {
        // Wait a few frames to let other systems finish
        yield return null;
        yield return null;
        yield return null;

        player.transform.position = spawnPosition.position;
        //Debug.Log("Player forced to position after delay: " + spawnPosition.position);
    }
}
