using UnityEngine;
using UnityEngine.UI;
using System;

public class NicknameManager : MonoBehaviour
{
    [Header("닉네임 UI 컴포넌트")]
    [Tooltip("닉네임 입력 패널")]
    public GameObject nicknamePanel;

    [Tooltip("닉네임 입력 필드")]
    public InputField nicknameInputField;

    [Tooltip("닉네임 확인 버튼")]
    public Button nicknameConfirmButton;

    [Tooltip("닉네임 표시 텍스트")]
    public Text nicknameDisplayText;

    [Tooltip("닉네임 초기화 버튼 (선택사항)")]
    public Button nicknameResetButton;

    [Header("닉네임 설정")]
    [Tooltip("닉네임 최대 길이")]
    [Range(1, 50)]
    public int maxNicknameLength = 20;

    [Tooltip("자동으로 닉네임 패널 표시 여부")]
    public bool autoShowPanelOnStart = true;

    // 이벤트 정의
    public static event Action<string> OnNicknameChanged;
    public static event Action OnNicknameConfirmed;
    public static event Action OnNicknamePanelShown;
    public static event Action OnNicknamePanelHidden;
    public static event Action OnNicknameReset; // 새로운 이벤트 추가

    // 상수
    private const string NICKNAME_SAVE_KEY = "PlayerNickname";

    // 프라이빗 변수
    private string _currentNickname = "";
    private bool _isNicknameSet = false;

    //현재 있는 닉네임을 반환
    public string CurrentNickname
    {
        get => _currentNickname;
        private set
        {
            if (_currentNickname != value)
            {
                _currentNickname = value;
                OnNicknameChanged?.Invoke(_currentNickname);
            }
        }
    }

    public bool IsNicknameSet => _isNicknameSet;

    void Start()
    {
        InitializeNicknameSystem();
    }

    private void InitializeNicknameSystem()
    {
        SetupUI();
        LoadSavedNickname();

        // 저장된 닉네임이 없을 때만 패널 표시
        if (autoShowPanelOnStart && !_isNicknameSet)
        {
            ShowNicknamePanel();
        }
        else if (_isNicknameSet)
        {
            // 닉네임이 이미 설정되어 있다면 패널을 숨기고 UI 업데이트
            HideNicknamePanel();
            UpdateNicknameDisplay();

            // 메인 UI가 보이도록 GameLobbyManager에 알림
            OnNicknameConfirmed?.Invoke();
        }
    }

    private void SetupUI()
    {
        // 닉네임 확인 버튼 이벤트 연결
        if (nicknameConfirmButton != null)
        {
            nicknameConfirmButton.onClick.AddListener(OnConfirmButtonClicked);
        }
        else
        {
            Debug.LogWarning("[NicknameManager] Nickname Confirm Button이 할당되지 않았습니다!");
        }

        // 닉네임 초기화 버튼 이벤트 연결 (새로 추가)
        if (nicknameResetButton != null)
        {
            nicknameResetButton.onClick.AddListener(OnResetButtonClicked);
        }

        // Enter 키로도 닉네임 확인 가능하도록 설정
        if (nicknameInputField != null)
        {
            nicknameInputField.onEndEdit.AddListener(OnInputFieldEndEdit);
        }
        else
        {
            Debug.LogWarning("[NicknameManager] Nickname Input Field가 할당되지 않았습니다!");
        }
    }

    // 저장된 닉네임을 불러옴
    private void LoadSavedNickname()
    {
        if (PlayerPrefs.HasKey(NICKNAME_SAVE_KEY))
        {
            string savedNickname = PlayerPrefs.GetString(NICKNAME_SAVE_KEY);

            if (IsValidNickname(savedNickname))
            {
                SetNickname(savedNickname, false); // 저장하지 않음 (이미 저장된 값이므로)
                _isNicknameSet = true;
                UpdateNicknameDisplay();
                Debug.Log($"[NicknameManager] 저장된 닉네임 로드: {savedNickname}");
            }
        }
    }

    // 닉네임 패널을 표시
    public void ShowNicknamePanel()
    {
        if (nicknamePanel != null)
        {
            nicknamePanel.SetActive(true);

            // 입력 필드에 포커스 설정
            if (nicknameInputField != null)
            {
                nicknameInputField.Select();
                nicknameInputField.ActivateInputField();

                // 기존 닉네임이 있다면 입력 필드에 표시
                if (_isNicknameSet)
                {
                    nicknameInputField.text = CurrentNickname;
                }
            }

            OnNicknamePanelShown?.Invoke();
            Debug.Log("[NicknameManager] 닉네임 패널이 표시되었습니다.");
        }
        else
        {
            Debug.LogWarning("[NicknameManager] Nickname Panel이 할당되지 않았습니다!");
        }
    }

