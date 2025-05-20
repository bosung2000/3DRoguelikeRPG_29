using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TutorialController : MonoBehaviour
{
    [Serializable]
    public class TutorialStep
    {
        public string instructionText;
        public Func<bool> conditionToComplete;
        public Action onStepStart;
    }

    [Header("튜토리얼 단계 설정")]
    public List<TutorialStep> steps = new List<TutorialStep>();

    [Header("튜토리얼 오브젝트")]
    public GameObject monsterGroupp;
    public GameObject shopNPC;

    [Header("UI")]
    public GameObject instructionPanel;
    public TextMeshProUGUI instructionText;

    private int currentStepIndex = 0;
    private bool isRunning = false;

    private Vector3 startPosition;
    private Player player;
    private bool hasAttacked = false;
    private bool hasUsedSkill = false;
    private bool hasEquipped = false;
    private bool hasPurchased = false;
    private bool hasKilledEnemy = false;
    private bool hasUsedMap = false;
    private bool hasEquippedSkill = false;
    private bool hasOpenedMap = false;
    private bool hasMovedToNextRoom = false;


    private void Start()
    {
        player = FindObjectOfType<Player>();

        // 시작 위치 기억
        startPosition = player.transform.position;

        // 초기 상태로 오브젝트 숨김
        //if (monsterGroupp) monsterGroupp.SetActive(false);
        if (shopNPC) shopNPC.SetActive(false);

        SetupSteps();
        StartTutorial();
    }

    private void SetupSteps()
    {
        steps = new List<TutorialStep>
        {
            new TutorialStep
            {
                instructionText = "조이스틱 또는 방향키로 이동하세요.",
                conditionToComplete = () => Vector3.Distance(startPosition, player.transform.position) > 2f,
                onStepStart = () => Debug.Log("[튜토리얼] 이동 시작")
            },
            new TutorialStep
            {
                instructionText = "공격 버튼을 눌러 기본 공격을 해보세요.",
                conditionToComplete = () => hasAttacked,
                onStepStart = () => Debug.Log("[튜토리얼] 공격 시작")
            },
            new TutorialStep
            {
                instructionText = "스킬을 장착해보세요.",
                conditionToComplete = () => hasEquippedSkill,
                onStepStart = () => Debug.Log("[튜토리얼] 스킬 장착 시작")
            },
            new TutorialStep
            {
                instructionText = "스킬 버튼을 눌러 스킬을 사용하세요.",
                conditionToComplete = () => hasUsedSkill,
                onStepStart = () => Debug.Log("[튜토리얼] 스킬 사용 시작")
            },
            new TutorialStep
            {
                instructionText = "몬스터를 처치해보세요.",
                conditionToComplete = () => hasKilledEnemy,
                onStepStart = () =>
                {
                    Debug.Log("[튜토리얼] 몬스터 처치 시작");
                    if (monsterGroupp)
                    {
                        //monsterGroupp.SetActive(true);
                         var roomZone = monsterGroupp.GetComponent<RoomZone>();
                        if (roomZone != null)
                            roomZone.ActivateRoom();
                    }
                }
            },
            new TutorialStep
            {
                instructionText = "상점에서 아이템을 구매해보세요.",
                conditionToComplete = () => hasPurchased,
                onStepStart = () =>
                {
                    Debug.Log("[튜토리얼] 상점 이용 시작");
                    if (shopNPC) shopNPC.SetActive(true);
                }
            },
            new TutorialStep
            {
                instructionText = "장비를 인벤토리에서 장착해보세요.",
                conditionToComplete = () => hasEquipped,
                onStepStart = () => Debug.Log("[튜토리얼] 장비 장착 시작")
            },
            new TutorialStep
            {
                instructionText = "맵을 열고 다음 방으로 이동하세요.",
                conditionToComplete = () => hasMovedToNextRoom,
                onStepStart = () => Debug.Log("[튜토리얼] 맵 이동 시작")
            },

        };
    }

    public void OnAttackPerformed() => hasAttacked = true;
    public void OnSkillUsed() => hasUsedSkill = true;
    public void OnItemEquipped() => hasEquipped = true;
    public void OnItemPurchased() => hasPurchased = true;
    public void OnEnemyKilled() => hasKilledEnemy = true;
    public void OnMapUsed() => hasUsedMap = true;

    public void StartTutorial()
    {
        if (steps.Count == 0)
        {
            Debug.LogWarning("튜토리얼 단계가 비어있습니다.");
            return;
        }

        isRunning = true;
        StartCoroutine(RunTutorial());
    }

    private IEnumerator RunTutorial()
    {
        while (currentStepIndex < steps.Count)
        {
            var step = steps[currentStepIndex];

            instructionText.text = step.instructionText;
            instructionPanel.SetActive(true);

            step.onStepStart?.Invoke();

            yield return new WaitUntil(() => step.conditionToComplete?.Invoke() == true);

            currentStepIndex++;
        }

        FinishTutorial();
    }

    private void FinishTutorial()
    {
        isRunning = false;
        instructionPanel.SetActive(false);
        Debug.Log("튜토리얼 완료!");

        //재화초기화'
        CurrencyManager.Instance?.ResetCurrencyToInitial();

    }

    public void SkipTutorial()
    {
        StopAllCoroutines();
        FinishTutorial();
    }

    public void OnSkillEquipped()
    {
        hasEquippedSkill = true;
    }
    public void OnMapOpened()
    {
        hasOpenedMap = true;
    }

    public void OnRoomChanged()
    {
        if (hasOpenedMap)
        {
            hasMovedToNextRoom = true;
        }
    }

}
