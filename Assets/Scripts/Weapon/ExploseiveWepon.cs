using UnityEngine;

public class ExplosiveWeapon : WeaponSystem
{
    public float explosiveRange = 3f;

    protected override void Attack()
    {
        // 랜덤 방향으로 폭탄 투척
        float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        Vector3 throwPosition = player.transform.position + (Vector3)(direction * 2f);

        GameObject explosive = new GameObject("Explosive");
        explosive.transform.position = throwPosition;

        // 스프라이트 추가
        SpriteRenderer sr = explosive.AddComponent<SpriteRenderer>();
        Texture2D texture = new Texture2D(20, 20);
        Color[] pixels = new Color[20 * 20];
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = Color.blue;
        }
        texture.SetPixels(pixels);
        texture.Apply();
        sr.sprite = Sprite.Create(texture, new Rect(0, 0, 20, 20), new Vector2(0.5f, 0.5f));

        // 폭발 스크립트 추가
        Explosive exp = explosive.AddComponent<Explosive>();
        exp.Initialize(explosiveRange, Mathf.FloorToInt(player.attackDamage * 1.5f));
    }
}
