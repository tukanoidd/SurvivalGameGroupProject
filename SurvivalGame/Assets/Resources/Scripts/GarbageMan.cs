using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditorInternal;
#endif

public class GarbageMan : MonoBehaviour
{
    public static List<GameObject> ashTray;
    public int maximumAshParticles = 100;
    public static List<GameObject> creatures;
    public int creatureCap = 50;
    public string[] tags;
    public int ashParticlesInScene;
    public List<GameObject> gameObjectsInScene; 
    public int numberOfGameObjectsInScene;
    public int numberOfCreaturesInScene;

    // Start is called before the first frame update
    private void Awake()
    {
        gameObjectsInScene = new List<GameObject>();
        ashTray = new List<GameObject>();
        creatures = new List<GameObject>();
    }

    void Start()
    {
        #if UNITY_EDITOR
        tags = UnityEditorInternal.InternalEditorUtility.tags;
        #endif
    }

    // Update is called once per frame
    void Update()
    {
        numberOfGameObjectsInScene = 0;
        gameObjectsInScene.Clear();

        #if UNITY_EDITOR
        for (int i = 0; i < UnityEditorInternal.InternalEditorUtility.tags.Length; i++)
        {
            numberOfGameObjectsInScene += GameObject.FindGameObjectsWithTag(tags[i]).Length;
        }
        #endif
        
        if (ashTray.Count >= maximumAshParticles)
        {
            Destroy(ashTray[0]);
            ashTray.RemoveAt(0);
            Debug.Log("GarbageMan: Ashing...");
        }

        ashParticlesInScene = ashTray.Count;

        if (creatures.Count >= creatureCap)
        {
            Destroy(creatures[0]);
            creatures.RemoveAt(0);
        }

        numberOfCreaturesInScene = creatures.Count;
    }
}
