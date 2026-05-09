using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public int maxLives = 3;

    [Header("UI References")]
    public Slider healthBar;
    public HUDManager hudManager;

    [Header("Respawn")]
    public float respawnDelay = 1.5f;
    public GameObject deathEffect;

    [Header("Game Over UI")]
    public GameObject gameOverCanvas;

    private float currentHealth;
    private int currentLives;
    private bool isDead = false;
    private Vector3 respawnPosition;

    // Keeps health between levels
    private static float savedHealth = -1f;
    private static int savedLives = -1;

    private Animator animator;

    private const string ANIM_HIT = "TakeHit";
    private const string ANIM_DEAD = "Die";

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        // First level load
        if (savedHealth < 0)
        {
            currentHealth = maxHealth;
            currentLives = maxLives;
        }
        // Coming from another level
        else
        {
            currentHealth = savedHealth;
            currentLives = savedLives;
        }

        respawnPosition = transform.position;

        UpdateHealthUI();
        hudManager?.UpdateLives(currentLives);
    }

    void OnDestroy()
    {
        savedHealth = currentHealth;
        savedLives = currentLives;
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        PlayerCombat pc = GetComponent<PlayerCombat>();

        // 100% block
        if (pc != null && pc.IsBlocking)
        {
            return;
        }

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        animator?.SetTrigger(ANIM_HIT);
        UpdateHealthUI();

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    public void SetRespawnPoint(Vector3 position)
    {
        respawnPosition = position;
    }

    void Die()
    {
        if (isDead) return;

        isDead = true;

        animator?.SetTrigger(ANIM_DEAD);

        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }

        currentLives--;
        hudManager?.UpdateLives(currentLives);

        if (currentLives <= 0)
        {
            Invoke(nameof(GameOver), respawnDelay);
            AudioManager.Instance.PlayGameOver();
        }
        else
        {
            AudioManager.Instance.PlayPlayerDeath();
            Invoke(nameof(Respawn), respawnDelay);
        }
    }

    void Respawn()
    {
        isDead = false;
        currentHealth = maxHealth;

        transform.position = respawnPosition;

        UpdateHealthUI();
        animator?.Rebind();
    }

    void GameOver()
    {
        if (gameOverCanvas != null)
        {
            gameOverCanvas.SetActive(true);
        }

        Time.timeScale = 0f;
    }

    void UpdateHealthUI()
    {
        if (healthBar != null)
        {
            healthBar.value = currentHealth / maxHealth;
        }
    }

    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;
    public int CurrentLives => currentLives;
    public bool IsDead => isDead;
}