    //닉네임 패널 숨기기
    public void HideNicknamePanel()
    {
        if (nicknamePanel != null)
        {
            nicknamePanel.SetActive(false);
            OnNicknamePanelHidden?.Invoke();
            Debug.Log("[NicknameManager] 닉네임 패널이 숨겨졌습니다.");
        }
    }

    private void UpdateNicknameDisplay()
    {
        if (nicknameDisplayText != null && _isNicknameSet)
        {
            nicknameDisplayText.text = CurrentNickname;
            nicknameDisplayText.gameObject.SetActive(true);
        }
    }

    private void OnInputFieldEndEdit(string input)
    {
        // Enter 키가 눌렸을 때만 처리
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            ConfirmNickname();
        }
    }

    /// <summary>
    /// 확인 버튼 클릭 시 호출되는 메서드
    /// </summary>
    private void OnConfirmButtonClicked()
    {
        ConfirmNickname();
    }

    //개발용 초기화 버튼 이벤트
    private void OnResetButtonClicked()
    {
        ResetNickname();
    }

    private void ConfirmNickname()
    {
        if (nicknameInputField == null)
        {
            Debug.LogError("[NicknameManager] Nickname Input Field가 없습니다!");
            return;
        }

        string inputNickname = nicknameInputField.text.Trim();

        // 닉네임 유효성 검사
        if (!IsValidNickname(inputNickname))
        {
            return;
        }

        // 닉네임 설정 및 저장
        SetNickname(inputNickname, true);
        _isNicknameSet = true;

        // UI 업데이트
        UpdateNicknameDisplay();
        HideNicknamePanel();

        // 이벤트 발생
        OnNicknameConfirmed?.Invoke();

        Debug.Log($"[NicknameManager] 닉네임이 설정되었습니다: {inputNickname}");
    }

    //개발용(닉네임 초기화)
    public void ResetNickname()
    {
        GameDataManager.Instance.ResetData();

        Debug.Log("[NicknameManager] 닉네임을 초기화합니다.");

        // 닉네임 데이터 초기화
        CurrentNickname = "";
        _isNicknameSet = false;

        // PlayerPrefs에서 저장된 닉네임 삭제
        if (PlayerPrefs.HasKey(NICKNAME_SAVE_KEY))
        {
            PlayerPrefs.DeleteKey(NICKNAME_SAVE_KEY);
            PlayerPrefs.Save();
        }

        // UI 초기화
        if (nicknameDisplayText != null)
        {
            nicknameDisplayText.gameObject.SetActive(false);
        }

        if (nicknameInputField != null)
        {
            nicknameInputField.text = "";
        }

        // 이벤트 발생
        OnNicknameReset?.Invoke();

        // 닉네임 패널 표시
        ShowNicknamePanel();

        Debug.Log("[NicknameManager] 닉네임이 초기화되었습니다.");
    }

    private bool IsValidNickname(string nickname)
    {
        // 빈 문자열 검사
        if (string.IsNullOrEmpty(nickname))
        {
            Debug.LogWarning("[NicknameManager] 닉네임을 입력해주세요!");
            return false;
        }

        // 길이 검사
        if (nickname.Length > maxNicknameLength)
        {
            Debug.LogWarning($"[NicknameManager] 닉네임은 {maxNicknameLength}자 이하로 입력해주세요!");
            return false;
        }

        return true;
    }

    private void SetNickname(string nickname, bool saveToPlayerPrefs)
    {
        CurrentNickname = nickname;

        if (saveToPlayerPrefs)
        {
            SaveNickname(nickname);
        }
    }

    private void SaveNickname(string nickname)
    {
        PlayerPrefs.SetString(NICKNAME_SAVE_KEY, nickname);
        PlayerPrefs.Save();
        Debug.Log($"[NicknameManager] 닉네임이 저장되었습니다: {nickname}");
    }

    public bool SetNicknameExternal(string nickname)
    {
        if (!IsValidNickname(nickname))
        {
            return false;
        }

        SetNickname(nickname, true);
        _isNicknameSet = true;
        UpdateNicknameDisplay();

        return true;
    }

    void OnDestroy()
    {
        // 이벤트 리스너 정리
        if (nicknameConfirmButton != null)
        {
            nicknameConfirmButton.onClick.RemoveListener(OnConfirmButtonClicked);
        }

        if (nicknameResetButton != null)
        {
            nicknameResetButton.onClick.RemoveListener(OnResetButtonClicked);
        }

        if (nicknameInputField != null)
        {
            nicknameInputField.onEndEdit.RemoveListener(OnInputFieldEndEdit);
        }
    }
}