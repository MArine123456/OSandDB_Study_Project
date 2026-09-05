using UnityEngine;

public class TriangleEnemy : Enemy
{
    public float shootRange = 8f;
    public float shootCooldown = 2f;
    private float lastShootTime;

    // 총알 스프라이트 캐싱
    private static Sprite cachedProjectileSprite;

    protected override void Start()
    {
        enemyType = EnemyType.Triangle;
        maxHealth = 20;
        moveSpeed = 1.5f;
        attackDamage = 8;
        scoreValue = 20;
        expValue = 8;
        enemyGiftingGold = 3;
        base.Start();
    }

    protected override void Update()
    {
        base.Update();

        // 원거리 공격
        if (player != null && Vector2.Distance(transform.position, player.transform.position) <= shootRange)
        {
            if (Time.time - lastShootTime >= shootCooldown)
            {
                ShootAtPlayer();
                lastShootTime = Time.time;
            }
        }
    }

    void ShootAtPlayer()
    {
        GameObject projectile = new GameObject("EnemyProjectile");
        projectile.transform.position = transform.position;
        SpriteRenderer sr = projectile.AddComponent<SpriteRenderer>();

        if (cachedProjectileSprite == null)
        {
            Texture2D texture = new Texture2D(8, 8);
            Color[] pixels = new Color[8 * 8];
            for (int i = 0; i < pixels.Length; i++) pixels[i] = Color.yellow;
            texture.SetPixels(pixels);
            texture.Apply();
            cachedProjectileSprite = Sprite.Create(texture, new Rect(0, 0, 8, 8), new Vector2(0.5f, 0.5f));
        }

        sr.sprite = cachedProjectileSprite;

        CircleCollider2D collider = projectile.AddComponent<CircleCollider2D>();
        collider.isTrigger = true;
        collider.radius = 0.2f;

        EnemyProjectile proj = projectile.AddComponent<EnemyProjectile>();
        proj.Initialize(player.transform.position, attackDamage);
    }
}

