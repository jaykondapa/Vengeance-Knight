using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages an enemy's HP and death.
/// Attach to every enemy GameObject.
/// </summary>
public class EnemyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;

    [Header("UI")]
    public Slider enemyHealthBar;       // World-space slider above the enemy head
    public GameObject healthBarObject;  // Parent object — hidden until combat starts

    [Header("On Death")]
    public GameObject deathEffect;      // Optional particle effect
    public bool hasSpear = false;       // If true, drops a spear pickup on death
    public GameObject spearPickupPrefab;

    private float currentHealth;
    private bool  isDead = false;
    private Animator animator;

    private const string ANIM_HIT  = "TakeHit";
    private const string ANIM_DEAD = "Die";

    void Start()
    {
        currentHealth = maxHealth;

        animator = GetComponent<Animator>();

        // Hide health bar until the player engages
        if (healthBarObject != null)
            healthBarObject.SetActive(false);
    }

    // ─────────────────────────────────────────────
    //  PUBLIC API
    // ─────────────────────────────────────────────

    /// <summary>Called by PlayerCombat or SpearProjectile when hitting this enemy.</summary>
    public void TakeDamage(float amount)
    {
        if (isDead) return;

        // Show health bar on first hit
        if (healthBarObject != null && !healthBarObject.activeSelf)
            healthBarObject.SetActive(true);

        currentHealth -= amount;
        currentHealth  = Mathf.Clamp(currentHealth, 0f, maxHealth);

        UpdateHealthBar();
        animator?.SetTrigger(ANIM_HIT);

        if (currentHealth <= 0f)
            Die();
    }

    // ─────────────────────────────────────────────
    //  DEATH
    // ─────────────────────────────────────────────

    void Die()
    {
        if (isDead) return;
        isDead = true;

        animator?.SetTrigger(ANIM_DEAD);

        if (deathEffect != null)
            Instantiate(deathEffect, transform.position, Quaternion.identity);

        // Drop spear pickup if this enemy carried one
        if (hasSpear && spearPickupPrefab != null)
            Instantiate(spearPickupPrefab, transform.position, Quaternion.identity);

        // Disable collider so it can't be hit again or block the player
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        // Destroy after death animation plays (~2 seconds)
        Destroy(gameObject, 2f);
    }

    // ─────────────────────────────────────────────
    //  UI
    // ─────────────────────────────────────────────

    void UpdateHealthBar()
    {
        if (enemyHealthBar != null)
            enemyHealthBar.value = currentHealth / maxHealth;
    }

    public bool IsDead => isDead;
}
