using UnityEngine;

public class HoverPillowCastController : MonoBehaviour
{

    public bool isGrounded;
    bool atLeastOneGrounded;


    public Transform[] hoverPoints;
    private float hoverHeight = 1f;
    float hoverForce = 100f;
    float reGroundForce = 50f;
    float hoverDamp = 5f;
    public LayerMask groundMask;

    Rigidbody rigidBody;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
    }
    private void FixedUpdate()
    {
        CastHoverZone();
    }

    void CastHoverZone()
    {

        atLeastOneGrounded = false;

        foreach (Transform point in hoverPoints)
        {
            Ray ray = new Ray(point.position, Vector3.down);
            float rayLength = hoverHeight + 0.5f; // Give extra tolerance

            if (Physics.Raycast(ray, out RaycastHit hit, rayLength, groundMask))
            {
                float distance = hit.distance;
                float compressionRatio = 1f - (distance / hoverHeight);
                float upwardForce = hoverForce * compressionRatio;
                float verticalSpeed = Vector3.Dot(rigidBody.GetPointVelocity(point.position), Vector3.up);
                float dampingForce = hoverDamp * verticalSpeed;

                rigidBody.AddForceAtPosition(Vector3.up * (upwardForce - dampingForce), point.position, ForceMode.Force);
                atLeastOneGrounded = true;

            }
            else
            {
                // Soft downward pull to maintain gravity feel without snapping
                Vector3 downwardPull = Vector3.down * (reGroundForce * 0.2f);
                rigidBody.AddForceAtPosition(downwardPull, point.position, ForceMode.Force);
            }
        }

        if (atLeastOneGrounded)
        {
            isGrounded = true;


        }
        else
        {
            isGrounded = false;
        }

    }
}
