using System.Collections;
using UnityEngine;

public class MissileController : MonoBehaviour
{

    float missileSpeed = 300f;
    public GameObject missileExplosion;
    Rigidbody rigidBody;

    bool isArmed = false;
    Vector3 startPosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();

        startPosition = transform.position;
        rigidBody.AddForce(this.transform.forward * 50f, ForceMode.Impulse);
    }


    // Update is called once per frame
    void Update()
    {
        float distanceFromPlayer = Vector3.Distance(this.transform.position, startPosition);
        if(distanceFromPlayer >= 2)
        {
            isArmed = true;
        }
        rigidBody.linearVelocity += (this.transform.forward * missileSpeed) * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(isArmed)
        {
            DetonateMissile();
        }
      
        
    }

    void DetonateMissile()
    {
        GameObject newExplosion = Instantiate(missileExplosion, this.transform.position, Quaternion.identity);


        
     
            StartCoroutine(DelayDestroyMissile(0.1f));
        

    }

    IEnumerator DelayDestroyMissile(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(this.gameObject);
    }
}
