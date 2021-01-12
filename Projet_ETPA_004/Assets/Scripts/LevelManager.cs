using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;
    private int levelIndex;
    public int levelCount;
    public int scoreGoals;

    [Header("Timer Parameters")]
    public float setTimer;
    public float timer;
    public float m3;
    public float m2;
    public float m1;

    [Header("Temperature Parameters")]
        /*Variables when temperature is increasing*/
    public bool isPaused = false;
    public float callIncreaseTemperature = 3;
    public float temperatureIncreaseValue = 1;
        /*Variables when temperature is decreasing*/
    public bool isAcOn = false;
    public float timerTemperature;
    public float temperature = 19;
    public float maxTemperature = 40;

    [Header("Speed Parameters")]
    public float timeBetweenSpawn;
    public float conveyorBeltSpeed;
    public float conveyorBeltMaxSpeed;

    [Header("Start Options")]
    public bool isGameInit;
    public float startLevelTimer = 5f;

    [Header("Pause Options")]
    public bool isGamePaused = false;

    [Header("Game Over")]
    public bool isGameOver = false;

    public ParticleSystem smokeParticules;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        levelIndex = SceneManager.GetActiveScene().buildIndex;

        isGameInit = true;

        StartCoroutine(IncreaseTemperature());
        SetGoalsLevels();

        m3 = setTimer * .75f;
        m2 = setTimer * .5f;
        m1 = setTimer * .25f;
    }

    public void SetGoalsLevels()
    {
        if (levelIndex == 2)
        {
            levelCount = 1;
            scoreGoals = 2;
        }

        if (levelIndex == 3)
        {
            levelCount = 1;
            scoreGoals = 3;
        }
    }

    private void FixedUpdate()
    {
        IncreaseDifficulty();

        if (isGameInit)
        {
            StartLevel();
        }

        if (isGameOver)
        {
            UIManager.instance.LevelClear();
        }
    }

    public void StartLevel()
    {
        timer = setTimer;
        timeBetweenSpawn = 5f;
        UIManager.instance.imageHolder.gameObject.SetActive(true);
    
    }

    public void IncreaseDifficulty()
    {
        if (timer < m3 && !isGameInit)
        {
            timeBetweenSpawn = 0.1f;
        }

        if (timer < m2 && !isGameInit)
        {
            temperatureIncreaseValue = 2;
            timeBetweenSpawn = 1;
        }

        if (timer < m1 && !isGameInit)
        {
            temperatureIncreaseValue = 3;
            timeBetweenSpawn = 0.1f;
        }
    }

    public void AirConditionOn()
    {
        StartCoroutine(DecreaseTemperature());
        isPaused = true;
        isAcOn = true;
        timerTemperature = 10f;
    }

    public IEnumerator IncreaseTemperature() 
    {
        while (true)
        {
            yield return new WaitForSeconds(callIncreaseTemperature);

            if (!isPaused && !isGameInit && !LevelManager.instance.isGameOver)
            {
                temperature += temperatureIncreaseValue;

                if (temperature >= maxTemperature)
                {
                    temperature = maxTemperature;
                }
            }

            if ((temperature > (maxTemperature * .75f)) && temperature < maxTemperature)
            {
                conveyorBeltSpeed = conveyorBeltMaxSpeed;
                var main = smokeParticules.main;
                main.startLifetime = (UIManager.instance.temperatureBar.fillAmount - .5f) * 100f;

            }
            if (temperature < (maxTemperature * .7f))
            {
                conveyorBeltSpeed = 0.5f; ;
            }
        }
    }

    public IEnumerator DecreaseTemperature()
    {
        while (true)
        {
            yield return null;

            if (isAcOn)
            {
                timerTemperature -= Time.deltaTime;
                temperature -= Time.deltaTime;
            }

            if (timerTemperature <= 0)
            {
                isPaused = false;
                isAcOn = false;
            }

            if (temperature <= 19f)
            {
                isAcOn = false;

                yield return new WaitForSeconds(10);

                isPaused = false;
            }
        }
    }
}
