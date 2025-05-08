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
    [SerializeField] Button LobbyBtn;
    [SerializeField] Button endBtn;
    [SerializeField] Button soundBtn;
    [SerializeField] Button settingBtn;

    [SerializeField] GameObject soundMenu;
    [SerializeField] GameObject settingMenu;
    bool active;

    private void Start()
    {
        if (startBtn != null) startBtn.onClick.AddListener(OnClickStart);
        if (LobbyBtn != null) LobbyBtn.onClick.AddListener(OnClickLobbyBtn);
        endBtn.onClick.AddListener(OnClickEndBtn);
        settingBtn.onClick.AddListener(OnClickSettingBtn);
        soundBtn.onClick.AddListener(OnClickSoundBtn);
    }

    public void OnClickStart()
    {
        SceneManager.LoadScene("Main_HB");
    }

    public void OnClickLobbyBtn()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Lobby_HB");
    }
    public void OnClickEndBtn()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
       Application.Quit();  
#endif
    }

    public void OnClickSoundBtn()
    {
        if (!active)
        {
            Time.timeScale = 0;
            soundMenu.SetActive(true);
            active = true;
        }
        else
        {
            Time.timeScale = 1;
            soundMenu.SetActive(false);
            active = false;
        }
    }

    public void OnClickSettingBtn()
    {
        if (!active)
        {
            Time.timeScale = 0;
            settingMenu.SetActive(true);
            active = true;
        }
        else
        {
            Time.timeScale = 1;
            settingMenu.SetActive(false);
            active = false;
        }
    }
}
