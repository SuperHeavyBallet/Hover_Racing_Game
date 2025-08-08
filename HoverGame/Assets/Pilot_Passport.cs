using System.Collections.Generic;
using UnityEngine;

public class Pilot_Passport : MonoBehaviour
{

    public static Pilot_Passport Instance { get; private set; }

    public string pilotName { get; private set; }

    public Sprite pilotAvatar {  get; private set; }
    public Sprite inputPilotAvatar;

    public int pilot_FameLevel;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        SetPilotName("MCDougles");
        SetPilotAvatar(inputPilotAvatar);

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SetPilotName(string newPilotName)
    {
        pilotName = newPilotName;
    }

    void SetPilotAvatar(Sprite newPilotAvatar)
    {
        pilotAvatar = newPilotAvatar;
    }

    public string GetPilotName()
    {
        return pilotName;
    }

    public Sprite GetPilotAvatar()
    {
        return pilotAvatar;
    }

    void SetPilotFameLevel(int level)
    {
        pilot_FameLevel = Mathf.Clamp(level, 0, 100);
    }

    public int GetPilotFameLevel()
    {
        return pilot_FameLevel;
    }
}
