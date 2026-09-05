using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class Explosive : MonoBehaviour
{
    private float explosionRange;
    private int damage;
    private float fuseTime = 1f;

    // 캐싱된 폭발 스프라이트를 저장하기 위한 정적 변수
    private static Sprite cachedExplosionSprite;

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

        if (cachedExplosionSprite == null)
        {
            int texSize = 64;
            //int size = Mathf.FloorToInt(explosionRange * 64);
            Texture2D texture = new Texture2D(texSize, texSize);
            Color[] pixels = new Color[texSize * texSize];
            Vector2 center = new Vector2(texSize / 2f, texSize / 2f);

            for (int i = 0; i < pixels.Length; i++)
            {
                int x = i % texSize;
                int y = i / texSize;
                float distance = Vector2.Distance(new Vector2(x, y), center);
                if (distance <= texSize / 2f)
                {
                    pixels[i] = new Color(1, 1, 0, 0.7f - (distance / (texSize / 2f)) * 0.7f);
                }
                else
                {
                    pixels[i] = Color.clear;
                }
            }
            texture.SetPixels(pixels);
            texture.Apply();
            sr.sprite = Sprite.Create(texture, new Rect(0, 0, texSize, texSize), new Vector2(0.5f, 0.5f));
        }

        sr.sprite = cachedExplosionSprite;

        // 폭발 범위에 맞게 스프라이트 크기 조정
        float spriteScale = (explosionRange * 2f) / (64f / 100f);
        explosion.transform.localScale = Vector3.one * spriteScale;

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
