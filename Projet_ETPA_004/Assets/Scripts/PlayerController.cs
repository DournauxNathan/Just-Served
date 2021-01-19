﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private float horizontalInput, verticalInput;
    private bool isInteractPressed;
    private bool sprintInput;
    private bool spaceInput;

    private GameObject hold;

    [Header("Player's Movement")]
    public float minSpeed = 2.5f;
    public float maxSpeed = 3.5f;
    public static GameObject foodObject;
    public ParticleSystem dustParticles;

    [Header("Stress Parameters")]
    public float currentStressLevel = 0f;
    public float maxStressLevel = 100f;
    [Range(0.01f, 0.05f)]
    public float stressFactor;
    public bool isTooStress = false;
    public ParticleSystem burnoutParticles;

    [Header("Others Parameters")]
    public bool isPlatePicked = false;
    public bool isAcPressed = false;
    private GameObject obj;
    public ParticleSystem WinParticles;

    [Header("Sounds effects")]
    private AudioSource playerAudio;
    public AudioClip burnoutSound;
    public AudioClip dropSound;
    public AudioClip switchSound;
    public AudioClip sendSound;


    // Start is called before the first frame update
    void Start()
    {

        hold = GameObject.Find("Object Holder");

        playerAudio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!LevelManager.instance.isGameInit && !LevelManager.instance.isGameOver)
        {
            Move();
            DecreaseStreeLevel();
        }

        isInteractPressed = Input.GetButtonDown("Fire1");
        sprintInput = Input.GetButton("Sprint");
        spaceInput = Input.GetButtonDown("Fire3");

        if (Input.GetKey(KeyCode.Escape))
        {
            Time.timeScale = 0;
            UIManager.instance.pauseScreen.gameObject.SetActive(true);
            LevelManager.instance.isGamePaused = true;
        }
    }

    public void Move()
    {
        if (!isTooStress)
        {
            horizontalInput = Input.GetAxis("Horizontal");
            verticalInput = Input.GetAxis("Vertical");

            if (Input.GetKeyDown(KeyCode.LeftShift) || sprintInput && !isTooStress)
            {
                dustParticles.Play();
            }

            if (sprintInput && isPlatePicked ||Input.GetKey(KeyCode.LeftShift) && isPlatePicked)
            {
                minSpeed = maxSpeed;
                IncreaseStressLevel(stressFactor);
            }
            else if (Input.GetKey(KeyCode.LeftShift) || sprintInput)
            {
                minSpeed = maxSpeed;
            }
            else
            {
                dustParticles.Stop();
                minSpeed = 2.5f;
            }

            Vector3 movement = new Vector3(horizontalInput, 0.0f, verticalInput);

            if (movement != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(movement);

                this.transform.rotation = Quaternion.Slerp(this.transform.rotation, transform.rotation, 0.15F);

                transform.Translate(movement * minSpeed * Time.deltaTime, Space.World);
            }
        }
    }

    public void IncreaseStressLevel(float stressValue)
    {
        currentStressLevel += stressValue;

        if (currentStressLevel >= maxStressLevel)
        {
            burnoutParticles.Play();

            isTooStress = true;

            currentStressLevel = maxStressLevel;

            playerAudio.PlayOneShot(burnoutSound, 1.0f);

            playerAudio.PlayOneShot(dropSound, 1.0f);

            //Destroy the gameobject hold by the player 
            Destroy(foodObject);
        }
    }

    public void DecreaseStreeLevel()
    {
        if (!isPlatePicked)
        {
            currentStressLevel -= stressFactor;

            if (currentStressLevel <= 0)
            {
                currentStressLevel = 0;
            }
        }

        if (spaceInput && isTooStress || Input.GetKeyDown(KeyCode.Space) && isTooStress)
        {
            currentStressLevel -= .5f;
            
            //If the stress is equal to zero then...
            if (currentStressLevel == 0)
            {
                //Set the stress of the character to false
                isTooStress = false;

                //The player has no more dishes
                isPlatePicked = false;

                burnoutParticles.Stop();
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {           
        if (other.CompareTag("isSend"))
        {   
            if (isInteractPressed && isPlatePicked || Input.GetKeyDown(KeyCode.E) && isPlatePicked)
            {

                string currentImageName = UIManager.instance.foodSprites[UIManager.instance.randomIndex].name + "(Clone)";

                if (foodObject.name == currentImageName)
                {
                    UIManager.instance.ChangeImage();
                    UIManager.instance.UpdateScore(3);
                }
                else
                {
                    //Increment the score of point value
                    UIManager.instance.UpdateScore(1);
                }
                playerAudio.PlayOneShot(sendSound, 1.0f);
                WinParticles.Play();

                //Destroy the gameobject hold by the player 
                Destroy(foodObject);

                //The player has no more dishes
                isPlatePicked = false;
            }
        }

        if (isInteractPressed && !isPlatePicked || Input.GetKeyDown(KeyCode.E) && !isPlatePicked)
        {
            if (other.CompareTag("AC Button") && !LevelManager.instance.isAcOn)
            {
                LevelManager.instance.AirConditionOn();
                playerAudio.PlayOneShot(switchSound,1f);
            }

            if (other.CompareTag("Food"))
            {
                foodObject = other.gameObject;

                foodObject.GetComponent<MoveinDirection>().enabled = false;
                foodObject.transform.SetParent(hold.transform);
                foodObject.gameObject.transform.position = hold.transform.position;

                isPlatePicked = true;
            }
        }


    }
}

