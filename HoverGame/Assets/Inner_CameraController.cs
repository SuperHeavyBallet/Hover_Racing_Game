using UnityEngine;

public class Inner_CameraController : MonoBehaviour
{
    GameObject cameraContainer;

    float maxLateralShift = 0.025f;
    float maxVerticalShift = 0.01f;
    float shakeDuration = 0.5f;
    float shakeSpeed = 10f;

    private Vector3 basePosition;
    private float shakeTimer = 0f;

    bool isBoosting;

    // Start is called before the first frame update
    void Start()
    {
        cameraContainer = this.gameObject;
        basePosition = transform.localPosition; // Local since it's nested inside the ship
    }

    void Update()
    {
        

            float offsetX = Mathf.PerlinNoise(Time.time * shakeSpeed, 0f) * 2f - 1f;
            float offsetY = Mathf.PerlinNoise(0f, Time.time * shakeSpeed) * 2f - 1f;

            offsetX *= maxLateralShift;
            offsetY *= maxVerticalShift;

            transform.localPosition = basePosition + new Vector3(offsetX, offsetY, 0f);
   
    }

    public void TriggerShake(float duration = 0.5f)
    {
        shakeTimer = duration;

        isBoosting = true;
        Debug.Log("TRIGGER SHAKE");
    }

    public void StopShake()
    {
        isBoosting = false;
    }

    public void SetShakeAmount(float amount)
    {
        maxLateralShift = amount / 2;
        maxVerticalShift = amount;
    }
}
