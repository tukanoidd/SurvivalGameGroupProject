using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ember : Ash
{
    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        temperature = glowTemp + 100;
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
    }
}
