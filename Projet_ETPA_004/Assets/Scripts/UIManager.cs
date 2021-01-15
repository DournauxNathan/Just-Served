using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    private PlayerController playerControllerScript;

    [Header("UI Manager - Start Level")]
    public GameObject goalsImage;
    public GameObject readyImage;
    public GameObject goImage;

    [Header("UI Manager - Level")]
    public TextMeshProUGUI scoreText;
    public int score;

    public TextMeshProUGUI timerText;

    public Image[] stressIndicator;

    public TextMeshProUGUI temperatureText;
    public Image temperatureBar;

    public List<Sprite> foodSprites;
    public Image imageHolder;
    public int randomIndex;

    [Header("UI Manager - Level Paused")]
    public GameObject pauseScreen;

    [Header("UI Manager - Level Clear")]
    [SerializeField]
    public float pourcentage;
    private float endTimer = 5;
    public GameObject finishScreen;
    public GameObject finishImage;
    public TextMeshProUGUI levelStatusText;
    public TextMeshProUGUI scoreTextEnd;
    public GameObject[] stars;

    public Text goal1;
    public Text goal2;
    public Text goal3;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        playerControllerScript = GameObject.Find("Player").GetComponent<PlayerController>();

        goalsImage.gameObject.SetActive(false);
        readyImage.gameObject.SetActive(false);
        goImage.gameObject.SetActive(false);
        finishImage.gameObject.SetActive(false);
        finishScreen.gameObject.SetActive(false);
        pauseScreen.gameObject.SetActive(false);
        imageHolder.gameObject.SetActive(false);
        
        foreach (GameObject objet in stars)
        {
            objet.SetActive(false);
        }

        score = 0;

        ChangeImage();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (LevelManager.instance.isGameInit)
        {
            LevelManager.instance.startLevelTimer -= Time.deltaTime;

            goalsImage.gameObject.SetActive(true);

            if (LevelManager.instance.startLevelTimer <= 3)
            {
                goalsImage.gameObject.SetActive(false);
                readyImage.gameObject.SetActive(true);
            }

            if (LevelManager.instance.startLevelTimer <= 1)
            {
                readyImage.gameObject.SetActive(false);
                goImage.gameObject.SetActive(true);
            }
        }

        if (LevelManager.instance.startLevelTimer <= 0)
        {
            goImage.gameObject.SetActive(false);
            LevelManager.instance.startLevelTimer = 0;

            LevelManager.instance.isGameInit = false;
        }

        UpdateTemperature();
        UpdateStressIndicator();
        UpdateTimer();
        UpdateLevelStatue();

        goal1.text = (LevelManager.instance.scoreGoals * 0.2f).ToString(); 
        goal2.text = (LevelManager.instance.scoreGoals * .5f + .5f).ToString();
        goal3.text = LevelManager.instance.scoreGoals.ToString();
    }   

    public void ChangeImage()
    {
        randomIndex = Random.Range(0, foodSprites.Count);
        imageHolder.GetComponent<Image>().sprite = foodSprites[randomIndex];
    }

    public void UpdateTimer()
    {
        LevelManager.instance.timer -= Time.deltaTime;

        if (LevelManager.instance.timer <= 0 && !LevelManager.instance.isGameInit)
        {
            LevelManager.instance.timer = 0;
            timerText.text = "00:00";
            LevelManager.instance.isGameOver = true;
        }
        else
        {
            int minutes = Mathf.FloorToInt(LevelManager.instance.timer / 60F);
            int seconds = Mathf.FloorToInt(LevelManager.instance.timer % 60F);

            timerText.text = minutes.ToString("00") + ":" + seconds.ToString("00");
        }
    }   

    public void UpdateTemperature()
    {
        temperatureText.text = LevelManager.instance.temperature.ToString("0") + "°C";
        temperatureBar.fillAmount = LevelManager.instance.temperature / LevelManager.instance.maxTemperature;

    }

    public void UpdateStressIndicator()
    {
        if (playerControllerScript.currentStressLevel < (playerControllerScript.maxStressLevel * .25f))
        {
            stressIndicator[0].gameObject.SetActive(true);
            stressIndicator[1].gameObject.SetActive(false);
            stressIndicator[2].gameObject.SetActive(false);
        }
        else if (playerControllerScript.currentStressLevel >= (playerControllerScript.maxStressLevel * .25f) && playerControllerScript.currentStressLevel < (playerControllerScript.maxStressLevel * .75f))
        {
            stressIndicator[0].gameObject.SetActive(false);
            stressIndicator[1].gameObject.SetActive(true);
            stressIndicator[2].gameObject.SetActive(false);
        }
        else if(playerControllerScript.currentStressLevel >= (playerControllerScript.maxStressLevel * .75f))
        {
            stressIndicator[0].gameObject.SetActive(false);
            stressIndicator[1].gameObject.SetActive(false);
            stressIndicator[2].gameObject.SetActive(true);
        }

        if (playerControllerScript.isTooStress)
        {
            stressIndicator[2].GetComponent<Image>().color = new Color32(162, 48, 72, 100);
        }

        if (!playerControllerScript.isTooStress)
        {
            stressIndicator[0].GetComponent<Image>().color = new Color32(0, 255, 0, 100);
        }
    }

    public void UpdateScore(int _score)
    {
        score += _score;

        scoreText.text = "" + score;

        scoreTextEnd.text = "" + score;
    }

    public void UpdateLevelStatue()
    {
        levelStatusText.text = "Level " + LevelManager.instance.levelCount + " Cleared";
    }

    public void LevelClear()
    {
        finishImage.gameObject.SetActive(true);

        endTimer -= Time.deltaTime;

        if (endTimer <= 0)
        {
            endTimer = 0;

            bool activeScreen = true;

            if (activeScreen)
            {
                finishImage.gameObject.SetActive(false);
                finishScreen.gameObject.SetActive(true);
                activeScreen = false;
            }

            pourcentage = float.Parse(score.ToString()) / float.Parse(LevelManager.instance.scoreGoals.ToString()) * 100f;

            if (pourcentage >= 33 && pourcentage < 66)
            {
                stars[0].SetActive(true);
            }
            else if (pourcentage >= 66f && pourcentage < 70)
            {
                stars[0].SetActive(true);
                stars[1].SetActive(true);
            }
            else if (pourcentage > 70)
            {
                stars[0].SetActive(true);
                stars[1].SetActive(true);
                stars[2].SetActive(true);
            }
        }
    }
}
