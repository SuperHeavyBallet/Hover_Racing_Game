using UnityEngine;
using System.Collections;
public class DestroyAfterDelay : MonoBehaviour
{
    public float delay;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(DestroyThisAfterDelay());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator DestroyThisAfterDelay()
    {
        yield return new WaitForSeconds(delay);
        Destroy(this.gameObject);
    }
}
