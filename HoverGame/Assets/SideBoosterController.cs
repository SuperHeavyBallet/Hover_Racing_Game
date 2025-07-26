using UnityEngine;

public class SideBoosterController : MonoBehaviour
{

  
    public GameObject fire;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        fire.SetActive(false);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ActivateFire(bool shouldFire)
    {


        Debug.Log("SHOULD FIRE? " + shouldFire);

        if(shouldFire)
        {
            fire.SetActive(true);
        }
        else
        {
            fire.SetActive(false);
        }
    }



   
}
