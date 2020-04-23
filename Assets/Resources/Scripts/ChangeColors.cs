using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeColors : MonoBehaviour
{
    [SerializeField] private GameObject plane;
    [SerializeField] private GameObject cube;

    // Start is called before the first frame update
    void Start()
    {
        plane.GetComponent<Renderer>().material.color = Color.red;
        cube.GetComponent<Renderer>().material.color = Color.green;
    }

    // Update is called once per frame
    void Update()
    {
    }
}