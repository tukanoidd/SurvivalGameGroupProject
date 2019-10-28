using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stone : Combustable
{
    public float hardness; //Hardness of the object
    public float breakForce; //Force required to break the object into pieces

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
}