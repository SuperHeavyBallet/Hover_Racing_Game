using UnityEngine;

public class Inner_CameraController : MonoBehaviour
{
    GameObject cameraContainer;

    public float maxLateralShift = 0.001f;
    public float maxVerticalShift = 0.001f;
    public float shakeDuration = 0.5f;
    public float shakeSpeed = 10f;

    private Vector3 basePosition;
    private float shakeTimer = 0f;

    // Start is called before the first frame update
    void Start()
    {
        cameraContainer = this.gameObject;
        basePosition = transform.localPosition; // Local since it's nested inside the ship
    }

    void Update()
    {
        if (shakeTimer > 0f)
        {
            shakeTimer -= Time.deltaTime;

            float offsetX = Mathf.PerlinNoise(Time.time * shakeSpeed, 0f) * 2f - 1f;
            float offsetY = Mathf.PerlinNoise(0f, Time.time * shakeSpeed) * 2f - 1f;

            offsetX *= maxLateralShift;
            offsetY *= maxVerticalShift;

            transform.localPosition = basePosition + new Vector3(offsetX, offsetY, 0f);
        }
        else
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, basePosition, Time.deltaTime * 10f);
        }
    }

    public void TriggerShake(float duration = 0.5f)
    {
        shakeTimer = duration;
    }
}
