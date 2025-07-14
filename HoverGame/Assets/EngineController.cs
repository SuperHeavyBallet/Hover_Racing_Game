using UnityEngine;

public class EngineController : MonoBehaviour
{

    public GameObject exhaustFlame;
    public bool isFiring;
    public GameObject ship;
    public Ship_Movement shipMovement;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(shipMovement.isFiring)

        {
            exhaustFlame.SetActive(true);
        }
        else
        {
            exhaustFlame.SetActive(false);
        }
    }
}
