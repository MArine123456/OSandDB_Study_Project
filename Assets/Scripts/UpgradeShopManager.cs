using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class UpgradeShopManager : MonoBehaviour
{
    [Header("Shop Panel")]
    public GameObject upgradeShopPanel;
    public GameDataManager gameData;
    public Button closeButton;

    [Header("Upgrade Buttons")]
    public Button maxHealthButton;
    public Button attackDamageButton;
    public Button moveSpeedButton;
    public Button attackSpeedButton;
    public Button healthGenButton;
    public Button increaseExpButton;

    [Header("Level Display Texts")]
    public TextMeshProUGUI maxHealthLevelText;
    public TextMeshProUGUI attackDamageLevelText;
    public TextMeshProUGUI moveSpeedLevelText;
    public TextMeshProUGUI attackSpeedLevelText;
    public TextMeshProUGUI healthGenLevelText;
    public TextMeshProUGUI increaseExpLevelText;

    public Text maxHealthUpgradeGoldText;
    public Text attackDamageUpgradeGoldText;
    public Text attackSpeedUpgradeGoldText;
    public Text moveSpeedUpgradeGoldText;
    public Text healthRegenUpgradeGoldText;
    public Text increaseExpUpgradeGoldText;

    [Header("Hover Effect Settings")]
    public Color normalButtonColor = Color.white;
    public Color hoverButtonColor = Color.yellow;
    public Color disabledButtonColor = Color.gray;

    [Header("Text Color Settings")]
    public Color normalTextColor = Color.white;
    public Color maxLevelTextColor = Color.red; // 최고 레벨 도달 시 텍스트 색상

    public Text goldText;

    // 각 능력치의 현재 레벨 (1~10)
    private int maxHealthLevel = 1;
    private int attackDamageLevel = 1;
    private int moveSpeedLevel = 1;
    private int attackSpeedLevel = 1;
    private int HealthRegenLevel = 1;
    private int IncreaseExpLevel = 1;

    private int maxHealthNeedUpgradeGold = 100;
    private int attackDamageNeedUpgradeGold = 100;
    private int moveSpeedNeedUpgradeGold = 100;
    private int attackSpeedNeedUpgradeGold = 100;
    private int HealthRegenNeedUpgradeGold = 100;
    private int IncreaseExpNeedUpgradeGold = 100;

    private const int MAX_LEVEL = 10; // 최대 업그레이드 레벨

    // 버튼의 원래 색상 저장
    private Color maxHealthOriginalColor;
    private Color attackDamageOriginalColor;
    private Color moveSpeedOriginalColor;
    private Color attackSpeedOriginalColor;
    private Color HealthRegenOriginalColor;
    private Color IncreaseExpOriginalColor;

    private void Awake()
    {
        gameData = GameObject.Find("GameDataManager").GetComponent<GameDataManager>();
    }

    void Start()
    {
        CloseUpgradeShop();

        SetupButtonEvents(); // 버튼 클릭 이벤트 설정
        StoreOriginalColors(); // 버튼 원래 색상 저장
        SetupHoverEffects(); // 버튼에 마우스 오버 효과 설정
        UpdateAllLevelDisplays(); // 레벨 및 골드 텍스트 초기화
    }

    void Update()
    {
        // ESC 키로 업그레이드 패널 닫기
        if (upgradeShopPanel != null && upgradeShopPanel.activeInHierarchy)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                CloseUpgradeShop();
            }
        }
    }

    // 버튼 클릭 이벤트 설정
    void SetupButtonEvents()
    {
        if (closeButton != null)
            closeButton.onClick.AddListener(CloseUpgradeShop);

        if (maxHealthButton != null)
            maxHealthButton.onClick.AddListener(() => UpgradeAbility("MaxHealth"));

        if (attackDamageButton != null)
            attackDamageButton.onClick.AddListener(() => UpgradeAbility("AttackDamage"));

        if (moveSpeedButton != null)
            moveSpeedButton.onClick.AddListener(() => UpgradeAbility("MoveSpeed"));

        if (attackSpeedButton != null)
            attackSpeedButton.onClick.AddListener(() => UpgradeAbility("AttackSpeed"));

        if (healthGenButton != null)
            healthGenButton.onClick.AddListener(() => UpgradeAbility("HealthRegen"));

        if (increaseExpButton != null)
            increaseExpButton.onClick.AddListener(() => UpgradeAbility("IncreaseExp"));
    }

    // 각 버튼의 원래 색상을 저장
    void StoreOriginalColors()
    {
        if (maxHealthButton != null)
            maxHealthOriginalColor = maxHealthButton.GetComponent<Image>().color;
        if (attackDamageButton != null)
            attackDamageOriginalColor = attackDamageButton.GetComponent<Image>().color;
        if (moveSpeedButton != null)
            moveSpeedOriginalColor = moveSpeedButton.GetComponent<Image>().color;
        if (attackSpeedButton != null)
            attackSpeedOriginalColor = attackSpeedButton.GetComponent<Image>().color;
        if (healthGenButton != null)
            HealthRegenOriginalColor = healthGenButton.GetComponent<Image>().color;
        if (increaseExpButton != null)
            IncreaseExpOriginalColor = increaseExpButton.GetComponent<Image>().color;
    }

    // 모든 버튼에 마우스 오버 효과 추가
    void SetupHoverEffects()
    {
        AddHoverEffect(maxHealthButton, maxHealthOriginalColor, () => maxHealthLevel);
        AddHoverEffect(attackDamageButton, attackDamageOriginalColor, () => attackDamageLevel);
        AddHoverEffect(moveSpeedButton, moveSpeedOriginalColor, () => moveSpeedLevel);
        AddHoverEffect(attackSpeedButton, attackSpeedOriginalColor, () => attackSpeedLevel);
        AddHoverEffect(healthGenButton, HealthRegenOriginalColor, () => HealthRegenLevel);
        AddHoverEffect(increaseExpButton, IncreaseExpOriginalColor, () => IncreaseExpLevel);
    }

    // 버튼에 마우스 오버 효과 설정
    void AddHoverEffect(Button button, Color originalColor, System.Func<int> levelGetter)
    {
        if (button == null) return;

        EventTrigger trigger = button.GetComponent<EventTrigger>() ?? button.gameObject.AddComponent<EventTrigger>();

        var pointerEnter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
        pointerEnter.callback.AddListener((data) => {
            if (levelGetter() < MAX_LEVEL)
                button.GetComponent<Image>().color = hoverButtonColor;
        });
        trigger.triggers.Add(pointerEnter);

        var pointerExit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
        pointerExit.callback.AddListener((data) => {
            button.GetComponent<Image>().color = levelGetter() >= MAX_LEVEL ? disabledButtonColor : originalColor;
        });
        trigger.triggers.Add(pointerExit);
    }

    // 업그레이드 상점 열기
    public void OpenUpgradeShop()
    {
        if (upgradeShopPanel != null)
        {
            upgradeShopPanel.SetActive(true);
            UpdateCurrentGold();
            Debug.Log("업그레이드 상점 열림");
        }
    }

    // 업그레이드 상점 닫기
    public void CloseUpgradeShop()
    {
        if (upgradeShopPanel != null)
        {
            upgradeShopPanel.SetActive(false);
            Debug.Log("업그레이드 상점 닫힘");
        }
    }

    // 특정 능력 업그레이드 처리
    void UpgradeAbility(string abilityName)
    {
        switch (abilityName)
        {
            case "MaxHealth":
                if (maxHealthLevel < MAX_LEVEL && gameData.gold >= maxHealthNeedUpgradeGold)
                {
                    gameData.gold -= maxHealthNeedUpgradeGold;
                    maxHealthLevel++;
                    maxHealthNeedUpgradeGold = maxHealthLevel * 100;
                    UpdateLevelDisplay(maxHealthLevelText, maxHealthLevel, maxHealthUpgradeGoldText, maxHealthNeedUpgradeGold);
                    UpdateButtonState(maxHealthButton, maxHealthLevel, maxHealthOriginalColor);
                    Debug.Log($"최대 체력 업그레이드 -> 현재 레벨: {maxHealthLevel}");
                }
                break;
            case "AttackDamage":
                if (attackDamageLevel < MAX_LEVEL && gameData.gold >= attackDamageNeedUpgradeGold)
                {
                    gameData.gold -= attackDamageNeedUpgradeGold;
                    attackDamageLevel++;
                    attackDamageNeedUpgradeGold = attackDamageLevel * 100;
                    UpdateLevelDisplay(attackDamageLevelText, attackDamageLevel, attackDamageUpgradeGoldText, attackDamageNeedUpgradeGold);
                    UpdateButtonState(attackDamageButton, attackDamageLevel, attackDamageOriginalColor);
                    Debug.Log($"공격력 업그레이드 -> 현재 레벨: {attackDamageLevel}");
                }
                break;
            case "MoveSpeed":
                if (moveSpeedLevel < MAX_LEVEL && gameData.gold >= moveSpeedNeedUpgradeGold)
                {
                    gameData.gold -= moveSpeedNeedUpgradeGold;
                    moveSpeedLevel++;
                    moveSpeedNeedUpgradeGold = moveSpeedLevel * 100;
                    UpdateLevelDisplay(moveSpeedLevelText, moveSpeedLevel, moveSpeedUpgradeGoldText, moveSpeedNeedUpgradeGold);
                    UpdateButtonState(moveSpeedButton, moveSpeedLevel, moveSpeedOriginalColor);
                    Debug.Log($"이동 속도 업그레이드 -> 현재 레벨: {moveSpeedLevel}");
                }
                break;
            case "AttackSpeed":
                if (attackSpeedLevel < MAX_LEVEL && gameData.gold >= attackSpeedNeedUpgradeGold)
                {
                    gameData.gold -= attackSpeedNeedUpgradeGold;
                    attackSpeedLevel++;
                    attackSpeedNeedUpgradeGold = attackSpeedLevel * 100;
                    UpdateLevelDisplay(attackSpeedLevelText, attackSpeedLevel, attackSpeedUpgradeGoldText, attackSpeedNeedUpgradeGold);
                    UpdateButtonState(attackSpeedButton, attackSpeedLevel, attackSpeedOriginalColor);
                    Debug.Log($"공격 속도 업그레이드 -> 현재 레벨: {attackSpeedLevel}");
                }
                break;
            case "HealthRegen":
                if (HealthRegenLevel < MAX_LEVEL && gameData.gold >= HealthRegenNeedUpgradeGold)
                {
                    gameData.gold -= HealthRegenNeedUpgradeGold;
                    HealthRegenLevel++;
                    HealthRegenNeedUpgradeGold = HealthRegenLevel * 100;
                    UpdateLevelDisplay(healthGenLevelText, HealthRegenLevel, healthRegenUpgradeGoldText, HealthRegenNeedUpgradeGold);
                    UpdateButtonState(healthGenButton, HealthRegenLevel, HealthRegenOriginalColor);
                    Debug.Log($"체력 회복 업그레이드 -> 현재 레벨: {HealthRegenLevel}");
                }
                break;
            case "IncreaseExp":
                if (IncreaseExpLevel < MAX_LEVEL && gameData.gold >= IncreaseExpNeedUpgradeGold)
                {
                    gameData.gold -= IncreaseExpNeedUpgradeGold;
                    IncreaseExpLevel++;
                    IncreaseExpNeedUpgradeGold = IncreaseExpLevel * 100;
                    UpdateLevelDisplay(increaseExpLevelText, IncreaseExpLevel, increaseExpUpgradeGoldText, IncreaseExpNeedUpgradeGold);
                    UpdateButtonState(increaseExpButton, IncreaseExpLevel, IncreaseExpOriginalColor);
                    Debug.Log($"경험치 획득량 증가 업그레이드 -> 현재 레벨: {IncreaseExpLevel}");
                }
                break;
        }
        UpdateCurrentGold();
    }

    // 각 능력의 레벨과 필요 골드를 표시하는 UI 갱신
    void UpdateLevelDisplay(TextMeshProUGUI levelText, int level, Text upgradeGold, int needUpgradeGold)
    {
        if (levelText != null)
        {
            levelText.text = $"Lv : {level}";
            upgradeGold.text = needUpgradeGold.ToString();

            if (level >= MAX_LEVEL)
            {
                levelText.color = maxLevelTextColor;
                levelText.fontStyle = FontStyles.Bold;
            }
            else
            {
                levelText.color = normalTextColor;
                levelText.fontStyle = FontStyles.Normal;
            }
        }
    }

    // 버튼 상태 갱신 (최대 레벨이면 비활성화)
    void UpdateButtonState(Button button, int level, Color originalColor)
    {
        if (button == null) return;

        button.interactable = level < MAX_LEVEL;
        button.GetComponent<Image>().color = level >= MAX_LEVEL ? disabledButtonColor : originalColor;
    }

    // 모든 레벨 UI 갱신
    void UpdateAllLevelDisplays()
    {
        UpdateLevelDisplay(maxHealthLevelText, maxHealthLevel, maxHealthUpgradeGoldText, maxHealthNeedUpgradeGold);
        UpdateLevelDisplay(attackDamageLevelText, attackDamageLevel, attackDamageUpgradeGoldText, attackDamageNeedUpgradeGold);
        UpdateLevelDisplay(moveSpeedLevelText, moveSpeedLevel, moveSpeedUpgradeGoldText, moveSpeedNeedUpgradeGold);
        UpdateLevelDisplay(attackSpeedLevelText, attackSpeedLevel, attackSpeedUpgradeGoldText, attackSpeedNeedUpgradeGold);
        UpdateLevelDisplay(healthGenLevelText, HealthRegenLevel, healthRegenUpgradeGoldText, HealthRegenNeedUpgradeGold);
        UpdateLevelDisplay(increaseExpLevelText, IncreaseExpLevel, increaseExpUpgradeGoldText, IncreaseExpNeedUpgradeGold);
    }

    // 현재 골드를 UI에 표시
    void UpdateCurrentGold()
    {
        goldText.text = gameData.gold.ToString();
    }

    // 오브젝트가 파괴될 때 이벤트 정리
    void OnDestroy()
    {
        if (closeButton != null) closeButton.onClick.RemoveAllListeners();
        if (maxHealthButton != null) { maxHealthButton.onClick.RemoveAllListeners(); ClearEventTriggers(maxHealthButton); }
        if (attackDamageButton != null) { attackDamageButton.onClick.RemoveAllListeners(); ClearEventTriggers(attackDamageButton); }
        if (moveSpeedButton != null) { moveSpeedButton.onClick.RemoveAllListeners(); ClearEventTriggers(moveSpeedButton); }
        if (attackSpeedButton != null) { attackSpeedButton.onClick.RemoveAllListeners(); ClearEventTriggers(attackSpeedButton); }
        if (healthGenButton != null) { healthGenButton.onClick.RemoveAllListeners(); ClearEventTriggers(healthGenButton); }
        if (increaseExpButton != null) { increaseExpButton.onClick.RemoveAllListeners(); ClearEventTriggers(increaseExpButton); }
    }

    // 버튼의 EventTrigger를 정리하는 함수
    void ClearEventTriggers(Button button)
    {
        EventTrigger trigger = button.GetComponent<EventTrigger>();
        if (trigger != null)
        {
            trigger.triggers.Clear();
        }
    }
}
