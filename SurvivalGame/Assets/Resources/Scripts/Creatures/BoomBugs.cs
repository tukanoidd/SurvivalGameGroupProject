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
        name = "BoomBug";
        energy = 0;
        energyThreshold = 3000;
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
        if (mainEnergySource != null)
        {
            var amount = 2;
            if (mainEnergySource.gameObject != null)
            {
                mainEnergySource.GetComponent<Combustable>().temperature += amount;
                mainEnergySource.GetComponent<Combustable>().heatedBy = gameObject;
            }
            else
            {
                findEnergySource();
            }

            energy -= amount;
            recharging = true;
            if (mainEnergySource.GetComponent<Combustable>().isBurning || energy < 1f)
            {
                vicinity.Remove(mainEnergySource);
                recharging = false;
                mainEnergySource = null;
            }
        }
        else
        {
            findEnergySource();
        }
    }

    void findEnergySource()
    {
        lookingForEnergy = true;

        if (mainEnergySource == null)
        {
            if (vicinity.Count == 0)
            {
                GetRandomFocalPoint();
            }
            else
            {
                float smallest;
                int index = 0;

                for (int i = vicinity.Count - 1; i >= 0; i--)
                {
                    if (vicinity[i] != null)
                    {
                        smallest = Vector3.Distance(transform.position, vicinity[i].transform.position);
                        index = i;

                        if (i > 0 && vicinity[i - 1] != null)
                        {
                            if (Vector3.Distance(transform.position, vicinity[i - 1].transform.position) < smallest)
                            {
                                smallest = Vector3.Distance(transform.position, vicinity[i - 1].transform.position);
                                index = i - 1;
                            }
                        }
                    }
                }

                if (vicinity[index] != null)
                {
                    focalPoint = vicinity[index].transform.position;
                    mainEnergySource = vicinity[index];
                }
                else
                {
                    GetRandomFocalPoint();
                }
            }
        }
        else
        {
            focalPoint = mainEnergySource.transform.position;
            if (Vector3.Distance(transform.position, focalPoint) < 3)
            {
                recharge();
            }
        }
    }
}