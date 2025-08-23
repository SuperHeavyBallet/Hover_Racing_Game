using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class LapCounterController : MonoBehaviour
{
    public event Action CountdownFinished;

    public LevelCatalogue LevelCatalogue;

    public LevelDefinition thisLevel;

    int numberOfLaps;
    int currentLap;


    public bool raceHasStarted = false;

    public GameObject waitUIText;
    public GameObject goUIText;

    public TextMeshProUGUI lapCountDisplay;

    private void Awake()
    {
       numberOfLaps = thisLevel.numberOfLaps;
        currentLap = 0;
        UpdateLapCountDisplay();
        StartCoroutine(Countdown(5));
        goUIText.SetActive(false);
        waitUIText.SetActive(true);
    }

    void UpdateLapCountDisplay()
    {
        lapCountDisplay.text = currentLap + " / " + numberOfLaps;
    }


    public void AddLap()
    {
        currentLap++;
        UpdateLapCountDisplay();
        if(currentLap >= numberOfLaps)
        {
            StopRace();
        }
    }

    void StopRace()
    {
        Time.timeScale = 0;
    }

    IEnumerator Countdown(int delay)
    {
        yield return new WaitForSeconds(delay);
        raceHasStarted = true;
        waitUIText.SetActive(false);
        goUIText.SetActive(true);

        //fire the event
        CountdownFinished?.Invoke();

        StartCoroutine(TurnOffGoText(1));
    }

    IEnumerator TurnOffGoText(int delay)
    {
        yield return new WaitForSeconds(delay);
        goUIText.SetActive(false);
        StopAllCoroutines();
    }


}
