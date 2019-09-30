using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPlacer : MonoBehaviour
{
    private Dictionary<Vector3, GameObject> occupiedVertices;
    TextureData.Layer[] terrainLayers;
    private int skipIncrement = 50;

    void Start()
    {
        occupiedVertices = new Dictionary<Vector3, GameObject>();
        terrainLayers = transform.parent.GetComponent<TerrainGenerator>().textureSettings.layers;
    }

    public void PlaceObjects(Vector3[] vertices)
    {
        if (vertices != null && occupiedVertices.Count == 0)
        {
            for (int i = terrainLayers.Length - 1; i >= 0; i--)
            {
                for (int j = 0; j < vertices.Length; j += skipIncrement)
                {
                    skipIncrement = Random.Range(50, 100);
                    if (j + skipIncrement < vertices.Length)
                    {
                        if (terrainLayers[i].startHeight >= vertices[j].y)
                        {
                            TextureData.SpawnableObject obj =
                                terrainLayers[i].objects[Random.Range(0, 3)];
                            if (obj.prefabs.Length > 0)
                            {
                                if (Random.Range(0, 1) <= obj.chance)
                                {
                                    try
                                    {
                                        var prefab = obj.prefabs[Random.Range(0, obj.prefabs.Length-1)];
                                        Vector3 euler = transform.eulerAngles;
                                        euler.y = Random.Range(0f, 360f);

                                        var spawned = Instantiate(prefab, vertices[j] + transform.position, Quaternion.identity,
                                            transform);

                                        spawned.transform.eulerAngles = euler;
                                        
                                        occupiedVertices.Add(vertices[j], prefab);
                                    }
                                    catch
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        else
        {
            /*foreach (var entry in occupiedVertices)
            {
                Instantiate(entry.Value, entry.Key, Quaternion.identity, transform);
            }*/
        }
    }
}