using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Ce script gère tous les paramètre du joueur, ses interactions du joueurs au sein d'un niveau
/// </summary>
public class PlayerController : MonoBehaviour
{
    // Variables avec les différents inputs utiliser
    private float horizontalInput, verticalInput;
    private bool isInteractPressed;
    private bool sprintInput;
    private bool spaceInput;

    // Objet permettant de tenir des objets
    private GameObject hold;

    [Header("Player's Movement")]
    // Vitesse minimal du joueur
    public float minSpeed = 2.5f;

    // Vitesse maximal du joueur
    public float maxSpeed = 3.5f;

    // Variable static qui contiendra l'objet que le joueur veut prendre
    public static GameObject foodObject;

    // Particules de course du personnage 
    public ParticleSystem dustParticles;

    [Header("Stress Parameters")]
    // Valeur courante du niveau de stress 
    public float currentStressLevel = 0f;

    //Valeur maximal du nievau de stress
    public float maxStressLevel = 100f;

    // Intervalle permettant de variée la valeur du facteur de stress
    [Range(0.01f, 0.05f)]
    public float stressFactor;

    // Variable booleen qui vérifier si le personnage est trop stresser 
    public bool isTooStress = false;

    // Particule de fumée quand le personnage est trop stresser
    public ParticleSystem burnoutParticles;

    [Header("Others Parameters")]
    // Booleen qui verifier si le joueur tient un plat
    public bool isPlatePicked = false;

    // Booleen qui verifier si le bouton de du climateur est pressé
    public bool isAcPressed = false;

    // Particule de victoire quand un plat sera envoyé
    public ParticleSystem WinParticles;

    [Header("Sounds effects")]
    private AudioSource playerAudio;
    public AudioClip burnoutSound;
    public AudioClip dropSound;
    public AudioClip switchSound;
    public AudioClip sendSound;


    private Animator playerAnimator;

