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
    [SerializeField] Button dieLobbyBtn;
    [SerializeField] Button dieEndBtn;

    [SerializeField] GameObject soundMenu;
    [SerializeField] GameObject settingMenu;

    private void Start()
    {
        if (startBtn != null) startBtn.onClick.AddListener(OnClickStart);
        if (LobbyBtn != null) LobbyBtn.onClick.AddListener(OnClickLobbyBtn);
        if (endBtn != null) endBtn.onClick.AddListener(OnClickEndBtn);
        settingBtn.onClick.AddListener(OnClickSettingBtn);
        if (settingCloseBtn != null) settingCloseBtn.onClick.AddListener(OnClickSettingCloseBtn);
        soundBtn.onClick.AddListener(OnClickSoundBtn);
        if (soundCloseBtn != null) soundCloseBtn.onClick.AddListener(OnClickSoundCloseBtn);
        if (dieLobbyBtn != null) dieLobbyBtn.onClick.AddListener(OnClickLobbyBtn);
        if (dieEndBtn != null) dieEndBtn.onClick.AddListener(OnClickEndBtn);
    }

    public void OnClickStart()
    {
        //씬 시작 시 저장
        GameManager.Instance?.PlayerManager?.Currency?.SaveCurrency();

        SceneManager.LoadScene("Bosung_02");
    }

    public void OnClickLobbyBtn()
    {
        //씬 이동 시 저장
        GameManager.Instance?.PlayerManager?.Currency?.SaveCurrency();

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
