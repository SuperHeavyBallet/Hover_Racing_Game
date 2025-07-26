using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine;
using Unity.VisualScripting;


public class Ship_Movement : MonoBehaviour
{


    public TextMeshProUGUI boostText;
    public TextMeshProUGUI speedDisplay;

    public ShipMeshSelector shipMeshSelector;

    Rigidbody rigidBody;

    public Transform hoverCastPosition;

    public Camera worldCamera;
    public Camera playerCamera;


    public bool isGrounded;

    

    Vector2 recievedMoveInput = Vector2.zero;




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
    //public bool isFiring;

    public SceneStartup sceneStartup;
    public string VehicleClass_Received;

    public bool boostActivated;
    public bool limitActivated;

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

    /// <summary>

    float BASE_TopSpeed;
    float BASE_MovementForce;
    float BASE_RotationSpeed;
    float BASE_BoostConsumptionRate;
  

    public float CURRENT_TopSpeed;
    public float CURRENT_MovementForce;
    public float CURRENT_RotationSpeed;
    public float CURRENT_BoostConsumptionRate;
    public float CURRENT_SideBoostAmount;

    int STAT_ManualBoostAmount;
    float STAT_RotationSpeed;

    bool trackBoostActivated;

    int boostZoneAmount = 50;

    Ship_Constructor shipConstructor;

    List<ComponentName> componentList= new List<ComponentName>();

    public TextMeshProUGUI topSpeedText;
    public TextMeshProUGUI topPowerText;
    public TextMeshProUGUI topWeightText;

    Audio_Manager audioManager;
    public AudioClip AUDIO_boostTrigger;
    public AudioClip AUDIO_boostOFF;
    public AudioClip AUDIO_boostFlow;
    public AudioClip AUDIO_engineIdle;

    public bool boostersActive;

    public bool engineIdle = false;
    public bool hasBoostGulp = false;
    public GameObject boostGulpText;

    public Inner_CameraController innerCameraController;

    Ship_Passport shipPassport;

    public float shipWeight;

    public bool isHoldingThrust;
    public float receivedThrust;
    public float receivedSteering;
    public float receivedSideBoost;
    public bool isSideBoosting;
    float sideBoostAmount = 10f;
    
    Coroutine TurnOffSideBoost;

    GameObject chosenFrame;

    List<SideBoosterController> sideBoostControllersRight = new List<SideBoosterController>();
    List<SideBoosterController> sideBoostControllersLeft = new List<SideBoosterController>();

    /// </summary>

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        audioManager = GameObject.Find("AudioManager").GetComponent<Audio_Manager>();

        megaBoostText.gameObject.SetActive(false);
        rigidBody = GetComponent<Rigidbody>();

        // Store the original local position to oscillate from
        visualBasePosition = shipVisual.localPosition;

        
        shipConstructor = GetComponent<Ship_Constructor>();
        componentList = shipConstructor.GetShipLoadout();

        shipPassport = Ship_Passport.Instance;
        

        CalculatePerformance(componentList);
        CheckBoostGulp();

        engineIdle = true;
        audioManager.PlayEngineIdleSound(AUDIO_engineIdle);
        
