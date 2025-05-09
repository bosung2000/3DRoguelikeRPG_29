using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public enum SoundEffectType
{
    Attack,
    Hit,
    TakeDamage
}

[System.Serializable]
public class SoundEffectEntry
{
    public SoundEffectType type;
    public AudioClip clip;
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [Header("Audio Sources")]
    public AudioSource bgmSource;
    public AudioSource[] sfxPool;

    [Header("Audio Mixer")]
    public AudioMixer audioMixer;

    [Header("Effect Clips")]
    public List<SoundEffectEntry> effectEntries;

    [Header("UI Sliders")]
    public Slider masterSlider, bgmSlider, sfxSlider;

    private Dictionary<SoundEffectType, AudioClip> effectDict = new();

    private float masterVolume = 1f;
    private float bgmVolume = 1f;
    private float sfxVolume = 1f;

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

        foreach (var entry in effectEntries)
        {
            if (!effectDict.ContainsKey(entry.type))
                effectDict.Add(entry.type, entry.clip);
        }
    }

    private void Start()
    {
        LoadVolumeSettings();
        BindSliderEvents();
    }

    private void LoadVolumeSettings()
    {
        masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        bgmVolume = PlayerPrefs.GetFloat("BGMVolume", 1f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);

        masterSlider.value = masterVolume;
        bgmSlider.value = bgmVolume;
        sfxSlider.value = sfxVolume;

        UpdateAllVolumes();
    }

    private void BindSliderEvents()
    {
        masterSlider.onValueChanged.AddListener(SetMasterVolume);
        bgmSlider.onValueChanged.AddListener(SetBGMVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    public void SetMasterVolume(float volume)
    {
        masterVolume = volume;
        audioMixer.SetFloat("Master", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("MasterVolume", volume);
    }

    public void SetBGMVolume(float volume)
    {
        bgmVolume = volume;
        audioMixer.SetFloat("BGM", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("BGMVolume", volume);
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;
        audioMixer.SetFloat("SFX", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("SFXVolume", volume);
    }

    private void UpdateAllVolumes()
    {
        SetMasterVolume(masterVolume);
        SetBGMVolume(bgmVolume);
        SetSFXVolume(sfxVolume);
    }

    public void PlayEffect(SoundEffectType type, float volume = 1f)
    {
        if (effectDict.TryGetValue(type, out var clip))
        {
            PlayClip(clip, volume);
        }
    }

    public void PlayClip(AudioClip clip, float volume = 1f)
    {
        if (clip == null) return;

        foreach (var source in sfxPool)
        {
            if (!source.isPlaying)
            {
                source.PlayOneShot(clip, volume * sfxVolume * masterVolume);
                return;
            }
        }

        sfxPool[0].PlayOneShot(clip, volume * sfxVolume * masterVolume);
    }

    public void PlayBGM(AudioClip bgm, bool loop = true)
    {
        if (bgmSource.isPlaying)
            bgmSource.Stop();

        bgmSource.clip = bgm;
        bgmSource.loop = loop;
        bgmSource.volume = bgmVolume * masterVolume;
        bgmSource.Play();
    }
}
