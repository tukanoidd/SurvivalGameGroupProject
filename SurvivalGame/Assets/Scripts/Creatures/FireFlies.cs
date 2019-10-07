using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireFlies : Creature
{
    //public Light boidGlow;

    // Start is called before the first frame update
    protected override void Start()
    {
        energy = 1000;
        minimumEnergySourceTemp = 0f;
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        if (alive)
        {
            if(!recharging)
            {
                energy -= 0.5f;
                var speed = (energy/energyFull * movementSpeed);
                step += 1;
                
                if(energy > 0)
                {
                    if(energy < (energyFull/2))
                    {
                        hungry = true;
                    }

                    if(hungry)
                    {
                        findEnergySource();
                        move(focalPoint);
                    } else {
                        GetRandomFocalPoint();
                        move(focalPoint);
                        recharging = false;
                    }
                            
                } else {
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
