using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetSunLight : MonoBehaviour
{
    public Transform stars;

    private Material sky;
    private bool lighton = false;

    private void Start()
    {
        sky = RenderSettings.skybox;
    }

    private void Update()
    {
        stars.transform.rotation = transform.rotation;

        if (Input.GetKeyDown(KeyCode.T))
        {
            lighton = !lighton;
        }

        Color final = Color.white * Mathf.LinearToGammaSpace(lighton ? 5 : 0);
    }
}