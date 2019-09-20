using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCamera : MonoBehaviour
{
    [SerializeField] private float mouseSensitivity;

    [SerializeField] private Camera _camera;

    [SerializeField] private GameObject head;

    private bool lookingAtObject = false;
    private GameObject lookedAtObject;

    private float xAxisClamp;

    [SerializeField] private Texture2D crosshairBlack;
    [SerializeField] private Texture2D crosshairWhite;
    [SerializeField] private float crosshairSize = 30f;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        xAxisClamp = 0.0f;
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    private void OnGUI()
    {
        if (lookingAtObject && lookedAtObject)
        {
            var lookedAtColor = lookedAtObject.GetComponent<Renderer>().material.color;
            var sum = lookedAtColor.r + lookedAtColor.g + lookedAtColor.b;
            var checkerNum = 383f;

            var text = checkerNum >= 383 ? crosshairBlack : crosshairWhite;

            if (checkerNum >= 383)
            {
                GUI.DrawTexture(new Rect(Screen.width/2, Screen.height/2, crosshairSize, crosshairSize), crosshairBlack);    
            }
            else
            {
                GUI.DrawTexture(new Rect(Screen.width/2, Screen.height/2, crosshairSize, crosshairSize), crosshairWhite);
            }
        }
        else
        {
            GUI.DrawTexture(new Rect(Screen.width/2, Screen.height/2, crosshairSize, crosshairSize), crosshairBlack);
        }
    }

    // Update is called once per frame
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
        } else if (xAxisClamp < -90.0f)
        {
            xAxisClamp = -90.0f;
            mouseY = 0.0f;
            ClampXAxisRotationToValue(90.0f);
        }
        
        _camera.transform.Rotate(Vector3.left * mouseY);
        transform.Rotate(Vector3.up * mouseX);

        CheckLookingAt();
    }

    private void ClampXAxisRotationToValue(float value)
    {
        Vector3 eulerRotation = _camera.transform.eulerAngles;
        eulerRotation.x = value;
        _camera.transform.eulerAngles = eulerRotation;
    }

    void CheckLookingAt()
    {
        Ray lookingRay = new Ray(head.transform.position, _camera.transform.forward);
        RaycastHit rayLookingAtObjectHit;

        lookingAtObject = Physics.Raycast(lookingRay, out rayLookingAtObjectHit);

        if (lookingAtObject)
        {
            lookedAtObject = rayLookingAtObjectHit.transform.gameObject;
        }
    }
}
