using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Animations;
#endif
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class CharacterPickupItems : MonoBehaviour
{
    [SerializeField] private GameObject head;
    [SerializeField] private GameObject shoulder;
    [SerializeField] private GameObject hammer;
    [SerializeField] private GameObject axe;
    [SerializeField] private GameObject axeHead;
    [SerializeField] private Camera _camera;
    private Animator animator;

    public GameObject hammerPrefab;
    public GameObject axePrefab;

    private bool showPickup = false;
    private bool pickedUp = false;
    private bool lookingAtObj = false;
    private string pickupName;
    private float pickupTemp;

    [HideInInspector] public bool pickedHammer = false;
    [HideInInspector] public bool pickedAxe = false;

    private RaycastHit planeHit;

    [SerializeField] private float rayLength;
    [SerializeField] private float throwForce;
    [SerializeField] private float hitRayLength;
    [SerializeField] private float hitForce;

    [HideInInspector] public GameObject hitGameObject;
    public GameObject pickedObject;
    private Vector3 scale;

    [SerializeField] private GameObject[] logsPrefabs;
    [SerializeField] private int numLogsPerTrunk = 2;

    private GameObject stickPrefab;
    private Vector3 stickYOffset;
    [SerializeField] private int numSticksPerBranchOrBush = 4;

    private GameObject plankPrefab;
    private Vector3 plankYOffset;
    [SerializeField] private int numPlanksPerLog = 2;

    [SerializeField] private GameObject[] largeRocksPrefabs;
    [SerializeField] private GameObject[] largeRedRocksPrefabs;
    [SerializeField] private GameObject[] mediumRocksPrefabs;
    [SerializeField] private GameObject[] mediumRedRocksPrefabs;
    [SerializeField] private GameObject[] smallRocksPrefabs;
    [SerializeField] private GameObject[] smallRedRocksPrefabs;

    [SerializeField] private int numMedRocksPerLargeRock = 2;
    [SerializeField] private int numSmallRocksPerMedRock = 2;

    [SerializeField] private GameObject lookingAtObjectUIText;
    [SerializeField] private GameObject pickUpItemUIText;
    [SerializeField] private GameObject pickedUpItemUIText;
    [SerializeField] private GameObject pickedUpToolUIText;

    private GameObject sparkObj;
    private ParticleSystem sparks;

    void Start()
    {
        animator = GetComponent<Animator>();

        pickedHammer = false;
        pickedAxe = false;

        stickPrefab = Resources.Load<GameObject>("Prefabs/Wood/Stick");
        stickYOffset = GetYOffset(stickPrefab);

        plankPrefab = Resources.Load<GameObject>("Prefabs/Wood/Plank");
        plankYOffset = GetYOffset(plankPrefab);

        sparkObj = Resources.Load<GameObject>("Prefabs/Effects/Sparks");
        sparks = sparkObj.GetComponent<ParticleSystem>();
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

                    if (hitGameObject.CompareTag("tool"))
                    {
                        Destroy(pickedObject);
                        if (hitGameObject.GetComponent<Combustable>().name == "Hammer")
                        {
                            pickedHammer = true;
                            hammer.SetActive(true);
                        }
                        else if (hitGameObject.GetComponent<Combustable>().name == "Axe")
                        {
                            pickedAxe = true;
                            axe.SetActive(true);
                        }

                        shoulder.SetActive(true);
                    }
                    else
                    {
                        pickedHammer = false;
                        pickedAxe = false;
                        pickedObject.GetComponent<Combustable>().isPicked = true;  
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
                    

                    if (pickedHammer || pickedAxe)
                    {
                        shoulder.SetActive(false);

                        if (pickedHammer)
                        {
                            hammer.SetActive(false);
                            pickedObject = Instantiate(hammerPrefab, hammer.transform.position, Quaternion.identity);
                        }
                        else if (pickedAxe)
                        {
                            axe.SetActive(false);
                            pickedObject = Instantiate(axePrefab, axe.transform.position, Quaternion.identity);
                        }

                        pickedHammer = false;
                        pickedAxe = false;
                    }
                    else if (!pickedAxe && !pickedHammer)
                    {
                        pickedObject.transform.parent = null;
                        pickedObject.GetComponent<Combustable>().isPicked = false;
                        pickedObject.transform.localScale = scale;
                    }

                    pickedObject.GetComponent<Rigidbody>().isKinematic = false;
                    pickedObject.GetComponent<Rigidbody>()
                        .AddForce(playerForwardDirection * throwForce, ForceMode.Impulse);
                    pickedObject.GetComponent<Combustable>().isThrown = true;
                }
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                if (pickedUp)
                {
                    pickedUp = false;
                    showPickup = false;

                    if (pickedHammer || pickedAxe)
                    {
                        shoulder.SetActive(false);

                        if (pickedHammer)
                        {
                            hammer.SetActive(false);
                            pickedObject = Instantiate(hammerPrefab, hammer.transform.position, Quaternion.identity);
                        }
                        else if (pickedAxe)
                        {
                            axe.SetActive(false);
                            pickedObject = Instantiate(axePrefab, hammer.transform.position, Quaternion.identity);
                        }

                        pickedHammer = false;
                        pickedAxe = false;
                    }
                    else if (!pickedAxe && !pickedHammer)
                    {
                        pickedObject.transform.parent = null;
                        pickedObject.GetComponent<Combustable>().isPicked = false;
                        pickedObject.transform.localScale = scale;
                    }

                    pickedObject.GetComponent<Rigidbody>().isKinematic = false;
                    pickedObject.GetComponent<Combustable>().isThrown = true;
                }
            }

            if (Input.GetMouseButtonDown(0))
            {
                if (pickedHammer || pickedAxe)
                {
                    animator.SetBool("ToolSwing", true);

                    CheckHitObject(playerHeadPosition, playerForwardDirection);
                }
            }
            else
            {
                animator.SetBool("ToolSwing", false);
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

        CheckGUI();
    }

    private void CheckGUI()
    {
        lookingAtObjectUIText.GetComponent<Text>().text =
            lookingAtObj ? pickupName + " - (Temp: " + pickupTemp + ")" : "";

        lookingAtObjectUIText.SetActive(lookingAtObj);
        pickUpItemUIText.SetActive(showPickup);
        pickedUpItemUIText.SetActive(pickedUp);
        pickedUpToolUIText.SetActive(lookingAtObj && (pickedAxe || pickedHammer));
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

                if (hitObj.GetComponent<Combustable>() != null && !hitObj.GetComponent<Human>())
                {
                    if (!hitObj.GetComponent<Combustable>().hasBeenHitByHammer ||
                        !hitObj.GetComponent<Combustable>().hasBeenHitByAxe)
                    {
                        if (pickedHammer)
                        {
                            hitObj.GetComponent<Combustable>().hasBeenHitByHammer = true;
                            hitObj.GetComponent<Rigidbody>().isKinematic = false;
                        }
                        else if (pickedAxe)
                        {
                            hitObj.GetComponent<Combustable>().hasBeenHitByAxe = true;
                            hitObj.GetComponent<Rigidbody>().isKinematic = false;
                        }

                        if (pickedAxe && hitObj.GetComponent<Combustable>().hasBeenHitByAxe)
                        {
                            if (hitObj.GetComponent<Wood>() != null || hitObj.GetComponent<Plant>() != null)
                            {
                                if (hitObj.transform.parent != null)
                                {
                                    if (!hitObj.transform.parent.name.Contains("Terrain"))
                                    {
                                        DetachChildren(hitObj.transform.parent, playerForwardDirection);
                                    }
                                }
                                else
                                {
                                    DetachChildren(hitObj.transform, playerForwardDirection);
                                }
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
                    }

                    if (hitObj.GetComponent<Combustable>().hasBeenHitByHammer ||
                        hitObj.GetComponent<Combustable>().hasBeenHitByAxe)
                    {
                        if (hitObj.GetComponent<Combustable>().hasBeenHitByHammer && pickedHammer)
                        {
                            if (hitObj.CompareTag("Rock Large"))
                            {
                                hitObj.SetActive(false);

                                for (int i = 0; i < numMedRocksPerLargeRock; i++)
                                {
                                    var medRockPrefab =
                                        mediumRocksPrefabs[Random.Range(0, mediumRocksPrefabs.Length - 1)];
                                    var yOffset = GetYOffset(medRockPrefab);

                                    var medRock = Instantiate(medRockPrefab,
                                        hitObj.transform.position + yOffset, hitObj.transform.rotation);
                                    medRock.GetComponent<Rigidbody>().isKinematic = false;
                                }

                                Destroy(hitObj);
                            }
                            else if (hitObj.CompareTag("Rock Medium"))
                            {
                                hitObj.SetActive(false);

                                for (int i = 0; i < numSmallRocksPerMedRock; i++)
                                {
                                    var smallRockPrefab =
                                        smallRocksPrefabs[Random.Range(0, smallRocksPrefabs.Length - 1)];
                                    var yOffset = GetYOffset(smallRockPrefab);

                                    var smallRock = Instantiate(smallRockPrefab, hitObj.transform.position + yOffset,
                                        hitObj.transform.rotation);
                                    smallRock.GetComponent<Rigidbody>().isKinematic = false;
                                }

                                Destroy(hitObj);
                            }
                            else if (hitObj.CompareTag("Red Rock Large"))
                            {
                                hitObj.SetActive(false);

                                for (int i = 0; i < numMedRocksPerLargeRock; i++)
                                {
                                    var medRedRockPrefab =
                                        mediumRedRocksPrefabs[Random.Range(0, mediumRedRocksPrefabs.Length - 1)];

                                    var yOffset = GetYOffset(medRedRockPrefab);

                                    var medRedRock = Instantiate(medRedRockPrefab, hitObj.transform.position + yOffset,
                                        hitObj.transform.rotation);
                                    medRedRock.GetComponent<Rigidbody>().isKinematic = false;
                                }

                                Destroy(hitObj);
                            }
                            /*else if (hitObj.CompareTag("Red Rock Medium"))
                            {
                                hitObj.SetActive(false);
    
                                for (int i = 0; i < numSmallRocksPerMedRock; i++)
                                {
                                    Instantiate(smallRedRocksPrefabs[Random.Range(0, smallRedRocksPrefabs.Length - 1)],
                                        hitObj.transform.position, hitObj.transform.rotation);
                                }
    
                                Destroy(hitObj);
                            } */
                            else if (hitObj.CompareTag("Flint"))
                            {
                                SparksAndAmber(rayHitObjectHit.point);
                            }
                        }
                        else if (hitObj.GetComponent<Combustable>().hasBeenHitByAxe && pickedAxe)
                        {
                            if (hitObj.CompareTag("Trunk"))
                            {
                                hitObj.SetActive(false);

                                for (int i = 0; i < numLogsPerTrunk; i++)
                                {
                                    var logPrefab = logsPrefabs[Random.Range(0, logsPrefabs.Length)];

                                    var yOffset = GetYOffset(logPrefab);

                                    var log = Instantiate(logPrefab, hitObj.transform.position + yOffset,
                                        hitObj.transform.rotation);
                                    log.GetComponentInChildren<Rigidbody>().isKinematic = false;
                                }

                                Destroy(hitObj);
                            }
                            else if (hitObj.CompareTag("Log"))
                            {
                                hitObj.SetActive(false);

                                for (int i = 0; i < numPlanksPerLog; i++)
                                {
                                    var plank = Instantiate(plankPrefab, hitObj.transform.position + plankYOffset,
                                        hitObj.transform.rotation);
                                    plank.GetComponent<Rigidbody>().isKinematic = false;
                                }

                                Destroy(hitObj);
                            }
                            else if (hitObj.CompareTag("Branch") || hitObj.CompareTag("Bush"))
                            {
                                if (hitObj.CompareTag("Bush"))
                                {
                                    DetachChildren(hitObj.transform, playerForwardDirection);
                                }

                                hitObj.SetActive(false);

                                for (int i = 0; i < numSticksPerBranchOrBush; i++)
                                {
                                    var stick = Instantiate(stickPrefab, hitObj.transform.position + stickYOffset,
                                        hitObj.transform.rotation);
                                    stick.GetComponent<Rigidbody>().isKinematic = false;
                                }

                                Destroy(hitObj);
                            }
                            else if (hitObj.tag.Contains("Rock"))
                            {
                                SparksAndAmber(rayHitObjectHit.point);
                            }
                        }
                    }
                }
            }

            return;
        }

        counter++;
    }

    Vector3 GetYOffset(GameObject obj)
    {
        Renderer rend;

        if (obj.GetComponent<Renderer>() != null)
        {
            rend = obj.GetComponent<Renderer>();
        }
        else
        {
            rend = obj.GetComponentInChildren<Renderer>();
        }

        var size = rend.bounds.size;
        return Vector3.up * Mathf.Min(size.x, size.y, size.z);
    }

    void DetachChildren(Transform parent, Vector3 playerForwardDirection)
    {
        if (parent.childCount > 0)
        {
            var children = new List<Rigidbody>();

            for (int i = 0; i < parent.childCount; i++)
            {
                if (parent.transform.GetChild(i).GetComponent<Rigidbody>() != null)
                {
                    children.Add(parent.transform.GetChild(i).GetComponent<Rigidbody>());
                }
                else
                {
                    children.Add(parent.transform.GetChild(i).GetComponentInChildren<Rigidbody>());
                }
            }

            parent.DetachChildren();

            foreach (var child in children)
            {
                child.isKinematic = false;
                child.AddForce(playerForwardDirection * hitForce, ForceMode.Impulse);
            }
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
            if (rayPickupHit.transform.gameObject != null && rayPickupHit.transform.gameObject.name != "Player")
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

    public void SparksAndAmber(Vector3 position)
    {
        var sparkInstance = Instantiate(sparkObj, position, Quaternion.identity);
        sparkInstance.transform.parent = gameObject.transform;

        var obj = Resources.Load<GameObject>("Prefabs/Effects/Ember");
        var ember = Instantiate(obj, position, Quaternion.identity);


        Destroy(sparkInstance, sparks.duration);
    }
}