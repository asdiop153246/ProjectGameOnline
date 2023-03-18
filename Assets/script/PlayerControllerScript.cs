using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerControllerScript : NetworkBehaviour
{
    [SerializeField] private LayerMask groundMask;
    public float speed = 5.0f;
    public float rotationSpeed = 10.0f;
    public Transform orientation;
    private Camera mainCamera;
    private Animator animator;
    private Rigidbody rb;
    private bool running;
    private Vector3 movementDirection;
    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        running = false;
        mainCamera = Camera.main;
    }

    void moveForward()
    {
        float verticalInput = Input.GetAxis("Vertical");
        float horizontalInput = Input.GetAxis("Horizontal");
        movementDirection = transform.forward * verticalInput + transform.right * horizontalInput;

        if (movementDirection.magnitude > 1f)
        {
            movementDirection.Normalize();
        }
        rb.MovePosition(transform.position + movementDirection * speed * Time.fixedDeltaTime);

        if (!running)
        {
            running = true;

        }

        else if (running)
        {
            running = false;

        }
    }

    void FaceMousePosition()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = mainCamera.transform.position.y - transform.position.y;
        Vector3 worldMousePosition = mainCamera.ScreenToWorldPoint(mousePosition);
        Vector3 direction = worldMousePosition - transform.position;
        direction.y = 0f; // Keep the direction only in the x and z axes

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        }

    }

    private void FixedUpdate()
    {
        if (!IsOwner) return;
        moveForward();
        FaceMousePosition();
    }
}

