using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class BasicMovement2 : MonoBehaviour
{
    public InputAction playerControls;
    private CharacterController characterController;
    [SerializeField] private Rigidbody rb;
    Vector2 moveDirection = Vector3.zero;
    [SerializeField] private float movespeed;
    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        rb.constraints = RigidbodyConstraints.FreezePositionY |
        RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationX;
    }
    private void OnEnable(){
        playerControls.Enable();
    }
    private void OnDisable(){
        playerControls.Disable();
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        moveDirection = playerControls.ReadValue<Vector2>();
        Move();
    }
    void Move(){
        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        characterController.Move(move);
        // rb.velocity = new Vector3(moveDirection.x*movespeed, 0f, moveDirection.y*movespeed).normalized;
    }
}

