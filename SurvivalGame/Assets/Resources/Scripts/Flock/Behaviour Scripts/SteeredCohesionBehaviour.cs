using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Behaviour/Steered Cohesion")]
public class SteeredCohesionBehaviour : FlockBehaviour
{
    private Vector3 currentVelocity;
    public float agentSmoothTime = 0.5f;
    
    public override Vector3 CalculateMove(FlockAgent agent, List<Transform> context, Flock flock)
    {
        //no neighbors - nothing
        if (context.Count == 0) return Vector3.zero;
        
        //add all points and average
        Vector3 cohesionMove = Vector3.zero;
        foreach (Transform item in context)
        {
            cohesionMove += item.position;
        }

        cohesionMove /= context.Count;

        //create offset from agent position
        cohesionMove -= agent.transform.position;
        cohesionMove = Vector3.SmoothDamp(agent.transform.forward, cohesionMove, ref currentVelocity, agentSmoothTime);
        
        return cohesionMove;
    }
}
