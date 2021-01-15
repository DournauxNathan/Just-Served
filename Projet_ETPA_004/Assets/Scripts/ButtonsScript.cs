using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonsScript : MonoBehaviour
{

    /*Functions in the Main Menu*/
    public void PressToPlay()
    {
        GameManager.instance.LoadLevel();
    }
    public void Settings()
    {

    }

    public void QuitGame()
    {
        Debug.Log("QUIT");
        Application.Quit();
    }

    /*Function use in Levels*/
    public void PauseGame()
    {
        Time.timeScale = 0;
        UIManager.instance.pauseScreen.gameObject.SetActive(true);
        LevelManager.instance.isGamePaused = true;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        UIManager.instance.pauseScreen.gameObject.SetActive(false);
        LevelManager.instance.isGamePaused = false;
    }

    public void QuitLevel()
    {
        GameManager.instance.UnLoadLevel();
    }
    public void Continue()
    {
        SceneManager.LoadSceneAsync((int)SceneIndexes.TITLE_SCREEN, LoadSceneMode.Additive);
    }
    public void Restart()
    {
        SceneManager.LoadSceneAsync((int)SceneIndexes.LEVEL_1);
    }

}
