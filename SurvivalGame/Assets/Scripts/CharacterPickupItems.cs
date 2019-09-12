﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterPickupItems : MonoBehaviour
{
    [SerializeField] private GameObject head;
    [SerializeField] private GameObject palm;
    [SerializeField] private Camera _camera;

    private bool showPickup = false;
    private bool pickedUp = false;
    private string pickupName;

    [SerializeField] private float showPickupRayLength;
    [SerializeField] private float throwForce;

    private GameObject hitGameObject;
    private GameObject pickedObject;
    private Vector3 scale;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 playerHeadPosition = head.transform.position;
        Vector3 playerForwardDirection = _camera.transform.forward;

        PickupRaycast(playerHeadPosition, playerForwardDirection);

        if (showPickup || pickedUp)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (!pickedUp)
                {
                    pickedUp = true;
                    showPickup = true;
                    pickedObject = hitGameObject;
                    scale = pickedObject.transform.localScale;
                    pickedObject.GetComponent<Rigidbody>().isKinematic = true;
                    pickedObject.transform.parent = _camera.transform;
                }
                else
                {
                    pickedUp = false;
                    showPickup = false;

                    pickedObject.GetComponent<Rigidbody>().isKinematic = false;
                    pickedObject.transform.parent = null;
                    
                    pickedObject.transform.localScale = scale;
                    
                    pickedObject.GetComponent<Rigidbody>().AddForce(_camera.transform.forward * throwForce);
                }
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                if (pickedUp)
                {
                    pickedUp = false;
                    showPickup = false;

                    pickedObject.GetComponent<Rigidbody>().isKinematic = false;
                    pickedObject.transform.parent = null;
                    
                    pickedObject.transform.localScale = scale;
                }
            }
        }

        if (pickedUp)
        {
            if (pickedObject.GetComponent<CheckPickedCollision>().collided)
            {
                pickedUp = false;
                showPickup = false;

                pickedObject.GetComponent<Rigidbody>().isKinematic = false;
                pickedObject.transform.parent = null;   
            }

            pickedObject.transform.localScale = scale;
        }
    }
    
    private void OnGUI()
    {
        var interactionStyle = new GUIStyle();
        interactionStyle.fontSize = 40;
        interactionStyle.normal.textColor = Color.white;
        
        if (showPickup)
        {
            GUI.Label(new Rect(Screen.width/2, Screen.height*7/8, 50, 50),
                "E",
                interactionStyle);
        }
    }

    void PickupRaycast(Vector3 playerHeadPosition, Vector3 playerForwardDirection)
    {
        playerHeadPosition = head.transform.position;
        playerForwardDirection = _camera.transform.forward;
       
        Ray pickupRay = new Ray(playerHeadPosition, playerForwardDirection);
        RaycastHit rayPickupHit;

        bool hitFound = Physics.Raycast(pickupRay, out rayPickupHit, showPickupRayLength);
        if (hitFound)
        {
            hitGameObject = rayPickupHit.transform.gameObject;

            if (hitGameObject.CompareTag("Material"))
            {
                showPickup = true;
                pickupName = hitGameObject.name;
                Debug.Log(pickupName);
            }
            else
            {
                showPickup = false;
            }
        }
    }
}
