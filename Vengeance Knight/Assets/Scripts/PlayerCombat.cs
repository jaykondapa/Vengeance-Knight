using UnityEngine;

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

    [Header("Audio")]
    public AudioClip attackSound;
    public AudioClip blockSound;
    public AudioClip parrySound;

    private Animator animator;
    private AudioSource audioSource;
    private PlayerHealth playerHealth;

    private float attackTimer = 0f;
    private float blockTimer = 0f;

    private bool isBlocking = false;
    private bool isParrying = false;
    private bool isAttacking = false;

    private const string ANIM_ATTACK = "Attack";
    private const string ANIM_BLOCK = "IsBlocking";
    private const string ANIM_PARRY = "Parry";

    void Awake()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        playerHealth = GetComponent<PlayerHealth>();
    }

    void Update()
    {
        if (attackTimer > 0f)
            attackTimer -= Time.deltaTime;

        if (blockTimer > 0f)
            blockTimer -= Time.deltaTime;

        HandleBlock();   // 🔥 block first (priority)
        HandleAttack();
    }

    void HandleAttack()
    {
        if (Input.GetMouseButtonDown(0) && attackTimer <= 0f && !isBlocking)
        {
            attackTimer = attackCooldown;
            isAttacking = true;

            animator?.SetTrigger(ANIM_ATTACK);
            PlaySound(attackSound);

            Invoke(nameof(EndAttack), 0.6f);
        }
    }

    public void DealDamage()
    {
        Vector3 attackPoint = transform.position + transform.forward * attackRange * 0.8f;

        Collider[] hits = Physics.OverlapSphere(attackPoint, attackRange, enemyLayer);

        foreach (Collider hit in hits)
        {
            EnemyHealth enemy = hit.GetComponentInParent<EnemyHealth>();

            if (enemy != null)
            {
                enemy.TakeDamage(Mathf.RoundToInt(attackDamage));
            }
        }
    }

    void EndAttack()
    {
        isAttacking = false;
    }

    void HandleBlock()
    {
        // HOLD Q
        if (Input.GetKey(KeyCode.Q) && blockTimer <= 0f)
        {
            if (!isBlocking) // trigger only once
            {
                isBlocking = true;
                isParrying = true;

                animator?.SetBool(ANIM_BLOCK, true);
                PlaySound(blockSound);

                Invoke(nameof(EndParryWindow), parryWindow);
            }
        }

        // RELEASE Q
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

    public bool TryBlock(float incomingDamage, bool isRangedAttack, Transform attacker)
    {
        if (!isBlocking)
            return false;

        // FULL BLOCK
        PlaySound(blockSound);
        return true;
    }

    void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    // MOVEMENT LOCK
    public bool CanMove()
    {
        return !isAttacking && !isBlocking;
    }

    public bool IsBlocking => isBlocking;
    public bool IsAttacking => isAttacking;

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Vector3 attackPoint = transform.position + transform.forward * attackRange * 0.8f;

        Gizmos.DrawWireSphere(attackPoint, attackRange);
    }
}