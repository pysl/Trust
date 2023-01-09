using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    private LevelController lc;

    [SerializeField] private Dropdown resolutionDropdown; 
    [SerializeField] private Toggle fullscreenToggle;
    // Start is called before the first frame update
    void Start()
    {
        lc = GameObject.Find("LevelController").GetComponent<LevelController>();
        fullscreenToggle.isOn = Screen.fullScreen;
    }

    public void SetResolution() {
        /*
        supported resolutions are 16:9 aspect ratios
            1920x1080
            1600x900
            1280x720
            1024x576
        */
        switch (resolutionDropdown.value)
        {
            case 0:
                Screen.SetResolution(1920, 1080, true);
                Debug.Log("Set resolution to 1920x1080");
                break;
            case 1:
                Screen.SetResolution(1600, 900, true);
                Debug.Log("Set resolution to 1600x900");
                break;
            case 2:
                Screen.SetResolution(1280, 720, true);
                Debug.Log("Set resolution to 1280x720");
                break;
            case 3:
                Screen.SetResolution(1024, 576, true);
                Debug.Log("Set resolution to 1024x576");
                break;
        }
    }
    public void SetFullscreen() => Screen.fullScreen = fullscreenToggle.isOn;
}
