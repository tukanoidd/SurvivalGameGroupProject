using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassShaderManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Renderer>().sharedMaterial.SetFloat("_Height", GetComponent<Renderer>().bounds.size.y);
    }

    // Update is called once per frame
    void Update()
    {
    }
}