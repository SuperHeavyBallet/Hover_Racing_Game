using UnityEngine;

public class PlayerPositionSpawner : MonoBehaviour
{
    public GameObject player;
    public Transform spawnPosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("PLACE PLAYER");
        player.transform.position = spawnPosition.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
