using UnityEngine;

public class Explosive : MonoBehaviour
{
    private float explosionRange;
    private int damage;
    private float fuseTime = 1f;

    public void Initialize(float range, int dmg)
    {
        explosionRange = range;
        damage = dmg;
        StartCoroutine(ExplodeAfterDelay());
    }

    System.Collections.IEnumerator ExplodeAfterDelay()
    {
        yield return new UnityEngine.WaitForSeconds(fuseTime);
        Explode();
    }

    void Explode()
    {
        // 폭발 이펙트 생성
        GameObject explosion = new GameObject("Explosion");
        explosion.transform.position = transform.position;

        SpriteRenderer sr = explosion.AddComponent<SpriteRenderer>();
        sr.color = new Color(1, 1, 0, 0.7f);

        int size = Mathf.FloorToInt(explosionRange * 64);
        Texture2D texture = new Texture2D(size, size);
        Color[] pixels = new Color[size * size];
        Vector2 center = new Vector2(size / 2f, size / 2f);

        for (int i = 0; i < pixels.Length; i++)
        {
            int x = i % size;
            int y = i / size;
            float distance = Vector2.Distance(new Vector2(x, y), center);

            if (distance <= size / 2f)
            {
                pixels[i] = new Color(1, 1, 0, 0.7f - (distance / (size / 2f)) * 0.7f);
            }
            else
            {
                pixels[i] = Color.clear;
            }
        }
        texture.SetPixels(pixels);
        texture.Apply();
        sr.sprite = Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));

        // 범위 내 적들에게 피해
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, explosionRange);
        foreach (var collider in enemies)
        {
            if (collider.CompareTag("Enemy"))
            {
                Enemy enemy = collider.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damage);
                }
            }
        }

        // 폭발 이펙트 제거
        Destroy(explosion, 0.5f);
        Destroy(gameObject);
    }
}
