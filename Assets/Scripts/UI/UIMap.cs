using TMPro;
using UnityEngine;

public class UIMap : PopupUI
{
    [SerializeField] private MapManager mapManager;
    [SerializeField] private GameObject map;
    [SerializeField] private TextMeshProUGUI Txt_Stage;
    private void Start()
    {
        closeButton.onClick.AddListener(Closebtn);
    }

    private void OnEnable()
    {
        Txt_Stage.text = $"Stage : {StageManager.Instance.CurrentStage}";
        if (mapManager == null)
        {
            mapManager = GameManager.Instance.MapManager;
            mapManager.initStart(map);
        }
    }


    private void Closebtn()
    {
        base.Close();
    }
}
