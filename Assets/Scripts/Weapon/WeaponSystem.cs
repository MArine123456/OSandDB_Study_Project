using UnityEngine;
using System.Collections.Generic;

public enum WeaponType
{
    Projectile = 0, // 기본무기
    Melee = 1,      // 근접 무기
    Explosive = 2   // 폭발 무기
}
public abstract class WeaponSystem : MonoBehaviour
{
    protected PlayerController player;
    protected float lastAttackTime;
    protected Camera mainCamera;

    protected Vector2 weapondirection;

    public virtual void Initialize(PlayerController playerRef)
    {
        player = playerRef;
    }

    private void Start()
    {
        mainCamera = Camera.main;
    }

    protected virtual void Update()
    {
        Vector3 mousePos = Input.mousePosition;
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, mainCamera.nearClipPlane));
        weapondirection = (worldPos - transform.position).normalized;

        if (player != null && Time.time - lastAttackTime >= 1f / player.attackSpeed)
        {
            Attack();
            lastAttackTime = Time.time;
        }
    }

    protected abstract void Attack();
}
