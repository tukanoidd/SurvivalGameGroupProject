using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ash : Combustable
{
    // Start is called before the first frame update
    virtual protected void Start()
    {
        name = "Ash";
        tempInitial = temperature;                    //Needed for referencing
        flashpoint = -1;                              //At what temperature will the object burst into flames
        fuel = 0;                                     //How long will the object continue to burn
        heatTransfer = 10;                            //On a scale of 1-10 how well does the object conduct heat
        maxTemp = -1;                                 //At what temperature will the object vaporize
        glowTemp = 600;
        burnRate = -1;   
        renderer = GetComponent<MeshRenderer>();
        color = renderer.material.color;
        GarbageMan.ashTray.Add(gameObject);
    }

    // Update is called once per frame
    virtual protected void Update()
    {
        base.Update();

        if(temperature > glowTemp)
        {
            name = "Ember";
        }
    }
}
