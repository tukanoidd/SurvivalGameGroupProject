using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Behaviour/Avoidence")]
public class AvoidenceBehaviour : FilteredFlockBehaviour
{
    public override Vector3 CalculateMove(FlockAgent agent, List<Transform> context, Flock flock)
    {
        //no neighbors - maintain current alignment
        if (context.Count == 0) return Vector3.zero;
        
        Vector3 avoidenceMove = Vector3.zero;
        int nAvoid = 0;
        List<Transform> filteredContext = (filter == null) ? context : filter.Filter(agent, context);
        foreach (Transform item in filteredContext)
        {
            if (Vector3.SqrMagnitude(item.position - agent.transform.position) < flock.SquareAvoidanceRadius)
            {
                nAvoid++;
                avoidenceMove += agent.transform.position - item.position;
                
            }
        }

        if (nAvoid > 0) avoidenceMove /= nAvoid;
        
        return avoidenceMove;
    }
}
