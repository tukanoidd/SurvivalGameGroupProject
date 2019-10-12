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
    
    void recharge()
    {
        if (energy < energyFull)
        {
            var sourceTemp = mainEnergySource.GetComponent<Combustable>().temperature;
            if(sourceTemp > 0)
            {
                var amount = 2;
                mainEnergySource.GetComponent<Combustable>().temperature -= amount;
                energy += amount;
                recharging = true;
            }
            else
            {
                vicinity.Remove(mainEnergySource);
                recharging = false;
                //GetRandomFocalPoint();
                mainEnergySource = null;
            }
        }
        else
        {
            hungry = false;
        }   
    }
    
    void findEnergySource()
    {
        var totalTemp = 0f;

        lookingForEnergy = true;

        if(mainEnergySource == null)
        {
            if(vicinity.Count == 0)
            {
                var dist = Random.Range(0f,(float) viewRadius);
                focalPoint = GetPointOnUnitSphereCap(transform.rotation, viewAngle)*dist;
                move(focalPoint);
            }

            for(int i = 0; i < vicinity.Count; i++)
            {
                if(vicinity[i] != null)
                {
                    if(vicinity[i].GetComponent<Combustable>() != null)
                    {
                        var obj = vicinity[i];
                        var temp = obj.GetComponent<Combustable>().temperature;
                        totalTemp += temp;
                    }
                }
            }

            var random = Random.Range(minimumEnergySourceTemp, (float) totalTemp);

            for(int j = 0; j < vicinity.Count; j++)
            {
                if(vicinity[j] != null)
                {
                    if(vicinity[j].GetComponent<Combustable>() != null)
                    {
                        var temp = vicinity[j].GetComponent<Combustable>().temperature;
                        
                        if(temp > random)
                        {
                            focalPoint = vicinity[j].transform.position;
                            mainEnergySource = vicinity[j];
                        }
                    }
                }
            }
        } else {
            focalPoint = mainEnergySource.transform.position;
            if(Vector3.Distance(transform.position, focalPoint) < 1)
            {
                recharge();
            }
        }
    }
}
