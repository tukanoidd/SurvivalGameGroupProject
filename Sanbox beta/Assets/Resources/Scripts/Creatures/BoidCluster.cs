using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidCluster : Creature
{
    public Light boidGlow;

    // Start is called before the first frame update
    protected override void Start()
    {
        energy = 1000;
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        boidGlow.intensity = Mathf.Clamp((float) energy,0,0.5f);
    }
}
