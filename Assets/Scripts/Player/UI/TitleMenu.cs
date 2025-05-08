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
    [SerializeField] Button settingCloseBtn;
    [SerializeField] Button soundCloseBtn;

    [SerializeField] GameObject soundMenu;
    [SerializeField] GameObject settingMenu;

    private void Start()
    {
        if (startBtn != null) startBtn.onClick.AddListener(OnClickStart);
        if (LobbyBtn != null) LobbyBtn.onClick.AddListener(OnClickLobbyBtn);
        endBtn.onClick.AddListener(OnClickEndBtn);
        settingBtn.onClick.AddListener(OnClickSettingBtn);
        settingCloseBtn.onClick.AddListener(OnClickSettingCloseBtn);
        soundBtn.onClick.AddListener(OnClickSoundBtn);
        soundCloseBtn.onClick.AddListener(OnClickSoundCloseBtn);
    }

    public void OnClickStart()
    {
        SceneManager.LoadScene("Bosung_02");
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
         Time.timeScale = 0;
         soundMenu.SetActive(true);
    }

    public void OnClickSoundCloseBtn()
    {
        Time.timeScale = 1;
        soundMenu.SetActive(false);
    }
    public void OnClickSettingBtn()
    {
         Time.timeScale = 0;
         settingMenu.SetActive(true);
    }
    public void OnClickSettingCloseBtn()
    {
        Time.timeScale = 1;
        settingMenu.SetActive(false);
    }
}
