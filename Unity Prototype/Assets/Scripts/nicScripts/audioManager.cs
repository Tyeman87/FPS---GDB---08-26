using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class audioManager : MonoBehaviour
{
    public static audioManager Instance;
    public AudioSource audPlayer;

    [SerializeField] AudioMixer mainMixer;
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider sfxSlider;

    private const string MusicPrefKey = "MusicVolume";
    private const string SFXPrefKey = "SFXVolume";
    private const float DefaultVolume = 0.25f;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        LoadAudioSettings();
    }
    void Update()
    {
        if (Time.timeScale == 0)
        {
            audPlayer.Pause();
        }
        else 
            audPlayer.UnPause();
    }

    public void SetMusicVolume(float sliderVal)
    {
        sliderVal = Mathf.Clamp(sliderVal, 0.0001f, 1f);
        float db = Mathf.Log10(sliderVal) * 20;

        mainMixer.SetFloat("musicVol", db);
        PlayerPrefs.SetFloat(MusicPrefKey, sliderVal);
    }

    public void SetSFXVolume(float sliderVal)
    {
        sliderVal = Mathf.Clamp(sliderVal, 0.0001f, 1f);
        float db = Mathf.Log10(sliderVal) * 20;

        mainMixer.SetFloat("sfxVol", db);
        PlayerPrefs.SetFloat(SFXPrefKey, sliderVal);
    }

    private void LoadAudioSettings()
    {
        float savedMusic = PlayerPrefs.GetFloat(MusicPrefKey, DefaultVolume);
        float savedSFX = PlayerPrefs.GetFloat(SFXPrefKey, DefaultVolume);

        SetMusicVolume(savedMusic);
        SetSFXVolume(savedSFX);

        if (musicSlider != null)
        {
            musicSlider.value = savedMusic;
            musicSlider.onValueChanged.AddListener(SetMusicVolume);
        }

        if (sfxSlider != null)
        {
            sfxSlider.value = savedSFX;
            sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        }


    }
}
