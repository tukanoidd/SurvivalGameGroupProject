﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    public GameObject stars;
    public GameObject minimapCamera;

    private float movementSpeed;

    [SerializeField] private float normalSpeed;
    [SerializeField] private float boostedSpeed;

    private CharacterController charController;

    [SerializeField] private AnimationCurve jumpFallOff;
    [SerializeField] private float jumpMultiplier;
    [SerializeField] private KeyCode jumpKey;

    private bool isJumping = false;

    private void Awake()
    {
        charController = GetComponent<CharacterController>();
    }

    private void Start()
    {
        movementSpeed = normalSpeed;
    }

    private void Update()
    {
        if (stars != null)
        {
            stars.transform.position = transform.position;
        }
        else if (minimapCamera != null)
        {
            minimapCamera.transform.position = new Vector3(transform.position.x, minimapCamera.transform.position.y,
                transform.position.z);
        }

        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            GetComponentInChildren<MenuManager>().TogglePause();
        }

        PlayerMovement();
    }

    private void PlayerMovement()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            movementSpeed = boostedSpeed;
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            movementSpeed = normalSpeed;
        }

        float vertInput = Input.GetAxis("Vertical") * movementSpeed * Time.fixedDeltaTime;
        float horizInput = Input.GetAxis("Horizontal") * movementSpeed * Time.fixedDeltaTime;

        Vector3 forwardMovement = transform.forward * vertInput;
        Vector3 rightMovement = transform.right * horizInput;

        charController.SimpleMove(forwardMovement + rightMovement);

        JumpInput();
    }

    private void JumpInput()
    {
        if (Input.GetKeyDown(jumpKey) && !isJumping)
        {
            isJumping = true;
            StartCoroutine(JumpEvent());
        }
    }

    private IEnumerator JumpEvent()
    {
        charController.slopeLimit = 90.0f;
        float timeInAir = 0.0f;

        do
        {
            float jumpForce = jumpFallOff.Evaluate(timeInAir);
            charController.Move(Vector3.up * jumpForce * jumpMultiplier * Time.fixedDeltaTime);
            timeInAir += Time.fixedDeltaTime;

            yield return null;
        } while (!charController.isGrounded && charController.collisionFlags != CollisionFlags.Above);

        charController.slopeLimit = 45.0f;
        isJumping = false;
    }
}