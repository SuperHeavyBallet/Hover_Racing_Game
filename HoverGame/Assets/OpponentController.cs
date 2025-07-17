using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class OpponentController : MonoBehaviour
{
    private List<GhostFrame> recordedPath = new List<GhostFrame>();
    private float ghostTimer = 0f;
    private int currentIndex = 0;

    private bool replayActive = false;
    private int ghostStartIndex = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LoadGhostRun();
    }

    // Update is called once per frame
    void Update()
    {
       

       // if (!replayActive || recordedPath.Count == 0)
          //  return;

        // Move the ghost along the path
        PlayGhost();
    }

    void PlayGhost()
    {
        ghostTimer += Time.deltaTime;

        float stepInterval = 0.05f;
        int nextIndex = ghostStartIndex + Mathf.FloorToInt(ghostTimer / stepInterval);

        if (nextIndex >= recordedPath.Count)
        {
            replayActive = false;
            return;
        }

        Vector3 from = (nextIndex == ghostStartIndex) ? transform.position : recordedPath[nextIndex - 1].ToVector3();
        Vector3 to = recordedPath[nextIndex].ToVector3();

        float t = (ghostTimer % stepInterval) / stepInterval;
        transform.position = Vector3.Lerp(from, to, t);

        Quaternion fromRot = (nextIndex == ghostStartIndex) ? transform.rotation : recordedPath[nextIndex - 1].ToQuaternion();
        Quaternion toRot = recordedPath[nextIndex].ToQuaternion();

        transform.rotation = Quaternion.Lerp(fromRot, toRot, t);
    }



    public void LoadGhostRun()
    {
        string directory = Application.persistentDataPath;
        string[] files = Directory.GetFiles(directory, "ghost_run_*.json");

        if (files.Length == 0)
        {
            Debug.LogWarning("No ghost run files found!");
            return;
        }
        else
        {
            Debug.Log("GOT THE FILE!");
        }

        // Pick a random file
        string randomFile = files[Random.Range(0, files.Length)];



        // Read and deserialize
        string json = File.ReadAllText(randomFile);

        // Deserialize to GhostRunData
        GhostRunData ghostRun = JsonUtility.FromJson<GhostRunData>(json);

        if (ghostRun == null || ghostRun.frames == null || ghostRun.frames.Count == 0)
        {
            Debug.LogWarning("Failed to parse ghost data.");
            return;
        }

        // Assign to your internal list
        recordedPath = ghostRun.frames;

       

        Debug.Log($"Loaded {recordedPath.Count} ghost frames.");

        Vector3 startPos = transform.position;
        float minDistance = 5f; // You can tweak this value
        ghostStartIndex = 0;

        for (int i = 0; i < recordedPath.Count; i++)
        {
            float dist = Vector3.Distance(startPos, recordedPath[i].ToVector3());
            if (dist >= minDistance)
            {
                ghostStartIndex = i;
                break;
            }
        }

        ghostTimer = 0f;
        currentIndex = 0;
        replayActive = true;

        Debug.Log($"Ghost will start from frame {ghostStartIndex}.");




    }
}
