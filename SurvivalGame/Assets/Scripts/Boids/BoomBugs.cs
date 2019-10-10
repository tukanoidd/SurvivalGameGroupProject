using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoomBugs : Creature
{
    //public Light boidGlow;
    public float energyThreshold;

    // Start is called before the first frame update
    protected override void Start()
    {
        energy = 0;
        energyThreshold = 1000;
        minimumEnergySourceTemp = 50f;
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        if (alive)
        {
            if (!recharging)
            {
                energy += 0.8f;
                var speed = (energy / energyFull * movementSpeed);
                step += 1;

                if (energy < energyThreshold)
                {
                    if (energy > (energyThreshold / 2))
                    {
                        hungry = true;
                    }

                    if (hungry)
                    {
                        findEnergySource();
                        move(focalPoint);
                    }
                    else
                    {
                        GetRandomFocalPoint();
                        move(focalPoint);
                        recharging = false;
                    }
                }
                else
                {
                    alive = false;
                    if (GetComponent<Rigidbody>() != null)
                    {
                        GetComponent<Rigidbody>().useGravity = true;
                        Destroy(gameObject, 10);
                    }
                }
            }
            else
            {
                recharge();
            }
            
        }
        
        //boidGlow.intensity = Mathf.Clamp((float) energy,0,0.5f);
    }
}