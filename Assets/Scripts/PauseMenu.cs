using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private LevelController lc;
    [SerializeField] private InputField playeridViewer;
    public SettingsMenu settingsMenu;
    public bool isPaused = false;


    public void SetPlayerID() => playeridViewer.text = lc.player.playerid.ToString();

    public void disconnect() => SceneManager.LoadScene(0);

    public void resume() { 
        isPaused = false;
        playeridViewer.text = lc.player.playerid.ToString();
        gameObject.SetActive(false);
    }

    public void pause() {
        isPaused = true;
        playeridViewer.text = lc.player.playerid.ToString();
        gameObject.SetActive(true);
    }

    public void settings() {
        settingsMenu.gameObject.SetActive(true);
    }


}
