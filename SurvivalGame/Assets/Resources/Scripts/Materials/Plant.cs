using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : Combustable
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        name = "Plant";
        temperature = 22f; //Object temperature
        tempInitial = 22f; //Needed for referencing
        flashpoint = 60; //At what temperature will the object burst into flames
        fuel = 200 * GetObjectVolume();
        ; //How long will the object continue to burn
        heatTransfer = 10; //On a scale of 1-10 how well does the object conduct heat
        heatResistance = 2;
        burnRate = 7;
        color = renderer.material.color;
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.transform.CompareTag("terrain"))
        {
            if (transform.CompareTag("Grass"))
            {
                GetComponent<Rigidbody>().isKinematic = true;
            }
        }
    }
}