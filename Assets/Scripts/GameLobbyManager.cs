using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GameLobbyManager : MonoBehaviour
{
    [Header("UI Buttons")]
    public Button gameStartButton;
    public Button upgradeShopButton;
    public Button exitButton;
    public Button nicknameChangeButton; // 닉네임 변경 버튼 추가

    [Header("Main UI")]
    public GameObject mainUIPanel;

    [Header("Managers")]
    public NicknameManager nicknameManager;
    public UpgradeShopManager upgradeShopManager;

    [Header("Hover Effect Settings")]
    public Color normalColor = Color.white;
    public Color hoverColor = Color.yellow;

    [Header("Scene Settings")]
    public string gameSceneName = "SampleScene";

    // 각 버튼의 원래 색상을 저장하는 변수들
    private Color gameStartOriginalColor;
    private Color upgradeShopOriginalColor;
    private Color exitOriginalColor;
    private Color nicknameChangeOriginalColor; // 닉네임 변경 버튼 색상 추가

    void Start()
    {
        // 컴포넌트 초기화
        InitializeComponents();

        // 버튼 클릭 이벤트 연결
        SetupButtons();

        // 각 버튼의 원래 색상 저장
        StoreOriginalColors();

        // 마우스 호버 효과 설정
        SetupHoverEffects();

        // 닉네임 이벤트 구독
        SubscribeToNicknameEvents();
    }

    void InitializeComponents()
    {
        // NicknameManager 확인
        if (nicknameManager == null)
        {
            nicknameManager = FindObjectOfType<NicknameManager>();
            if (nicknameManager == null)
            {
                Debug.LogWarning("[GameLobbyManager] NicknameManager를 찾을 수 없습니다!");
            }
        }

        // 닉네임이 설정되어 있는지에 따라 UI 상태 결정
        UpdateUIVisibility();
    }

    void UpdateUIVisibility()
    {
        if (nicknameManager != null && mainUIPanel != null)
        {
            // 닉네임이 설정되어 있으면 메인 UI 표시, 아니면 숨김
            mainUIPanel.SetActive(nicknameManager.IsNicknameSet);
        }
    }

    void SetupButtons()
    {
        // GameStart 버튼 이벤트 연결
        if (gameStartButton != null)
        {
            gameStartButton.onClick.AddListener(OnGameStartClicked);
        }
        else
        {
            Debug.LogWarning("[GameLobbyManager] GameStart Button이 할당되지 않았습니다!");
        }

        // UpgradeShop 버튼 이벤트 연결
        if (upgradeShopButton != null)
        {
            upgradeShopButton.onClick.AddListener(OnUpgradeShopClicked);
        }
        else
        {
            Debug.LogWarning("[GameLobbyManager] UpgradeShop Button이 할당되지 않았습니다!");
        }

        // Exit 버튼 이벤트 연결
        if (exitButton != null)
        {
            exitButton.onClick.AddListener(OnExitClicked);
        }
        else
        {
            Debug.LogWarning("[GameLobbyManager] Exit Button이 할당되지 않았습니다!");
        }

        // 닉네임 변경 버튼 이벤트 연결 (새로 추가)
        if (nicknameChangeButton != null)
        {
            nicknameChangeButton.onClick.AddListener(OnNicknameChangeClicked);
        }
        else
        {
            Debug.LogWarning("[GameLobbyManager] Nickname Change Button이 할당되지 않았습니다!");
        }
    }

    void StoreOriginalColors()
    {
        if (gameStartButton != null)
            gameStartOriginalColor = gameStartButton.GetComponent<Image>().color;

        if (upgradeShopButton != null)
            upgradeShopOriginalColor = upgradeShopButton.GetComponent<Image>().color;

        if (exitButton != null)
            exitOriginalColor = exitButton.GetComponent<Image>().color;

        if (nicknameChangeButton != null)
            nicknameChangeOriginalColor = nicknameChangeButton.GetComponent<Image>().color;
    }

    /// <summary>
    /// 모든 버튼에 마우스 호버 효과를 설정하는 메서드
    /// </summary>
    void SetupHoverEffects()
    {
        AddHoverEffect(gameStartButton, gameStartOriginalColor);
        AddHoverEffect(upgradeShopButton, upgradeShopOriginalColor);
        AddHoverEffect(exitButton, exitOriginalColor);
        AddHoverEffect(nicknameChangeButton, nicknameChangeOriginalColor);
    }

    void AddHoverEffect(Button button, Color originalColor)
    {
        if (button == null) return;

        // EventTrigger 컴포넌트 추가 또는 가져오기
        EventTrigger trigger = button.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = button.gameObject.AddComponent<EventTrigger>();
        }

        // 마우스가 버튼 위로 올라갈 때의 이벤트 설정
        EventTrigger.Entry pointerEnter = new EventTrigger.Entry();
        pointerEnter.eventID = EventTriggerType.PointerEnter;
        pointerEnter.callback.AddListener((data) => {
            button.GetComponent<Image>().color = hoverColor;
        });
        trigger.triggers.Add(pointerEnter);

        // 마우스가 버튼에서 벗어날 때의 이벤트 설정
        EventTrigger.Entry pointerExit = new EventTrigger.Entry();
        pointerExit.eventID = EventTriggerType.PointerExit;
        pointerExit.callback.AddListener((data) => {
            button.GetComponent<Image>().color = originalColor;
        });
        trigger.triggers.Add(pointerExit);
    }

    void SubscribeToNicknameEvents()
    {
        // 닉네임이 확인되면 메인 UI 표시
        NicknameManager.OnNicknameConfirmed += OnNicknameConfirmed;

        // 닉네임 패널이 표시되면 메인 UI 숨김
        NicknameManager.OnNicknamePanelShown += OnNicknamePanelShown;

        // 닉네임이 초기화되면 메인 UI 숨김 (새로 추가)
        NicknameManager.OnNicknameReset += OnNicknameReset;
    }

    void OnNicknameConfirmed()
    {
        UpdateUIVisibility();
        Debug.Log("[GameLobbyManager] 닉네임 확인됨, 메인 UI 표시");
    }

    /// <summary>
    /// 닉네임 패널이 표시되었을 때 호출되는 메서드
    /// </summary>
    void OnNicknamePanelShown()
    {
        if (mainUIPanel != null)
        {
            mainUIPanel.SetActive(false);
        }
        Debug.Log("[GameLobbyManager] 닉네임 패널 표시됨, 메인 UI 숨김");
    }

    //개발용 닉네임이 리셋되면 호출
    void OnNicknameReset()
    {
        if (mainUIPanel != null)
        {
            mainUIPanel.SetActive(false);
        }
        Debug.Log("[GameLobbyManager] 닉네임 초기화됨, 메인 UI 숨김");
    }

    /// <summary>
    /// Game Start 버튼 클릭 시 호출되는 메서드
    /// </summary>
    void OnGameStartClicked()
    {
        // 닉네임이 설정되지 않았다면 게임 시작 불가
        if (nicknameManager != null && !nicknameManager.IsNicknameSet)
        {
            Debug.LogWarning("[GameLobbyManager] 닉네임을 먼저 설정해주세요!");
            nicknameManager.ShowNicknamePanel();
            return;
        }

        Debug.Log("[GameLobbyManager] 게임 시작!");

        // 지정한 씬으로 전환 시도
        try
        {
            SceneManager.LoadScene(gameSceneName);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[GameLobbyManager] 씬 로드 실패: {gameSceneName}. 오류: {e.Message}");
            Debug.LogError("Build Settings에 씬이 추가되어 있는지 확인하세요!");
        }
    }

    /// <summary>
    /// UpgradeShop 버튼 클릭 시 호출되는 메서드
    /// </summary>
    void OnUpgradeShopClicked()
    {
        Debug.Log("[GameLobbyManager] 업그레이드 상점 버튼 클릭됨!");

        if (upgradeShopManager != null)
        {
            upgradeShopManager.OpenUpgradeShop();
        }
        else
        {
            Debug.LogWarning("[GameLobbyManager] UpgradeShopManager가 할당되지 않았습니다!");
        }
    }

    /// <summary>
    /// Exit 버튼 클릭 시 호출되는 메서드
    /// </summary>
    void OnExitClicked()
    {
        Debug.Log("[GameLobbyManager] 게임 종료!");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    /// <summary>
    /// 닉네임 변경 버튼 클릭 시 호출되는 메서드 (새로 추가)
    /// </summary>
    void OnNicknameChangeClicked()
    {
        Debug.Log("[GameLobbyManager] 닉네임 변경 버튼 클릭됨!");

        if (nicknameManager != null)
        {
            nicknameManager.ResetNickname();
        }
        else
        {
            Debug.LogWarning("[GameLobbyManager] NicknameManager가 할당되지 않았습니다!");
        }
    }

    /// <summary>
    /// 현재 플레이어의 닉네임을 반환하는 공개 메서드
    /// </summary>
    public string GetPlayerNickname()
    {
        if (nicknameManager != null)
        {
            return nicknameManager.CurrentNickname;
        }
        return "";
    }

    void OnDestroy()
    {
        // 이벤트 구독 해제
        NicknameManager.OnNicknameConfirmed -= OnNicknameConfirmed;
        NicknameManager.OnNicknamePanelShown -= OnNicknamePanelShown;
        NicknameManager.OnNicknameReset -= OnNicknameReset;

        // 버튼 이벤트 정리
        if (gameStartButton != null)
        {
            gameStartButton.onClick.RemoveListener(OnGameStartClicked);
            ClearEventTrigger(gameStartButton);
        }

        if (upgradeShopButton != null)
        {
            upgradeShopButton.onClick.RemoveListener(OnUpgradeShopClicked);
            ClearEventTrigger(upgradeShopButton);
        }

        if (exitButton != null)
        {
            exitButton.onClick.RemoveListener(OnExitClicked);
            ClearEventTrigger(exitButton);
        }

        if (nicknameChangeButton != null)
        {
            nicknameChangeButton.onClick.RemoveListener(OnNicknameChangeClicked);
            ClearEventTrigger(nicknameChangeButton);
        }
    }

    /// <summary>
    /// EventTrigger의 이벤트들을 정리하는 헬퍼 메서드
    /// </summary>
    /// <param name="button">정리할 버튼</param>
    void ClearEventTrigger(Button button)
    {
        EventTrigger trigger = button.GetComponent<EventTrigger>();
        if (trigger != null)
        {
            trigger.triggers.Clear();
        }
    }
}