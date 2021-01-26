using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Ce script permet de poser un plat sur la table 
/// </summary>
public class SlotManager : MonoBehaviour
{
    private PlayerController playerControllerScript;

    private Animator playerAnimator;

    // Start is called before the first frame update
    void Start()
    {
        playerControllerScript = GameObject.Find("Player").GetComponent<PlayerController>();
        playerAnimator = GameObject.Find("Player").GetComponent<Animator>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (Input.GetKeyDown(KeyCode.E) && playerControllerScript.isPlatePicked || Input.GetButtonDown("Fire1") && playerControllerScript.isPlatePicked)
        {

            playerAnimator.SetBool("Put", false);

            if (other.CompareTag("Player") && this.transform.childCount == 0)
            {
                PlayerController.foodObject.gameObject.transform.SetParent(this.transform);
                PlayerController.foodObject.gameObject.transform.position = this.transform.position;

                playerControllerScript.isPlatePicked = false;
            }
        }        
    }
}
