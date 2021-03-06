﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCamera : MonoBehaviour
{
    [SerializeField] private float mouseSensitivity;

    [SerializeField] private Camera _camera;

    [SerializeField] private List<GameObject> campfireTutorialTexts = null;

    private float xAxisClamp;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        xAxisClamp = 0.0f;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xAxisClamp += mouseY;

        if (xAxisClamp > 90.0f)
        {
            xAxisClamp = 90.0f;
            mouseY = 0.0f;
            ClampXAxisRotationToValue(278.0f);
        }
        else if (xAxisClamp < -90.0f)
        {
            xAxisClamp = -90.0f;
            mouseY = 0.0f;
            ClampXAxisRotationToValue(90.0f);
        }

        _camera.transform.Rotate(Vector3.left * mouseY);
        transform.Rotate(Vector3.up * mouseX);
        
        if (campfireTutorialTexts.Count > 0)
        {
            for (int i = 0; i < campfireTutorialTexts.Count; i++)
            {
                campfireTutorialTexts[i].transform.rotation = _camera.transform.rotation;
            }   
        }
    }

    private void ClampXAxisRotationToValue(float value)
    {
        Vector3 eulerRotation = _camera.transform.eulerAngles;
        eulerRotation.x = value;
        _camera.transform.eulerAngles = eulerRotation;
    }
}