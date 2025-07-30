using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MenuSceneLoader : MonoBehaviour
{

    MenuConstructorController menuConstructorController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {
        menuConstructorController = GetComponent<MenuConstructorController>();
    }
    public void LoadScene(string sceneName)
    {
        menuConstructorController.SetShipLoadout();
        StartCoroutine(LoadSceneDelayed(sceneName));
    }

    private IEnumerator LoadSceneDelayed(string sceneName)
    {
        yield return 0.1f; // one frame delay
        SceneManager.LoadScene(sceneName);
    }
}
