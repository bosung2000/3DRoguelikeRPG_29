using UnityEngine;
using UnityEngine.UI;

public class UITutorialPrompt : PopupUI
{
    [SerializeField] private Button yesButton;
    [SerializeField] private Button noButton;
    public Transform tutorialStartPoint;

    private GameData gameData;

    private void Start()
    {
        yesButton.onClick.AddListener(OnYesClicked);
        noButton.onClick.AddListener(OnNoClicked);
    }

    public void Initialize(GameData data)
    {
        gameData = data;
    }

    private void OnYesClicked()
    {
        Player player = FindObjectOfType<Player>();

        if (tutorialStartPoint == null)
        {
            GameObject found = GameObject.Find("TutorialStartPoint");
            if (found != null)
            {
                tutorialStartPoint = found.transform;
            }
            else
            {
                Debug.LogWarning("튜토리얼 스폰 포인트 찾기 실패.");
            }
        }

        // 이동 처리
        if (player != null && tutorialStartPoint != null)
        {
            player.transform.position = tutorialStartPoint.position;
        }

        gameData.isTutorialDone = true;
        GameManager.Instance.SaveManager.SaveData(gameData); //데이터 저장
        UIManager.Instance.ClosePopupUI(this);
    }

    private void OnNoClicked()
    {
        gameData.isTutorialDone = true;
        GameManager.Instance.SaveManager.SaveData(gameData); //데이터 저장
        UIManager.Instance.ClosePopupUI(this);
    }
}
