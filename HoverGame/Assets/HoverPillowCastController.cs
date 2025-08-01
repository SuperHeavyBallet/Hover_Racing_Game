using System.Drawing;
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

    Quaternion neutralRotation;

    Rigidbody rigidBody;

    private void Awake()
    {
        neutralRotation = transform.rotation;
        rigidBody = GetComponent<Rigidbody>();
    }
    private void FixedUpdate()
    {
        CastHoverZone();

      
    }

    void CastHoverZone()
    {

        atLeastOneGrounded = false;
        isGrounded = false;

        CheckIfGrounded();

        if (atLeastOneGrounded)
        {
            isGrounded = true;
        }
        else
        {
            Vector3 downwardPull = Vector3.down * (reGroundForce * 2f);
            rigidBody.AddForceAtPosition(downwardPull, this.transform.position, ForceMode.Force);
        }

    }

    void CheckIfGrounded()
    {
        foreach (Transform point in hoverPoints)
        {

            Ray ray = new Ray(point.position, Vector3.down);
            float rayLength = hoverHeight + 0.5f; // Give extra tolerance

            Debug.DrawRay(point.position, Vector3.down * rayLength, UnityEngine.Color.red);

            if (Physics.Raycast(ray, out RaycastHit hit, rayLength, groundMask))
            {

                float distance = hit.distance;
                float compressionRatio = 1f - (distance / hoverHeight);
                float upwardForce = hoverForce * compressionRatio;
                float verticalSpeed = Vector3.Dot(rigidBody.GetPointVelocity(point.position), Vector3.up);
                float dampingForce = hoverDamp * verticalSpeed;

                rigidBody.AddForceAtPosition(hit.normal * (upwardForce - dampingForce), point.position, ForceMode.Force);
                atLeastOneGrounded = true;

            }
            else
            {
                // Soft downward pull to maintain gravity feel without snapping
                Vector3 downwardPull = Vector3.down * (reGroundForce * 0.2f);
                rigidBody.AddForceAtPosition(downwardPull, point.position, ForceMode.Force);
            }
        }
    }
}


