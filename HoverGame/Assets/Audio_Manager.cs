using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

public class Audio_Manager : MonoBehaviour
{
    public Audio_Manager Instance;
    public AudioMixer AudioMixer;

    public AudioSource musicSource;
    public AudioSource sfxSource;
    AudioSource playerBoostSource;
    AudioSource playerEngineSourceA;
    AudioSource playerEngineSourceB;
    AudioSource engineCurrentSource;
    AudioSource engineNextSource;
    Coroutine fadeRoutine;


    AudioSource playerSwitchesSource;

    public float baseEnginePitch;

    AudioSourceRouter playerAudioSourceRouter;

    private void Awake()
    {


        //AudioMixer.SetFloat("EnginePitch", baseEnginePitch);

        playerAudioSourceRouter = GameObject.Find("PlayerAudioSources").GetComponent<AudioSourceRouter>();
        if (playerAudioSourceRouter != null)
        {
            playerBoostSource = playerAudioSourceRouter.GetPlayerBoostAudioSource();
            playerEngineSourceA = playerAudioSourceRouter.GetPlayerEngineAudioSourceA();
            playerEngineSourceB = playerAudioSourceRouter.GetPlayerEngineAudioSourceB();
        }


    }

    private void Start()
    {
        

        baseEnginePitch = playerEngineSourceA.pitch;
        engineCurrentSource = playerEngineSourceA;
        engineNextSource = playerEngineSourceB;
        engineCurrentSource.loop = true;
        engineNextSource.loop = true;
    }




    public void CrossfadeTo(AudioClip newClip, float fadeDuration = 1f)
    {
        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        fadeRoutine = StartCoroutine(FadeBetweenSources(playerEngineSourceB.clip, fadeDuration));
    }

    private IEnumerator FadeBetweenSources(AudioClip newClip, float duration)
    {
        engineNextSource.clip = newClip;
        engineNextSource.volume = 0f;
        engineNextSource.Play();

        float time = 0f;
        float startVolume = engineCurrentSource.volume;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            engineCurrentSource.volume = Mathf.Lerp(startVolume, 0f, t);
            engineNextSource.volume = Mathf.Lerp(0f, startVolume, t);
            yield return null;
        }

        engineCurrentSource.Stop();
        engineCurrentSource.volume = startVolume;

        // Swap sources
        var temp = engineCurrentSource;
        engineCurrentSource = engineNextSource;
        engineNextSource = temp;
        fadeRoutine = null;
    }

    public void UpdateEnginePitch(float forwardSpeed)
    {
        float newPitch = Mathf.Clamp(forwardSpeed * 0.01f + 1f, 0.8f, 2f);
        playerEngineSourceA.pitch = newPitch;
        playerEngineSourceB.pitch = newPitch;






    }
    public void ResetEnginePitch()
    {
        playerEngineSourceA.pitch = baseEnginePitch;
        playerEngineSourceB.pitch = baseEnginePitch;
    }

    public void PlayEngineIdleSound(AudioClip clip)
    {
        //CrossfadeTo(clip, 5.0f); // fade over 1 second

        playerEngineSourceA.Play();
    }

    public void StopEngineIdleSound()
    {
        playerEngineSourceA.Stop();
    }

    public void PlayBoostSound()
    {
        playerBoostSource.Play();
    }

    public void StopBoostSound()
    {
        playerBoostSource.Stop();
    }

    public void PlaySwitchesSound_OneShot(AudioClip clip)
    {
        playerSwitchesSource.PlayOneShot(clip);
    }

    public void PlayPlayerSound_OneShot(AudioClip clip)
    {
        playerBoostSource.PlayOneShot(clip);
    }

    public void PlayPlayerSound(AudioClip clip, bool loop = false)
    {
        playerBoostSource.clip = clip;
        playerBoostSource.loop = loop;
        playerBoostSource.Play();
    }

    public void StopPlayerSound()
    {
        playerBoostSource.Stop();
    }



    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }

    public void StopSFX()
    {
        sfxSource.Stop();
    }

    public void SetMusicVolume(float volume)
    {
        AudioMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);
    }
}
