﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

/// <summary>
/// Ce script gère tout les évéments des différents menus 
/// Transisition, chargements des scènes
/// </summary>
public class ButtonsScript : MonoBehaviour
{
    public GameObject settingsMenu;
    public GameObject servText;

    /*Functions in the Main Menu*/
    public void PressToPlay()
    {
        GameManager.instance.LoadLevel();
    }
    public void Settings()
    {
        settingsMenu.gameObject.SetActive(true);
    }

    public void Return()
    {
        settingsMenu.gameObject.SetActive(false);
    }


    public void QuitGame()
    {
        Application.Quit();
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        UIManager.instance.pauseScreen.gameObject.SetActive(false);
        LevelManager.instance.isGamePaused = false;

        servText.gameObject.SetActive(true);
    }

    public void QuitLevel()
    {

        Time.timeScale = 1;
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
