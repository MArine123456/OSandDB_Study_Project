using UnityEngine;

public enum EnemyType
{
    Circle,
    Square,
    Triangle,
    Hexagon
}

public class Enemy : MonoBehaviour
{
    [Header("기본 스탯")]
    public int maxHealth = 20;
    public float moveSpeed = 2f;
    public int attackDamage = 10;
    public int scoreValue = 10;
    public int expValue = 5;
    public int enemyGiftingGold = 1;

    public int currentHealth;
    public EnemyType enemyType;
    public GameObject goldPrefab;
    [SerializeField]
    protected PlayerController player;
    protected SpriteRenderer spriteRenderer;
    protected bool isHit = false;
    protected float hitFlashTime = 0.1f;
    protected float hitTimer = 0f;

    // 캐싱된 기본 스프라이트를 저장하기 위한 정적 변수
    private static Sprite cachedBaseSprite;

    protected virtual void Start()
    {
        currentHealth = maxHealth;
        player = FindObjectOfType<PlayerController>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
            CreateSprite();
        }

        // 시간에 따른 스탯 증가
        float timeMultiplier = 1f + (GameManager.Instance.gameTime / 60f) * 0.1f;
        maxHealth = Mathf.FloorToInt(maxHealth * timeMultiplier);
        currentHealth = maxHealth;
        moveSpeed *= timeMultiplier;
        attackDamage = Mathf.FloorToInt(attackDamage * timeMultiplier);
    }

    protected virtual void Update()
    {
        if (player != null)
        {
            MoveTowardsPlayer();
        }

        HandleHitFlash();
    }

    protected virtual void MoveTowardsPlayer()
    {
        Vector2 direction = (player.transform.position - transform.position).normalized;
        transform.Translate(direction * moveSpeed * Time.deltaTime);
    }

    void HandleHitFlash()
    {
        if (isHit)
        {
            hitTimer += Time.deltaTime;
            spriteRenderer.color = Color.white;

            if (hitTimer >= hitFlashTime)
            {
                isHit = false;
                hitTimer = 0f;
                spriteRenderer.color = GetDefaultColor();
            }
        }
    }

    public virtual void TakeDamage(int damage)
    {
        currentHealth -= damage;
        isHit = true;
        hitTimer = 0f;


        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public virtual void Die()
    {
        // 점수 및 경험치 증가
        GameManager.Instance.AddScore(scoreValue);
        GameManager.Instance.AddKill(); // 킬 카운트 증가 추가
        player.GainExp(expValue);
        DropGold();

        Destroy(gameObject);
    }

    protected virtual void CreateSprite()
    {
        // 캐싱된 스프라이트가 없으면 새로 생성
        if (cachedBaseSprite == null)
        {
            Texture2D texture = new Texture2D(32, 32);
            Color[] pixels = new Color[32 * 32];

            for (int i = 0; i < pixels.Length; i++)
            {
                int x = i % 32;
                int y = i / 32;
                float distance = Vector2.Distance(new Vector2(x, y), new Vector2(16, 16));
                pixels[i] = distance <= 15 ? GetDefaultColor() : Color.clear;
            }

            texture.SetPixels(pixels);
            texture.Apply();
            cachedBaseSprite = Sprite.Create(texture, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f));
        }

        spriteRenderer.sprite = cachedBaseSprite;
    }

    private void DropGold()
    {
        Gold gold = goldPrefab.GetComponent<Gold>();
        gold.giftingGold = enemyGiftingGold;
        Instantiate(goldPrefab, this.transform.position, Quaternion.identity);

    }

    protected virtual Color GetDefaultColor()
    {
        return Color.red;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            player.TakeDamage(attackDamage);
        }
    }
}
