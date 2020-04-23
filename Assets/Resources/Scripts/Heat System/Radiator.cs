using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Radiator : HeatSource
{
    public GameObject parent;
    public float parentTemp;
    
    // Start is called before the first frame update
    void Start()
    {
        parentTemp = parent.GetComponent<Combustable>().temperature;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
