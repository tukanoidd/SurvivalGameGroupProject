using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : Combustable
{
    public bool poisonous;
    public bool burnt;
    public float energy;
    public float cookingTemp;
    public float burnedTemp;
    public float cookingCounter;
    public float cookedPoint;

    // Start is called before the first frame update
    void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
    }

    public void Cooking()
    {
        cookingCounter += 0.01f;
        if (cookingCounter > (cookedPoint / 2))
        {
            poisonous = false;
        }

        energy += cookingCounter / 10;
    }
}
