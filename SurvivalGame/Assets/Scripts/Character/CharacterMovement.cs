using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
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
        if (Input.GetKeyDown(KeyCode.R))
        {
            Application.LoadLevel(Application.loadedLevel);
        }

        /*if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Break();
        }*/

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

        float vertInput = Input.GetAxis("Vertical") * movementSpeed * Time.deltaTime;
        float horizInput = Input.GetAxis("Horizontal") * movementSpeed * Time.deltaTime;

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
            charController.Move(Vector3.up * jumpForce * jumpMultiplier * Time.deltaTime);
            timeInAir += Time.deltaTime;

            yield return null;
        } while (!charController.isGrounded && charController.collisionFlags != CollisionFlags.Above);

        charController.slopeLimit = 45.0f;
        isJumping = false;
    }
}