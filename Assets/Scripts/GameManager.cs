using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("게임 UI")]
    public Text scoreText;
    public Text timeText;
    public Text levelText;
    public Slider healthSlider;
    public Slider expSlider;
    public GameObject levelUpPanel;
    public Transform levelUpButtonContainer;
    public Button levelUpButtonPrefab;

    [Header("게임오버 UI")]
    public GameObject gameOverPanel;

    [Header("게임 오브젝트")]
    public GameObject playerPrefab;
    public Camera mainCamera;

    // 게임 데이터
    public int score = 0;
    public float gameTime = 0f;
    public float noHitTime = 0f;
    public float scoreMultiplier = 1f;
    public bool isGamePaused = false;
    public int killCount = 0; // 킬 카운트 추가
    public int gold = 0; // 골드 추가

    private PlayerController player;
    private EnemySpawner enemySpawner;
    public GameDataManager gameData;

    void Awake()
    {
        gameData = GameObject.Find("GameDataManager").GetComponent<GameDataManager>();
        Instance = this;
        Time.timeScale = 1f;
    }

    void Start()
    {
        InitializeGame();
    }

    void Update()
    {
        if (!isGamePaused)
        {
            gameTime += Time.deltaTime;
            noHitTime += Time.deltaTime;

            // 시간당 점수 증가 (1초당 1점)
            score += Mathf.FloorToInt(Time.deltaTime);

            // 노히트 배수 계산 (10초마다 0.1씩 증가, 최대 3배)
            scoreMultiplier = Mathf.Min(3f, 1f + (noHitTime / 10f) * 0.1f);

            UpdateUI();
        }
    }

    void InitializeGame()
    {
        // 플레이어 생성
        GameObject playerObj = Instantiate(playerPrefab);
        player = playerObj.GetComponent<PlayerController>();

        // 카메라 설정
        CameraFollow cameraFollow = mainCamera.GetComponent<CameraFollow>();
        if (cameraFollow == null)
        {
            cameraFollow = mainCamera.gameObject.AddComponent<CameraFollow>();
        }
        cameraFollow.target = playerObj.transform;

        // 적 스포너 초기화
        enemySpawner = FindObjectOfType<EnemySpawner>();
        if (enemySpawner == null)
        {
            GameObject spawnerObj = new GameObject("EnemySpawner");
            enemySpawner = spawnerObj.AddComponent<EnemySpawner>();
        }

        levelUpPanel.SetActive(false);
        gameOverPanel.SetActive(false);
    }

    public void AddScore(int points)
    {
        score += Mathf.FloorToInt(points * scoreMultiplier);
    }

    public void AddKill()
    {
        killCount++;
    }

    public void PlayerHit()
    {
        noHitTime = 0f;
        scoreMultiplier = 1f;
    }

    public void ShowGameOver()
    {
        gold = player.gold;
        isGamePaused = true;
        Time.timeScale = 0f;
        gameOverPanel.SetActive(true);

        // 게임오버 패널의 GameOverUI 컴포넌트에 데이터 전달
        GameOverUI gameOverUI = gameOverPanel.GetComponent<GameOverUI>();
        if (gameOverUI != null)
        {
            gameOverUI.SetGameOverData(score, killCount, gold, gameTime);
        }
    }

    public void ShowLevelUpPanel(List<BuffData> buffOptions, float buttonPosition)
    {
        isGamePaused = true;
        Time.timeScale = 0f;
        levelUpPanel.SetActive(true);

        // 기존 버튼들 제거
        foreach (Transform child in levelUpButtonContainer)
        {
            Destroy(child.gameObject);
        }

        //새 버튼들 생성
        for (int i = 0; i < 3; i++)
        {
            var buff = buffOptions[i];

            Button button = Instantiate(levelUpButtonPrefab, levelUpButtonContainer);

            // 위치 조정 (X 방향으로 500씩 증가)
            RectTransform rectTransform = button.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.anchoredPosition = new Vector2(buttonPosition + (i + 1) * 500f, 0);
            }

            // 버튼 텍스트 및 기능 설정
            button.GetComponentInChildren<Text>().text = buff.name + "\n" + buff.description;
            button.onClick.AddListener(() => SelectBuff(buff));

            // 컴포넌트 강제 활성화 (필요 시)
            button.GetComponent<Image>().enabled = true;
            button.GetComponent<Button>().enabled = true;
            button.GetComponentInChildren<Text>().enabled = true;
        }
    }

    public void SelectBuff(BuffData buff)
    {
        player.ApplyBuff(buff);
        HideLevelUpPanel();
    }

    public void HideLevelUpPanel()
    {
        foreach (Transform child in levelUpButtonContainer)
        {
            Destroy(child.gameObject);
        }
        levelUpPanel.SetActive(false);
        isGamePaused = false;
        Time.timeScale = 1f;
    }

    public void UpdateUI()
    {
        if (scoreText) scoreText.text = "Score: " + score.ToString("N0");
        if (timeText) timeText.text = "Time: " + Mathf.FloorToInt(gameTime / 60) + ":" + (gameTime % 60).ToString("00");
        if (levelText) levelText.text = "Level: " + player.level;
        if (healthSlider)
        {
            healthSlider.value = (float)player.currentHealth / player.maxHealth;
        }
        if (expSlider)
        {
            expSlider.value = (float)player.currentExp / player.expToNextLevel;
        }
    }
}