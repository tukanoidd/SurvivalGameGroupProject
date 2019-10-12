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
                    if (energy > (energyThreshold / 4))
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

    void recharge()
    {
        var amount = 2;
        mainEnergySource.GetComponent<Combustable>().temperature += amount;
        mainEnergySource.GetComponent<Combustable>().heatedBy = gameObject;
        energy -= amount;
        recharging = true;
        if (mainEnergySource.GetComponent<Combustable>().isBurning || energy < 1f)
        {
            vicinity.Remove(mainEnergySource);
            recharging = false;
            mainEnergySource = null;
        }
    }
    
    void findEnergySource()
    {
        lookingForEnergy = true;

        if(mainEnergySource == null)
        {
            if(vicinity.Count == 0)
            {
                var dist = Random.Range(0f,(float) viewRadius);
                focalPoint = GetPointOnUnitSphereCap(transform.rotation, viewAngle)*dist;
                move(focalPoint);
            }
            else
            {
                float smallest;
                int index = 0;
                
                for (int i = vicinity.Count - 1; i >= 0; i--)
                {
                    smallest = Vector3.Distance(transform.position, vicinity[i].transform.position);
                    index = i;

                    if (i > 0)
                    {
                        if (Vector3.Distance(transform.position, vicinity[i-1].transform.position) < smallest)
                        {
                            smallest = Vector3.Distance(transform.position, vicinity[i-1].transform.position);
                            index = i-1;
                        }
                    }
                }
                
                focalPoint = vicinity[index].transform.position;
                mainEnergySource = vicinity[index];
            }
        } else {
            if (mainEnergySource.gameObject == null)
            {
                findEnergySource();
            }
            focalPoint = mainEnergySource.transform.position;
            if(Vector3.Distance(transform.position, focalPoint) < 1)
            {
                recharge();
            }
        }
    }
}