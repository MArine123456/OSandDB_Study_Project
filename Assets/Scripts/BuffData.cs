using UnityEngine;
using System.Collections.Generic;

public enum BuffType
{
    AttackDamage,
    AttackSpeed,
    MoveSpeed,
    Health,
    HealthRegen,
    NewWeapon,
    // 무기 업그레이드 타입 추가
    ProjectileUpgrade,
    MeleeUpgrade,
    ExplosiveUpgrade
}


[System.Serializable]
public class BuffData
{
    public string name;
    public string description;
    public BuffType type;
    public float value;
    public WeaponType weaponType;

    public BuffData(string n, string desc, BuffType t, float v)
    {
        name = n;
        description = desc;
        type = t;
        value = v;
    }

    public BuffData(string n, string desc, BuffType t, float v, WeaponType wType)
    {
        name = n;
        description = desc;
        type = t;
        value = v;
        weaponType = wType;
    }
}

[System.Serializable]
public class PlayerWeaponStatus
{
    public bool hasProjectile = true;
    public bool hasMelee = false;
    public bool hasExplosive = false;

    public int projectileCount = 1;
    public float projectileSpeed = 5f;
    public float meleeRange = 2f;
    public float explosiveRange = 3f;
}

public static class BuffDatabase
{
    private static List<BuffData> baseBuffs = new List<BuffData>
    {
        new BuffData("공격력 증가", "공격력 +5", BuffType.AttackDamage, 5f),
        new BuffData("공격속도 증가", "공격속도 +0.2", BuffType.AttackSpeed, 0.2f),
        new BuffData("이동속도 증가", "이동속도 +1", BuffType.MoveSpeed, 1f),
        new BuffData("체력 증가", "최대체력 +20", BuffType.Health, 20f),
        new BuffData("체력회복", "초당 체력회복 +2", BuffType.HealthRegen, 2f)
    };

    private static List<BuffData> weaponBuffs = new List<BuffData>
    {
        new BuffData("발사체 무기", "새로운 무기 획득", BuffType.NewWeapon, 0f, WeaponType.Projectile),
        new BuffData("근접 무기", "새로운 무기 획득", BuffType.NewWeapon, 0f, WeaponType.Melee),
        new BuffData("폭발 무기", "새로운 무기 획득", BuffType.NewWeapon, 0f, WeaponType.Explosive),
        
        // 무기 업그레이드 버프들
        new BuffData("발사체 개수 증가", "발사체 개수 +1", BuffType.ProjectileUpgrade, 1f),
        new BuffData("발사체 속도 증가", "발사체 속도 +2", BuffType.ProjectileUpgrade, 2f),
        new BuffData("근접 범위 증가", "근접 공격 범위 +0.5", BuffType.MeleeUpgrade, 0.5f),
        new BuffData("폭발 범위 증가", "폭발 범위 +1", BuffType.ExplosiveUpgrade, 1f),
    };

    public static List<BuffData> GetRandomBuffs(int count, PlayerWeaponStatus weaponStatus)
    {
        List<BuffData> availableBuffs = new List<BuffData>(baseBuffs);

        // 무기 상태에 따라 적절한 버프 추가
        foreach (var weaponBuff in weaponBuffs)
        {
            if (ShouldIncludeBuff(weaponBuff, weaponStatus))
            {
                availableBuffs.Add(weaponBuff);
            }
        }

        List<BuffData> result = new List<BuffData>();

        for (int i = 0; i < count && availableBuffs.Count > 0; i++)
        {
            int randomIndex = Random.Range(0, availableBuffs.Count);
            result.Add(availableBuffs[randomIndex]);
            availableBuffs.RemoveAt(randomIndex);
        }

        return result;
    }

    private static bool ShouldIncludeBuff(BuffData buff, PlayerWeaponStatus weaponStatus)
    {
        switch (buff.type)
        {
            case BuffType.NewWeapon:
                // 해당 무기를 가지고 있지 않을 때만 새 무기 버프 제공
                switch (buff.weaponType)
                {
                    case WeaponType.Projectile:
                        return !weaponStatus.hasProjectile;
                    case WeaponType.Melee:
                        return !weaponStatus.hasMelee;
                    case WeaponType.Explosive:
                        return !weaponStatus.hasExplosive;
                }
                break;

            case BuffType.ProjectileUpgrade:
                // 발사체 무기를 가지고 있을 때만 업그레이드 제공
                return weaponStatus.hasProjectile;

            case BuffType.MeleeUpgrade:
                // 근접 무기를 가지고 있을 때만 업그레이드 제공
                return weaponStatus.hasMelee;

            case BuffType.ExplosiveUpgrade:
                // 폭발 무기를 가지고 있을 때만 업그레이드 제공
                return weaponStatus.hasExplosive;
        }

        return true;
    }

    public static void ApplyBuff(BuffData buff, PlayerWeaponStatus weaponStatus)
    {
        switch (buff.type)
        {
            case BuffType.NewWeapon:
                // 새 무기 획득
                switch (buff.weaponType)
                {
                    case WeaponType.Projectile:
                        weaponStatus.hasProjectile = true;
                        Debug.Log("발사체 무기 획득!");
                        break;
                    case WeaponType.Melee:
                        weaponStatus.hasMelee = true;
                        Debug.Log("근접 무기 획득!");
                        break;
                    case WeaponType.Explosive:
                        weaponStatus.hasExplosive = true;
                        Debug.Log("폭발 무기 획득!");
                        break;
                }
                break;

            case BuffType.ProjectileUpgrade:
                if (buff.name.Contains("개수"))
                {
                    weaponStatus.projectileCount += (int)buff.value;
                    Debug.Log($"발사체 개수 증가: {weaponStatus.projectileCount}");
                }
                else if (buff.name.Contains("속도"))
                {
                    weaponStatus.projectileSpeed += buff.value;
                    Debug.Log($"발사체 속도 증가: {weaponStatus.projectileSpeed}");
                }
                break;

            case BuffType.MeleeUpgrade:
                weaponStatus.meleeRange += buff.value;
                Debug.Log($"근접 범위 증가: {weaponStatus.meleeRange}");
                break;

            case BuffType.ExplosiveUpgrade:
                weaponStatus.explosiveRange += buff.value;
                Debug.Log($"폭발 범위 증가: {weaponStatus.explosiveRange}");
                break;

            case BuffType.AttackDamage:
                // 기존 버프 처리
                Debug.Log($"공격력 증가: +{buff.value}");
                break;

            case BuffType.AttackSpeed:
                Debug.Log($"공격속도 증가: +{buff.value}");
                break;

            case BuffType.MoveSpeed:
                Debug.Log($"이동속도 증가: +{buff.value}");
                break;

            case BuffType.Health:
                Debug.Log($"체력 증가: +{buff.value}");
                break;

            case BuffType.HealthRegen:
                Debug.Log($"체력회복 증가: +{buff.value}");
                break;
        }
    }
}
