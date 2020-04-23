using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stone : Craftable
{
    public float hardness; //Hardness of the object
    public float breakForce; //Force required to break the object into pieces
    public List<GameObject> brokenPrefabs;

    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        name = "Stone";
        temperature = 22f; //Object temperature
        tempInitial = 22f; //Needed for referencing
        flashpoint = -1; //At what temperature will the object burst into flames
        fuel = 0;
        heatTransfer = 6; //On a scale of 1-10 how well does the object conduct heat
        heatResistance = 7;
        hardness = 10;
        breakForce = hardness * GetObjectVolume();
        burnRate = 0;
        color = renderer.material.color;
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
    }

    public void Break()
    {
        if (brokenPrefabs.Count > 0)
        {
            var prevPos = transform.position;
            for (int i = 0; i < brokenPrefabs.Count; i++)
            {
                var newStone = Instantiate(brokenPrefabs[i], prevPos + GetYOffset(brokenPrefabs[i]), transform.rotation);
                if (newStone.GetComponent<Rigidbody>() != null)
                {
                    newStone.GetComponent<Rigidbody>().isKinematic = false;
                } else if (newStone.GetComponentsInChildren<Rigidbody>().Length > 0)
                {
                    foreach (var rigidB in newStone.GetComponentsInChildren<Rigidbody>())
                    {
                        rigidB.isKinematic = false;
                    }
                }
                prevPos += GetYOffset(brokenPrefabs[i]);
            }
            
            Destroy(gameObject);
        }
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
}