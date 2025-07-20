using UnityEngine;

public class AudioSourceRouter : MonoBehaviour
{
    public AudioSource playerBoostAudioSource;
    public AudioSource playerEngineAudioSourceA;
    public AudioSource playerEngineAudioSourceB;
    public AudioSource playerSwitchesAudioSource;

    public AudioSource GetPlayerBoostAudioSource()
    {
        return playerBoostAudioSource;
    }

    public AudioSource GetPlayerEngineAudioSourceA()
    {
        return playerEngineAudioSourceA;
    }

    public AudioSource GetPlayerEngineAudioSourceB()
    {
        return playerEngineAudioSourceB;
    }

    public AudioSource PlayerSwitchesAudioSource()
    { 
        return playerSwitchesAudioSource;
    }
}
