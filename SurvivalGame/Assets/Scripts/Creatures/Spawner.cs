using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
                var obj = Instantiate(spawnObject, transform.position, Quaternion.identity);
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
        var intensity = sun.GetComponent<Light>().intensity;
        var maxIntensity = sun.GetComponent<AutoIntensity>().maxIntensity;
        var minInensity = sun.GetComponent<AutoIntensity>().minIntensity;

        if (startAtNight && (counter < spawnAmount && intensity < (maxIntensity - maxIntensity / 4)))
        {
            var obj = Instantiate(spawnObject, transform.position, Quaternion.identity);
            spawnGroup.Add(obj);
            counter++;
        }
        else if (!startAtNight && intensity > (minInensity + minInensity / 2))
        {
            var obj = Instantiate(spawnObject, transform.position, Quaternion.identity);
            spawnGroup.Add(obj);
            counter++;
        }
    }
}