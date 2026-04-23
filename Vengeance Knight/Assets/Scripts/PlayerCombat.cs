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
    public float parryWindow = 0.3f;       // Time window for a perfect parry
    public float blockCooldown = 0.2f;
    public float parryCounterDamage = 50f; // Bonus damage after successful parry

    [Header("Spear Settings")]
    public float spearThrowForce = 20f;
    public GameObject spearPrefab;         // Assign your spear prefab in Inspector
    public Transform spearThrowPoint;      // Empty GameObject at player's hand

    [Header("Audio")]
    public AudioClip attackSound;
    public AudioClip blockSound;
    public AudioClip parrySound;
    public AudioClip spearThrowSound;

    // Internal state
    private Animator animator;
    private AudioSource audioSource;
    private PlayerHealth playerHealth;

    private float attackTimer = 0f;
    private float blockTimer = 0f;

    private bool isBlocking = false;
    private bool isParrying = false;
    private bool hasSpear = false;

    // Animation parameter names — must match your Animator Controller
    private const string ANIM_ATTACK   = "Attack";
    private const string ANIM_BLOCK    = "IsBlocking";
    private const string ANIM_PARRY    = "Parry";
    private const string ANIM_THROW    = "ThrowSpear";
    private const string ANIM_HAS_SPEAR = "HasSpear";

    void Awake()
    {
        animator     = GetComponent<Animator>();
        audioSource  = GetComponent<AudioSource>();
        playerHealth = GetComponent<PlayerHealth>();
    }

    void Update()
    {
        // Count down cooldown timers
        if (attackTimer > 0f) attackTimer -= Time.deltaTime;
        if (blockTimer  > 0f) blockTimer  -= Time.deltaTime;

        HandleAttack();
        HandleBlock();
        HandleSpear();
    }

    // ─────────────────────────────────────────────
    //  ATTACK  (Left Mouse Button)
    // ─────────────────────────────────────────────
    void HandleAttack()
    {
        if (Input.GetMouseButtonDown(0) && attackTimer <= 0f && !isBlocking)
        {
            attackTimer = attackCooldown;
            animator?.SetTrigger(ANIM_ATTACK);
            PlaySound(attackSound);

            // Detect enemies in attack range in front of player
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

    // ─────────────────────────────────────────────
    //  BLOCK & PARRY  (Q key)
    // ─────────────────────────────────────────────
    void HandleBlock()
    {
        // Start blocking
        if (Input.GetKeyDown(KeyCode.Q) && blockTimer <= 0f)
        {
            isBlocking = true;
            isParrying = true;                         // Parry window opens on key press
            animator?.SetBool(ANIM_BLOCK, true);
            PlaySound(blockSound);

            // Parry window closes after parryWindow seconds
            Invoke(nameof(EndParryWindow), parryWindow);
        }

        // Stop blocking when Q released
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

    // ─────────────────────────────────────────────
    //  Called by EnemyAttack when an enemy hits us
    // ─────────────────────────────────────────────
    /// <summary>
    /// Returns true if the attack was blocked/parried (caller should skip damage).
    /// </summary>
    public bool TryBlock(float incomingDamage, bool isRangedAttack, Transform attacker)
    {
        if (!isBlocking) return false;

        if (isParrying && !isRangedAttack)
        {
            // Perfect parry — zero damage, trigger counter
            animator?.SetTrigger(ANIM_PARRY);
            PlaySound(parrySound);

            // Auto counter-attack the attacker
            EnemyHealth enemy = attacker?.GetComponent<EnemyHealth>();
            enemy?.TakeDamage(parryCounterDamage);

            return true;
        }

        if (isRangedAttack)
        {
            // Ranged attacks are fully blockable — no damage
            PlaySound(blockSound);
            return true;
        }

        // Regular block — reduce damage by 70%, player still takes some
        float reducedDamage = incomingDamage * 0.3f;
        playerHealth?.TakeDamage(reducedDamage);
        PlaySound(blockSound);
        return true; // Handled — don't apply full damage
    }

    // ─────────────────────────────────────────────
    //  SPEAR  (F = pickup,  E = throw)
    // ─────────────────────────────────────────────
    void HandleSpear()
    {
        // Throw spear
        if (hasSpear && Input.GetKeyDown(KeyCode.E))
        {
            ThrowSpear();
        }
    }

    /// <summary>Called by SpearPickup trigger when player walks over a spear.</summary>
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

        // Spawn and launch spear
        GameObject spear = Instantiate(spearPrefab, spearThrowPoint.position, spearThrowPoint.rotation);
        Rigidbody rb = spear.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = transform.forward * spearThrowForce;
        }
    }

    // ─────────────────────────────────────────────
    //  Helpers
    // ─────────────────────────────────────────────
    void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
            audioSource.PlayOneShot(clip);
    }

    public bool IsBlocking => isBlocking;
    public bool HasSpear   => hasSpear;

    // Draw attack range in Scene view for easier debugging
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(
            transform.position + transform.forward * attackRange * 0.5f,
            attackRange
        );
    }
}
