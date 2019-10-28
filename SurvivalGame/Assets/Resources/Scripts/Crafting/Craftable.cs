using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Craftable : MonoBehaviour
{
    public List<GameObject> snappingPoints;
    public bool isSnappingPointParent;

    // Start is called before the first frame update
    void Start()
    {
        snappingPoints = GetComponentsInChildren<SnappingPoint>().Select(snp => snp.gameObject).ToList();
        isSnappingPointParent = false;
    }
}