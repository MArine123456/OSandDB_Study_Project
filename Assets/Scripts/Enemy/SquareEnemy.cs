using UnityEngine;

public class SquareEnemy : Enemy
{
    // 캐싱된 사각형 스프라이트를 저장하기 위한 정적 변수
    private static Sprite cachedSquareSprite;

    protected override void Start()
    {
        enemyType = EnemyType.Square;
        maxHealth = 100;
        moveSpeed = 1f;
        attackDamage = 15;
        scoreValue = 50;
        expValue = 10;
        enemyGiftingGold = 4;
        base.Start();
    }

    protected override void CreateSprite()
    {
        // 사각형 스프라이트 생성
        if (cachedSquareSprite == null)
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
            cachedSquareSprite = Sprite.Create(texture, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f));
        }

        spriteRenderer.sprite = cachedSquareSprite;
    }
}
