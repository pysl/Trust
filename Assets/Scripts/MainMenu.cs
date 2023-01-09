using UnityEngine.UI;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private InputField ip;
    [SerializeField] private InputField port;
    [SerializeField] private InputField playerid;
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private Slider progressBar;

    void Start()
    {
        //load ip and port from PlayerPrefs
        ip.text = PlayerPrefs.GetString("ip");
        if (PlayerPrefs.GetInt("port") != 0) port.text = PlayerPrefs.GetInt("port").ToString();
        if (PlayerPrefs.GetInt("playerid") != 0 && PlayerPrefs.GetInt("playerid") != -1) 
            playerid.text = PlayerPrefs.GetInt("playerid").ToString();
    }

    void Update () {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Debug.Log(loadingScreen.activeSelf);
            if (loadingScreen.activeSelf) {
                loadingScreen.SetActive(false);
                //stop loading scene
                Debug.Log("Stoping loading");
                SceneManager.UnloadSceneAsync(1);
            } else {
                Debug.Log("Quit");
                Application.Quit();
            }
        }
        if (Input.GetKeyDown(KeyCode.Return)) joinGame();
        if (Input.GetKeyDown(KeyCode.F11)) Screen.fullScreen = !Screen.fullScreen;
    }
    public void joinGame()
    {
        //save ip and port in PlayerPrefs
        if (ip.text == "") ip.text = "localhost";
        if (port.text == "") port.text = "3000";
        if (playerid.text == "") PlayerPrefs.SetInt("playerid", -1);
        else PlayerPrefs.SetInt("playerid", int.Parse(playerid.text));
        PlayerPrefs.SetString("ip", ip.text);
        PlayerPrefs.SetInt("port", int.Parse(port.text));
        //load game scene
        StartCoroutine(loadSceneAsyncronously(1));
    }
    IEnumerator loadSceneAsyncronously(int sceneIndex)
    {
        loadingScreen.SetActive(true);
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
        while (!operation.isDone)
        {
            progressBar.value = operation.progress;
            yield return null;
        }
    }

}
