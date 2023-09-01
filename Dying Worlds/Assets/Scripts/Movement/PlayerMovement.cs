using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    private float moveSpeed;
    public float walkSpeed;
    public float sprintSpeed;

    private float desiredMoveSpeed;
    private float lastDesiredMoveSpeed;

    public float speedIncreaseMultiplier;
    public float slopeIncreaseMultiplier;

    public float sprintSpeedIncreaseMultiplier;
    public float sprintSlopeIncreaseMultiplier;

    [Header("Headbobbing")]
    public float headbobSpeed;
    public float headbobAmount;

    private Vector3 originalCameraPosition;
    private float headbobTimer;
    private Vector3 originalCamPosition;


    [Header("Slide")]
    public float slideSpeed;
    public float slideSpeedIncreaseMultiplier;
    public float slideSlopeIncreaseMultiplier;

    public float groundDrag;

    [Header("Jumping")]
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;

    [Header("Crouching")]
    public float crouchSpeed;
    public float crouchYScale;
    private float startYScale;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;

    [Header("Slope Handling")]
    public float maxSlopeAngle;
    private RaycastHit slopeHit;
    private bool exitingSlope;


    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    public MovementState state;
    public enum MovementState
    {
        walking,
        sprinting,
        crouching,
        sliding,
        air
    }


    public float MoveSpeed
    {
        get { return moveSpeed; }
    }

    public bool sliding;

   private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        readyToJump = true;

        startYScale = transform.localScale.y;

        // Initialize originalCamPosition with the initial camera position
        originalCamPosition = transform.GetChild(0).localPosition;
    }

    private void Update()
    {
        // ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        MyInput();
        SpeedControl();
        StateHandler();

        // handle drag
        if (grounded)
            rb.drag = groundDrag;
        else
            rb.drag = 0;

        // Headbobbing
        if (grounded && state != MovementState.crouching && state != MovementState.sliding)
        {
            HandleHeadbobbing();
        }
        else
        {
            ResetHeadbob();
        }
    }

    private void HandleHeadbobbing()
    {
        float headbobFrequency = walkSpeed * 0.4f;
        float headbobAmplitude = 0.1f;

        float horizontalMovement = Mathf.Abs(horizontalInput);
        float verticalMovement = Mathf.Abs(verticalInput);

        float headbobFactor = Mathf.Sqrt(horizontalMovement * horizontalMovement + verticalMovement * verticalMovement);

        float horizontalHeadbob = Mathf.Sin(Time.time * headbobFrequency) * headbobAmplitude * headbobFactor;

        float verticalHeadbob;
        if (Input.GetKey(sprintKey))
        {
            verticalHeadbob = Mathf.Cos(Time.time * headbobFrequency * 3) * headbobAmplitude * headbobFactor * 2f; // Beschleunigung der vertikalen Kopfbewegung bei gedrückter Sprint-Taste
        }
        else
        {
            verticalHeadbob = Mathf.Cos(Time.time * headbobFrequency * 2) * headbobAmplitude * headbobFactor;
        }


        float currentHeadbobSpeed = headbobSpeed;

        Vector3 newCamPosition = originalCamPosition + new Vector3(horizontalHeadbob, verticalHeadbob, 0);

        // Apply the new camera position
        transform.GetChild(0).localPosition = newCamPosition;

    }

    private void ResetHeadbob()
    {
        headbobTimer = Mathf.PI / 2;
        orientation.localPosition = Vector3.Lerp(orientation.localPosition, originalCameraPosition, Time.deltaTime * 2f);
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // when to jump
        if (Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }

        // start crouch
        if (Input.GetKeyDown(crouchKey))
        {
            // Ändere nur die Skalierung des Spielerobjekts
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }

        // stop crouch
        if (Input.GetKeyUp(crouchKey))
        {
            // Setze die Skalierung des Spielerobjekts zurück
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
        }
    }

   private void StateHandler()
    {
        // Mode - Sliding
        if (sliding)
        {
            state = MovementState.sliding;

            if (OnSlope() && rb.velocity.y < 0.1f)
            {
                desiredMoveSpeed = slideSpeed;
                speedIncreaseMultiplier = slideSpeedIncreaseMultiplier;
                slopeIncreaseMultiplier = slideSlopeIncreaseMultiplier;
            }
            else
            {
                desiredMoveSpeed = slideSpeed; // Du kannst den gewünschten Slide-Speed anpassen
                speedIncreaseMultiplier = 1f; // Keine zusätzliche Multiplikation im Slide-Modus
                slopeIncreaseMultiplier = 1f; // Keine zusätzliche Multiplikation im Slide-Modus
            }
        }

        // Mode - Crouching
        else if (Input.GetKey(crouchKey))
        {
            state = MovementState.crouching;
            desiredMoveSpeed = crouchSpeed;
            speedIncreaseMultiplier = 1f;
            slopeIncreaseMultiplier = 1f;
        }

        // Mode - Sprinting
        else if (grounded && Input.GetKey(sprintKey))
        {
            state = MovementState.sprinting;
            desiredMoveSpeed = sprintSpeed;
            speedIncreaseMultiplier = sprintSpeedIncreaseMultiplier;
            slopeIncreaseMultiplier = sprintSlopeIncreaseMultiplier;
        }

        // Mode - Walking
        else if (grounded)
        {
            state = MovementState.walking;
            desiredMoveSpeed = walkSpeed;
        }

        // Mode - Air
        else
        {
            state = MovementState.air;
        }

        // check if desiredMoveSpeed has changed drastically
        if (Mathf.Abs(desiredMoveSpeed - lastDesiredMoveSpeed) > 4f && moveSpeed != 0)
        {
            StopAllCoroutines();
            StartCoroutine(SmoothlyLerpMoveSpeed());
        }
        else
        {
            moveSpeed = desiredMoveSpeed;
        }

        lastDesiredMoveSpeed = desiredMoveSpeed;
    }

    private IEnumerator SmoothlyLerpMoveSpeed()
    {
        // smoothly lerp movementSpeed to desired value
        float time = 0;
        float difference = Mathf.Abs(desiredMoveSpeed - moveSpeed);
        float startValue = moveSpeed;

        while (time < difference)
        {
            moveSpeed = Mathf.Lerp(startValue, desiredMoveSpeed, time / difference);

            if (OnSlope())
            {
                float slopeAngle = Vector3.Angle(Vector3.up, slopeHit.normal);
                float slopeAngleIncrease = 1 + (slopeAngle / 90f);

                time += Time.deltaTime * speedIncreaseMultiplier * slopeIncreaseMultiplier * slopeAngleIncrease;
            }
            else
                time += Time.deltaTime * speedIncreaseMultiplier;

            yield return null;
        }

        moveSpeed = desiredMoveSpeed;
    }

    private void MovePlayer()
    {
        // calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // on slope
        if (OnSlope() && !exitingSlope)
        {
            rb.AddForce(20f * moveSpeed * GetSlopeMoveDirection(moveDirection), ForceMode.Force);

            if (rb.velocity.y > 0)
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
        }

        // on ground
        else if (grounded)
            rb.AddForce(10f * moveSpeed * moveDirection.normalized, ForceMode.Force);

        // in air
        else if (!grounded)
            rb.AddForce(10f * airMultiplier * moveSpeed * moveDirection.normalized, ForceMode.Force);

        // turn gravity off while on slope
        rb.useGravity = !OnSlope();
    }

    private void SpeedControl()
    {
        // limiting speed on slope
        if (OnSlope() && !exitingSlope)
        {
            if (rb.velocity.magnitude > moveSpeed)
                rb.velocity = rb.velocity.normalized * moveSpeed;
        }

        // limiting speed on ground or in air
        else
        {
            Vector3 flatVel = new(rb.velocity.x, 0f, rb.velocity.z);

            // limit velocity if needed
            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }

        }
    }

    private void Jump()
    {
        exitingSlope = true;

        // reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }
    private void ResetJump()
    {
        readyToJump = true;

        exitingSlope = false;
    }

    public bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    public Vector3 GetSlopeMoveDirection(Vector3 direction)
    {
        return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
    }

    internal void DashInDirection(Vector3 normalized, float dashForce)
    {
        throw new NotImplementedException();
    }
}