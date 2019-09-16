using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class FlockAgent : MonoBehaviour
{
    private Flock agentFlock;
    public Flock AgentFlock => agentFlock;

    private Collider agentCollider;

    public Collider AgentCollider => agentCollider;

    void Start()
    {
        agentCollider = GetComponent<Collider>();
    }

    public void Move(Vector3 velocity)
    {
        transform.forward = velocity;
        transform.position += velocity * Time.deltaTime;
    }

    public void Initialize(Flock flock)
    {
        agentFlock = flock;
    }
}
