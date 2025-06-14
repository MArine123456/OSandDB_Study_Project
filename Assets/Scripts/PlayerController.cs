using UnityEngine;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    [Header("기본 스탯")]
    public int maxHealth = 100;
    public float moveSpeed = 5f;
    public float attackDamage = 10f;
    public float attackSpeed = 1f;
    public float healthRegen = 0f;

    [Header("레벨 시스템")]
    public int level = 1;
    public int currentExp = 0;
    public int expToNextLevel = 100;

    public int currentHealth;
    public int gold = 0;
    private Vector2 moveDirection;
    private Camera mainCamera;
    private float lastRegenTime;

    private List<WeaponSystem> weapons = new List<WeaponSystem>();
    private List<BuffData> appliedBuffs = new List<BuffData>();
    public PlayerWeaponStatus weaponStatus = new PlayerWeaponStatus();

    [SerializeField]
    private GameObject weaponObj;

    void Start()
    {
        currentHealth = maxHealth;
        mainCamera = Camera.main;

        // 기본 무기 추가
        AddWeapon(WeaponType.Projectile);
    }

    void Update()
    {
        HandleMovement();
        HandleRotation();
        HandleHealthRegen();
    }

    void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        moveDirection = new Vector2(horizontal, vertical).normalized;
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);
    }

    void HandleRotation()
    {
        Vector3 mousePos = Input.mousePosition;
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, mainCamera.nearClipPlane));
        Vector2 direction = (worldPos - transform.position).normalized;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    void HandleHealthRegen()
    {
        if (healthRegen > 0 && Time.time - lastRegenTime >= 1f)
        {
            currentHealth = Mathf.Min(maxHealth, currentHealth + Mathf.FloorToInt(healthRegen));
            lastRegenTime = Time.time;
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }
    public void GainExp(int exp)
    {
        currentExp += exp;

        while (currentExp >= expToNextLevel)
        {
            LevelUp();
        }
    }

    void LevelUp()
    {
        currentExp -= expToNextLevel;
        level++;
        expToNextLevel = Mathf.FloorToInt(expToNextLevel * 1.2f);

        List<BuffData> buffOptions = BuffDatabase.GetRandomBuffs(3, weaponStatus);
        GameManager.Instance.ShowLevelUpPanel(buffOptions, 0f);
    }

    public void ApplyBuff(BuffData buff)
    {
        BuffDatabase.ApplyBuff(buff, weaponStatus);

        switch (buff.type)
        {
            case BuffType.AttackDamage:
                attackDamage += buff.value;
                break;
            case BuffType.AttackSpeed:
                attackSpeed += buff.value;
                break;
            case BuffType.MoveSpeed:
                moveSpeed += buff.value;
                break;
            case BuffType.Health:
                maxHealth += (int)buff.value;
                currentHealth += (int)buff.value;
                break;
            case BuffType.HealthRegen:
                healthRegen += buff.value;
                break;
            case BuffType.NewWeapon:
                AddWeapon(buff.weaponType);
                break;
            case BuffType.ProjectileUpgrade:
                ProjectileWeapon projectileWeapon = GetComponentInChildren<ProjectileWeapon>();
                if (buff.name.Contains("개수"))
                {
                    projectileWeapon.projectileCount++;
                }
                else if (buff.name.Contains("속도"))
                {
                    projectileWeapon.projectileSpeed += 1f;
                }
                break;
            case BuffType.MeleeUpgrade: //범위 업글
                MeleeWeapon meleeWeapon = GetComponentInChildren<MeleeWeapon>();
                meleeWeapon.meleeRange += 0.5f;
                break;
            case BuffType.ExplosiveUpgrade://범위 업글
                ExplosiveWeapon explosiveWeapon = GetComponentInChildren<ExplosiveWeapon>();
                explosiveWeapon.explosiveRange += 0.5f;
                break;
        }
    }

    void AddWeapon(WeaponType weaponType)
    {
        weaponObj = new GameObject("Weapon_" + weaponType.ToString());
        weaponObj.transform.SetParent(transform);
        weaponObj.transform.position = this.transform.position;

        WeaponSystem weapon = null;
        switch (weaponType)
        {
            case WeaponType.Projectile:
                weapon = weaponObj.AddComponent<ProjectileWeapon>();
                break;
            case WeaponType.Melee:
                weapon = weaponObj.AddComponent<MeleeWeapon>();
                break;
            case WeaponType.Explosive:
                weapon = weaponObj.AddComponent<ExplosiveWeapon>();
                break;
        }

        if (weapon != null)
        {
            weapon.Initialize(this);
            weapons.Add(weapon);
        }
    }

    void Die()
    {
        Debug.Log("Player Died! Final Score: " + GameManager.Instance.score);
        GameManager.Instance.ShowGameOver();
        Destroy(gameObject);
    }
}
