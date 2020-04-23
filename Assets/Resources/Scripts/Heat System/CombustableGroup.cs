using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombustableGroup : MonoBehaviour
{
    public int groupSize = 2;
    public GameObject groupObject;
    public List<GameObject> group;
    public float spawnRadius;
    public bool setTemp = true;
    public float setTempValue = 0f;
    public bool showGroupSumTemperature = true;
    public float groupTempSum;

    private Vector3[] positions;

    // Start is called before the first frame update
    void Start()
    {
        group = new List<GameObject>();
        initialize();
    }

    // Update is called once per frame
    void Update()
    {
        if (showGroupSumTemperature)
        {
            groupTempSum = 0;

            for (int i = 0; i < groupSize; i++)
            {
                groupTempSum += group[i].GetComponent<Combustable>().temperature;
            }
        }
    }

    void initialize()
    {
        if (spawnRadius == null)
        {
            var objSize = groupObject.GetComponent<Renderer>().bounds.size;
            spawnRadius = (getLargestSide(objSize) * groupSize);
        }

        for (int i = 0; i < groupSize; i++)
        {
            Vector3 random = transform.position + Random.insideUnitSphere * spawnRadius;

            var obj = Instantiate(groupObject, random, Quaternion.identity);
            obj.transform.parent = gameObject.transform;

            if (setTemp)
            {
                obj.GetComponent<Combustable>().temperature = setTempValue;
            }

            group.Add(obj);
        }
    }

    float getLargestSide(Vector3 sizeVector)
    {
        var max = -1f;

        for (int i = 0; i < 3; i++)
        {
            if (sizeVector[i] > max)
            {
                max = sizeVector[i];
            }
        }

        return max;
    }
}