using UnityEngine;

public class HealthPack : MonoBehaviour
{
    public int healAmount = 50;

    void Start()
    {
        Destroy(gameObject, 30f);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null && player.currentHealth < player.maxHealth)
            {
                player.currentHealth = Mathf.Min(player.maxHealth, player.currentHealth + healAmount);
                Destroy(gameObject);
            }
        }
    }
}