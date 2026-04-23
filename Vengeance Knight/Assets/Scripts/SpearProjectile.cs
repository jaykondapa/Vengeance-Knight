using UnityEngine;

/// <summary>
/// Attach to the thrown Spear prefab.
/// Instantly kills ranged enemies, deals heavy damage to melee enemies.
/// </summary>
public class SpearProjectile : MonoBehaviour
{
    public float damageToMelee   = 75f;   // Heavy but not guaranteed kill
    public float damageToRanged  = 9999f; // Instant kill

    // Tags to identify enemy types — make sure these match your Unity tags
    public string rangedEnemyTag = "RangedEnemy";
    public string meleeEnemyTag  = "MeleeEnemy";

    public float lifetime = 5f; // Auto-destroy after 5 seconds if it misses

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter(Collider other)
    {
        // Hit a ranged enemy — instant kill
        if (other.CompareTag(rangedEnemyTag))
        {
            other.GetComponent<EnemyHealth>()?.TakeDamage(damageToRanged);
            Destroy(gameObject);
            return;
        }

        // Hit a melee enemy — heavy damage
        if (other.CompareTag(meleeEnemyTag))
        {
            other.GetComponent<EnemyHealth>()?.TakeDamage(damageToMelee);
            Destroy(gameObject);
            return;
        }

        // Hit environment — stick into it
        if (!other.CompareTag("Player"))
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.isKinematic = true;
            }
            // Destroy after a short delay
            Destroy(gameObject, 3f);
        }
    }
}
