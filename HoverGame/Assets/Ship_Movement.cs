using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine;
using Unity.VisualScripting;


public class Ship_Movement : MonoBehaviour
{
    /*
    public enum VehicleClass
    {
        light,
        medium,
        heavy
    }

    public VehicleClass vehicleClass = VehicleClass.medium;*/

    public TextMeshProUGUI boostText;
    public TextMeshProUGUI speedDisplay;

    public ShipMeshSelector shipMeshSelector;

    Rigidbody rigidBody;

    public Transform hoverCastPosition;

    public Camera worldCamera;
    public Camera playerCamera;
    

    float movementForce = 80f;
    float rotationSpeed = 200f;

    

    Vector2 recievedMoveInput = Vector2.zero;


    float STAT_BOOSTTopSpeed = 400f;

    public float STAT_NormalTopSpeed = 300f;
    public float STAT_BoostTopSpeed = 400f;
    public float STAT_TopSpeed;

    private float baseTopSpeed;
    private float baseMovementForce;

    public float STAT_NormalMovementForce = 80f;
    public float STAT_BoostMovementForce = 100f;
    public float STAT_MovementForce;

    public float forwardSpeed;


    public Transform[] hoverPoints;
    private float hoverHeight = 1f;
    float hoverForce = 100f;
    float reGroundForce = 50f;
    float hoverDamp = 5f;
    public LayerMask groundMask;

    bool atLeastOneGrounded;

    public Transform shipVisual;
    float maxTiltAngle = 15f; // max roll angle
    float tiltSpeed = 5f;
    private float currentTilt = 0f;

    public Transform pivotPoint;

    public GameObject[] engines;
    public bool isFiring;

    public SceneStartup sceneStartup;
    public string VehicleClass_Received;

    public bool boostActivated;

    private List<IEngineFireListener> engineFireListeners = new List<IEngineFireListener>();

    float bounceAmplitude = 0.1f;   // Max up/down movement
    float bounceSpeed = 2f;         // How fast it bounces

    float idleBounceSpeed = 2f;
    float idleBounceAmplitude = 0.1f;

    float movingBounceSpeed = 4f;
    float movingBounceAmplitude = 0.025f;

    private Vector3 visualBasePosition;              // Starting point
    private float bounceTimer;                       // Internal time tracker

    public float currentBoostFuel = 100f;
    public float maxBoostFuel = 100f;
    public TextMeshProUGUI currentBoostFuelText;
    public bool enteredBoostZone;
    private Coroutine extraBoost;
    public TextMeshProUGUI megaBoostText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        
        megaBoostText.gameObject.SetActive(false);
        rigidBody = GetComponent<Rigidbody>();
        SelectShip();

