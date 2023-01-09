using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ErrorSceneManager : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        //go to main menu if press escape
        if(Input.GetKeyDown(KeyCode.Escape)) goToMainMenu();
    }
    //load main menu scene
    public void goToMainMenu() => SceneManager.LoadScene(0);
}
