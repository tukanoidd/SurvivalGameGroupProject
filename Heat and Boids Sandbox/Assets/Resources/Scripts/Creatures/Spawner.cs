using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject spawnObject;
    public List<GameObject> spawnGroup;
    public int spawnAmount;
    public int spawnInterval;
    public int counter;
    public bool onStart;

    // Start is called before the first frame update
    void Start()
    {
        spawnGroup = new List<GameObject>();

        if(onStart)
        {
            for(int i = 0; i < spawnAmount; i++)
            {
                var obj = Instantiate(spawnObject, transform.position, Quaternion.identity);
                spawnGroup.Add(obj);
            }
        } else {
            InvokeRepeating("spawn",spawnInterval,spawnInterval);
        }
    }

    // Update is called once per frame
    void Update()
    {
    
    }

    void spawn()
    {
        if(counter < spawnAmount)
        {
            var obj = Instantiate(spawnObject, transform.position, Quaternion.identity);
            spawnGroup.Add(obj);
            counter++;
        }
    }
}
