using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine;
using Unity.VisualScripting;
using System.Drawing;


public class Ship_Movement : MonoBehaviour
{

    UI_Controller UI_Router;

    Rigidbody rigidBody;

    public Camera worldCamera;
    public Camera playerCamera;

    Vector2 recievedMoveInput = Vector2.zero;
    bool isRecievingMoveInput;

    public float forwardSpeed;

    public Transform shipVisual;
    float maxTiltAngle = 15f; // max roll angle
    float tiltSpeed = 5f;
    private float currentTilt = 0f;

    public Transform pivotPoint;

    public GameObject[] engines;


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

   
    public float currentNormalFuel = 500f;
    
    public float currentBoostFuel = 100f;

    public bool enteredBoostZone { get; private set; } = false;
    private Coroutine extraBoost;


    public float CURRENT_TopSpeed;
    public float CURRENT_MovementForce;
    public float CURRENT_RotationSpeed;
    public float CURRENT_NormalFuelConsumptionRate;
    public float CURRENT_BoostFuelConsumptionRate;
    public float CURRENT_SideBoostAmount;


    bool trackBoostActivated;

    int boostZoneAmount = 50;


    Ship_Constructor shipConstructor;

    Dictionary<ComponentSlotType, ComponentName> componentList = new();


    Audio_Manager audioManager;
    public AudioClip AUDIO_boostTrigger;
    public AudioClip AUDIO_boostOFF;
    public AudioClip AUDIO_boostFlow;
    public AudioClip AUDIO_engineIdle;

    public bool boostersActive;

    public bool engineIdle = false;
    public bool hasBoostGulp = false;

    public Inner_CameraController innerCameraController;

    Ship_Passport shipPassport;


    public bool isHoldingThrust;
    public float receivedThrust;
    public float receivedSteering;
    public float receivedSideBoost;
    public bool isSideBoosting;
    

    Coroutine TurnOffSideBoost;

    GameObject chosenFrame;

    List<SideBoosterController> sideBoostControllersRight = new List<SideBoosterController>();
    List<SideBoosterController> sideBoostControllersLeft = new List<SideBoosterController>();

    FuelController fuelController;

    public bool inPitStop { get; private set; } = false;
    private bool boostGulpActive;
    BoostFuelController boostFuelController;

    ShipMovementCalculator shipMovementCalculator;

    private void Awake()
    {
        InitialiseReferences();
    }

    void Start()
    {
        SetupStartComponents();
    }

    void Update()
    {
        UpdateStats();
        CheckBoostFiring();
        FireBoostEngines(boostersActive);
        UpdateCameraShake();
        UpdateAudio();
        UpdateUI();
    }

    public void ActivateBoostZone(bool conditional)
    {
        trackBoostActivated = conditional;

        if(conditional == true) enteredBoostZone = true;


        if (extraBoost != null) StopCoroutine(extraBoost);

        extraBoost = StartCoroutine(ResetSpeed());
    }

    public void ExitedBoostZone()
    {
        enteredBoostZone = false;
    }

    public void ActivatePitStop(bool conditional)
    {
        inPitStop=conditional;

        if(conditional == true) Refuel();
        
    }

    void FixedUpdate()
    {
        ApplyMovement();
        UpdateVisualTilt();
        UpdateVisualBounce();
    }

    #region // Sounds and Visuals /////////////////////////////////////////////////
    void UpdateAudio()
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

