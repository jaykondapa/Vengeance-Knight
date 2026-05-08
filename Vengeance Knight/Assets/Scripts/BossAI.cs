using UnityEngine;

public class BossAI : MonoBehaviour
{
    public Transform player;
    public Animator animator;

    [Header("Movement")]
    public bool canPatrol = false;
    public float patrolRadius = 0f;
    public float patrolSpeed = 0f;
    public float chaseSpeed = 1f;
    public float rotationSpeed = 10f;

    [Header("Combat")]
    public float detectionRange = 10f;
    public float attackRange = 2.5f;
    public float attackCooldown = 1.5f;

    [Header("Damage")]
    public float normalAttackDamage = 35f;
    public float heavyAttackDamage = 60f;

    private float attackTimer;
    private bool isAttacking = false;
    private int attackCounter = 0;

    private EnemyHealth enemyHealth;
    private Rigidbody rb;

    void Start()
    {
        enemyHealth = GetComponent<EnemyHealth>();
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (enemyHealth != null && enemyHealth.IsDead)
            return;

        if (player == null)
            return;

        attackTimer -= Time.deltaTime;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= attackRange)
        {
            HandleAttack();
        }
        else if (distance <= detectionRange)
        {
            ChasePlayer();
        }
        else
        {
            Idle();
        }
    }

    void HandleAttack()
    {
        LookAtPlayer();

        if (rb != null)
        {
            rb.linearVelocity =
                new Vector3(0, rb.linearVelocity.y, 0);
        }

        if (attackTimer <= 0f && !isAttacking)
        {
            isAttacking = true;

            animator.SetFloat("Speed", 0f);

            // Every 4th attack = heavy attack
            if (attackCounter == 3)
            {
                attackCounter = 0;

                animator.ResetTrigger("Attack");
                animator.SetTrigger("HeavyAttack");

                attackTimer = attackCooldown + 2.5f;

                Invoke(nameof(EndAttack), 2.2f);
            }
            else
            {
                animator.ResetTrigger("HeavyAttack");
                animator.SetTrigger("Attack");

                attackCounter++;

                attackTimer = attackCooldown;

                Invoke(nameof(EndAttack), 1.0f);
            }
        }
    }

    void EndAttack()
    {
        isAttacking = false;

        animator.ResetTrigger("Attack");
        animator.ResetTrigger("HeavyAttack");
    }

    // Normal attack animation event
    public void DealDamage()
    {
        if (player == null) return;

        float dist =
            Vector3.Distance(transform.position, player.position);

        if (dist <= attackRange)
        {
            PlayerHealth ph =
                player.GetComponent<PlayerHealth>();

            if (ph != null)
            {
                ph.TakeDamage(normalAttackDamage);
                Debug.Log("Boss normal hit");
            }
        }
    }

    // Heavy attack animation event
    public void DealHeavyDamage()
    {
        if (player == null) return;

        float dist =
            Vector3.Distance(transform.position, player.position);

        if (dist <= attackRange)
        {
            PlayerHealth ph =
                player.GetComponent<PlayerHealth>();

            if (ph != null)
            {
                ph.TakeDamage(heavyAttackDamage);
                AudioManager.Instance.PlayBossHeavyAttackSuccess();
                Debug.Log("Boss HEAVY hit");
            }
        }
    }

    void ChasePlayer()
    {
        if (isAttacking)
        {
            animator.SetFloat("Speed", 0f);
            return;
        }

        Vector3 dir =
            (player.position - transform.position).normalized;

        Move(dir, chaseSpeed);

        animator.SetFloat("Speed", 1f);
    }

    void Idle()
    {
        animator.SetFloat("Speed", 0f);
    }

    void Move(Vector3 direction, float speed)
    {
        if (isAttacking) return;

        transform.position +=
            direction * speed * Time.deltaTime;

        if (direction != Vector3.zero)
        {
            Quaternion targetRot =
                Quaternion.LookRotation(direction);

            transform.rotation =
                Quaternion.Slerp(
                    transform.rotation,
                    targetRot,
                    rotationSpeed * Time.deltaTime
                );
        }
    }

    void LookAtPlayer()
    {
        Vector3 dir =
            player.position - transform.position;

        dir.y = 0;

        if (dir != Vector3.zero)
        {
            Quaternion rot =
                Quaternion.LookRotation(dir);

            transform.rotation =
                Quaternion.Slerp(
                    transform.rotation,
                    rot,
                    rotationSpeed * Time.deltaTime
                );
        }
    }
}