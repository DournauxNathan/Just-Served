using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Ce script gère toutes les évéments d'un niveau de jeu.
/// Gestion du temps de jeu, de la température, la vitesse d'apparition 
/// des objets et leurs vitesse de déplacement.
/// </summary>
public class LevelManager : MonoBehaviour
{
    // Le LevelManager est un singleton, on y accède avec "LevelManager.instance"
    public static LevelManager instance;

    private int levelIndex;
    public int levelCount;
    public int scoreGoals;

    // Paramètre liée au temps de jeu 
        [Header("Timer Parameters")]
        //
        public float setTimer;
        //
        public float timer;
        //Variable qui contient les valeurs du temps pour augmenter la diffculté du niveau en fonction du temps
        private float m3, m2, m1;

    [Header("Temperature Parameters")]
        public bool isPaused = false;

        // Temps qui permet de return la coroutine qui augmente la temperature 
        public float callIncreaseTemperature = 3;

        // Valeur de la temperature qui augmente (Augmente au fur du temps)
        public float temperatureIncreaseValue = 1;

        //// L'objet airConditioner correspondant au climatiseur dans le niveau
        private GameObject airConditioner;

        // Composant <Render> pour changer la couleur de l'objet airConditioner
        private Renderer acRend;

        // Variable bouleen qui vérifie si le climatiseur est en route
        public bool isAcOn = false;

        // Durée d'activation du climatiseur
        public float timerTemperature;

        // Variable de la température courante
        public float temperature = 19;

        // Variable de la température maximal
        public float maxTemperature = 40;

    [Header("Speed Parameters")]
        // Temps correspondant au temps de spawn entre chaque objet
        public float timeBetweenSpawn;

        // Vitesse du tapis roulant sur lequel les objets apparaient
        public float conveyorBeltSpeed;

        // Vitesse maximal du tapis roulant
        public float conveyorBeltMaxSpeed;

        [Header("Start Options")]
        // Variable booleen permet initaliser le niveau 
        public bool isGameInit;

        // Compte à rebours durant l'initialisation du niveau
        public float startLevelTimer = 5f;

    [Header("Pause Options")]
        // Variable booleen qui vérifie si le jeu est en pause (Grâce à un input)
        public bool isGamePaused = false;

    [Header("Game Over")]
        //Variable booleen qui vérifie si le jeu est terminé
    public bool isGameOver = false;

    [Header("Particles Section")]
        public ParticleSystem smokeParticules;
        public ParticleSystem smokeParticules2;
        public ParticleSystem smokeParticules3;
        public ParticleSystem smokeParticules4;

    [Header("Audio Section")]
        private AudioSource levelAudioSource;

        // AudioClip quand le niveau est réussi
        public AudioClip levelClear;

        // AudioClip quand le niveau est échoué
        public AudioClip levelFail;

        // AudioClip liée à la température
        public AudioClip alertTemperature;


    // Start is called before the first frame update
    void Start()
    {
        // Instance se réfère à ce script
        instance = this;

        SetGoalsLevels();

        //Lorsque le jeu démarre, l'initialisation du niveau est vrai
        isGameInit = true;

        // Reference de l'objet climatiseur
        airConditioner = GameObject.Find("Air Conditioner");
        // Reference au composant Renderer de l'objet climatiseur
        acRend = airConditioner.GetComponent<Renderer>();

        levelIndex = SceneManager.GetActiveScene().buildIndex;

        // Reference du Composant AudioSource
        levelAudioSource = GetComponent<AudioSource>();

        StartCoroutine(IncreaseTemperature());

        //Set les paliers de difficultés en fonction du temps 
        m3 = setTimer * .75f;
        m2 = setTimer * .5f;
        m1 = setTimer * .25f;
    }

    /// <summary>
    /// Cette méthode définit l'objectif à atteindre en fonction du du niveau
    /// Plus les niveaux augmentent, plus le score à obtenir augmentent (par exemple)
    /// </summary>
    public void SetGoalsLevels()
    {
        if (levelIndex == 2)
        {
            levelCount = 1;
            scoreGoals = 15;
        }

        if (levelIndex == 3)
        {
            levelCount = 2;
            scoreGoals = 26;
        }
    }
   
    private void FixedUpdate()
    {
        IncreaseDifficulty();

        if (isGameInit)
        {
            // Dès lors que le jeu est initialisé 
            smokeParticules.Stop();
            // On appelle la fonction StartLevel
            StartLevel();
        }

        if (isGameOver)
        {
            // Dès lors que le jeu est finie

            // On appelle fonction LevelClear du script UIManager 
            UIManager.instance.LevelClear();

            if (UIManager.instance.pourcentage >= 33)
            {
                // Si le pourcentage du score est inférieur ou égale à 33% alor
               
                // On lance l'audioClip quand le niveau est échoué
                //levelAudioSource.PlayOneShot(levelClear, 1.0f);
            }
            else
            {
                // On lance l'audioClip quand le niveau est réussi
                //levelAudioSource.PlayOneShot(levelFail, 1.0f);
            }
        }

        
        var main = smokeParticules.main;
        main.startLifetime = (UIManager.instance.temperatureBar.fillAmount);
    }