    // Start is called before the first frame update
    void Start()
    {
        hold = GameObject.Find("Object Holder");

        playerAnimator = GetComponent<Animator>();
        playerAudio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Setup des inputs liée à la manette
        isInteractPressed = Input.GetButtonDown("Fire1");
        sprintInput = Input.GetButton("Sprint");
        spaceInput = Input.GetButtonDown("Fire3");

        if (!LevelManager.instance.isGameInit && !LevelManager.instance.isGameOver)
        {
            // Si le niveau n'est pas initialisée et que le jeu n'est pas terminé...
            Move();
            DecreaseStreeLevel();
        }

        if (Input.GetKey(KeyCode.Escape))
        {
            // Le raccourci Espace va...
            
            // mettre le échelle de temps à 0 (Toutes évéments en dehors de la fonction Update() ne s'éxecute pas)
            Time.timeScale = 0;

            // Activer l'écran de pause
            UIManager.instance.pauseScreen.gameObject.SetActive(true);

            // le jeu en pause
            LevelManager.instance.isGamePaused = true;

            GameObject servText = GameObject.Find("service");
            servText.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Cette méthode  permet au joueur de se déplacer
    /// </summary>
    public void Move()
    {
        if (!isTooStress)
        {
            // Si le personnage n'est pas trop stresser

            horizontalInput = Input.GetAxis("Horizontal");
            verticalInput = Input.GetAxis("Vertical");

            if (Input.GetKeyDown(KeyCode.LeftShift) || sprintInput)
            {
                // Le raccourci Shift Gauche ou le raccourci Bumper Gauche lance les particules de course
                dustParticles.Play();

            }

            if (Input.GetKey(KeyCode.LeftShift) && isPlatePicked || sprintInput && isPlatePicked)
            {
                //Si on tient un plat

                // Le raccourci Shift Gauche ou le raccourci Bumper Gauche set la vitesse minimal du joueur à la vitesse maximal...
                minSpeed = maxSpeed;

                // et appelle la fonction qui augmente le niveau de stress 
                IncreaseStressLevel(stressFactor);
            }
            else if (Input.GetKey(KeyCode.LeftShift) || sprintInput)
            {
                // Le raccourci Shift Gauche ou le raccourci Bumper Gauche
                minSpeed = maxSpeed;
            }
            else
            {
                // Si aucun raccourci n'est activé

                // On stop les particules 
                dustParticles.Stop();

                // et la valeur de la vitesse revient par défaut
                minSpeed = 2.5f;
            }

            Vector3 movement = new Vector3(horizontalInput, 0.0f, verticalInput);

            if (movement != Vector3.zero)
            {
                // Si le vecteur de mouvement n'est pas égale à 0...

                playerAnimator.SetFloat("Speed_f", 1);

                transform.rotation = Quaternion.LookRotation(movement);

                this.transform.rotation = Quaternion.Slerp(this.transform.rotation, transform.rotation, 0.15F);

                transform.Translate(movement * minSpeed * Time.deltaTime, Space.World);
            }
            else
            {
                playerAnimator.SetFloat("Speed_f", 0);
            }
        }
    }

    /// <summary>
    /// Cette méthode augmente le stress du personnage quand il court et qu'il tient un plat
    /// </summary>
    public void IncreaseStressLevel(float stressValue)
    {
        currentStressLevel += stressValue;

        if (currentStressLevel >= maxStressLevel)
        {
            // // Dès que le niveau de stress du personnage atteind le niveau de stress maximale 

            // On lance les particules de fumée
            burnoutParticles.Play();

            // Le personnage devient trop stressé
            isTooStress = true;

            // Le nievau de stress est égale au niveau de stress maximale
            currentStressLevel = maxStressLevel;

            // On active les audios 
            playerAudio.PlayOneShot(burnoutSound, 1.0f);
            playerAudio.PlayOneShot(dropSound, 1.0f);

            // On peut supprimer le plat que le personnage tient
            Destroy(foodObject);


            playerAnimator.SetFloat("Speed_f", 0);
        }
    }

    /// <summary>
    /// Cette méthode diminue le stress du personnage quand il ne tient pas un plat
    /// </summary>
    public void DecreaseStreeLevel()
    {
        if (!isPlatePicked)
        {
            // Si le joueur ne tient pas de plat

            // Le niveau de stress diminue
            currentStressLevel -= stressFactor;

            if (currentStressLevel <= 0)
            {
                // Dès que le niveau de stress est égale ou égale à 0 
                currentStressLevel = 0;
            }
        }

        if (Input.GetKeyDown(KeyCode.Space) && isTooStress || spaceInput && isTooStress)
        {
            // Le raccourci Espace ou le raccourci Rond / B / A  diminue le niveau de stress
            currentStressLevel -= .5f;
                        
            if (currentStressLevel == 0)
            {
                //Si le niveau de stress est égale a à

                //Le personnage n'est plus trop stresser
                isTooStress = false;

                //Le personnage ne tient plus de plat (car il le fait tombé)
                isPlatePicked = false;

                // On stop les particules de fumée
                burnoutParticles.Stop();
            }
        }
    }

    /// <summary>
    /// Cette méthode permet de détecter les objet avec lesquel on collision
    /// </summary>
    private void OnTriggerStay(Collider other)
    {           
        if (other.CompareTag("isSend"))
        {   
            // Dès que l'autre object a pour tag isSend

            if (isInteractPressed && isPlatePicked || Input.GetKeyDown(KeyCode.E) && isPlatePicked)
            {
                // Si le personnage tient un plat...

                // Le raccourci E ou le raccourci Carré / B / A  permet de poser / envoyer le plat que l'on tient

                playerAnimator.SetBool("Put", true);

                playerAnimator.SetBool("Pick", false);
                string currentImageName = UIManager.instance.foodSprites[UIManager.instance.randomIndex].name + "(Clone)";

                if (foodObject.name == currentImageName)
                {
                    // Si le plat envoyé à le même nom que l'image afficher...

                    //On change l'image
                    UIManager.instance.ChangeImage();

                    //On incrémente le score du 3;
                    UIManager.instance.UpdateScore(3);
                }
                else
                {
                    //Dans le cas inverse, on incrémente le score de 1
                    UIManager.instance.UpdateScore(1);
                }

                // On joue le son d'envoye ainsi que les particules de réussite
                playerAudio.PlayOneShot(sendSound, 1.0f);
                WinParticles.Play();

                //On détruit le plat que le joueur tient
                Destroy(foodObject);

                //Le joueur ne tient plus de plats
                isPlatePicked = false;
            }
        }

        if (isInteractPressed && !isPlatePicked || Input.GetKeyDown(KeyCode.E) && !isPlatePicked)
        {
            // Si le personnage ne tient pas un plat...

            /* Le raccourci E ou le raccourci Carré / X / Y  
            // 1) d'appuyer sur le bouton qui active le climatiseur 
            // 2/ de prend un plat
            */

            if (other.CompareTag("AC Button") && !LevelManager.instance.isAcOn)
            {
                // Dès que l'autre object a pour tag Ac Button

                //On lance la fonction qui active le climatiseur depuis le script LevelManager
                LevelManager.instance.AirConditionOn();

                // On lance l'audio qui active l'interrupteur
                playerAudio.PlayOneShot(switchSound,1f);
            }

            if (other.CompareTag("Food"))
            {
                // Dès que l'autre object a pour tag Food

                playerAnimator.SetBool("Pick", true);

                // L'autre object prend la valeur de foodObject
                foodObject = other.gameObject;

                // On déasactive le script de l'objet pris pour qu'il ne bouge plus 
                foodObject.GetComponent<MoveinDirection>().enabled = false;

                //On set le transform de l'objet à l'objet qui permet de tenir un plat 
                foodObject.transform.SetParent(hold.transform);

                //Le plat prend la position de l'objet qui permet de tenir un plat
                foodObject.gameObject.transform.position = hold.transform.position;


                // Le personnage tient un plat
                isPlatePicked = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("isSend"))
        {
            playerAnimator.SetBool("Put", false);
        }

        if (other.CompareTag("Food"))
        {
            playerAnimator.SetBool("Pick", false);
        }
    }
}

