using UnityEngine;

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager Instance;

    [Header("플레이어 스탯")]
    public int gold = 0;
    public float playerMaxHealth = 0;
    public float playerAttackDamage = 0;
    public float playerMoveSpeed = 0;
    public float playerAttackSpeed = 0;
    public float playerHealthRegen = 0;
    public float playerIncreaseEXP = 0;

    [Header("업그레이드 현황")]
    public int maxHealthUpgradeLevel;
    public int attackDamageUpgradeLevel;
    public int moveSpeedUpgradeLevel;
    public int attackSpeedUpgradeLevel;
    public int healthRegenUpgradeLevel;
    public int increaseExpLevel;

    void Awake()
    {
        gold = 0;
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환 시 유지
        }
        else
        {
            Destroy(gameObject);
        }
    }




}
