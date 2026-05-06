using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public Transform player;
    public Animator animator;

    [Header("Movement")]
    public bool canPatrol = true;
    public float patrolRadius = 5f;
    public float patrolSpeed = 2f;
    public float chaseSpeed = 4f;
    public float rotationSpeed = 10f;

    [Header("Combat")]
    public float detectionRange = 6f;
    public float attackRange = 2f;
    public float attackCooldown = 1.5f;
    public float attackDamage = 15f;

    private Vector3 patrolTarget;
    private float attackTimer;
    private bool isAttacking = false;

    private EnemyHealth enemyHealth;
    Rigidbody rb;

    void Start()
    {
        enemyHealth = GetComponent<EnemyHealth>();
        rb = GetComponent<Rigidbody>();
        SetNewPatrolPoint();
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
        else if (canPatrol)
        {
            Patrol();
        }
        else
        {
            Idle();
        }
    }

    // ATTACK LOGIC
    void HandleAttack()
    {
        LookAtPlayer();

         if (rb != null)
        {
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
        }

        if (attackTimer <= 0f && !isAttacking)
        {
            attackTimer = attackCooldown;
            isAttacking = true;

            animator.SetFloat("Speed", 0);
            animator.SetTrigger("Attack");

            Invoke(nameof(EndAttack), 1.0f);
        }
    }

    void EndAttack()
    {
        isAttacking = false;
        animator.ResetTrigger("Attack");

    }

    // CALLED BY ANIMATION EVENT
    public void DealDamage()
    {
        if (player == null) return;

        float dist = Vector3.Distance(transform.position, player.position);

        if (dist <= attackRange)
        {
            PlayerHealth ph = player.GetComponent<PlayerHealth>();

            if (ph != null)
            {
                ph.TakeDamage(Mathf.RoundToInt(attackDamage));
                Debug.Log("Enemy HIT player");
            }
        }
    }

    // CHASE
    void ChasePlayer()
    {
        if (isAttacking)
        {
            animator.SetFloat("Speed", 0f); //FORCE STOP
            return;
        }

        Vector3 dir = (player.position - transform.position).normalized;
        Move(dir, chaseSpeed);

        animator.SetFloat("Speed", 1f);
    }

    // PATROL
    void Patrol()
    {
        if (isAttacking)
        {
            animator.SetFloat("Speed", 0f); //FORCE STOP
            return;
        }

        Vector3 dir = (patrolTarget - transform.position);
        dir.y = 0;

        if (dir.magnitude < 1f)
        {
            SetNewPatrolPoint();
            return;
        }

        Move(dir.normalized, patrolSpeed);

        animator.SetFloat("Speed", 0.5f);
    }

    void SetNewPatrolPoint()
    {
        Vector2 random = Random.insideUnitCircle * patrolRadius;
        patrolTarget = new Vector3(
            transform.position.x + random.x,
            transform.position.y,
            transform.position.z + random.y
        );
    }

    // IDLE
    void Idle()
    {
        if (isAttacking)
        {
            animator.SetFloat("Speed", 0f);
            return;
        }

        animator.SetFloat("Speed", 0f);
    }

    // MOVEMENT
    void Move(Vector3 direction, float speed)
    {
        //DO NOT MOVE WHILE ATTACKING
        if (isAttacking) return;

        transform.position += direction * speed * Time.deltaTime;

        if (direction != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRot,
                rotationSpeed * Time.deltaTime
            );
        }
    }

    void LookAtPlayer()
    {
        Vector3 dir = (player.position - transform.position);
        dir.y = 0;

        if (dir != Vector3.zero)
        {
            Quaternion rot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                rot,
                rotationSpeed * Time.deltaTime
            );
        }
    }
}