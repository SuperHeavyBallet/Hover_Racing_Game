using UnityEngine;

public class ObjectCounterPivot : MonoBehaviour, IObjectCounterRotate
{

    public GameObject objectToRotate;
    public Transform PivotToRotateAround;

    public float minYaw = -20f;
    public float maxYaw = 20f;
    private float currentYaw = 0f;
    public float returnSpeed = 150f;

    public bool isReceivingInput = false;


    // Update is called once per frame
    void Update()
    {
        if (objectToRotate != null)
        {
            if (!isReceivingInput)
            {
                currentYaw = Mathf.MoveTowards(currentYaw, 0f, returnSpeed * Time.deltaTime);
                objectToRotate.transform.localRotation = Quaternion.Euler(0f, currentYaw, 0f);
            }

            // Reset flag each frame
            isReceivingInput = false;
        }

    }

    public void OnShouldCounterRotate(float turnAmount)
    {

        Debug.Log("RECEIVED ROTATE CALL");

        isReceivingInput = true;
        currentYaw = Mathf.Clamp(currentYaw + turnAmount, minYaw, maxYaw);

        // Set rotation relative to initial rotation
        objectToRotate.transform.localRotation = Quaternion.Euler(0f, currentYaw, 0f);
    }
}
