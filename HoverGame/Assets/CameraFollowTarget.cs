using UnityEngine;

public class CameraFollowTarget : MonoBehaviour
{

    public Transform target;                  // The object to follow (e.g. your ship)
    Vector3 offset = new Vector3(0f, 1.5f, -3f); // Default position offset behind the target
    float smoothSpeed = 50f;            // Lerp speed for position
    float rotationSmoothSpeed = 50f;    // Lerp speed for rotation

    public Rigidbody targetRigidbody;         // Optional: for speed-based zoom
    float zoomMultiplier = 0.001f;      // How much camera moves back based on speed
    float maxZoomOut = 0.25f;            // Max additional distance allowed from speed

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void FixedUpdate()
    {
        FollowTarget();
    }

    void FollowTarget()
    {
        if (target == null) return;

        // Calculate dynamic zoom based on target speed
        float speed = targetRigidbody != null ? targetRigidbody.linearVelocity.magnitude : 0f;
        float zoomOutDistance = Mathf.Clamp(speed * zoomMultiplier, 0f, maxZoomOut);

        // Adjust offset dynamically
        Vector3 dynamicOffset = offset - (target.forward * zoomOutDistance);

        // Desired camera position based on target position + offset
        Vector3 desiredPosition = target.position + target.rotation * dynamicOffset;

        // Smoothly move camera to desired position
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        // Smoothly rotate camera to look in the same direction as the target
        Quaternion desiredRotation = Quaternion.LookRotation(target.forward, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotation, rotationSmoothSpeed * Time.deltaTime);
    }
}
