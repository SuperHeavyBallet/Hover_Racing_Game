using UnityEngine;
using UnityEngine.SceneManagement;
public class LoadScene : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
   

    public void LoadLevel(string level)
    {
        SceneManager.LoadScene(level);
    }
}
