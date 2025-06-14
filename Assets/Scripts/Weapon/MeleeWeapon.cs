using UnityEngine;

public class MeleeWeapon : WeaponSystem
{
    public float meleeRange = 2f;
    private GameObject meleeArea;

    public override void Initialize(PlayerController playerRef)
    {
        base.Initialize(playerRef);
        CreateMeleeArea();
    }

    void CreateMeleeArea()
    {
        meleeArea = new GameObject("MeleeArea");
        meleeArea.transform.SetParent(transform);
        meleeArea.transform.localPosition = Vector3.zero;

        
        SpriteRenderer sr = meleeArea.AddComponent<SpriteRenderer>();
        sr.color = new Color(1, 1, 1, 0.3f);

        Texture2D texture = new Texture2D(64, 64);
        Color[] pixels = new Color[64 * 64];
        for (int i = 0; i < pixels.Length; i++)
        {
            int x = i % 64;
            int y = i / 64;
            float distance = Vector2.Distance(new Vector2(x, y), new Vector2(32, 32));

            if (distance <= 30)
            {
                pixels[i] = new Color(1, 0, 0, 0.1f);
            }
            else
            {
                pixels[i] = Color.clear;
            }
        }
        texture.SetPixels(pixels);
        texture.Apply();
        sr.sprite = Sprite.Create(texture, new Rect(0, 0, 64, 64), new Vector2(0.5f, 0.5f));

        // 콜라이더 추가
        CircleCollider2D collider = meleeArea.AddComponent<CircleCollider2D>();
        collider.isTrigger = true;
        collider.radius = meleeRange;

        meleeArea.SetActive(false);
    }

    protected override void Attack()
    {
        if (meleeArea != null)
        {
            meleeArea.SetActive(true);
            StartCoroutine(DeactivateMeleeArea());

            // 범위 내 모든 적에게 피해
            Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, meleeRange);
            foreach (var collider in enemies)
            {
                if (collider.CompareTag("Enemy"))
                {
                    Enemy enemy = collider.GetComponent<Enemy>();
                    if (enemy != null)
                    {
                        enemy.TakeDamage(Mathf.FloorToInt(player.attackDamage));
                    }
                }
            }
        }
    }

    System.Collections.IEnumerator DeactivateMeleeArea()
    {
        yield return new UnityEngine.WaitForSeconds(0.2f);
        if (meleeArea != null)
        {
            meleeArea.SetActive(false);
        }
    }
}
