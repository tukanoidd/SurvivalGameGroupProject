using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Human : Combustable
{
    public float health;
    public float hunger;
    
    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        name = "Human";
        flashpoint = 150;                              //At what temperature will the object burst into flames
        heatTransfer = 6;                            //On a scale of 1-10 how well does the object conduct heat
        maxTemp = 200;                                 //At what temperature will the object vaporize
        burnRate = 4;   
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
    }
}
