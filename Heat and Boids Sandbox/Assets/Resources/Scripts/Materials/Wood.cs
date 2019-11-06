using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wood : Combustable
{
    private float fuelInitial;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        name = "Wood";
        temperature = 25f;                             //Object temperature
        tempInitial = 25f;                              //Needed for referencing
        flashpoint = 150;                               //At what temperature will the object burst into flames
        fuel = 300 * GetObjectVolume();                 //How long will the object continue to burn
        fuelInitial = fuel;
        heatTransfer = 9;                               //On a scale of 1-10 how well does the object conduct heat
        heatResistance = 4;
        burnRate = 5;     
        renderer = GetComponent<MeshRenderer>();
        renderer.material = Resources.Load<Material>("Materials/Wood");
        color = renderer.material.color;
    }

    // Update is called once per frame
    protected override void Update()
    {
        tempExternal = temperature - tempInitial;
        base.Update();
        if(fuel <= 0)
        {
            renderer.material.color = Color.black;
        }
    }
}
