using UnityEngine;

/// <summary>
/// Handles all player combat: attacking, blocking, parrying, spear pickup/throw.
/// Attach this to the Player GameObject alongside PlayerMovement.cs
/// </summary>
public class PlayerCombat : MonoBehaviour
{
    [Header("Attack Settings")]
    public float attackDamage = 25f;
    public float attackRange = 1.5f;
    public float attackCooldown = 0.6f;
    public LayerMask enemyLayer;

    [Header("Parry Settings")]
    public float parryWindow = 0.3f;
    public float blockCooldown = 0.2f;
    public float parryCounterDamage = 50f;

    [Header("Spear Settings")]
    public float spearThrowForce = 20f;
    public GameObject spearPrefab;
    public Transform spearThrowPoint;

    [Header("Audio")]
    public AudioClip attackSound;
    public AudioClip blockSound;
    public AudioClip parrySound;
    public AudioClip spearThrowSound;

    // Components
    private Animator animator;
    private AudioSource audioSource;
    private PlayerHealth playerHealth;

    // Timers
    private float attackTimer = 0f;
    private float blockTimer = 0f;

    // States
    private bool isBlocking = false;
    private bool isParrying = false;
    private bool hasSpear = false;
    private bool isAttacking = false;

    // Animation parameter names
    private const string ANIM_ATTACK = "Attack";
    private const string ANIM_BLOCK = "IsBlocking";
    private const string ANIM_PARRY = "Parry";
    private const string ANIM_THROW = "ThrowSpear";
    private const string ANIM_HAS_SPEAR = "HasSpear";

    void Awake()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        playerHealth = GetComponent<PlayerHealth>();
    }

    void Update()
    {
        // Cooldowns
        if (attackTimer > 0f)
            attackTimer -= Time.deltaTime;

        if (blockTimer > 0f)
            blockTimer -= Time.deltaTime;

        HandleAttack();
        HandleBlock();
        HandleSpear();
    }

    // =========================================================
    // ATTACK
    // =========================================================

    void HandleAttack()
    {
        if (Input.GetMouseButtonDown(0) && attackTimer <= 0f && !isBlocking)
        {
            attackTimer = attackCooldown;
            isAttacking = true;

            animator?.SetTrigger(ANIM_ATTACK);
            PlaySound(attackSound);

            // Unlock movement slightly before cooldown ends
            Invoke(nameof(EndAttack), 0.35f);

            // Detect enemies
            Collider[] hits = Physics.OverlapSphere(
                transform.position + transform.forward * attackRange * 0.5f,
                attackRange,
                enemyLayer
            );

            foreach (Collider hit in hits)
            {
                EnemyHealth enemy = hit.GetComponent<EnemyHealth>();

                if (enemy != null)
                {
                    enemy.TakeDamage(attackDamage);
                }
            }
        }
    }

    void EndAttack()
    {
        isAttacking = false;
    }

    // =========================================================
    // BLOCK & PARRY
    // =========================================================

    void HandleBlock()
    {
        // Start block
        if (Input.GetKeyDown(KeyCode.Q) && blockTimer <= 0f)
        {
            isBlocking = true;
            isParrying = true;

            animator?.SetBool(ANIM_BLOCK, true);
            PlaySound(blockSound);

            Invoke(nameof(EndParryWindow), parryWindow);
        }

        // Stop block
        if (Input.GetKeyUp(KeyCode.Q))
        {
            isBlocking = false;
            isParrying = false;

            blockTimer = blockCooldown;

            animator?.SetBool(ANIM_BLOCK, false);

            CancelInvoke(nameof(EndParryWindow));
        }
    }

    void EndParryWindow()
    {
        isParrying = false;
    }

    // =========================================================
    // BLOCK CHECK
    // =========================================================

    /// <summary>
    /// Returns true if damage was blocked/parried.
    /// </summary>
    public bool TryBlock(float incomingDamage, bool isRangedAttack, Transform attacker)
    {
        if (!isBlocking)
            return false;

        // PERFECT PARRY
        if (isParrying && !isRangedAttack)
        {
            animator?.SetTrigger(ANIM_PARRY);
            PlaySound(parrySound);

            EnemyHealth enemy = attacker?.GetComponent<EnemyHealth>();

            if (enemy != null)
            {
                enemy.TakeDamage(parryCounterDamage);
            }

            return true;
        }

        // RANGED BLOCK
        if (isRangedAttack)
        {
            PlaySound(blockSound);
            return true;
        }

        // NORMAL BLOCK
        float reducedDamage = incomingDamage * 0.3f;

        playerHealth?.TakeDamage(reducedDamage);

        PlaySound(blockSound);

        return true;
    }

    // =========================================================
    // SPEAR
    // =========================================================

    void HandleSpear()
    {
        // Throw spear
        if (hasSpear && Input.GetKeyDown(KeyCode.E))
        {
            ThrowSpear();
        }
    }

    public void PickUpSpear()
    {
        hasSpear = true;

        animator?.SetBool(ANIM_HAS_SPEAR, true);

        Debug.Log("Spear picked up! Press E to throw.");
    }

    void ThrowSpear()
    {
        if (spearPrefab == null || spearThrowPoint == null)
        {
            Debug.LogWarning("PlayerCombat: spearPrefab or spearThrowPoint not assigned!");
            return;
        }

        hasSpear = false;

        animator?.SetBool(ANIM_HAS_SPEAR, false);
        animator?.SetTrigger(ANIM_THROW);

        PlaySound(spearThrowSound);

        // Spawn spear
        GameObject spear = Instantiate(
            spearPrefab,
            spearThrowPoint.position,
            spearThrowPoint.rotation
        );

        Rigidbody rb = spear.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.linearVelocity = transform.forward * spearThrowForce;
        }
    }

    // =========================================================
    // HELPERS
    // =========================================================

    void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    // =========================================================
    // PUBLIC GETTERS
    // =========================================================

    public bool IsBlocking => isBlocking;
    public bool HasSpear => hasSpear;
    public bool IsAttacking => isAttacking;

    // =========================================================
    // DEBUG GIZMOS
    // =========================================================

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(
            transform.position + transform.forward * attackRange * 0.5f,
            attackRange
        );
    }
}