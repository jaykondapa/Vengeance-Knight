using UnityEngine;

/// <summary>
/// Place this on the Spear Pickup prefab that enemies drop.
/// When the player walks over it and presses F, they pick it up.
/// </summary>
public class SpearPickup : MonoBehaviour
{
    private bool playerInRange = false;
    private PlayerCombat playerCombat;

    [Header("Visual")]
    public GameObject pickupPrompt; // Optional "Press F" UI prompt above the spear

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.F))
        {
            playerCombat?.PickUpSpear();
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            playerCombat  = other.GetComponent<PlayerCombat>();

            if (pickupPrompt != null)
                pickupPrompt.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            playerCombat  = null;

            if (pickupPrompt != null)
                pickupPrompt.SetActive(false);
        }
    }
}
