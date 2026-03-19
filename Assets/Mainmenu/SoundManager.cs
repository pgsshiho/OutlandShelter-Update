using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    private static SoundManager instance;
    [SerializeField] private AudioSource bgm;
    [SerializeField] private AudioSource sfx;
    public static AudioSource BGM => instance.bgm;
    public static AudioSource SFX => instance.sfx;
    public AudioMixer audioMixer;
    public Slider masterVolumeSlider;
    public Slider bgmSlider;
    public Slider sfxSlider;

    void Awake()
    {
        instance = this;

        float savedVolume = PlayerPrefs.GetFloat("Master", 1f);
        float savedBGMVolume = PlayerPrefs.GetFloat("BGM", 1f);
        float savedSFXVolume = PlayerPrefs.GetFloat("SFX", 1f);

        masterVolumeSlider.value = savedVolume;
        bgmSlider.value = savedBGMVolume;
        sfxSlider.value = savedSFXVolume;

        SetMasterVolume(savedVolume);
        SetBGMVolume(savedBGMVolume);
        SetSFXVolume(savedSFXVolume);
    }

    public void SetMasterVolume(float value)
    {
        if (value <= 0.0001f) value = 0.0001f; // 로그 방지
        audioMixer.SetFloat("Master", Mathf.Log10(value) * 20);
        PlayerPrefs.SetFloat("Master", value);
    }

    public void SetBGMVolume(float value)
    {
        if (value <= 0.0001f) value = 0.0001f; // 로그 방지
        audioMixer.SetFloat("BGM", Mathf.Log10(value) * 20);
        PlayerPrefs.SetFloat("BGM", value);
    }

    public void SetSFXVolume(float value)
    {
        if (value <= 0.0001f) value = 0.0001f; // 로그 방지
        audioMixer.SetFloat("SFX", Mathf.Log10(value) * 20);
        PlayerPrefs.SetFloat("SFX", value);
    }
}
