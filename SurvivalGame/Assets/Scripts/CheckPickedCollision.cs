using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPickedCollision : MonoBehaviour
{
    public bool collided;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        collided = true;
    }

    private void OnTriggerExit(Collider other)
    {
        collided = false;
    }
}
