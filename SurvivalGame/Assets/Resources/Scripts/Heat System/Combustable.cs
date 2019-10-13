using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class Combustable : MonoBehaviour
{
    public string name;
    public float temperature = 25f;                              //Object temperature
    public float tempInitial;                                    //Needed for referencing
    public float tempExternal;                                   //The heat that the object will radiate once the heat source has been extinguished
    public float flashpoint = 150;                               //At what temperature will the object burst into flames
    public float fuel = 1000;                                    //How long will the object continue to burn
    public float glowTemp = 1100;                                //At what temperature will the object start glowing hot
    public float heatTransfer = 2;                               //On a scale of 1-10 how well does the object conduct heat
    public float heatResistance = 5;                             //On a scale of 1-10 how resistant is the object to heat? (1 = extermely non-resistant, 10 = extremely resistant)
    public float maxTemp;                                        //At what temperature will the object vaporize (Unlikely scenario)
    public float burnRate = 7;                                   //On a scale of 1-10 how fast does the fuel/object burn
    public bool doesIgnite = true;
    public bool isBurning = false;                               //Is this object on fire?
    public float heatGlow = 0;                                   //Used for changing texture with temp increase
    public float impactForce = 0;                                //How much force is the object experiencing
    public ContactPoint[] contactPoints;                         //Impact contact points
    public GameObject impactedObject;
    public Color red = new Color(255f,0,0,1);
    public Color blue = new Color(0,0,255f,1);
    public Color color;
    public MeshRenderer renderer;
    public Collider collider;
    public GameObject flameObj;
    public GameObject smokeObj;
    public GameObject groupParent;
    public GameObject heatedBy;
    public bool vaporized = false;
    public bool preIgnited = false;
    public bool isGrounded;
    public bool isThrown = false;
    public bool hasBeenMovedAfterThrow = false;
    
    // Start is called before the first frame update
    protected virtual void Start()
    {
        tempInitial = temperature;

        if(GetComponent<Collider>() != null)
        {
            collider = GetComponent<Collider>();
        }

        if(GetComponent<MeshRenderer>() != null)
        {
            renderer = GetComponent<MeshRenderer>();
            color = renderer.material.color;
        }

        maxTemp = (heatResistance * 200) + (GetObjectVolume() * 50);

        smokeObj = Resources.Load<GameObject>("Prefabs/TorchSmoke");
        flameObj = Resources.Load<GameObject>("Prefabs/FlameMain");

        if(preIgnited)
        {
            heatedBy = flameObj;
        }
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        radiate();
        tempExternal = temperature - tempInitial;

        if(maxTemp > 0)
        {
            if(temperature >= maxTemp)
            {
                if(!vaporized)
                {
                    vaporize();
                }
            }
        }

        if(temperature >= glowTemp)
        {
            renderer.material.color = Color.Lerp(renderer.material.color,red, 0.001f);
        }

        if(temperature < 0)
        {
            float ratio = Mathf.Abs(temperature/tempInitial)*30;
            float clamp = Mathf.Clamp(ratio,0,255f);

            Color blu = new Color(0,0,clamp,1);
            renderer.material.color = blu; //Color.Lerp(renderer.material.color,blu, 0.001f);
        }

        if(flashpoint > 0)
        {
            if(temperature >= flashpoint && fuel > 0)
            {
                if(doesIgnite)
                {
                    ignite();
                }
            }
        }

        if(fuel <= 0)
        {
            isBurning = false;
            radiate();
            renderer.material.color = renderer.material.color;
        }

    }

    void radiate()
    {
        if(temperature > tempInitial)
        {
            renderer.material.color = Color.Lerp(renderer.material.color,color, 0.003f);
            float percentage = temperature * (heatTransfer/10f);
            temperature -= (percentage/1000);

            if(temperature < 800)
            {
                renderer.material.color = Color.Lerp(renderer.material.color,color, 0.01f);
            }

            if(fuel <= 0 && temperature < flashpoint)
            {
                Destroy(flameObj);
            }

            if(fuel <= 0 && temperature < flashpoint/2)
            {
                Destroy(smokeObj, 5);
            }
        }
    }

    void ignite()
    {
        if(!isBurning)
        {
            Vector3 thisObjectSizeVector = renderer.bounds.size;
            //Vector3 flameCenter = new Vector3(gameObject.transform.position.x,gameObject.transform.position.y + (thisObjectSizeVector.y/2),gameObject.transform.position.z);
            Vector3 closestPointToFlame = collider.ClosestPointOnBounds(heatedBy.transform.GetChild(0).position);
            Vector3 flameCenter = closestPointToFlame;

            GameObject flame = Instantiate(flameObj, flameCenter, Quaternion.identity);
            GameObject smoke = Instantiate(smokeObj, flame.transform.position, Quaternion.identity);

            flameObj = flame;
            smokeObj = smoke;

             var emptyObject = new GameObject();
            emptyObject.transform.parent = gameObject.transform;
            smoke.transform.parent = emptyObject.transform;
            flame.transform.parent = emptyObject.transform;

            SphereCollider flameSOI = flame.GetComponent<SphereCollider>();
            flame.GetComponent<HeatSource>().parent = gameObject;
            flame.GetComponent<HeatSource>().SOI_Collider = flameSOI;
            flame.GetComponent<Flame>().flameObj = flame;
            isBurning = true;
        }
    }

    public void vaporize()
    {
        vaporized = true;
        var ashGroup = Resources.Load<GameObject>("Prefabs/CombustableGroup");
        ashGroup.GetComponent<CombustableGroup>().groupObject = Resources.Load<GameObject>("Prefabs/Ash");
        ashGroup.GetComponent<CombustableGroup>().spawnRadius = getLargestSide(renderer.bounds.size);

        var amount = Mathf.RoundToInt(Mathf.Clamp(GetObjectVolume(),1,10));

        for(int i=0; i < amount; i++)
        {
            var ashGroupObj = Instantiate(ashGroup, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }

    public float GetObjectVolume()
    {
        float volume;
        
        if (renderer == null)
        {
            Bounds bounds = new Bounds();
            foreach (var renderer in GetComponentsInChildren<MeshRenderer>())
            {
                bounds.Encapsulate(renderer.bounds);
            }   
            
            volume = bounds.size.x + bounds.size.y + bounds.size.z;
        }
        else
        {
            volume = renderer.bounds.size.x + renderer.bounds.size.y + renderer.bounds.size.z;    
        }
        
        return volume;
    }

    float getLargestSide(Vector3 sizeVector)
    {
        var max = -1f;

        for(int i = 0; i < 3; i++)
        {
            if(sizeVector[i] > max)
            {
                max = sizeVector[i];
            }
        }

        return max;
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("terrain"))
        {
            isGrounded = true;
        }
        
        impactForce = collision.relativeVelocity.magnitude;
        contactPoints = collision.contacts;
        impactedObject = collision.gameObject;
        Vector3 objectPosition = impactedObject.transform.position;
        Quaternion objectRotation = impactedObject.transform.rotation;

        Vector3 directionOut;
        float distanceOut;

        if (impactedObject.GetComponent<Collider>() != null && collider != null)
        {
            bool overlapped = Physics.ComputePenetration(collider, transform.position, transform.rotation,
                impactedObject.GetComponent<Collider>(), objectPosition, objectRotation, out directionOut, out distanceOut);

            if (impactedObject.CompareTag("terrain") && distanceOut > 1) 
            {
                GetComponent<Rigidbody>().isKinematic = true;
            }
        }
        
        isThrown = false;
    }
}