    void UpdateUI()
    {
        if (forwardSpeed > 0)
        {
            UI_Router.UpdateSpeedDisplay(Mathf.RoundToInt(forwardSpeed));
        }
        else
        {
            UI_Router.UpdateSpeedDisplay(00);
        }

     
        UI_Router.ShowBoostText(boostersActive);
        UI_Router.ShowBoostGulpText(boostGulpActive);
        UI_Router.ShowMegaBoostText(trackBoostActivated);
        UI_Router.ShowPitStopText(inPitStop);


        boostFuelController.UpdateBoostFuelCountDisplay(currentBoostFuel);

    }
    void UpdateVisualTilt()
    {
        float sideBoostTilt = 1;

        if (isSideBoosting)
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
    void UpdateCameraShake()
    {
        innerCameraController.SetShakeAmount(forwardSpeed / 2000);
    }
    #endregion

    #region // Checks /////////////////////////////////////////////////
    void InitialiseReferences()
    {
        audioManager = GameObject.Find("AudioManager").GetComponent<Audio_Manager>();

        UI_Router = GameObject.Find("PLAYER_UI").GetComponent<UI_Controller>();

        UI_Router.ShowMegaBoostText(false);
        UI_Router.ShowMegaBoostText(false);

        rigidBody = GetComponent<Rigidbody>();

        fuelController = GetComponent<FuelController>();
        boostFuelController = GetComponent<BoostFuelController>();

        shipConstructor = GetComponent<Ship_Constructor>();
        shipMovementCalculator = GetComponent<ShipMovementCalculator>();
        

        
    }

    void SetupStartComponents()
    {
        visualBasePosition = shipVisual.localPosition;
        shipPassport = Ship_Passport.Instance;
        componentList = shipPassport.GetShipLoadout();
        shipMovementCalculator.CalculatePerformance(componentList);
        hasBoostGulp = shipPassport.CheckBoostGulpPresent();

        engineIdle = true;
        audioManager.PlayEngineIdleSound(AUDIO_engineIdle);

        AssignSideBoosters();

    }

    void FireBoostEngines(bool firing)
    {
        foreach (var listener in engineFireListeners)
        {
            if (listener != null)
            {
                listener.OnShipBoostFiring(firing);
            }
        }
    }

    void CheckBoostFiring()
    {
        if (boostActivated && currentBoostFuel > 0)
        {
            boostersActive = true;

            currentBoostFuel -= CURRENT_BoostFuelConsumptionRate;
        }
        else if (boostActivated && currentBoostFuel <= 0)
        {
            boostActivated = false;
            boostersActive = false;
            audioManager.StopPlayerSound();
            audioManager.PlayPlayerSound_OneShot(AUDIO_boostOFF);
        }

        else
        {

            audioManager.StopPlayerSound();
            audioManager.PlayPlayerSound_OneShot(AUDIO_boostOFF);
            
            boostersActive = false;
            boostActivated = false;

            if (currentBoostFuel < shipMovementCalculator.maxBoostFuel)
            {
                currentBoostFuel += 0.5f;
            }

        }


        if (hasBoostGulp && forwardSpeed > (CURRENT_TopSpeed / 4) && currentBoostFuel < shipMovementCalculator.maxBoostFuel)
        {
            boostGulpActive = true;
            
            currentBoostFuel += forwardSpeed / 500;
        }
        else
        {
            boostGulpActive = false;
        }

        currentBoostFuel = Mathf.Clamp(currentBoostFuel, 0f, shipMovementCalculator.maxBoostFuel);

        
        
    }
  

    void Refuel()
    {
        currentNormalFuel = shipMovementCalculator.maxNormalFuel;
        currentBoostFuel = shipMovementCalculator.maxBoostFuel;

        fuelController.UpdateFuelCountDisplay(currentNormalFuel);
        boostFuelController.UpdateBoostFuelCountDisplay(currentBoostFuel);
    }

    #endregion

    #region // Activate Effects /////////////////////////////////////////////////
    public void ActivateBoost(bool isBoosting)
    {
        boostActivated = isBoosting;

        if (isBoosting == true && currentBoostFuel > 0)
        {
            audioManager.PlayPlayerSound_OneShot(AUDIO_boostTrigger);
            audioManager.PlayBoostSound();

        }

        else if (isBoosting == false && boostersActive)
        {
            audioManager.StopBoostSound();
            audioManager.PlayPlayerSound_OneShot(AUDIO_boostOFF);
        }


    }
    public void AddSideBoost_Right(bool addSideBoost_Right)
    {
        if (addSideBoost_Right == true)
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
        if (addSideBoost_Left == true)
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

    void UpdateEngineListeners()
    {
        if (receivedThrust > 0)
        {
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
    public void RegisterEngineFireListener(IEngineFireListener listener)
    {
        if (!engineFireListeners.Contains(listener))
        {
            engineFireListeners.Add(listener);
        }
    }
    public void UnregisterEngineFireListener(IEngineFireListener listener)
    {
        if (engineFireListeners.Contains(listener))
        {
            engineFireListeners.Remove(listener);
        }
    }
    void AssignSideBoosters()
    {
        if (shipPassport != null)
        {
            chosenFrame = shipPassport.GetShipFrame();

            if(chosenFrame != null)
            {
                Frame_Layout frame_Layout = chosenFrame.GetComponent<Frame_Layout>();

                if (frame_Layout != null)
                {
                    sideBoostControllersLeft = frame_Layout.GetBoosters_Left();
                    sideBoostControllersRight = frame_Layout.GetBoosters_Right();
                }
                else { Debug.Log("FRAME LAYOUT PRESENT"); }
            }
            else { Debug.Log("CHOSEN FRAME PRESENT"); }

        }
        else { Debug.Log("NO PASSPORT PRESENT"); }

       
    }
    #endregion 

    #region // Stat Adjusts /////////////////////////////////////////////////
    void UpdateStats()
    {
        CURRENT_TopSpeed = shipMovementCalculator.CalculateCurrentTopSpeed(boostActivated, trackBoostActivated, limitActivated);
        CURRENT_MovementForce = shipMovementCalculator.CalculateCurrentMovementForce(boostActivated, trackBoostActivated, limitActivated);
        CURRENT_RotationSpeed = shipMovementCalculator.CalculateCurrentRotationSpeed(boostActivated, trackBoostActivated, limitActivated);
        CURRENT_NormalFuelConsumptionRate = shipMovementCalculator.BASE_NormaFuelConsumptionRate;
        CURRENT_BoostFuelConsumptionRate = shipMovementCalculator.BASE_BoostFuelConsumptionRate;
        CURRENT_SideBoostAmount = shipMovementCalculator.CalculateCurrentSideBoostAmount();
    }
    public void UpdateThrust(bool holdingThrust)
    {
        isHoldingThrust = holdingThrust;
        receivedThrust = isHoldingThrust ? 1 : 0;

        UpdateEngineListeners();
        UpdateBounceAmount();

    }
    public void UpdateSteering(Vector2 movementValue)
    {
        Vector3 euler = transform.rotation.eulerAngles;

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


    private IEnumerator ResetSpeed()
    {
        yield return new WaitForSeconds(5);
        trackBoostActivated = false;
        extraBoost = null;
    }
    
    #endregion

    #region // Movement and Physics /////////////////////////////////////////////////
    void ApplyMovement()
    {

        Vector3 flatVelocity = rigidBody.linearVelocity;
        flatVelocity.y = 0f; // Only consider horizontal movement
        forwardSpeed = Vector3.Dot(flatVelocity, transform.forward);

        if(isHoldingThrust)
        {
            currentNormalFuel -= CURRENT_NormalFuelConsumptionRate;
            fuelController.UpdateFuelCountDisplay(currentNormalFuel);
        }

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
        float downwardPull = shipMovementCalculator.shipWeight / 1000f;
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
    void UpdateEngineNozzleRotations(float turnAmount)
    {
        foreach (var listener in engineFireListeners)
        {
            listener.OnShipRotateNozzle(turnAmount * -1);
        }
    }
    #endregion

 
    
}
