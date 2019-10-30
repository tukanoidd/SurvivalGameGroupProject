using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnappingPoint : MonoBehaviour
{
    public bool isAvailable = true;
    public GameObject parent;
    public Joint joint;
    public Rigidbody rigidbody;

    // Start is called before the first frame update
    void Start()
    {
        parent = transform.parent.gameObject;
        joint = GetComponent<Joint>();
        rigidbody = GetComponent<Rigidbody>();
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
                    
                    if (transform.parent.GetComponent<Combustable>().isPicked || transform.parent.GetComponent<Combustable>().isConnected)
                    {
                        other.transform.parent.position -= other.transform.position - transform.position;
                        
                        joint.connectedAnchor = otherSnappingPoint.transform.position;
                        joint.connectedBody = otherSnappingPoint.transform.parent.GetComponent<Rigidbody>();

                        transform.parent.GetComponent<Craftable>().isSnappingPointParent = true;
                        
                        isAvailable = false;
                        other.GetComponent<SnappingPoint>().isAvailable = false;

                        transform.parent.GetComponent<Combustable>().isConnected = true;
                        other.transform.parent.GetComponent<Combustable>().isConnected = true;
                    }
                }
            }
        }
    }
}