using UnityEngine;

public class Gold : MonoBehaviour
{
    public PlayerController player;

    public int giftingGold;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            player.gold += giftingGold;
            Destroy(gameObject);
        }
    }
}
