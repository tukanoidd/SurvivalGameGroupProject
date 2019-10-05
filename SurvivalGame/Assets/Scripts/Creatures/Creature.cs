using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Creature : MonoBehaviour
{
    [Serializable]
    public enum Type
    {
        FireFlies,
        BoomBugs
    }

    public Type type;
    public float energy;
    public float energyFull;
    public float movementSpeed;
    public float viewAngle;
    public float viewRadius;
    public float step;
    public float minimumEnergySourceTemp;
    public Vector3 focalPoint;
    public bool moving;
    public bool recharging;
    public bool lookingForEnergy;
    public bool hungry;
    public bool alive;
    public List<GameObject> vicinity;
    public SphereCollider SOI_Collider;
    public GameObject mainEnergySource;
    
    public BoidSettings settings;

    [HideInInspector] public Vector3 position;
    [HideInInspector] public Vector3 forward;
    private Vector3 _velocity;

    private Vector3 _acceleration;

    private Transform _cachedTransform;

    void Awake()
    {
        _cachedTransform = transform;
    }
    
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
        GetRandomFocalPoint();
        Initialize(settings);
        alive = true;
        energyFull = energy;
        viewAngle = 45;
        viewRadius = 20f;
        movementSpeed = 7;
        moving = false;
        vicinity = new List<GameObject>();

        if (GetComponent<SphereCollider>() == null)
        {
            SOI_Collider = gameObject.AddComponent<SphereCollider>();
            SOI_Collider.radius = viewRadius;
        }
        else
        {
            SOI_Collider = GetComponent<SphereCollider>();
            SOI_Collider.radius = viewRadius;
        }
    }

    public void Initialize(BoidSettings settings)
    {
        this.settings = settings;

        position = _cachedTransform.position;
        forward = _cachedTransform.forward;

        float startSpeed = (this.settings.minSpeed + this.settings.maxSpeed) / 2;
        _velocity = transform.forward * startSpeed;
    }
    
    Vector3 SteerTowards(Vector3 vector)
    {
        Vector3 v = vector.normalized * settings.maxSpeed - _velocity;
        return Vector3.ClampMagnitude(v, settings.maxSteerForce);
    }
    
    bool isHeadingForCollision()
    {
        RaycastHit hit;
        if (Physics.SphereCast(position, settings.boundsRadius, forward, out hit, settings.collisionAvoidDst,
            settings.obstacleMask))
        {
            return true;
        }

        return false;
    }

    Vector3 ObstacleRays()
    {
        Vector3[] rayDirections = BoidHelper.directions;

        for (int i = 0; i < rayDirections.Length; i++)
        {
            Vector3 dir = _cachedTransform.TransformDirection(rayDirections[i]);
            Ray ray = new Ray(position, dir);
            if (!Physics.SphereCast(ray, settings.boundsRadius, settings.collisionAvoidDst, settings.obstacleMask))
            {
                return dir;
            }
        }

        return forward;
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
        focalPoint = position + forward + (position.y > 30 ? Vector3.down : Vector3.zero) + (position.y < 0 ? Vector3.up : Vector3.zero);
    }

    protected virtual void move(Vector3 direction)
    {
        /*
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
        }
        else
        {
            GetRandomFocalPoint();
            moving = true;
        }
        */
        
        if(Vector3.Distance(transform.position, focalPoint) < 1)
        {
            moving = false;
        }
        else
        {
            Vector3 acceleration = Vector3.zero;

            if (direction != null)
            {
                Vector3 offsetToTarget = (direction - position);
                acceleration = SteerTowards(offsetToTarget) * settings.targetWeight;
            }

            if (isHeadingForCollision())
            {
                Vector3 collisionAvoidDir = ObstacleRays();
                Vector3 collisionAvoidForce = SteerTowards(collisionAvoidDir) * settings.avoidCollisionWeight;
                acceleration += collisionAvoidForce;
            }

            _velocity += acceleration * Time.deltaTime;
            float speed = _velocity.magnitude;
            Vector3 dir = _velocity / speed;
            speed = Mathf.Clamp(speed, settings.minSpeed, settings.maxSpeed);
            _velocity = dir * speed;

            _cachedTransform.position += _velocity * Time.deltaTime;
            _cachedTransform.forward = dir;
            position = _cachedTransform.position;
            forward = dir;
            moving = true;
        }
    }

    protected virtual void recharge()
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
            if(Vector3.Distance(transform.position, focalPoint) < 5)
            {
                recharge();
            }
        }
    }
}
