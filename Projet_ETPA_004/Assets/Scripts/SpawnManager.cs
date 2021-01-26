using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Ce script gère l'apparition des objets dans un niveau
/// </summary>
public class SpawnManager : MonoBehaviour
{
    // Liste d'objet qui contient les prefabs des plats
    public List<GameObject> foodPrefabs;

    // Start is called before the first frame update
    void Start()
    {
        // On démarre la coroutine
        StartCoroutine(SpawnPlates());
    }

    private IEnumerator SpawnPlates()
    {
        while (true)
        {
            yield return new WaitForSeconds(5);

            if (!LevelManager.instance.isGameInit && !LevelManager.instance.isGameOver)
            {
                // Si le jeu est instantier et que le jeu n'est pas terminé...

                // Variable integer avec une valeur random allant de 0 à la longueur de la liste d'objet
                int randomIndex = UnityEngine.Random.Range(0, foodPrefabs.Count);

                //On instantie un prefabs plat depuis la position de l'objet qui contient ce script
                GameObject foodSpawning = Instantiate(foodPrefabs[randomIndex], this.transform.position, foodPrefabs[randomIndex].transform.rotation);
            }
        }        
    }
}
