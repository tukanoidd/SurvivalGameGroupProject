using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Animations;
#endif
using UnityEngine;

public class CharacterPickupItems : MonoBehaviour
{
    [SerializeField] private GameObject head;
    [SerializeField] private GameObject shoulder;
    [SerializeField] private GameObject hammer;
    [SerializeField] private Camera _camera;
    private Animator animator;

    public GameObject hammerPrefab;
    
    private bool showPickup = false;
    private bool pickedUp = false;
    private bool lookingAtObj = false;
    private string pickupName;
    private float pickupTemp;
    [HideInInspector] public bool pickedHammer = false;

    private RaycastHit planeHit;

    [SerializeField] private float rayLength;
    [SerializeField] private float throwForce;
    [SerializeField] private float hitRayLength;
    [SerializeField] private float hitForce;
    

    [HideInInspector] public GameObject hitGameObject;
    private GameObject pickedObject;
    private Vector3 scale;

    private Quaternion origShoulderRot;

    void Start()
    {
        origShoulderRot = shoulder.transform.rotation;
        animator = GetComponent<Animator>();
    }

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

                    if (hitGameObject.CompareTag("hammer"))
                    {
                        Destroy(pickedObject);
                        pickedHammer = true;
                        shoulder.SetActive(true);
                    }
                    else
                    {
                        pickedHammer = false;
                        scale = pickedObject.transform.lossyScale;
                        pickedObject.GetComponent<Rigidbody>().isKinematic = true;
                        pickedObject.GetComponent<Combustable>().hasBeenMovedAfterThrow = false;
                        pickedObject.GetComponent<Combustable>().isGrounded = false;
                        pickedObject.transform.parent = _camera.transform;
                    }
                }
                else
                {
                    showPickup = false;
                    pickedUp = false;
                    
                    if (pickedHammer)
                    {
                        shoulder.SetActive(false);
                        pickedObject = Instantiate(hammerPrefab, hammer.transform.position, Quaternion.identity);
                        pickedObject.GetComponent<Rigidbody>()
                            .AddForce(playerForwardDirection * throwForce, ForceMode.Impulse);
                        pickedObject.GetComponent<Combustable>().isThrown = true;

                        pickedHammer = false;
                    }
                    else
                    {
                        pickedObject.GetComponent<Rigidbody>().isKinematic = false;
                        pickedObject.GetComponent<Rigidbody>()
                            .AddForce(playerForwardDirection * throwForce, ForceMode.Impulse);
                        pickedObject.GetComponent<Combustable>().isThrown = true;
                        
                        pickedObject.transform.parent = null;

                        pickedObject.transform.localScale = scale;   
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                if (pickedUp)
                {
                    pickedUp = false;
                    showPickup = false;

                    if (pickedHammer)
                    {
                        shoulder.SetActive(false);
                        pickedObject = Instantiate(hammerPrefab, hammer.transform.position, Quaternion.identity);
                        pickedObject.GetComponent<Hammer>().isThrown = true;
                        pickedHammer = false;
                    }
                    else
                    {
                        pickedObject.GetComponent<Rigidbody>().isKinematic = false;
                        pickedObject.GetComponent<Combustable>().isThrown = true;
                        
                        pickedObject.transform.parent = null;

                        pickedObject.transform.localScale = scale;
                    }
                }
            }

            if (Input.GetMouseButtonDown(0))
            {
                if (pickedHammer)
                {
                    animator.SetBool("HammerSwing", true);

                    CheckHitObject(playerHeadPosition, playerForwardDirection);
                }
            }
            else
            {
                animator.SetBool("HammerSwing", false);
            }
        }

        if (pickedObject != null && !pickedUp && !pickedObject.GetComponent<Combustable>().isGrounded &&
            pickedObject.GetComponent<Combustable>().isThrown &&
            !pickedObject.GetComponent<Combustable>().hasBeenMovedAfterThrow)
        {
            try
            {
                var pos = pickedObject.transform.position;
                var size = pickedObject.GetComponent<Renderer>().bounds.size;
                if (pos.y < planeHit.point.y)
                {
                    pickedObject.transform.position = new Vector3(pos.x,
                        planeHit.point.y + Mathf.Max(size.x, size.y, size.z), pos.z);
                    pickedObject.GetComponent<Combustable>().hasBeenMovedAfterThrow = true;
                }
            }
            catch
            {
            }
        }
    }

    void CheckHitObject(Vector3 playerHeadPosition, Vector3 playerForwardDirection)
    {
        int counter = 0;
        int maxFrames = 50;
        
        Ray rayHitObject = new Ray(playerHeadPosition, playerForwardDirection);
        RaycastHit rayHitObjectHit = new RaycastHit();

        while (counter < maxFrames)
        {
            if (Physics.Raycast(rayHitObject, out rayHitObjectHit, hitRayLength))
            {
                var hitObj = rayHitObjectHit.transform.gameObject;
                if (hitObj.transform.parent != null)
                {
                    var parent = hitObj.transform.parent;
                    var children = new List<Rigidbody>();

                    for (int i = 0; i < parent.childCount; i++)
                    {
                        children.Add(parent.GetChild(i).GetComponent<Rigidbody>());
                    }
                    
                    hitObj.transform.parent.DetachChildren();

                    foreach (var child in children)
                    {
                        child.isKinematic = false;
                        child.AddForce(playerForwardDirection * hitForce, ForceMode.Impulse);
                    }
                }
                else
                {
                    if (hitObj.GetComponent<Rigidbody>() != null)
                    {
                        var rigBody = hitObj.GetComponent<Rigidbody>();
                        rigBody.isKinematic = false;
                        rigBody.AddForce(playerForwardDirection * hitForce);   
                    }
                }
                
                return;
            }

            counter++;
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

            GUI.Label(new Rect(Screen.width * 15 / 20, Screen.height * 9 / 10, 300, 50),
                "MRB: Show Information About Object");
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

                if (hitGameObject.GetComponent<Combustable>() != null || hitGameObject.CompareTag("hammer"))
                {
                    lookingAtObj = true;

                    if (!pickedUp)
                    {
                        showPickup = true;
                    }

                    if (hitGameObject.GetComponent<Combustable>() != null)
                    {
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
                    else if (hitGameObject.CompareTag("hammer"))
                    {
                        pickupName = hitGameObject.name;
                    }
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