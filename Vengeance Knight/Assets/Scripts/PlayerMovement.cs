using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;

    private PlayerControls controls;
    private Vector2 moveInput;
    private Animator animator;
    private Rigidbody rb;
    private PlayerCombat combat; // 🔥 ADD

    void Awake()
    {
        controls = new PlayerControls();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        combat = GetComponent<PlayerCombat>(); // 🔥 ADD
    }

    void OnEnable()
    {
        controls.Player.Enable();

        controls.Player.Move.performed += ctx =>
            moveInput = ctx.ReadValue<Vector2>();

        controls.Player.Move.canceled += ctx =>
            moveInput = Vector2.zero;
    }

    void OnDisable()
    {
        controls.Player.Disable();
    }

    void FixedUpdate()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        // 🔥 LOCK MOVEMENT DURING ATTACK (EXISTING)
        if (stateInfo.IsName("Attack") && stateInfo.normalizedTime < 0.75f)
        {
            rb.linearVelocity = new Vector3(
                0,
                rb.linearVelocity.y,
                0
            );

            animator.SetFloat("Speed", 0f);

            return;
        }

        // 🔥 NEW: LOCK MOVEMENT WHILE BLOCKING
        if (combat != null && combat.IsBlocking)
        {
            rb.linearVelocity = new Vector3(
                0,
                rb.linearVelocity.y,
                0
            );

            animator.SetFloat("Speed", 0f);

            return;
        }

        // Movement direction
        Vector3 move = new Vector3(
            -moveInput.y,
            0,
            moveInput.x
        );

        // Smooth movement
        Vector3 targetVelocity = move * speed;

        rb.linearVelocity = Vector3.Lerp(
            rb.linearVelocity,
            new Vector3(
                targetVelocity.x,
                rb.linearVelocity.y,
                targetVelocity.z
            ),
            10f * Time.fixedDeltaTime
        );

        // Better rotation using actual velocity
        Vector3 horizontalVelocity = new Vector3(
            rb.linearVelocity.x,
            0,
            rb.linearVelocity.z
        );

        if (horizontalVelocity.magnitude > 0.2f)
        {
            transform.forward = horizontalVelocity.normalized;
        }

        // Animation
        float speedValue = move.magnitude;

        if (speedValue < 0.1f)
        {
            speedValue = 0f;
        }

        animator.SetFloat("Speed", speedValue);
    }
}