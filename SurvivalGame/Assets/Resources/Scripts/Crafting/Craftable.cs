using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Craftable : Combustable
{
    public List<KeyValuePair<GameObject, SnappingPoint>> connectedObjects;
    public bool isSnappingPointParent;

    private GameObject hammerPrefab;
    private GameObject axePrefab;

    private KeyValuePair<GameObject, SnappingPoint> obj;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        connectedObjects = new List<KeyValuePair<GameObject, SnappingPoint>>();
        isSnappingPointParent = false;
        
        obj = new KeyValuePair<GameObject, SnappingPoint>();

        hammerPrefab = Resources.Load<GameObject>("Prefabs/Tools/Hammer");
        axePrefab = Resources.Load<GameObject>("Prefabs/Tools/Axe");
    }

    protected override void Update()
    {
        base.Update();
        
        if (name == "Stick" && connectedObjects.Count > 0)
        {
            if (CheckForConnection("Stone"))
            {
                var craftableConnectedObj = obj.Key.GetComponent<Craftable>();
                if (craftableConnectedObj.CheckForConnection("Stick"))
                {
                    craftableConnectedObj.UnjoinObjects();
                }
                
                UnjoinObjects();
                Instantiate(hammerPrefab, transform.position, transform.rotation);
            } else if (CheckForConnection("Flint"))
            {
                var craftableConnectedObj = obj.Key.GetComponent<Craftable>();
                if (craftableConnectedObj.CheckForConnection("Stick"))
                {
                    craftableConnectedObj.UnjoinObjects();
                }
                
                UnjoinObjects();
                Instantiate(axePrefab, transform.position, transform.rotation);
            }
        }
    }

    bool CheckForConnection(String name)
    {
        connectedObjects = connectedObjects.Where(connectedObject => connectedObject.Key != null).ToList();
        return connectedObjects.Any((connectedObject) =>
        {
            obj = connectedObject;
            return connectedObject.Key.GetComponent<Combustable>().name == name;
        });
    }
    void UnjoinObjects()
    {
        foreach (var connectedObject in connectedObjects)
        {
            if (!connectedObject.Equals(obj))
            {
                connectedObject.Value.isAvailable = true;
                connectedObject.Value.parent.GetComponent<Craftable>().isSnappingPointParent = false;
                connectedObject.Value.joint.connectedBody = null;

                connectedObject.Key.GetComponent<Craftable>().connectedObjects.Remove(obj);
            }
        }
        
        Destroy(gameObject);
    }
}