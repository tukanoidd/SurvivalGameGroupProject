using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Spawner : MonoBehaviour
{
    public GameObject spawnObject;
    private GameObject sun;
    public List<GameObject> spawnGroup;
    public int spawnAmount;
    public int spawnInterval;
    public int counter;
    public bool onStart;
    public bool startAtNight;

    // Start is called before the first frame update
    void Start()
    {
        sun = GameObject.Find("Sun");
        spawnGroup = new List<GameObject>();

        if (onStart)
        {
            for (int i = 0; i < spawnAmount; i++)
            {
                Vector3 euler = transform.eulerAngles;
                euler.y = Random.Range(0f, 360f);
                
                var obj = Instantiate(spawnObject, transform.position + Vector3.up * 5f, Quaternion.identity);
                obj.transform.eulerAngles = euler;
                
                spawnGroup.Add(obj);
            }
        }
        else
        {
            InvokeRepeating("spawn", spawnInterval, spawnInterval);
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    void spawn()
    {
        var dot = sun.GetComponent<AutoIntensity>().dot;

        if (startAtNight && (counter < spawnAmount && dot <= 0))
        {
            var obj = Instantiate(spawnObject, transform.position, Quaternion.identity);
            spawnGroup.Add(obj);
            counter++;
        }
        else if (!startAtNight && dot > 0)
        {
            var obj = Instantiate(spawnObject, transform.position, Quaternion.identity);
            spawnGroup.Add(obj);
            counter++;
        }
    }
}