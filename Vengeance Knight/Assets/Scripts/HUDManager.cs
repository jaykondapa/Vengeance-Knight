using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Controls all HUD elements: health bar, lives counter, and stamina bar.
/// Attach to a HUDManager GameObject in your Canvas.
/// </summary>
public class HUDManager : MonoBehaviour
{
    [Header("Health Bar (Top Left)")]
    public Slider healthSlider;
    public Image  healthFill;           // The fill image of the health slider
    public Color  healthColorFull  = new Color(0.2f, 0.8f, 0.2f); // Green
    public Color  healthColorLow   = new Color(0.9f, 0.2f, 0.1f); // Red
    public float  lowHealthThreshold = 0.3f;                       // 30% HP = danger

    [Header("Lives Counter (Top Left, below health)")]
    public TextMeshProUGUI livesText;   // e.g., shows "♥ ♥ ♥"
    public Image[] lifeIcons;          // Optional: heart icon images (max 3)

    [Header("Stamina / Ability Bar")]
    public Slider staminaSlider;        // Below health bar (future use)

    [Header("Boss Health Bar (Top Center) — hidden by default")]
    public GameObject bossHealthBarObject;
    public Slider     bossHealthSlider;
    public TextMeshProUGUI bossNameText;

    void Start()
    {
        // Boss bar hidden until a boss fight starts
        if (bossHealthBarObject != null)
            bossHealthBarObject.SetActive(false);
    }

    // ─────────────────────────────────────────────
    //  PLAYER HEALTH
    // ─────────────────────────────────────────────

    /// <summary>Update health bar. normalizedValue is 0.0 to 1.0</summary>
    public void UpdateHealth(float normalizedValue)
    {
        if (healthSlider != null)
            healthSlider.value = normalizedValue;

        // Change fill color when health is low
        if (healthFill != null)
            healthFill.color = normalizedValue <= lowHealthThreshold
                ? healthColorLow
                : healthColorFull;
    }

    // ─────────────────────────────────────────────
    //  LIVES
    // ─────────────────────────────────────────────

    /// <summary>Update lives display. livesRemaining is 0, 1, 2, or 3.</summary>
    public void UpdateLives(int livesRemaining)
    {
        // Text fallback: "♥ x3"
        if (livesText != null)
            livesText.text = "♥ x" + livesRemaining;

        // Icon approach: show/hide heart icons
        if (lifeIcons != null)
        {
            for (int i = 0; i < lifeIcons.Length; i++)
            {
                if (lifeIcons[i] != null)
                    lifeIcons[i].enabled = (i < livesRemaining);
            }
        }
    }

    // ─────────────────────────────────────────────
    //  BOSS HEALTH BAR
    // ─────────────────────────────────────────────

    /// <summary>Show the boss health bar at the top center of the screen.</summary>
    public void ShowBossHealthBar(string bossName, float normalizedValue = 1f)
    {
        if (bossHealthBarObject != null)
            bossHealthBarObject.SetActive(true);

        if (bossNameText != null)
            bossNameText.text = bossName;

        UpdateBossHealth(normalizedValue);
    }

    public void UpdateBossHealth(float normalizedValue)
    {
        if (bossHealthSlider != null)
            bossHealthSlider.value = normalizedValue;
    }

    public void HideBossHealthBar()
    {
        if (bossHealthBarObject != null)
            bossHealthBarObject.SetActive(false);
    }

    // ─────────────────────────────────────────────
    //  STAMINA  (future use — wired up and ready)
    // ─────────────────────────────────────────────

    public void UpdateStamina(float normalizedValue)
    {
        if (staminaSlider != null)
            staminaSlider.value = normalizedValue;
    }
}
