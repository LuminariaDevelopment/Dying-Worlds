using TMPro;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] Transform orientation;
    [SerializeField] Transform playerObj;
    [SerializeField] Transform cameraPos;

    [Header("Movement")]
    [SerializeField] float groundCheckDistance;
    [SerializeField] float walk;
    [SerializeField] float sprint;
    [SerializeField] float jumpforce;
    [SerializeField] float groundDrag;
    [SerializeField] LayerMask ground;
    Rigidbody rb;
    Vector3 inputDir;

    bool sprinting;
    bool grounded;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance , ground);
        float inputx = Input.GetAxisRaw("Horizontal");
        float inputy = Input.GetAxisRaw("Vertical");
        inputDir = orientation.forward * inputy + orientation.right * inputx;
        if (inputDir != Vector3.zero)
        {
            float speed;
            if (sprinting && grounded)
            {
                speed = sprint;
            }
            else
            {
                speed = walk;
            }
            rb.AddForce(inputDir.normalized * speed * 10f, ForceMode.Force);
            rb.drag = groundDrag;
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            sprinting = true;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            sprinting = false;
        }
        if (Input.GetKeyDown(KeyCode.Space) && grounded)
        {
            rb.AddForce(Vector3.up * jumpforce * 10f, ForceMode.Impulse);
        }
    }
}