using TMPro;
using UnityEngine;

public class TrackTimer : MonoBehaviour
{
    public TextMeshProUGUI scoreText;

    private float timer = 0f;
    private bool timerActive = false;

    public TextMeshProUGUI highScoreText;
    public float lapScore;
    private float bestLapTime = 999999999;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (timerActive)
        {
            timer += Time.deltaTime;
            UpdateScoreDisplay();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("TimeStart"))
        {

            StartTimer();       // then start again for a new run
        }
        else if(other.gameObject.CompareTag("TimeStop"))
        {
            StopTimer();   // second entry = stop + check
        }
    }

    private void StartTimer()
    {
        
       
        timer = 0f;
        timerActive = true;
        UpdateScoreDisplay();
    }

    private void StopTimer()
    {
        lapScore = timer;
        timerActive = false;
        UpdateHighScore(lapScore); // Now we catch the actual completed lap time
    }

    private void UpdateScoreDisplay()
    {
        int minutes = Mathf.FloorToInt(timer / 60f);
        int seconds = Mathf.FloorToInt(timer % 60f);
        int milliseconds = Mathf.FloorToInt((timer % 1f) * 100); // hundredths of a second

        scoreText.text = string.Format("{0:00}:{1:00}.{2:00}", minutes, seconds, milliseconds);
    }

    void UpdateHighScore(float endTime)
    {
        Debug.Log("Timer");

        float newHighScore = endTime;

     
            if (highScoreText != null)
            {

                int minutes = Mathf.FloorToInt(newHighScore / 60f);
                int seconds = Mathf.FloorToInt(newHighScore % 60f);
                int milliseconds = Mathf.FloorToInt((newHighScore % 1f) * 100);

                highScoreText.text = string.Format("{0:00}:{1:00}.{2:00}", minutes, seconds, milliseconds);
            }
        

      

    }
}
