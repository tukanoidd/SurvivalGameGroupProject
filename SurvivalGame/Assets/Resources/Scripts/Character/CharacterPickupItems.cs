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
    private bool lookingAtObj = false;
    private string pickupName;
    private float pickupTemp;

    private RaycastHit planeHit;

    [SerializeField] private float rayLength;
    [SerializeField] private float throwForce;

    public GameObject hitGameObject;
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
                    showPickup = false;
                    pickedObject = hitGameObject;
                    pickedObject.GetComponent<Combustable>().hasBeenMovedAfterThrow = false;
                    pickedObject.GetComponent<Combustable>().isGrounded = false;
                    scale = pickedObject.transform.lossyScale;
                    pickedObject.GetComponent<Rigidbody>().isKinematic = true;
                    pickedObject.transform.parent = _camera.transform;
                }
                else
                {
                    pickedObject.GetComponent<Rigidbody>().isKinematic = false;
                    pickedObject.GetComponent<Rigidbody>().AddForce(_camera.transform.forward * throwForce, ForceMode.Impulse);
                    pickedObject.GetComponent<Combustable>().isThrown = true;

                    pickedUp = false;
                    pickedObject.transform.parent = null;

                    pickedObject.transform.localScale = scale;
                }
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                if (pickedUp)
                {
                    pickedUp = false;
                    showPickup = false;

                    pickedObject.GetComponent<Rigidbody>().isKinematic = false;
                    pickedObject.GetComponent<Combustable>().isThrown = true;
                    pickedObject.transform.parent = null;

                    pickedObject.transform.localScale = scale;
                }
            }
        }

        if (pickedObject != null && !pickedUp && !pickedObject.GetComponent<Combustable>().isGrounded && pickedObject.GetComponent<Combustable>().isThrown && !pickedObject.GetComponent<Combustable>().hasBeenMovedAfterThrow)
        {
            try
            {
                var pos = pickedObject.transform.position;
                var size = pickedObject.GetComponent<Renderer>().bounds.size;
                if (pos.y < planeHit.point.y)
                {
                    pickedObject.transform.position = new Vector3(pos.x, planeHit.point.y + Mathf.Max(size.x, size.y, size.z), pos.z);
                    pickedObject.GetComponent<Combustable>().hasBeenMovedAfterThrow = true;
                }
            }
            catch
            {
            }
        }
    }

    private void OnGUI()
    {
        var interactionStyle = new GUIStyle();
        interactionStyle.fontSize = 20;
        interactionStyle.normal.textColor = Color.white;

        if (showPickup)
        {
            GUI.Label(new Rect(Screen.width / 20, Screen.height * 9 / 10, 50, 50),
                "E: Pickup Item",
                interactionStyle);
        }

        if (pickedUp)
        {
            GUI.Label(new Rect(Screen.width / 20, Screen.height * 9 / 10, 50, 50),
                "E: Throw Item\nF: Let Go Of Item",
                interactionStyle);
        }

        if (lookingAtObj)
        {
            GUI.Label(new Rect(Screen.width / 50, Screen.height * 8 / 10, 50, 50),
                pickupName + " - (Temp: " + pickupTemp + ")",
                interactionStyle);
            
            GUI.Label(new Rect(Screen.width * 15 / 20, Screen.height * 9 / 10, 300, 50), "MRB: Show Information About Object");
        }
    }

    void PickupRaycast(Vector3 playerHeadPosition, Vector3 playerForwardDirection)
    {
        playerHeadPosition = head.transform.position;
        playerForwardDirection = _camera.transform.forward;

        Ray pickupRay = new Ray(playerHeadPosition, playerForwardDirection);
        RaycastHit rayPickupHit;
        
        float distToObj = 0;

        if (Physics.Raycast(pickupRay, out rayPickupHit, rayLength))
        {
            if (rayPickupHit.transform.gameObject != null)
            {
                hitGameObject = rayPickupHit.transform.gameObject;

                distToObj = rayPickupHit.distance;

                if (hitGameObject.GetComponent<Combustable>() != null)
                {
                    lookingAtObj = true;

                    if (!pickedUp)
                    {
                        showPickup = true;
                    }

                    pickupName = hitGameObject.GetComponent<Combustable>().name;

                    if (hitGameObject.GetComponent<Combustable>().fuel <= 0 &&
                        hitGameObject.GetComponent<Combustable>().name == "Wood")
                    {
                        pickupName = hitGameObject.GetComponent<Combustable>().name + " (Burnt)";
                    }

                    if (hitGameObject.GetComponent<Combustable>().isBurning)
                    {
                        pickupName = hitGameObject.GetComponent<Combustable>().name + " (Burning)";
                    }

                    pickupTemp = hitGameObject.GetComponent<Combustable>().temperature;
                }
                else
                {
                    showPickup = false;
                }
            }
        }
        else
        {
            showPickup = false;
            lookingAtObj = false;
        }

        Ray checkTerrainRay = new Ray(playerHeadPosition, playerForwardDirection);
        RaycastHit terrainRayHit;
        if (Physics.Raycast(checkTerrainRay, out terrainRayHit, distToObj))
        {
            if (hitGameObject.CompareTag("terrain"))
            {
                planeHit = rayPickupHit;
            }
        }
    }
}