using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ember : Ash
{
    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        temperature = glowTemp + 50;
        heatTransfer = 10;
        maxTemp = 200;
        name = "Ember";
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
        if (temperature > 200)
        {
            Debug.Log(temperature);
        }
        temperature = Mathf.Clamp(temperature, 0, maxTemp - 1);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Grass"))
        {
            fuel = 100;
        }
    }
}