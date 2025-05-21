using UnityEngine;
using UnityEngine.UI;

public class UITutorialPrompt : PopupUI
{
    [SerializeField] private Button yesButton;
    [SerializeField] private Button noButton;
    public Transform tutorialStartPoint;

    private GameData gameData;
    private Portal portal;



    private void Start()
    {
        portal= FindObjectOfType<Portal>();
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
        TutorialController tutorial = FindObjectOfType<TutorialController>();

        if (tutorialStartPoint == null)
        {
            GameObject found = GameObject.Find("TutorialStartPoint");
            if (found != null)
                tutorialStartPoint = found.transform;
        }

        if (player != null && tutorialStartPoint != null)
        {
            if (portal != null)
                portal.TutorialMove();
        }

        if (tutorial != null)
        {
            tutorial.StartTutorialManually(); 
        }

        gameData.isTutorialDone = true;
        UIManager.Instance.ClosePopupUI(this);
        GameManager.Instance.SaveManager.SaveData(gameData);
    }




    private void OnNoClicked()
    {
        gameData.isTutorialDone = true;
        UIManager.Instance.ClosePopupUI(this);
        GameManager.Instance.SaveManager.SaveData(gameData); //데이터 저장
    }
}
