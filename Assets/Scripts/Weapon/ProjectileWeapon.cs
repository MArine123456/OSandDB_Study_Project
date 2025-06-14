using UnityEngine;

public class ProjectileWeapon : WeaponSystem
{
    public int projectileCount = 1;
    public float projectileSpeed = 10f;

    protected override void Attack()
    {
        for (int i = 0; i < projectileCount; i++)
        {
            GameObject projectile = new GameObject("Projectile");
            projectile.transform.position = player.transform.position;

            // 스프라이트 추가
            SpriteRenderer sr = projectile.AddComponent<SpriteRenderer>();
            Texture2D texture = new Texture2D(12, 12);
            Color[] pixels = new Color[12 * 12];
            for (int j = 0; j < pixels.Length; j++)
            {
                pixels[j] = Color.green;
            }
            texture.SetPixels(pixels);
            texture.Apply();
            sr.sprite = Sprite.Create(texture, new Rect(0, 0, 12, 12), new Vector2(0.5f, 0.5f));

            // 콜라이더 추가
            CircleCollider2D collider = projectile.AddComponent<CircleCollider2D>();
            collider.isTrigger = true;
            collider.radius = 0.25f;

            // 발사체 스크립트 추가
            Projectile proj = projectile.AddComponent<Projectile>();

            // 방향 설정 (여러 발사체인 경우 부채꼴 모양으로)
            float angleSpread = 30f;
            float baseAngle = player.transform.eulerAngles.z;
            float angle = baseAngle;

            if (projectileCount > 1)
            {
                angle = baseAngle + (i - (projectileCount - 1) / 2f) * (angleSpread / (projectileCount - 1));
            }

            Vector2 direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
            proj.Initialize(direction, projectileSpeed, Mathf.FloorToInt(player.attackDamage));
        }
    }
}
