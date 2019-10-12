﻿using System;
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
                if(!vicinity.Contains(other.gameObject) && other.GetComponent<Combustable>() != null)
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
        Initialize();
        alive = true;
        energyFull = energy;
        viewAngle = 45;
        viewRadius = 20f;
        movementSpeed = 7;
        moving = true;
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

    public void Initialize()
    {
        if (settings != null)
        {
            position = _cachedTransform.position;
            forward = _cachedTransform.forward;

            float startSpeed = (settings.minSpeed + settings.maxSpeed) / 2;
            _velocity = transform.forward * startSpeed;   
        }
    }
    
    Vector3 SteerTowards(Vector3 vector)
    {
        if (settings != null)
        {
            Vector3 v = vector.normalized * settings.maxSpeed - _velocity;
            return Vector3.ClampMagnitude(v, settings.maxSteerForce);   
        }

        return Vector3.zero;
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
        Vector3[] rayDirections = BoidHelper.directions;
        var random = Mathf.FloorToInt(Random.Range(0, rayDirections.Length));
        focalPoint = rayDirections[random] * viewRadius;
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
        if (settings != null)
        {
            if(Vector3.Distance(transform.position, focalPoint) < 1 && recharging)
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
    }

    protected virtual void recharge()
    {
        if (type == Type.FireFlies)
        {
            
        } else if (type == Type.BoomBugs)
        {
  
        }
    }
}