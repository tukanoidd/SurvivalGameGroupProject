using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class BoidSettings : ScriptableObject
{
    public float minSpeed = 2f;
    public float maxSpeed = 5f;

    public float perceptionRadius = 2.5f;
    public float avoidanceRadius = 1f;

    public float maxSteerForce = 3f;

    public float separateWeight = 1f;

    public float targetWeight = 1;

    [Header("Collisions")] public LayerMask obstacleMask;
    public float boundsRadius = .27f;
    public float avoidCollisionWeight = 10;
    public float collisionAvoidDst = 5;
}