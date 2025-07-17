using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Ghost_Ship_Controller : MonoBehaviour
{

    private List<GhostFrame> shipPositions = new List<GhostFrame>();
    private bool canAddNewPosition = true;
    Coroutine delayAddNewPosition;

    

    GameObject ship;
    Vector3 shipPosition;
    Quaternion shipRotation;

    bool shouldRecord = false;

    public GameObject ghost;

    public float frameRate = 0.05f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ship = this.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if(shouldRecord && canAddNewPosition)
        {
            canAddNewPosition = false;
            AddNewPosition();

            if (delayAddNewPosition != null)
            {
                StopAllCoroutines();
            }

            delayAddNewPosition = StartCoroutine(DelayAddingPositions());
        }

    }

    public void StartRecordingRun()
    {
        shouldRecord = true;
    }

    public void CaptureRun()
    {
        SaveGhostToFile();
        SendRunToGhost();
    }

    void AddNewPosition()
    {
        shipPosition = ship.transform.position;
        shipRotation = ship.transform.rotation;

        shipPositions.Add(new GhostFrame(shipPosition, shipRotation));
        

    }

    private IEnumerator DelayAddingPositions()
    {
        yield return new WaitForSeconds(frameRate);
        canAddNewPosition = true;



    }



    void SaveGhostToFile()
    {
        string directory = Application.persistentDataPath;

        // Get all files that match the naming pattern
        string[] existingFiles = Directory.GetFiles(directory, "ghost_run_*.json");

        // Calculate the new file number
        int newFileNumber = existingFiles.Length + 1;


        // Generate the new file name
        string fileName = $"ghost_run_{newFileNumber}.json";
        string fullPath = Path.Combine(directory, fileName);

        // Serialize and save
        string json = JsonUtility.ToJson(new GhostRunData(shipPositions), true);
        File.WriteAllText(fullPath, json);

        Debug.Log("Ghost run saved to: " + fullPath);
    }

    void SendRunToGhost()
    {
        GhostRunReceiver ghostReceiver = ghost.GetComponent<GhostRunReceiver>();

        if(ghostReceiver != null)
        {
            ghostReceiver.ReceiveGhostRun(shipPositions);
        }
    }

    [System.Serializable]
    public class GhostRunData
    {
        public List<GhostFrame> frames;

        public GhostRunData(List<GhostFrame> positions)
        {
            this.frames = positions;
        }
    }
}
