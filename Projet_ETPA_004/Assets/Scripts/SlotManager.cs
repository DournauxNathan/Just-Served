﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotManager : MonoBehaviour
{
    private PlayerController playerControllerScript;

    // Start is called before the first frame update
    void Start()
    {
        playerControllerScript = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (Input.GetKeyDown(KeyCode.E) && playerControllerScript.isPlatePicked || Input.GetButtonDown("Fire1") && playerControllerScript.isPlatePicked)
        {
            if (other.CompareTag("Player") && this.transform.childCount == 0)
            {
                PlayerController.foodObject.gameObject.transform.SetParent(this.transform);
                PlayerController.foodObject.gameObject.transform.position = this.transform.position;

                playerControllerScript.isPlatePicked = false;
            }
        }        
    }
}
