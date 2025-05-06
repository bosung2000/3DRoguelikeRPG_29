using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// 타이틀 메뉴 및 씬 전환
/// </summary>
public class TitleMenu : MonoBehaviour
{
    [SerializeField] Button startBtn;
    [SerializeField] Button endBtn;
    [SerializeField] Button settingBtn;
    //[SerializeField] Button closeSetting;

    [SerializeField] GameObject settingMenu;
    bool active;

    private void Start()
    {
        startBtn.onClick.AddListener(OnClickStart);
        endBtn.onClick.AddListener(OnClickEndBtn);
        settingBtn.onClick.AddListener(OnClickSettingBtn);
        //closeSetting.onClick.AddListener(OnClickCloseSetting);
    }
    public void OnClickStart()
    {
        SceneManager.LoadScene("Main_02");
    }

    public void OnClickEndBtn()
    {
        EditorApplication.isPlaying = false;

        Application.Quit();
    }
    public void OnClickSettingBtn()
    {
        if (active)
        {
            settingMenu.SetActive(false);
            active = false;
        }
        else
        {
            settingMenu.SetActive(true);
            active = true;
        }
    }
    //public void OnClickCloseSetting()
    //{
    //    settingMenu.SetActive(false);
    //}
}