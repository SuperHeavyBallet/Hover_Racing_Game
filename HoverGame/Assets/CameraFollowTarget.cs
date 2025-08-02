using UnityEngine;

public class CameraFollowTarget : MonoBehaviour
{
    public Transform target;
    public Rigidbody targetRigidbody;

    public Vector3 offset = new Vector3(0f, 0.1f, 0.7f);
    float smoothSpeed = 100f;
    public float rotationSmoothSpeed = 40f;

    float zoomMultiplier = 0.00025f;
    float maxZoomOut = 0.25f;
    float currentZoom = 0f;
    float zoomLerpSpeed = 5f;

    private void Start()
    {
       // offset = this.transform.position;
    }
    void LateUpdate()
    {
        FollowTarget();
    }

    void FollowTarget()
    {
        if (target == null) return;

        // float speed = targetRigidbody != null ? targetRigidbody.linearVelocity.magnitude : 0f;


        // float targetZoom = Mathf.Clamp(speed * zoomMultiplier, 0f, maxZoomOut);
        // currentZoom = Mathf.Lerp(currentZoom, targetZoom, Time.deltaTime * zoomLerpSpeed);

        // Vector3 dynamicOffset = offset - (target.forward * currentZoom);
        // Vector3 desiredPosition = target.position + target.rotation * dynamicOffset;

        //transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * smoothSpeed);

        //Quaternion desiredRotation = Quaternion.LookRotation(target.forward, Vector3.up);
        //transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, Time.deltaTime * rotationSmoothSpeed);

        // Simple static offset
        Vector3 desiredPosition = target.position + target.rotation * offset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * smoothSpeed);

        // Smooth rotation to follow target direction
        Quaternion desiredRotation = Quaternion.LookRotation(target.forward, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, Time.deltaTime * rotationSmoothSpeed);
    

    
    }

}
