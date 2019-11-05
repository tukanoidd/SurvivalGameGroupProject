using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Craftable : Combustable
{
    public List<GameObject> snappingPoints;
    public List<GameObject> connectedObjects;
    public bool isSnappingPointParent;

    // Start is called before the first frame update
    void Start()
    {
        snappingPoints = GetComponentsInChildren<SnappingPoint>().Select(snp => snp.gameObject).ToList();
        isSnappingPointParent = false;
    }

    void Update()
    {
        if (name == "Stick" && connectedObjects.Count > 0)
        {
            Debug.Log("STICK");
            if (connectedObjects.Any((connectedObject) => connectedObject.GetComponent<Combustable>().name == "Stone"))
            {
                Debug.Log("Hammer");
            } else if (connectedObjects.Any((connectedObject) => connectedObject.GetComponent<Combustable>().name == "Flint"))
            {
                Debug.Log("Axe");
            }
        }
    }
}