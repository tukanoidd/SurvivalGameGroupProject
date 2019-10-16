using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Human : Combustable
{
    public float health;
    public float hunger;
    public float hungerRate = 0.02f;
    
    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        name = "Human";
        flashpoint = 150;                              //At what temperature will the object burst into flames
        heatTransfer = 6;                            //On a scale of 1-10 how well does the object conduct heat
        temperature = 37;
        tempInitial = temperature;
        hunger = 0;
        health = 100;
        maxTemp = 200;                                 //At what temperature will the object vaporize
        burnRate = 4;   
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
        if (hunger < 100)
        {
            hunger += hungerRate;
            temperature -= hungerRate / 2;
        }

        if (hunger > 50)
        {
            temperature -= hungerRate;
        }

        if (temperature > 70)
        {
            health -= 0.05f;
        }
        
        if (temperature < 20)
        {
            health -= hungerRate;
        }
    }

    void Eat(Food food)
    {
        
    }
}
