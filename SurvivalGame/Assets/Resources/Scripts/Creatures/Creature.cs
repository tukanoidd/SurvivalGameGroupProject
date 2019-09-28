using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creature : MonoBehaviour
{
    public float energy;
    public float energyFull;
    public float movementSpeed;
    public float viewAngle;
    public float viewRadius;
    public float step;
    public Vector3 focalPoint;
    public bool moving;
    public bool recharging;
    public bool lookingForEnergy;
    public bool hungry;
    public bool alive;
    public List<GameObject> vicinity;
    public SphereCollider SOI_Collider;
    public GameObject mainEnergySource;

    private void OnTriggerEnter(Collider other)
    {
        if(vicinity.Count < 11)
        {
            if(other.gameObject != null)
            {
                if(!vicinity.Contains(other.gameObject))
                {
                    vicinity.Add(other.gameObject);
                }
            }
        }
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        alive = true;
        energyFull = energy;
        viewAngle = 45;
        viewRadius = 20;
        movementSpeed = 7;
        moving = false;
        vicinity = new List<GameObject>();
        SOI_Collider = GetComponent<SphereCollider>();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if(alive)
        {
            if(!recharging)
            {
                energy -= 0.5f;
            }
            var speed = (energy/energyFull * movementSpeed);
            step += 1;

            if(energy > 0)
            {
                if(energy < (energyFull/2))
                {
                    hungry = true;
                }
                if(moving)
                {
                    move(focalPoint);
                } else {
                    if(step % viewRadius == 0)
                    {
                        if(energy < energyFull)
                        {
                            GetRandomFocalPoint();
                            move(focalPoint);
                            findEnergySource();
                        } else {
                            GetRandomFocalPoint();
                            move(focalPoint);
                            recharging = false;
                        }
                    }
                }
            } else {
                alive = false;
            }
        } else {
            GetComponent<Rigidbody>().useGravity = true;
        }
    }

    public static Vector3 GetPointOnUnitSphereCap(Quaternion targetDirection, float angle)
    {
        var angleInRad = Random.Range(0.0f,angle) * Mathf.Deg2Rad;
        var PointOnCircle = (Random.insideUnitCircle.normalized)*Mathf.Sin(angleInRad);
        var V = new Vector3(PointOnCircle.x,PointOnCircle.y,Mathf.Cos(angleInRad));
        return targetDirection*V;
    }

    protected virtual void GetRandomFocalPoint()
    {
        var dist = Random.Range(0f,(float) viewRadius);
        focalPoint = GetPointOnUnitSphereCap(transform.rotation, viewAngle)*dist;
    }

    protected virtual void move(Vector3 direction)
    {
        if(direction != null)
        {
            if(Vector3.Distance(transform.position, focalPoint) < 1)
            {
                moving = false;
            } else {
                var dir = (direction - transform.position).normalized;
                var rot = Quaternion.LookRotation(dir);

                transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * movementSpeed);
                transform.position += transform.forward * Time.deltaTime * movementSpeed;
                Debug.DrawRay(transform.position, direction, Color.red);
                moving = true;
            }
        } else {
            GetRandomFocalPoint();
            moving = true;
        }
    }

    protected virtual void recharge()
    {
        var sourceTemp = mainEnergySource.GetComponent<Combustable>().temperature;
        if(sourceTemp > 0)
        {
            var amount = 2;
            mainEnergySource.GetComponent<Combustable>().temperature -= amount;
            energy += amount;
            recharging = true;
        } else {
            vicinity.Remove(mainEnergySource);
            recharging = false;
            //GetRandomFocalPoint();
            mainEnergySource = null;
        }
    }

    protected virtual void findEnergySource()
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

            var random = Random.Range(0f, (float) totalTemp);

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
            if(Vector3.Distance(transform.position, focalPoint) < 5)
            {
                recharge();
            }
        }
    }
}
