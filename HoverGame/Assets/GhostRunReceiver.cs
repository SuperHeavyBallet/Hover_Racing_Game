using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class GhostRunReceiver : MonoBehaviour
{
    private List<GhostFrame> recordedPath = new List<GhostFrame>();
    private float ghostTimer = 0f;
    //private int currentIndex = 0;

    private bool replayActive = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!replayActive || recordedPath.Count == 0)
            return;

        // Move the ghost along the path
        PlayGhost();
    }

    void PlayGhost()
    {
        ghostTimer += Time.deltaTime;

        float stepInterval = 0.05f;
        int nextIndex = Mathf.FloorToInt(ghostTimer / stepInterval);

        if(nextIndex >= recordedPath.Count)
        {
            replayActive = false;
            return;
        }


        Vector3 from = recordedPath[Mathf.Max(0, nextIndex -1)].ToVector3();
        Vector3 to = recordedPath[nextIndex].ToVector3();

        float t = (ghostTimer % stepInterval) / stepInterval;
        transform.position = Vector3.Lerp(from, to, t);

        Quaternion fromRot = recordedPath[Mathf.Max(0, nextIndex -1)].ToQuaternion();
        Quaternion toRot = recordedPath[nextIndex].ToQuaternion();

        transform.rotation = Quaternion.Lerp(fromRot, toRot, t);
    }


    public void ReceiveGhostRun(List<GhostFrame> shipPositions)
    {
        Debug.Log("Received Run! Count: " + shipPositions.Count);

        recordedPath = new List<GhostFrame>(shipPositions);
        this.transform.position = recordedPath[0].ToVector3();
        this.transform.rotation = recordedPath[0].ToQuaternion();

        ghostTimer = 0f;
        //currentIndex = 0;
        replayActive = true;
    }
}
