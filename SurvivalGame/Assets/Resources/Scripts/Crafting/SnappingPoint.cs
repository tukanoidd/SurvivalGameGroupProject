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
                    other.transform.parent.position -= other.transform.position - transform.position;
                    
                    var otherSnappingPoint = other.GetComponent<SnappingPoint>();
                
                    /*
                    otherSnappingPoint.joint.connectedAnchor = transform.position;
                    otherSnappingPoint.joint.connectedBody = otherSnappingPoint.transform.parent.GetComponent<Rigidbody>();
                    */
                
                    joint.connectedAnchor = otherSnappingPoint.transform.position;
                    joint.connectedBody = otherSnappingPoint.transform.parent.GetComponent<Rigidbody>();
                
                    isAvailable = false;
                    other.gameObject.GetComponent<SnappingPoint>().isAvailable = false;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
