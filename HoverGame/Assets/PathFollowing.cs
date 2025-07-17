using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class PathFollowing : MonoBehaviour
{

    public Transform[] wayPoints;

    public int currentTargetIndex = 0;
    public int nextTargetIndex = 0;

    public float speed = 10f;

    Transform nextTarget;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (wayPoints.Length == 0)
        {
            Debug.LogWarning("No waypoints assigned.");
            return;
        }

        nextTarget = wayPoints[nextTargetIndex];
    }

    // Update is called once per frame
    void Update()
    {
        if (wayPoints.Length == 0) return;

        float distance = Vector3.Distance(transform.position, nextTarget.position);

        if (distance < 10f) // You can tweak this threshold
        {
            UpdateWayPoints();
        }

        MoveToNextWaypoint();
    }

    void UpdateWayPoints()
    {
        nextTargetIndex = (nextTargetIndex + 1) % wayPoints.Length;
        nextTarget = wayPoints[nextTargetIndex];
    }

    void MoveToNextWaypoint()
    {
        Vector3 flatTargetPos = new Vector3(nextTarget.position.x, transform.position.y, nextTarget.position.z);

        transform.position = Vector3.MoveTowards(transform.position, flatTargetPos, speed * Time.deltaTime);


        Vector3 direction = (flatTargetPos - transform.position).normalized;

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f); // Adjust rotation speed here
        }

    }
}
