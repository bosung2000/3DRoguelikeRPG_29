using UnityEngine;
using UnityEngine.UI;

public class UITutorialPrompt : PopupUI
{
    [SerializeField] private Button yesButton;
    [SerializeField] private Button noButton;

    public Transform tutorialStartPoint;

    private void Start()
    {
        yesButton.onClick.AddListener(OnYesClicked);
        noButton.onClick.AddListener(OnNoClicked);
    }

    private void OnYesClicked()
    {
        Player player = FindObjectOfType<Player>();
        if (player != null && tutorialStartPoint != null)
        {
            player.transform.position = tutorialStartPoint.position;
        }

        UIManager.Instance.ClosePopupUI(this);
    }

    private void OnNoClicked()
    {
        UIManager.Instance.ClosePopupUI(this);
    }
}
