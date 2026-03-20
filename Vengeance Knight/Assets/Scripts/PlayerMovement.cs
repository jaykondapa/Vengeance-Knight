using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;

    private PlayerControls controls;
    private Vector2 moveInput;
    private Animator animator;
    private Rigidbody rb;

    void Awake()
    {
        controls = new PlayerControls();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    void OnEnable()
    {
        controls.Player.Enable();
        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;
    }

    void OnDisable()
    {
        controls.Player.Disable();
    }

    void FixedUpdate()
{
    // Movement direction
    Vector3 move = new Vector3(-moveInput.y, 0, moveInput.x);

    // Preserve vertical velocity (VERY IMPORTANT)
    Vector3 velocity = rb.linearVelocity;
    Vector3 targetVelocity = move * speed;

    rb.linearVelocity = new Vector3(
        targetVelocity.x,
        velocity.y,
        targetVelocity.z
    );

    // Rotation
    if (move != Vector3.zero)
    {
        transform.forward = move;
    }

    // Animation
    if (animator != null)
    {
        animator.SetFloat("Speed", move.magnitude);
    }
}
}