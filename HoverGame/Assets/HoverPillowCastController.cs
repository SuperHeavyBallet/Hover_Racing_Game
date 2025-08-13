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

    Ship_Movement shipMovementController;

    private void Awake()
    {
        neutralRotation = transform.rotation;
        rigidBody = GetComponent<Rigidbody>();
        shipMovementController = GetComponent<Ship_Movement>();
    }
 

    public void CastHoverZone()
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
            shipMovementController.AddDownwardDrag();
        }

        if(isGrounded)
        {
            // This prevents any steering, probably remove
            //AlignWithTerrainNormal();
            Vector3 torque = Vector3.Cross(transform.up, Vector3.up) * 10f;
            rigidBody.AddTorque(torque, ForceMode.Acceleration);
        }
        else
        {
            // This ends up preventing steering when not grounded, probably remove it
            Quaternion currentRotation = transform.rotation;
            Quaternion targetRotation = Quaternion.Euler(0f, currentRotation.eulerAngles.y, 0f); // upright but keep yaw
           //transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, Time.fixedDeltaTime * 2f);
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

    void AlignWithTerrainNormal()
    {
        Vector3 averageNormal = Vector3.zero;
        int hitCount = 0;

        foreach (Transform point in hoverPoints)
        {
            if (Physics.Raycast(point.position, Vector3.down, out RaycastHit hit, hoverHeight + 0.5f, groundMask))
            {
                averageNormal += hit.normal;
                hitCount++;
            }
        }

        if (hitCount > 0)
        {
            averageNormal.Normalize();

            // Get current yaw (heading) from rotation
            Vector3 currentForward = transform.forward;
            Vector3 projectedForward = Vector3.ProjectOnPlane(currentForward, averageNormal).normalized;

            if (projectedForward.sqrMagnitude < 0.01f) projectedForward = transform.forward;

            Quaternion targetRotation = Quaternion.LookRotation(projectedForward, averageNormal);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * 5f);
        }
    }
}


