using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveinDirection : MonoBehaviour
{
    public float destroyAtZPosition;
    public float destroyAtXPosition;

    private PlayerController playerControllerScript;

    // Start is called before the first frame update
    void Start()
    {
        playerControllerScript = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!LevelManager.instance.isGameInit && !LevelManager.instance.isGameOver)
        {
            DestroyPlates();
            transform.Translate(Vector3.forward * LevelManager.instance.conveyorBeltSpeed * Time.deltaTime);
        }
    }

    public void DestroyPlates()
    {
        if (this.transform.position.z >= destroyAtZPosition)
        {
            Destroy(this.gameObject);
        }
    }
}
