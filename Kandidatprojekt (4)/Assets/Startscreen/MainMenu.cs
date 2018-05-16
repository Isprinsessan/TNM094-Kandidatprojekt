using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {

    public GameObject loadingScreen;
    public Slider loadingBar;

    private AsyncOperation operation;


    public void LoadLobby(int sceneIndex)
    {
        loadingScreen.SetActive(true);
        StartCoroutine(LoadAsync(sceneIndex));
    }
    public void CloseGame()
    {
        Application.Quit();
    }

    IEnumerator LoadAsync(int sceneIndex)
    {
        operation = SceneManager.LoadSceneAsync(sceneIndex);        
        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            loadingBar.value = progress;
            Debug.Log("Det händer saker: " + progress);
            yield return null;
        }
    }
}
