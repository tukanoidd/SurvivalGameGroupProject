using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidManager : MonoBehaviour
{
    const int threadGroupSize = 1024;

    public BoidSettings settings;
    public ComputeShader compute;
    private Vector3 flockCenter;
    private GameObject flockSphere;
    private GameObject sphereParent;
    private SphereCollider SOI;
    
    Boid[] boids;

    void Start()
    {
        sphereParent = new GameObject("Behaviour Parent");
        sphereParent.AddComponent<Creature>();
        flockCenter = transform.position;
        flockSphere = Instantiate(sphereParent, flockCenter, Quaternion.identity, transform);
        SOI = flockSphere.AddComponent<SphereCollider>();
        boids = FindObjectsOfType<Boid>();
        foreach (Boid b in boids)
        {
            b.Initialize(settings, transform);
        }
    }

    void Update()
    {
        if (boids != null)
        {
            int numBoids = boids.Length;
            var boidData = new BoidData[numBoids];

            for (int i = 0; i < boids.Length; i++)
            {
                boidData[i].position = boids[i].position;
                boidData[i].direction = boids[i].forward;
            }

            var boidBuffer = new ComputeBuffer(numBoids, BoidData.Size);
            boidBuffer.SetData(boidData);

            compute.SetBuffer(0, "boids", boidBuffer);
            compute.SetInt("numBoids", boids.Length);
            compute.SetFloat("viewRadius", settings.perceptionRadius);
            compute.SetFloat("avoidRadius", settings.avoidanceRadius);

            int threadGroups = Mathf.CeilToInt(numBoids / (float) threadGroupSize);
            compute.Dispatch(0, threadGroups, 1, 1);

            boidBuffer.GetData(boidData);
            
            flockCenter = Vector3.zero;
            
            for (int i = 0; i < boids.Length; i++)
            {
                boids[i].avgFlockHeading = boidData[i].flockHeading;
                boids[i].centerOfFlockmates = boidData[i].flockCentre;
                flockCenter += boids[i].transform.position;
                boids[i].avgAvoidanceHeading = boidData[i].avoidanceHeading;
                boids[i].numPerceivedFlockmates = boidData[i].numFlockmates;
                boids[i].target = sphereParent.GetComponent<Creature>().focalPoint;

                boids[i].UpdateBoid();
            }

            flockCenter /= boids.Length;
            flockSphere.transform.position = flockCenter;
            SOI.isTrigger = true;
            SOI.radius = numBoids / 50;

            boidBuffer.Release();
        }
    }

    public struct BoidData
    {
        public Vector3 position;
        public Vector3 direction;

        public Vector3 flockHeading;
        public Vector3 flockCentre;
        public Vector3 avoidanceHeading;
        public int numFlockmates;

        public static int Size
        {
            get { return sizeof(float) * 3 * 5 + sizeof(int); }
        }
    }
}