        AssignSideBoosters();
       


    }

    void AssignSideBoosters()
    {
        if (shipConstructor != null)
        {
            chosenFrame = shipConstructor.GetFrameReference();
        }

        Frame_Layout frame_Layout = chosenFrame.GetComponent<Frame_Layout>();



        if (frame_Layout != null)
        {

            sideBoostControllersLeft = frame_Layout.GetBoosters_Left();
            sideBoostControllersRight = frame_Layout.GetBoosters_Right();

            Debug.Log(sideBoostControllersLeft.Count);
            Debug.Log(sideBoostControllersRight.Count);
        }
        else
        {
            Debug.Log("NO FRAME FOUND....");
        }
    }

    void CheckBoostGulp()
    {
        foreach(var component in componentList)
        {
            if(component == ComponentName.boostGulp)
            {
                hasBoostGulp = true;
            }
        }

    }

    void CalculatePerformance(List<ComponentName> components)
    {
        int rawSpeed = 0;
        int rawPower = 0;
        int totalWeight = 0;
        int fuel = 100;
        float boostConsumptionRate = 0;

        float engineCount = 0;
        float jetEngineCount = 0;
        float fuelTankCount = 0;
        float aireonCount = 0;

        foreach (var c in components)
        {
            switch (c)
            {
                case ComponentName.lightFrame: totalWeight += 50; fuel += 30; break;
                case ComponentName.mediumFrame: totalWeight += 70; fuel += 10; break;
                case ComponentName.heavyFrame: totalWeight += 100; break;

                case ComponentName.engine: totalWeight += 100; rawPower += 15; rawSpeed += 30; engineCount += 1; break;
                case ComponentName.jetEngine: totalWeight += 70; rawPower += 20; rawSpeed += 25; jetEngineCount += 1; break;

                case ComponentName.fuelTank: totalWeight += 10; fuel += 50; fuelTankCount += 1; break;
                case ComponentName.aireon: totalWeight -= 10; rawSpeed += 10; boostConsumptionRate -= 0.1f; aireonCount += 1; break;
                default: break;
            }
        }

        float weightFactor = Mathf.Max(totalWeight, 1);

        BASE_TopSpeed = Mathf.Max(10f, rawSpeed - weightFactor * 0.1f);
        BASE_MovementForce = Mathf.Max(10f, rawPower - weightFactor * 0.05f);

        float minWeight = 100f;
        float maxWeight = 1000f;
        float t = Mathf.InverseLerp(maxWeight, minWeight, weightFactor); // Note the reverse
        BASE_RotationSpeed = Mathf.Lerp(30f, 150f, t); // Heavy = 30, Light = 150

        BASE_BoostConsumptionRate = 0.25f + boostConsumptionRate;

        shipWeight = Mathf.Max(0, totalWeight);

        maxBoostFuel = fuel;
        currentBoostFuel = fuel;
        STAT_ManualBoostAmount = rawPower;

        topSpeedText.text = $"Top Speed: {BASE_TopSpeed:F1}";
        topPowerText.text = $"Top Power: {BASE_MovementForce:F1}";
        topWeightText.text = $"Weight: {totalWeight}";

    
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


    float CalculateCurrentTopSpeed(bool isManualBoosting, bool isTrackBoosting, bool isLimiting)
    {

        int workingTopSpeed = 0;

        if (isManualBoosting)
        {
            workingTopSpeed += STAT_ManualBoostAmount;
        }

        if ( isTrackBoosting)
        {
            workingTopSpeed += boostZoneAmount;
        }

        if (isLimiting)
        {
            workingTopSpeed -= 50;
        }
        

        return BASE_TopSpeed + workingTopSpeed;
    }

    float CalculateCurrentRotationSpeed(bool isManualBoosting, bool isTrackBoosting, bool isLimiting)
    {
        int workingRotationSpeed = 0;

        if (isLimiting)
        {
            workingRotationSpeed += 100;
        }

        return BASE_RotationSpeed + workingRotationSpeed;
    }

    float CalculateCurrentMovementForce(bool isManualBoosting, bool isTrackBoosting, bool isLimiting)
    {
        int workingMovementForce = 0;
        if (isManualBoosting)
        {
            workingMovementForce += STAT_ManualBoostAmount;
        }
        if (isTrackBoosting)
        {
            workingMovementForce += boostZoneAmount;
        }

        if (isLimiting)
        {
             workingMovementForce -= 50;
        }

        return BASE_MovementForce + workingMovementForce;
    }

    float CalculateCurrentSideBoostAmount()
    {
        float weightFactor = Mathf.Clamp01(shipWeight / 500f);
        float agilityFactor = 1f - weightFactor;

        float topSpeed = CURRENT_TopSpeed;
        if (topSpeed <= 0.01f) topSpeed = 0.01f; // Prevent division by zero

        float speedLerp = Mathf.Lerp(sideBoostAmount, sideBoostAmount * 4f, forwardSpeed / topSpeed);
        float weightedBoost = speedLerp * Mathf.Lerp(0.5f, 1.5f, agilityFactor);

        return weightedBoost;

    }

    // Update is called once per frame
    void Update()
    {
        
        CURRENT_TopSpeed = CalculateCurrentTopSpeed(boostActivated, trackBoostActivated, limitActivated);
        CURRENT_MovementForce = CalculateCurrentMovementForce(boostActivated, trackBoostActivated, limitActivated);
        CURRENT_RotationSpeed = CalculateCurrentRotationSpeed(boostActivated, trackBoostActivated, limitActivated);
        CURRENT_BoostConsumptionRate = BASE_BoostConsumptionRate;
        CURRENT_SideBoostAmount = CalculateCurrentSideBoostAmount();

        CheckBoostFiring();

        innerCameraController.SetShakeAmount(forwardSpeed / 2000);
        
        UpdateEngineSound();
       

        DisplaySpeed();

        

        
        
    }

    void UpdateEngineSound()
    {
        if (forwardSpeed * 1.25f > audioManager.baseEnginePitch)
        {

            audioManager.UpdateEnginePitch(forwardSpeed * 1.25f);
        }
        else
        {
            audioManager.ResetEnginePitch();
        }
    }

    void CheckBoostFiring()
    {

       

        if (boostActivated  && currentBoostFuel > 0)
        {

            boostersActive = true;
            boostText.text = "BOOST";
            foreach (var listener in engineFireListeners)
            {
                if (listener != null)
                {
                    listener.OnShipBoostFiring(true);
                }

            }

            currentBoostFuel -= CURRENT_BoostConsumptionRate;

         
        }
        else if (boostActivated && currentBoostFuel <= 0)
        {
            boostActivated = false;
            audioManager.StopPlayerSound();
            audioManager.PlayPlayerSound_OneShot(AUDIO_boostOFF);
        }
        
        else
        {
            
            audioManager.StopPlayerSound();
            audioManager.PlayPlayerSound_OneShot(AUDIO_boostOFF);
            boostText.text = "--";
            boostersActive = false;
            foreach (var listener in engineFireListeners)
            {
                if (listener != null)
                {
                    listener.OnShipBoostFiring(false);
                }

            }

            if (currentBoostFuel < maxBoostFuel)
            {
                currentBoostFuel += 0.5f;

          
            }

        }

        
        if (hasBoostGulp && forwardSpeed > (CURRENT_TopSpeed / 4) && currentBoostFuel < maxBoostFuel)
        {
            boostGulpText.SetActive(true);
            currentBoostFuel += forwardSpeed / 500;
        }
        else
        {
            boostGulpText.SetActive(false);
        }

        currentBoostFuel = Mathf.Clamp(currentBoostFuel, 0f, maxBoostFuel);

        currentBoostFuelText.text = currentBoostFuel.ToString();
    }

    void DisplaySpeed()
    {
        if (forwardSpeed > 0)
        {
            speedDisplay.text = Mathf.RoundToInt(forwardSpeed).ToString();
        }
        else
        {
            speedDisplay.text = "00";
            //boostActivated = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {

            if (other.gameObject.CompareTag("BoostZone") && !enteredBoostZone)
            {
                enteredBoostZone = true;
                megaBoostText.gameObject.SetActive(true);

                trackBoostActivated = true;
                
                if (extraBoost != null)
                {
                    StopCoroutine(extraBoost);
                }

                extraBoost = StartCoroutine(ResetSpeed());
            }
            
        if(other.gameObject.CompareTag("PickUp"))
        {
              other.gameObject.SetActive(false);
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
        trackBoostActivated = false;
        extraBoost = null;
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
        float sideBoostTilt = 1;

        if(isSideBoosting)
        {
            sideBoostTilt = 1.5f;
        }
        // Assume input X is turn direction: -1 (left) to +1 (right)
        float targetTilt = -receivedSteering * maxTiltAngle * sideBoostTilt;

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

        if (isBoosting == true && currentBoostFuel > 0)
        {
           // innerCameraController.TriggerShake();

            audioManager.PlayPlayerSound_OneShot(AUDIO_boostTrigger);
            audioManager.PlayBoostSound();
            
        }
        
        else if(isBoosting == false && boostersActive)
        {
            audioManager.StopBoostSound();
            audioManager.PlayPlayerSound_OneShot(AUDIO_boostOFF);
        }

        
    }

    public void AddSideBoost_Right(bool addSideBoost_Right)
    {
        if(addSideBoost_Right == true)
        {
            receivedSideBoost = 1;
            isSideBoosting = true;
        }
        else
        {
            receivedSideBoost = 0;
            isSideBoosting = false;
        }
      
       

        foreach (SideBoosterController controller in sideBoostControllersRight)
        {

           
            controller.ActivateFire(isSideBoosting);
        }
    }

    public void AddSideBoost_Left(bool addSideBoost_Left)
    {
        if(addSideBoost_Left == true)
        {
            receivedSideBoost = -1;
            isSideBoosting = true;
        }
        else
        {
            receivedSideBoost = 0;
            isSideBoosting = false;
        }

      

        foreach (SideBoosterController controller in sideBoostControllersLeft)
        {

            
            controller.ActivateFire(isSideBoosting);
        }
    }

   

    public void ActivateLimit(bool isLimiting)
    {
        limitActivated = isLimiting;
    }

    public void UpdateThrust(bool holdingThrust)
    {
        isHoldingThrust = holdingThrust;

        if (isHoldingThrust)
        {
            receivedThrust = 1;
        }
        else
        {
            receivedThrust = 0;
        }

        UpdateEngineListeners();
        UpdateBounceAmount();

    }
    public void UpdateSteering(Vector2 movementValue)
    {

        


        // Get current rotation
        Vector3 euler = transform.rotation.eulerAngles;


        //recievedMoveInput = movementValue;

        receivedSteering = movementValue.x;

        

        // Convert to -180 to +180 range
        if (euler.x > 180) euler.x -= 360;
        if (euler.z > 180) euler.z -= 360;

        // Clamp pitch (X) and roll (Z)
        euler.x = Mathf.Clamp(euler.x, -2f, 2f);
        euler.z = Mathf.Clamp(euler.z, -2f, 2f);

        // Preserve yaw (Y)
        transform.rotation = Quaternion.Euler(euler);

        

     

    }

    void ApplyMovement()
    {

        Vector3 flatVelocity = rigidBody.linearVelocity;
        flatVelocity.y = 0f; // Only consider horizontal movement
        forwardSpeed = Vector3.Dot(flatVelocity, transform.forward);

        if (forwardSpeed < CURRENT_TopSpeed)
        {
            Vector3 forwardForce = transform.forward * receivedThrust * CURRENT_MovementForce;
            rigidBody.AddForce(forwardForce, ForceMode.Acceleration);
        }

        // Apply side boost impulse
        if (isSideBoosting)
        {
            Vector3 sideForce = transform.right * receivedSideBoost * CURRENT_SideBoostAmount;
            rigidBody.AddForce(sideForce, ForceMode.VelocityChange);

            if (TurnOffSideBoost == null)
            {
                TurnOffSideBoost = StartCoroutine(TurnOffSideBoostAfterTime(0.1f));
            }
            else
            {
                receivedSideBoost = 0;
                StopCoroutine(TurnOffSideBoost);
            }
        }

        // Clamp only forward component
        Vector3 currentVelocity = rigidBody.linearVelocity;
        Vector3 forwardComponent = Vector3.Project(currentVelocity, transform.forward);
        Vector3 lateralAndVertical = currentVelocity - forwardComponent;

        if (forwardComponent.magnitude > CURRENT_TopSpeed)
        {
            forwardComponent = forwardComponent.normalized * CURRENT_TopSpeed;
            rigidBody.linearVelocity = forwardComponent + lateralAndVertical;
        }

        // Apply downward drag from ship weight
        float downwardPull = shipWeight / 1000f;
        rigidBody.linearVelocity -= new Vector3(0f, downwardPull, 0f);





        UpdateRotation();
        

    }

    void UpdateRotation()
    {
        // Rotation (Y axis only)
        float turnAmount = receivedSteering * CURRENT_RotationSpeed * Time.deltaTime;
        transform.RotateAround(pivotPoint.position, Vector3.up, turnAmount);

        UpdateEngineNozzleRotations(turnAmount);
    }

    void UpdateEngineNozzleRotations(float turnAmount)
    {
        foreach (var listener in engineFireListeners)
        {
            listener.OnShipRotateNozzle(turnAmount * -1);
        }
    }

    IEnumerator TurnOffSideBoostAfterTime(float delay)
    {
        yield return new WaitForSeconds(delay);
        receivedSideBoost = 0;
        isSideBoosting = false;
        TurnOffSideBoost = null;

        Vector3 velocity = rigidBody.linearVelocity;
        Vector3 forward = transform.forward;
        Vector3 vertical = Vector3.up;

        Vector3 forwardComponent = Vector3.Project(velocity, forward);
        Vector3 verticalComponent = Vector3.Project(velocity, vertical);

        rigidBody.linearVelocity = forwardComponent + verticalComponent;
    }

    void UpdateEngineListeners()
    {
        if (receivedThrust > 0)
        {
            //isFiring = true;
            foreach (var listener in engineFireListeners)
            {
                if (listener != null)
                {
                    listener.OnShipEngineFiring(true);

                }

            }
            bounceSpeed = movingBounceSpeed;
            bounceAmplitude = movingBounceAmplitude;
        }
        else
        {
   
            foreach (var listener in engineFireListeners)
            {
                if (listener != null)
                {
                    listener.OnShipEngineFiring(false);
                }

            }
            bounceSpeed = idleBounceSpeed;
            bounceAmplitude = idleBounceAmplitude;
        }
    }

    void UpdateBounceAmount()
    {
        if (receivedThrust > 0)
        {
            
            bounceSpeed = movingBounceSpeed;
            bounceAmplitude = movingBounceAmplitude;
        }
        else
        {
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
        
        atLeastOneGrounded = false;

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

        if(atLeastOneGrounded)
        {
            isGrounded = true;
        }
        else
        {
            isGrounded= false;
        }

    }

    

}
