using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class BoidsSpawner : MonoBehaviour
{
    public enum GizmoType
    {
        Never,
        SelectedOnly,
        Always
    };

    public Boid prefab;
    public float spawnRadius = 10;
    public int spawnCount = 10;
    public Color color;
    public GizmoType showSpawnRegion;

    private void Awake()
    {
        for (int i = 0; i < spawnCount; i++)
        {
            Vector3 pos = transform.position + Random.insideUnitSphere * spawnRadius;
            Boid boid = Instantiate(prefab);
            boid.transform.position = pos;
            boid.transform.forward = Random.insideUnitSphere;

            boid.SetColor(color);
        }
    }

    void DrawGizmos()
    {
        Gizmos.color = new Color(color.r, color.g, color.b, 0.3f);
        Gizmos.DrawSphere(transform.position, spawnRadius);
    }

    private void OnDrawGizmos()
    {
        if (showSpawnRegion == GizmoType.Always)
        {
            DrawGizmos();
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (showSpawnRegion == GizmoType.SelectedOnly)
        {
            DrawGizmos();
        }
    }
}