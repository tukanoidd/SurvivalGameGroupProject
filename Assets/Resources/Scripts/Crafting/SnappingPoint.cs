using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnappingPoint : MonoBehaviour
{
    public bool isAvailable = true;
    public GameObject parent;
    public Joint joint;

    // Start is called before the first frame update
    void Start()
    {
        parent = transform.parent.gameObject;
        joint = GetComponent<Joint>();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<SnappingPoint>() != null)
        {
            if (other.gameObject.GetComponent<SnappingPoint>().isAvailable && isAvailable)
            {
                if (Vector3.Distance(other.transform.position, transform.position) < 0.8f)
                {
                    var otherSnappingPoint = other.GetComponent<SnappingPoint>();
                    
                    if (parent.GetComponent<Combustable>().isPicked || parent.GetComponent<Combustable>().isConnected)
                    {
                        other.transform.parent.position -= other.transform.position - transform.position;
                        
                        joint.connectedAnchor = otherSnappingPoint.transform.position;
                        joint.connectedBody = otherSnappingPoint.transform.parent.GetComponent<Rigidbody>();

                        parent.GetComponent<Craftable>().isSnappingPointParent = true;
                        
                        isAvailable = false;
                        other.GetComponent<SnappingPoint>().isAvailable = false;

                        parent.GetComponent<Combustable>().isConnected = true;
                        other.transform.parent.GetComponent<Combustable>().isConnected = true;
                        
                        parent.GetComponent<Craftable>().connectedObjects.Add(new KeyValuePair<GameObject, SnappingPoint>(other.gameObject.GetComponent<SnappingPoint>().parent, other.GetComponent<SnappingPoint>()));
                        other.GetComponent<SnappingPoint>().parent.GetComponent<Craftable>().connectedObjects
                            .Add(new KeyValuePair<GameObject, SnappingPoint>(parent.gameObject, this));
                    }
                }
            }
        }
    }
}