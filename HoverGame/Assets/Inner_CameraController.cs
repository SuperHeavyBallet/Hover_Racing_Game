using UnityEngine;

public class Inner_CameraController : MonoBehaviour
{
    public Transform target; // Assign this to the player
    public Vector3 offset = new Vector3(0f, 2f, -5f); // Distance behind the player
    public float smoothSpeed = 5f;

    float maxLateralShift = 0.025f;
    float maxVerticalShift = 0.01f;
    float shakeSpeed = 10f;

    private Vector3 basePosition;
    private float shakeTimer = 0f;

    void FixedUpdate()
    {
        if (target == null) return;

        // Base follow
        Vector3 desiredPosition = target.TransformPoint(offset);
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        // Optional shake overlay
        float offsetX = Mathf.PerlinNoise(Time.time * shakeSpeed, 0f) * 2f - 1f;
        float offsetY = Mathf.PerlinNoise(0f, Time.time * shakeSpeed) * 2f - 1f;
        offsetX *= maxLateralShift;
        offsetY *= maxVerticalShift;

        transform.position = smoothedPosition + new Vector3(offsetX, offsetY, 0f);
    }

    public void TriggerShake(float duration = 0.5f)
    {
        shakeTimer = duration;
        Debug.Log("TRIGGER SHAKE");
    }

    public void SetShakeAmount(float amount)
    {
        maxLateralShift = amount / 2;
        maxVerticalShift = amount;
    }
}