        // Store the original local position to oscillate from
        visualBasePosition = shipVisual.localPosition;



    }
    void SelectShip()
    {
        shipMeshSelector = this.GetComponent<ShipMeshSelector>();
        SceneStartup sceneStartup = SceneStartup.Instance;

        if (sceneStartup != null)
        {
            VehicleClass_Received = sceneStartup.GetVehicleClass();
        }

       
        SwitchShipClass(VehicleClass_Received);
    }

    public void RegisterEngineFireListener(IEngineFireListener listener)
    {

       

        if(!engineFireListeners.Contains(listener))
        {
            engineFireListeners.Add(listener);
        }

       

    }

    public void UnregisterEngineFireListener(IEngineFireListener listener)
    {
        if(engineFireListeners.Contains(listener))
        {
            engineFireListeners.Remove(listener);
        }
    }

    void SwitchShipClass(string vehicleClass)
    {
        

        if (vehicleClass == "light")
        {
            shipMeshSelector.ShipMeshSelect("Light");
            STAT_NormalTopSpeed = 400f;
            STAT_BoostTopSpeed = 500f;
            STAT_NormalMovementForce = 120f;
            STAT_BoostMovementForce = 200f;
            rotationSpeed = 280f;
            maxBoostFuel = 100f;
            
        }
        else if (vehicleClass == "medium")
        {
            shipMeshSelector.ShipMeshSelect("Medium");
            STAT_NormalTopSpeed = 300f;
            STAT_BoostTopSpeed = 400f;
            STAT_NormalMovementForce = 80f;
            STAT_BoostMovementForce = 140f;
            rotationSpeed = 200f;
            maxBoostFuel = 150f;
        }
        else if (vehicleClass == "heavy")
        {
            shipMeshSelector.ShipMeshSelect("Heavy");
            STAT_NormalTopSpeed = 300f;
            STAT_BoostTopSpeed = 400f;
            STAT_NormalMovementForce = 60f;
            STAT_BoostMovementForce = 120f;
            rotationSpeed = 100f;
            maxBoostFuel = 200f;
        }
        else
        {
            Debug.Log("Error");
        }

        currentBoostFuel = maxBoostFuel;
        baseTopSpeed = STAT_NormalTopSpeed;
        baseMovementForce = STAT_NormalMovementForce;
    }

    // Update is called once per frame
    void Update()
    {
       // HoverHeight();

        if(boostActivated && forwardSpeed > 2 && currentBoostFuel > 0)
        {
            STAT_TopSpeed = STAT_BoostTopSpeed;
            STAT_MovementForce = STAT_BoostMovementForce;
            boostText.text = "BOOST";
            foreach (var listener in engineFireListeners)
            {
                if (listener != null)
                {
                    listener.OnShipBoostFiring(true);
                }

            }

            currentBoostFuel -= 0.25f;
        }
        else
        {
            boostActivated = false;
            STAT_TopSpeed = STAT_NormalTopSpeed;
            STAT_MovementForce = STAT_NormalMovementForce;
            boostText.text = "--";
            foreach (var listener in engineFireListeners)
            {
                if (listener != null)
                {
                    listener.OnShipBoostFiring(false);
                }

            }

            if(currentBoostFuel < maxBoostFuel)
            {
                currentBoostFuel += 0.5f;
            }
            
        }

        currentBoostFuelText.text = currentBoostFuel.ToString();

        if (forwardSpeed > 0)
        {
            speedDisplay.text = Mathf.RoundToInt(forwardSpeed).ToString();
        }
        else
        {
            speedDisplay.text = "00";
            boostActivated = false;
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
     
         
            if (other.gameObject.CompareTag("BoostZone") && !enteredBoostZone)
            {
                enteredBoostZone = true;
                megaBoostText.gameObject.SetActive(true);

              

                

                // Stop any previous coroutine before starting a new one
                if (extraBoost != null)
                {
                    StopCoroutine(extraBoost);
                }

            STAT_NormalTopSpeed = baseTopSpeed * 1.5f;
            STAT_NormalMovementForce = baseMovementForce * 1.5f;



            extraBoost = StartCoroutine(ResetSpeed());




            }
        
   
        
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.CompareTag("BoostZone"))
        {
            enteredBoostZone = false;
        }
    }

    private IEnumerator ResetSpeed()
    {
        yield return new WaitForSeconds(5);
        megaBoostText.gameObject.SetActive(false);
        STAT_NormalTopSpeed = baseTopSpeed;
        STAT_NormalMovementForce = baseMovementForce;

        extraBoost = null; // Clear reference



    }

    void FixedUpdate()
    {
        ApplyMovement();
        CastHoverZone();
        UpdateVisualTilt();
        UpdateVisualBounce();

        
    }


    void UpdateVisualTilt()
    {
        // Assume input X is turn direction: -1 (left) to +1 (right)
        float targetTilt = -recievedMoveInput.x * maxTiltAngle;

        // Smoothly interpolate tilt
        currentTilt = Mathf.Lerp(currentTilt, targetTilt, Time.deltaTime * tiltSpeed);

        // Apply local Z rotation (roll)
        Quaternion visualRotation = Quaternion.Euler(0f, 0f, currentTilt);
        shipVisual.localRotation = visualRotation;

        Quaternion cameraRotation = Quaternion.Euler(0f, 0f, currentTilt / 1.5f);

       worldCamera.transform.localRotation = cameraRotation;
       playerCamera.transform.localRotation = cameraRotation;
    }

    void UpdateVisualBounce()
    {
        bounceTimer += Time.deltaTime * bounceSpeed;

        // Sin wave for smooth bounce
        float offsetY = Mathf.Sin(bounceTimer) * bounceAmplitude;

        Vector3 targetPosition = visualBasePosition + new Vector3(0f, offsetY, 0f);

        // Smoothly interpolate to target
        shipVisual.localPosition = Vector3.Lerp(shipVisual.localPosition, targetPosition, Time.deltaTime * 5f);
    }

    public void ActivateBoost(bool isBoosting)
    {
        boostActivated = isBoosting;

    }

    public void UpdateMovement(Vector2 movementValue)
    {
        recievedMoveInput = movementValue;

        Vector3 euler = transform.rotation.eulerAngles;

        // Convert to -180 to +180 range
        if (euler.x > 180) euler.x -= 360;
        if (euler.z > 180) euler.z -= 360;

        // Clamp pitch (X) and roll (Z)
        euler.x = Mathf.Clamp(euler.x, -2f, 2f);
        euler.z = Mathf.Clamp(euler.z, -2f, 2f);

        // Preserve yaw (Y)
        transform.rotation = Quaternion.Euler(euler);

        if(recievedMoveInput.y > 0)
        {
            isFiring = true;
            foreach (var listener in engineFireListeners)
            {
                if(listener != null)
                {
                    listener.OnShipEngineFiring(true);
                }
                
            }
            bounceSpeed = movingBounceSpeed;
            bounceAmplitude = movingBounceAmplitude;
        }
        else
        {
            isFiring = false; 
            foreach (var listener in engineFireListeners)
            {
                if(listener != null)
                {
                    listener.OnShipEngineFiring(false);
                }
                
            }
            bounceSpeed = idleBounceSpeed;
            bounceAmplitude = idleBounceAmplitude;
        }

    }

    void SendEngineSignal()
    {
        foreach (GameObject engine in engines)
        {
            if (engine.GetComponent<EngineController>())
            {

                EngineController controller = engine.GetComponent<EngineController>();
                controller.isFiring = true;
            }
        }

        
    }

    void CastHoverZone()
    {
        

        foreach(Transform point in hoverPoints)
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

        if(!atLeastOneGrounded)
        {
           
        }

    }

    void ApplyMovement()
    {
        // Movement

        // Get current forward velocity component
        Vector3 flatVelocity = rigidBody.linearVelocity;
        flatVelocity.y = 0f; // Only consider horizontal movement
        forwardSpeed = Vector3.Dot(flatVelocity, transform.forward);

        if (forwardSpeed < STAT_TopSpeed)
        {
            // Apply acceleration forward
            Vector3 forwardForce = transform.forward * recievedMoveInput.y * STAT_MovementForce;
            rigidBody.AddForce(forwardForce, ForceMode.Acceleration);
        }
        else
        {
            // Clamp velocity to top speed in forward direction
            Vector3 clampedVelocity = transform.forward * STAT_TopSpeed;

            // Keep vertical + sideways velocity (no full overwrite)
            Vector3 currentVelocity = rigidBody.linearVelocity;
            Vector3 lateralVelocity = currentVelocity - Vector3.Project(currentVelocity, transform.forward);
            clampedVelocity += lateralVelocity;

            rigidBody.linearVelocity = clampedVelocity;
        }


        

        // Rotation (Y axis only)
        float turnAmount = recievedMoveInput.x * rotationSpeed * Time.deltaTime;
        transform.RotateAround(pivotPoint.position, Vector3.up, turnAmount);

       


    }

    void HoverHeight()
    {
        RaycastHit hit;

        if (Physics.Raycast(hoverCastPosition.position, Vector3.down, out hit, hoverHeight, groundMask))
        {
            float distanceToGround = hit.distance;
            float hoverError = hoverHeight - distanceToGround;

            float hoverForce = hoverError * 10f; // spring force
            float damping = rigidBody.linearVelocity.y * 2f; // dampen bouncing

            rigidBody.AddForce(Vector3.up * (hoverForce - damping), ForceMode.Acceleration);
        }
    }
}
