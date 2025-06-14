using System.IO;
using UnityEngine;

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager Instance;

    [Header("플레이어 스탯")]
    public int gold = 0;
    public int score = 0;

    [Header("업그레이드 현황")]
    public int maxHealthUpgradeLevel;
    public int attackDamageUpgradeLevel;
    public int moveSpeedUpgradeLevel;
    public int attackSpeedUpgradeLevel;
    public int healthRegenUpgradeLevel;
    public int increaseExpUpgradeLevel;

    private string saveFilePath;

    void Awake()
    {

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환 시 유지

            saveFilePath = Path.Combine(Application.dataPath, "SaveData/save.json");
            LoadData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SaveData()
    {
        GameData data = new GameData
        {
            highScore = score,
            gold = gold,
            maxHealthUpgradeLevel = maxHealthUpgradeLevel,
            attackDamageUpgradeLevel = attackDamageUpgradeLevel,
            moveSpeedUpgradeLevel = moveSpeedUpgradeLevel,
            attackSpeedUpgradeLevel = attackSpeedUpgradeLevel,
            healthRegenUpgradeLevel = healthRegenUpgradeLevel,
            increaseExpUpgradeLevel = increaseExpUpgradeLevel,
        };

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(saveFilePath, json);

        Debug.Log($"게임 데이터 저장됨 : {saveFilePath}");
    }

    public void LoadData()
    {
        if(File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            GameData data = JsonUtility.FromJson<GameData>(json);

            score = data.highScore;
            gold = data.gold;
            maxHealthUpgradeLevel = data.maxHealthUpgradeLevel;
            attackDamageUpgradeLevel= data.attackDamageUpgradeLevel;
            moveSpeedUpgradeLevel= data.moveSpeedUpgradeLevel;
            attackSpeedUpgradeLevel = data.attackSpeedUpgradeLevel;
            healthRegenUpgradeLevel= data.healthRegenUpgradeLevel;
            increaseExpUpgradeLevel = data.increaseExpUpgradeLevel;

            Debug.Log("GameData Loaded");
        }
        else
        {
            Debug.Log("No Save Data");
        }
    }

    public void ResetData()
    {
        if(File.Exists(saveFilePath))
        {
            File.Delete(saveFilePath);
            Debug.Log("저장 데이터 삭제 완료");
        }
    }
}
