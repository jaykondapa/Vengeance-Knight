using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// Manages player HP, lives, death, respawn, and game-over logic.
/// Attach to the Player GameObject.
public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public int   maxLives  = 3;

    [Header("UI References")]
    public Slider healthBar;        // Assign in Inspector (top-left health slider)
    public HUDManager hudManager;   // Assign in Inspector

    [Header("Respawn")]
    public float respawnDelay = 1.5f;   // Seconds before respawning
    public GameObject deathEffect;      // Optional particle effect on death

    // Internal state
    private float currentHealth;
    private int   currentLives;
    private bool  isDead = false;
    private Vector3 respawnPosition;    // Saved when player enters a new area

    private Animator animator;
    private const string ANIM_HIT  = "TakeHit";
    private const string ANIM_DEAD = "Die";

    [Header("Game Over UI")]
    public GameObject gameOverCanvas;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        currentHealth   = maxHealth;
        currentLives    = maxLives;
        respawnPosition = transform.position;

        UpdateHealthUI();
        hudManager?.UpdateLives(currentLives);
    }

    /// Deal damage to the player. Called by EnemyAttack or environmental hazards.
    /// Combat blocking is handled first in PlayerCombat.TryBlock().
    public void TakeDamage(float amount)
    {
        if (isDead) return;

        // 🔥 BLOCK CHECK (NEW)
        PlayerCombat pc = GetComponent<PlayerCombat>();
        if (pc != null && pc.IsBlocking)
        {
            return; // 100% damage blocked
        }

        currentHealth -= amount;
        currentHealth  = Mathf.Clamp(currentHealth, 0f, maxHealth);

        animator?.SetTrigger(ANIM_HIT);
        UpdateHealthUI();

        if (currentHealth <= 0f)
            Die();
    }

    /// <summary>Save the current position as respawn point (call when area is cleared).</summary>
    public void SetRespawnPoint(Vector3 position)
    {
        respawnPosition = position;
    }

    //  DEATH & RESPAWN

    void Die()
    {
        if (isDead) return;
        isDead = true;

        animator?.SetTrigger(ANIM_DEAD);

        if (deathEffect != null)
            Instantiate(deathEffect, transform.position, Quaternion.identity);

        currentLives--;
        hudManager?.UpdateLives(currentLives);

        if (currentLives <= 0)
        {
            // No lives left — game over, restart from beginning
            Invoke(nameof(GameOver), respawnDelay);
        }
        else
        {
            // Still has lives — respawn at last checkpoint
            Invoke(nameof(Respawn), respawnDelay);
        }
    }

    void Respawn()
    {
        isDead        = false;
        currentHealth = maxHealth;

        transform.position = respawnPosition;
        UpdateHealthUI();
        animator?.Rebind(); // Reset all animation states
    }

    void GameOver()
    {
        if (gameOverCanvas != null)
            gameOverCanvas.SetActive(true);

        Time.timeScale = 0f;
    }

    //  UI

    void UpdateHealthUI()
    {
        if (healthBar != null)
            healthBar.value = currentHealth / maxHealth;
    }

    //  Getters (used by HUD and other scripts)
    public float CurrentHealth   => currentHealth;
    public float MaxHealth       => maxHealth;
    public int   CurrentLives    => currentLives;
    public bool  IsDead          => isDead;
}
