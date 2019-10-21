using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class ObjectPlacer : MonoBehaviour
{
    private Dictionary<Vector3, GameObject> occupiedVertices;
    TextureData.Layer[] terrainLayers;
    private int skipIncrement = 250;
    private bool hasSpawned = false;

    void Start()
    {
        if (transform.gameObject.name == "Terrain Chunk")
        {
            occupiedVertices = new Dictionary<Vector3, GameObject>();
            terrainLayers = transform.parent.GetComponent<TerrainGenerator>().textureSettings.layers;
        }
    }

    public void PlaceObjects(Vector3[] vertices)
    {
        if (vertices != null)
        {
            if (occupiedVertices != null && terrainLayers != null)
            {
                if (occupiedVertices.Count == 0 && !hasSpawned)
                {
                    for (int i = terrainLayers.Length - 1; i >= 0; i--)
                    {
                        for (int j = 0; j < vertices.Length; j += skipIncrement)
                        {
                            skipIncrement = Random.Range(100, 500);
                            if (j + skipIncrement < vertices.Length)
                            {
                                if (terrainLayers[i].startHeight <= vertices[j].y)
                                {
                                    TextureData.SpawnableObject obj =
                                        terrainLayers[i].objects[Random.Range(0, terrainLayers[i].objects.Length)];
                                    if (obj.prefabs.Length > 0)
                                    {
                                        if (Random.Range(0f, 1f) <= obj.chance)
                                        {
                                            if (!occupiedVertices.ContainsKey(vertices[j]))
                                            {
                                                var prefab = obj.prefabs[Random.Range(0, obj.prefabs.Length)];
                                                Vector3 euler = transform.eulerAngles;
                                                euler.y = Random.Range(0f, 360f);

                                                var spawned = Instantiate(prefab, vertices[j] + transform.position,
                                                    Quaternion.identity,
                                                    transform);

                                                spawned.transform.eulerAngles = euler;

                                                occupiedVertices.Add(vertices[j], prefab);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            hasSpawned = true;
        }
    }

    public void PlaceMinimapSphere(Transform parent, float scale)
    {
        var minimapSpherePrefab = Resources.Load<GameObject>("Prefabs/MinimapSphere");

        var minimapSphere =
            Instantiate(minimapSpherePrefab, parent);
        minimapSphere.layer = 11;

        minimapSphere.transform.localScale *= scale;

        if (parent.GetComponent<Creature>() != null)
        {
            switch (parent.GetComponent<Creature>().name)
            {
                case "BoomBug":
                    minimapSphere.GetComponent<Renderer>().material.color =
                        Color.red;
                    break;
                case "FireFly":
                    minimapSphere.GetComponent<Renderer>().material.color =
                        Color.blue;
                    break;
            }
        }
        else if (parent.GetComponent<Human>() != null)
        {
            minimapSphere.GetComponent<Renderer>().material.color = Color.green;
        }
    }
}