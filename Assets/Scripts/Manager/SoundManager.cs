using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    public AudioSource bgmSource;
    public AudioSource[] sfxSources;

    public Slider masterSlider, bgmSlider, sfxSlider;

    private float masterVolume = 1f;
    private float bgmVolume = 1f;
    private float sfxVolume = 1f;

    private Transform soundPanel;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        //soundPanel = transform.root.Find("UI/SoundSettingsPanel");
        //masterSlider = soundPanel.GetComponentInChildren<Slider>(true);
    }

    private void Start()
    {
        if (masterSlider != null && bgmSlider != null && sfxSlider != null)
        {
            LoadVolumeSettings();
        }
    }

    public void SetMasterVolume(float volume)
    {
        masterVolume = volume;
        UpdateVolumes();
        PlayerPrefs.SetFloat("MasterVolume", volume);
    }

    public void SetBGMVolume(float volume)
    {
        bgmVolume = volume;
        UpdateVolumes();
        PlayerPrefs.SetFloat("BGMVolume", volume);
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;
        UpdateVolumes();
        PlayerPrefs.SetFloat("SFXVolume", volume);
    }

    public void LoadVolumeSettings()
    {
        masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        bgmVolume = PlayerPrefs.GetFloat("BGMVolume", 1f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);

        masterSlider.value = masterVolume;
        bgmSlider.value = bgmVolume;
        sfxSlider.value = sfxVolume;

        UpdateVolumes();

        masterSlider.onValueChanged.AddListener(SetMasterVolume);
        bgmSlider.onValueChanged.AddListener(SetBGMVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    private void UpdateVolumes()
    {
        UpdateBGM();
        UpdateSFX();
    }

    private void UpdateBGM()
    {
        if (bgmSource != null)
            bgmSource.volume = masterVolume * bgmVolume;
    }

    private void UpdateSFX()
    {
        foreach (AudioSource sfx in sfxSources)
        {
            if (sfx != null)
                sfx.volume = masterVolume * sfxVolume;
        }
    }

    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (clip != null)
        {
            foreach (AudioSource sfx in sfxSources)
            {
                if (!sfx.isPlaying)
                {
                    sfx.PlayOneShot(clip, volume * sfxVolume * masterVolume);
                    return;
                }
            }
            sfxSources[0].PlayOneShot(clip, volume * sfxVolume * masterVolume);
        }
    }
}