    /// <summary>
    /// Cette méthode lance le niveau une fois que le niveau est initialiser
    /// </summary>
    public void StartLevel()
    {
        timer = setTimer;
        timeBetweenSpawn = 5f;

        // On actives l'écran qui contient les plats à envoyer
        UIManager.instance.imageHolder.gameObject.SetActive(true);
    
    }

    /// <summary>
    /// Cette méthode augmente la difficulté en fonction du temps
    /// </summary>
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

    private IEnumerator startParticules()
    {
        yield return null;
        Debug.Log("ok");
        smokeParticules.Play();
    }

    /// <summary>
    /// Cette méthode permet d'activer le climatiseur 
    /// </summary>
    public void AirConditionOn()
    {
        // Lance la coroutine qui diminue la température
        StartCoroutine(DecreaseTemperature());
        isPaused = true;

        // Le climatiseur est bien actif
        isAcOn = true;

        // On définit l'activation du climatiseur pendant 10s
        timerTemperature = 10f;
    }


    /// <summary>
    /// Cette coroutine augmente la température tout les secondes
    /// </summary>
    public IEnumerator IncreaseTemperature() 
    {
        while (true)
        {
            yield return new WaitForSeconds(callIncreaseTemperature);

            // Si la coroutine n'est pas en pause, que le jeu n'est en cours d'initialisation et que le jeu n'est pas terminé...
            if (!isPaused && !isGameInit && !LevelManager.instance.isGameOver)
            {

                // On augmente le température
                temperature += temperatureIncreaseValue;

                if (temperature >= maxTemperature)
                {
                    // Dès que la température atteind la température maximale 

                    // La température est égale à la température maximal
                    temperature = maxTemperature;
                }
            }

            if ((temperature > (maxTemperature * .75f)) && temperature < maxTemperature)
            {
                // Si la température est entre la température maximale et au 3/4 de la température maximale...

                //On change la couleur de l'objet airConditioner en rouge
                acRend.material.SetColor("_Color", Color.red);

                // La vitesse du tapis roulant devient la vitesse maximal du tapis
                conveyorBeltSpeed = conveyorBeltMaxSpeed;

                smokeParticules.Play();

                // On joue l'audio d'alert
                levelAudioSource.Play();

                // On lance la coroutines des particules
                StartCoroutine(startParticules());
            }

            if (temperature > 32)
            {
                smokeParticules2.Play();
            }
            if (temperature > 35)
            {
                smokeParticules3.Play();
            }
            if (temperature >= 38)
            {
                smokeParticules4.Play();
            }

        }
    }

    /// <summary>
    /// Cette coroutine diminue la température tout les secondes pendant 10s
    /// </summary>
    public IEnumerator DecreaseTemperature()
    {
        while (true)
        {
            yield return null;

            if (isAcOn)
            {
                // Si le climatiseur est actif...

                //On diminue le temps d'activation du climatiseur
                timerTemperature -= Time.deltaTime;

                //La température diminue toutes les secondes
                temperature -= Time.deltaTime;
            }

            if (timerTemperature <= 0)
            {
                // Si le temps d'activation du climatiseur est à 0...

                // On reactive la coroutine IncreaseTemperature
                isPaused = false;

                //On désactive le climatiseur
                isAcOn = false;
            }

            if (temperature < (maxTemperature * .75f))
            {
                // Si la température est inférieur au  3/4 de la température maximale...

                //On change la couleur de l'objet airConditioner en blanc 
                acRend.material.SetColor("_Color", Color.white);

                // On stop l'audio d'alert
                levelAudioSource.Stop();

                smokeParticules.Stop();

                // La vitesse du tapis roulant revient à la vitesse normale
                conveyorBeltSpeed = 0.5f; ;
            }

            if (temperature < 32)
            {
                smokeParticules2.Stop();
            }
            if (temperature < 35)
            {
                smokeParticules3.Stop();
            }
            if (temperature < 38)
            {
                smokeParticules4.Stop();
            }

            if (temperature <= 19f)
            {
                // Si la température atteint 19°C 

                // On désactive le climatiseur
                isAcOn = false;

                // On attend 10s avant de return de la coroutine
                yield return new WaitForSeconds(10);

                // On reactive la coroutine IncreaseTemperature
                isPaused = false;
            }
        }
    }
}
