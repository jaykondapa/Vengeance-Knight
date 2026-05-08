using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 100;
    int currentHealth;

    public Slider healthBar;
    public Transform healthBarUI;

    public bool IsDead { get; private set; }

    void Start()
    {
        currentHealth = maxHealth;

        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }
    }

    void LateUpdate()
    {
        if (healthBarUI != null && Camera.main != null)
        {
            healthBarUI.forward = Camera.main.transform.forward;
        }
    }

    public void TakeDamage(int damage)
    {
        if (IsDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (healthBar != null)
        {
            healthBar.value = currentHealth;
            AudioManager.Instance.PlayEnemyReceiveHit();
        }

        if (currentHealth <= 0)
        {
            AudioManager.Instance.PlayEnemyDeath();
            Die();
        }
    }

    void Die()
    {
        IsDead = true;

        if (EnemyManager.Instance != null)
        {
            EnemyManager.Instance.EnemyKilled();
        }

        // Trigger animation
        Animator anim = GetComponent<Animator>();
        if (anim != null)
        {
            anim.SetTrigger("Die");
        }

        // Disable AI
        EnemyAI ai = GetComponent<EnemyAI>();
        if (ai != null)
        {
            ai.enabled = false;
        }

        // Stop physics movement BUT KEEP COLLIDER
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        // Destroy after animation
        Destroy(gameObject, 4f);
    }
}