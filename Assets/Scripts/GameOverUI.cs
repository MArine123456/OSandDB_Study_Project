using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    [Header("UI 컴포넌트")]
    public TextMeshProUGUI gameOverTitle;
    public Text highScoreText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI killText;
    public TextMeshProUGUI goldText;
    public TextMeshProUGUI playTimeText;
    public Button mainLobbyButton;

    [Header("Scene Settings")]
    public string LobbySceneName = "MainLobby";

    void Start()
    {
        // 버튼 클릭 이벤트 설정
        if (mainLobbyButton != null)
        {
            mainLobbyButton.onClick.AddListener(OnMainLobbyButtonClicked);
        }
    }

    public void SetGameOverData(int highScore, int score, int killCount, int gold, float playTime)
    {
        highScoreText.text = highScore.ToString();
        // 스코어 설정
        if (scoreText != null)
        {
            scoreText.text = score.ToString();
        }

        // 킬 카운트 설정
        if (killText != null)
        {
            killText.text = killCount.ToString();
        }

        // 골드 설정
        if (goldText != null)
        {
            goldText.text = gold.ToString();
        }

        // 플레이 타임 설정 (분:초 형식)
        if (playTimeText != null)
        {
            int minutes = Mathf.FloorToInt(playTime / 60);
            int seconds = Mathf.FloorToInt(playTime % 60);
            playTimeText.text = string.Format("{0:D2}:{1:D2}", minutes, seconds);
        }
    }

    void OnMainLobbyButtonClicked()
    {
        if(GameDataManager.Instance.score < GameManager.Instance.score)
        {
            GameDataManager.Instance.score = GameManager.Instance.score;
            GameDataManager.Instance.SetDirty();
        }
        SceneManager.LoadScene(LobbySceneName);
    }
}