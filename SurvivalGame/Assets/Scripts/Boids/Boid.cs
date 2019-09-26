using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Boid : MonoBehaviour
{
    private BoidSettings _settings;

    [HideInInspector] public Vector3 position;
    [HideInInspector] public Vector3 forward;
    private Vector3 _velocity;

    private Vector3 _acceleration;
    [HideInInspector] public Vector3 avgFlockHeading;
    [HideInInspector] public Vector3 avgAvoidanceHeading;
    [HideInInspector] public Vector3 centerOfFlockmates;
    [HideInInspector] public int numPerceivedFlockmates;

    private Material _material;
    private Transform _cachedTransform;
    private Transform _target;

    void Awake()
    {
        _material = transform.GetComponentInChildren<MeshRenderer>().material;
        _cachedTransform = transform;
    }

    public void Initialize(BoidSettings settings, Transform target)
    {
        _target = target;
        _settings = settings;

        position = _cachedTransform.position;
        forward = _cachedTransform.forward;

        float startSpeed = (_settings.minSpeed + _settings.maxSpeed) / 2;
        _velocity = transform.forward * startSpeed;
    }

    public void SetColor(Color color)
    {
        if (_material != null)
        {
            _material.color = color;
        }
    }

    Vector3 SteerTowards(Vector3 vector)
    {
        Vector3 v = vector.normalized * _settings.maxSpeed - _velocity;
        return Vector3.ClampMagnitude(v, _settings.maxSteerForce);
    }

    bool isHeadingForCollision()
    {
        RaycastHit hit;
        if (Physics.SphereCast(position, _settings.boundsRadius, forward, out hit, _settings.collisionAvoidDst,
            _settings.obstacleMask))
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
            if (!Physics.SphereCast(ray, _settings.boundsRadius, _settings.collisionAvoidDst, _settings.obstacleMask))
            {
                return dir;
            }
        }

        return forward;
    }

    public void UpdateBoid()
    {
        Vector3 acceleration = Vector3.zero;

        if (_target != null)
        {
            Vector3 offsetToTarget = (_target.position - position);
            acceleration = SteerTowards(offsetToTarget) * _settings.targetWeight;
        }

        if (numPerceivedFlockmates != 0)
        {
            centerOfFlockmates /= numPerceivedFlockmates;

            Vector3 offsetToFlockmatesCenter = (centerOfFlockmates - position);

            var alignmentForce = SteerTowards(avgFlockHeading) * _settings.alignWeight;
            var cohesionForce = SteerTowards(offsetToFlockmatesCenter) * _settings.cohesionWeight;
            var separationForce = SteerTowards(avgAvoidanceHeading) * _settings.separateWeight;

            acceleration += alignmentForce;
            acceleration += cohesionForce;
            acceleration += separationForce;
        }

        if (isHeadingForCollision())
        {
            Vector3 collisionAvoidDir = ObstacleRays();
            Vector3 collisionAvoidForce = SteerTowards(collisionAvoidDir) * _settings.avoidCollisionWeight;
            acceleration += collisionAvoidForce;
        }

        _velocity += acceleration * Time.deltaTime;
        float speed = _velocity.magnitude;
        Vector3 dir = _velocity / speed;
        speed = Mathf.Clamp(speed, _settings.minSpeed, _settings.maxSpeed);
        _velocity = dir * speed;

        _cachedTransform.position += _velocity * Time.deltaTime;
        _cachedTransform.forward = dir;
        position = _cachedTransform.position;
        forward = dir;
    }
}