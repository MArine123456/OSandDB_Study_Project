using System.IO;
using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager Instance;

    [Header("게임 데이터 상태")]
    public int gold = 0;
    public int score = 0;

    [Header("업그레이드 현황")]
    public int maxHealthUpgradeLevel;
    public int attackDamageUpgradeLevel;
    public int moveSpeedUpgradeLevel;
    public int attackSpeedUpgradeLevel;
    public int healthRegenUpgradeLevel;
    public int increaseExpUpgradeLevel;

    private string dbPath;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            InitializeDatabase();
            LoadData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void InitializeDatabase()
    {
        // SQLite 데이터베이스 파일 경로 설정
        string dbName = "GameData.db";

#if UNITY_EDITOR || UNITY_STANDALONE
        dbPath = Path.Combine(Application.persistentDataPath, dbName);
#elif UNITY_ANDROID
    dbPath = Path.Combine(Application.persistentDataPath, dbName);
#elif UNITY_IOS
            dbPath = Path.Combine(Application.persistentDataPath, dbName);
#endif

        // 데이터베이스 테이블 생성
        CreateTable();
    }
    void CreateTable()
    {
        using (var connection = new SqliteConnection($"URI=file:{dbPath}"))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    CREATE TABLE IF NOT EXISTS GameData (
                        id INTEGER PRIMARY KEY, 
                        highScore INTEGER,
                        gold INTEGER,
                        maxHealthUpgradeLevel INTEGER,
                        attackDamageUpgradeLevel INTEGER,
                        moveSpeedUpgradeLevel INTEGER,
                        attackSpeedUpgradeLevel INTEGER,
                        healthRegenUpgradeLevel INTEGER,
                        increaseExpUpgradeLevel INTEGER
                    )";
                command.ExecuteNonQuery();
            }

            connection.Close();
        }

        Debug.Log($"SQLite 데이터베이스 초기화 완료: {dbPath}");
    }

    public void SaveData()
    {
        using (var connection = new SqliteConnection($"URI=file:{dbPath}"))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                // 기존 데이터가 있는지 확인
                command.CommandText = "SELECT COUNT(*) FROM GameData WHERE id = 1";
                int count = System.Convert.ToInt32(command.ExecuteScalar());

                if (count > 0)
                {
                    // 업데이트
                    command.CommandText = @"
                        UPDATE GameData SET 
                            highScore = @highScore,
                            gold = @gold,
                            maxHealthUpgradeLevel = @maxHealthUpgradeLevel,
                            attackDamageUpgradeLevel = @attackDamageUpgradeLevel,
                            moveSpeedUpgradeLevel = @moveSpeedUpgradeLevel,
                            attackSpeedUpgradeLevel = @attackSpeedUpgradeLevel,
                            healthRegenUpgradeLevel = @healthRegenUpgradeLevel,
                            increaseExpUpgradeLevel = @increaseExpUpgradeLevel
                        WHERE id = 1";
                }
                else
                {
                    // 새로 삽입
                    command.CommandText = @"
                        INSERT INTO GameData (id, highScore, gold, maxHealthUpgradeLevel, 
                                            attackDamageUpgradeLevel, moveSpeedUpgradeLevel, 
                                            attackSpeedUpgradeLevel, healthRegenUpgradeLevel, 
                                            increaseExpUpgradeLevel) 
                        VALUES (1, @highScore, @gold, @maxHealthUpgradeLevel, 
                               @attackDamageUpgradeLevel, @moveSpeedUpgradeLevel, 
                               @attackSpeedUpgradeLevel, @healthRegenUpgradeLevel, 
                               @increaseExpUpgradeLevel)";
                }

                // 파라미터 설정
                command.Parameters.AddWithValue("@highScore", score);
                command.Parameters.AddWithValue("@gold", gold);
                command.Parameters.AddWithValue("@maxHealthUpgradeLevel", maxHealthUpgradeLevel);
                command.Parameters.AddWithValue("@attackDamageUpgradeLevel", attackDamageUpgradeLevel);
                command.Parameters.AddWithValue("@moveSpeedUpgradeLevel", moveSpeedUpgradeLevel);
                command.Parameters.AddWithValue("@attackSpeedUpgradeLevel", attackSpeedUpgradeLevel);
                command.Parameters.AddWithValue("@healthRegenUpgradeLevel", healthRegenUpgradeLevel);
                command.Parameters.AddWithValue("@increaseExpUpgradeLevel", increaseExpUpgradeLevel);

                command.ExecuteNonQuery();
            }

            connection.Close();
        }

        Debug.Log("SQLite에 게임 데이터 저장 완료");
    }

    public void LoadData()
    {
        using (var connection = new SqliteConnection($"URI=file:{dbPath}"))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM GameData WHERE id = 1";

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        score = reader.GetInt32("highScore");
                        gold = reader.GetInt32("gold");
                        maxHealthUpgradeLevel = reader.GetInt32("maxHealthUpgradeLevel");
                        attackDamageUpgradeLevel = reader.GetInt32("attackDamageUpgradeLevel");
                        moveSpeedUpgradeLevel = reader.GetInt32("moveSpeedUpgradeLevel");
                        attackSpeedUpgradeLevel = reader.GetInt32("attackSpeedUpgradeLevel");
                        healthRegenUpgradeLevel = reader.GetInt32("healthRegenUpgradeLevel");
                        increaseExpUpgradeLevel = reader.GetInt32("increaseExpUpgradeLevel");

                        Debug.Log("SQLite에서 게임 데이터 로드 완료");
                    }
                    else
                    {
                        Debug.Log("저장된 데이터가 없습니다. 기본값으로 시작합니다.");
                    }
                }
            }

            connection.Close();
        }
    }

    public void ResetData()
    {
        using (var connection = new SqliteConnection($"URI=file:{dbPath}"))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "DELETE FROM GameData WHERE id = 1";
                command.ExecuteNonQuery();
            }

            connection.Close();
        }

        Debug.Log("SQLite 저장 데이터 삭제 완료");
    }

    // 애플리케이션 종료 시 자동 저장
    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            SaveData();
        }
    }

    void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            SaveData();
        }
    }

    void OnDestroy()
    {
        SaveData();
    }
}