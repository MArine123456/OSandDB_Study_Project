using UnityEngine;

public class SquareEnemy : Enemy
{
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
        Texture2D texture = new Texture2D(32, 32);
        Color[] pixels = new Color[32 * 32];

        for (int i = 0; i < pixels.Length; i++)
        {
            int x = i % 32;
            int y = i / 32;

            if (x >= 4 && x < 28 && y >= 4 && y < 28)
            {
                pixels[i] = GetDefaultColor();
            }
            else
            {
                pixels[i] = Color.clear;
            }
        }

        texture.SetPixels(pixels);
        texture.Apply();

        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f));
        spriteRenderer.sprite = sprite;
    }
}
