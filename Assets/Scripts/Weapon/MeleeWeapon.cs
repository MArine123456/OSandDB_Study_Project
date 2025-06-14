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
        sr.color = Color.white;
        sr.sortingOrder = 10; // 다른 오브젝트 위에 표시되도록

        Texture2D texture = new Texture2D(64, 64);
        Color[] pixels = new Color[64 * 64];
        float textureRadius = 32f; // 텍스처의 반지름 (64x64 텍스처의 중심에서 가장자리까지)

        for (int i = 0; i < pixels.Length; i++)
        {
            int x = i % 64;
            int y = i / 64;
            float distance = Vector2.Distance(new Vector2(x, y), new Vector2(32, 32));
            if (distance <= textureRadius)
            {
                // 중심에서 가장자리로 갈수록 투명도 감소
                float alpha = 1f - (distance / textureRadius);
                pixels[i] = new Color(1, 0.3f, 0.3f, alpha * 0.6f); // 빨간색 계열
            }
            else
            {
                pixels[i] = Color.clear;
            }
        }
        texture.SetPixels(pixels);
        texture.Apply();
        sr.sprite = Sprite.Create(texture, new Rect(0, 0, 64, 64), new Vector2(0.5f, 0.5f));

        // 스프라이트 크기를 실제 공격 범위에 맞게 조정
        float spriteScale = (meleeRange * 2f) / (64f / 100f); // 64픽셀을 Unity 단위로 변환하여 스케일 계산
        meleeArea.transform.localScale = Vector3.one * spriteScale;

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
            StartCoroutine(AttackEffect());

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

    System.Collections.IEnumerator AttackEffect()
    {
        SpriteRenderer sr = meleeArea.GetComponent<SpriteRenderer>();
        float duration = 0.3f;
        float elapsedTime = 0f;

        // 공격 시작 - 밝고 강한 색상
        sr.color = new Color(1f, 0.2f, 0.2f, 1f);

        // 크기 변화 효과 (펄스 효과) - 실제 공격 범위 기준
        float baseScale = (meleeRange * 2f) / (64f / 100f);
        Vector3 originalScale = Vector3.one * baseScale;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / duration;

            // 페이드 아웃 효과
            float alpha = 1f - progress;
            sr.color = new Color(1f, 0.2f + progress * 0.3f, 0.2f, alpha);

            // 펄스 효과 (크기 변화) - 기본 크기에서 약간만 변화
            float scaleMultiplier = 1f + Mathf.Sin(progress * Mathf.PI * 4) * 0.1f;
            meleeArea.transform.localScale = originalScale * scaleMultiplier;

            yield return null;
        }

        // 원래 크기로 복원하고 비활성화
        meleeArea.transform.localScale = originalScale;
        meleeArea.SetActive(false);
    }
}