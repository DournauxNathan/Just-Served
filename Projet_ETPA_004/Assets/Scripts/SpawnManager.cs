using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public List<GameObject> foodPrefabs;


    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnPlates());
    }

    private IEnumerator SpawnPlates()
    {
        while (true)
        {
            yield return new WaitForSeconds(5);

            if (!LevelManager.instance.isGameInit && !LevelManager.instance.isGameOver)
            {
                int randomIndex = UnityEngine.Random.Range(0, foodPrefabs.Count);
                GameObject foodSpawning = Instantiate(foodPrefabs[randomIndex], this.transform.position, foodPrefabs[randomIndex].transform.rotation);
            }
        }        
    }

}
