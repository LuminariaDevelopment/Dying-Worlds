using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    public float sensitivityX = 100f;
    public float sensitivityY = 100f;

    public float normalFOV = 90f;
    public float sprintFOV = 110f;

    public Transform orientation;

    float xRotation = 0f;
    float yRotation = 0f;

    private const float MinXRotation = -90f;
    private const float MaxXRotation = 90f;

    private PlayerMovement playerMovement;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        playerMovement = GetComponentInParent<PlayerMovement>();
    }

    private void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivityX * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivityY * Time.deltaTime;

        yRotation += mouseX;
        xRotation -= mouseY;

        xRotation = Mathf.Clamp(xRotation, MinXRotation, MaxXRotation);

        // Rotate Cam and Orientation
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);

        // Adjust FOV based on player speed
        if (playerMovement != null)
        {
            Debug.Log("Player Speed: " + playerMovement.MoveSpeed); // Debug line

            float targetFOV = playerMovement.MoveSpeed > 9f ? sprintFOV : normalFOV;
            float newFOV = Mathf.Lerp(Camera.main.fieldOfView, targetFOV, 0.2f);

            Debug.Log("Target FOV: " + targetFOV + ", Current FOV: " + Camera.main.fieldOfView); // Debug line

            Camera.main.fieldOfView = newFOV;

            Debug.Log("New FOV: " + newFOV); // Debug line
        }
    }
}
