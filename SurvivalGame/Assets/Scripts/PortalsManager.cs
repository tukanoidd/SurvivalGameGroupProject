using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalsManager : MonoBehaviour
{
    [SerializeField] private GameObject player;
    
    [Serializable]
    private struct Portal
    {
        public GameObject portalEnter;
        public GameObject portalEnterCam;
        public GameObject portalExit;
        public GameObject portalExitCam;
    }

    [SerializeField] private Portal[] portals;  
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < portals.Length; i++)
        {
            Quaternion newRotation = player.transform.rotation;
            portals[i].portalEnterCam.transform.rotation = newRotation;
            newRotation.y = -newRotation.y;
            portals[i].portalExitCam.transform.rotation = newRotation;
        }
    }
}
