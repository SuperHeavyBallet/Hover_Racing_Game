using UnityEngine;
using TMPro;

public class FPSCounter : MonoBehaviour
{
    
    public TextMeshProUGUI fpsText;

    private float timer;
    private int frames;
    private float refreshRate = 0.5f; // Update FPS display every 0.5 seconds

    void Update()
    {
        timer += Time.unscaledDeltaTime;
        frames++;

        if (timer >= refreshRate)
        {
            float fps = frames / timer;
            fpsText.text = $"FPS: {Mathf.RoundToInt(fps)}";

            timer = 0f;
            frames = 0;
        }
    }
}
