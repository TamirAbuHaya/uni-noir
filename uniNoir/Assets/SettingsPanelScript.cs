using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using System.Collections.Generic;
using System.Linq;
using TMPro;

public class SettingsPanelScript : MonoBehaviour
{
    public Image[] sfxImages = new Image[4];
    public Image[] musicImages = new Image[2];
    public Image[] fullScreenImages = new Image[2];
    public Button sfxButton;
    public Button musicButton;
    public Button fullscreenButton;
    public AudioMixer audioMixer;
    public Button backToHomeButton;
    public Button exitGameButton;
    public TMP_Dropdown resolutionDropdown;


    public SceneTransitionManager sceneTransitionManager;


    private int sfxLevel = 3; // 0 to 3
    private int musicLevel = 1; // 0 or 1
    private bool isFullscreen;
    private Resolution[] resolutions;
    private int currentResolutionIndex = 0;


    private const string SFX_LEVEL_KEY = "SFXLevel";
    private const string MUSIC_LEVEL_KEY = "MusicLevel";
    private const string FULLSCREEN_KEY = "Fullscreen";
    private const string RESOLUTION_INDEX_KEY = "ResolutionIndex";



    void Start()
    {
        SetupResolutionDropdown();
        LoadSettings();
        UpdateUI();
        
        sfxButton.onClick.AddListener(ToggleSFX);
        musicButton.onClick.AddListener(ToggleMusic);
        backToHomeButton.onClick.AddListener(BackToHome);
        exitGameButton.onClick.AddListener(ExitGame);
        fullscreenButton.onClick.AddListener(ToggleFullscreen);
        resolutionDropdown.onValueChanged.AddListener(ChangeResolution);
    }

    void SetupResolutionDropdown()
    {
        resolutions = Screen.resolutions.Select(resolution => new Resolution { width = resolution.width, height = resolution.height }).Distinct().ToArray();
        resolutionDropdown.ClearOptions();

        List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(new TMP_Dropdown.OptionData(option));
            if (resolutions[i].width == Screen.width && resolutions[i].height == Screen.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

       void LoadSettings()
    {
        sfxLevel = PlayerPrefs.GetInt(SFX_LEVEL_KEY, 3);
        musicLevel = PlayerPrefs.GetInt(MUSIC_LEVEL_KEY, 1);
        isFullscreen = PlayerPrefs.GetInt(FULLSCREEN_KEY, 1) == 1;
        currentResolutionIndex = PlayerPrefs.GetInt(RESOLUTION_INDEX_KEY, currentResolutionIndex);
        
        ApplyAudioSettings();
        ApplyFullscreenSetting();
        ApplyResolutionSetting();
    }

    void SaveSettings()
    {
        PlayerPrefs.SetInt(SFX_LEVEL_KEY, sfxLevel);
        PlayerPrefs.SetInt(MUSIC_LEVEL_KEY, musicLevel);
        PlayerPrefs.SetInt(FULLSCREEN_KEY, isFullscreen ? 1 : 0);
        PlayerPrefs.SetInt(RESOLUTION_INDEX_KEY, currentResolutionIndex);
        PlayerPrefs.Save();
    }

    void UpdateUI()
    {
        
        for(int i = 0;i < sfxImages.Length ; i++)
        {
            if(i == sfxLevel){
                sfxImages[i].gameObject.SetActive(true);
            }
            else{
                 sfxImages[i].gameObject.SetActive(false);
            }
        }

        for (int i = 0; i < musicImages.Length; i++)
        {
             if(i != musicLevel){
                musicImages[i].gameObject.SetActive(true);
            }
            else{
                 musicImages[i].gameObject.SetActive(false);
            }
        }
        UpdateFullscreenButtonText();
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    void ToggleSFX()
    {
        sfxLevel = (sfxLevel + 1) % 4;
        ApplyAudioSettings();
        UpdateUI();
        SaveSettings();
    }

    void ToggleMusic()
    {
        musicLevel = 1 - musicLevel; // Toggle between 0 and 1
        ApplyAudioSettings();
        UpdateUI();
        SaveSettings();
    }
     void UpdateFullscreenButtonText()
    {
        if(isFullscreen){
            fullScreenImages[0].gameObject.SetActive(true);
            fullScreenImages[1].gameObject.SetActive(false);
        } 
        else{
            fullScreenImages[1].gameObject.SetActive(true);
            fullScreenImages[0].gameObject.SetActive(false);
        }
    }

    void ApplyAudioSettings()
    {
        // Assuming -80dB is silent and 0dB is full volume
        float sfxVolume = sfxLevel > 0 ? -20 * (3 - sfxLevel) : -80;
        float musicVolume = musicLevel > 0 ? 0 : -80;

        audioMixer.SetFloat("SFXVolume", sfxVolume);
        audioMixer.SetFloat("MusicVolume", musicVolume);
    }

     void ToggleFullscreen()
    {
        isFullscreen = !isFullscreen;
        ApplyFullscreenSetting();
        UpdateFullscreenButtonText();
        SaveSettings();
    }
     void ApplyFullscreenSetting()
    {
        Screen.fullScreen = isFullscreen;
    }
     void ChangeResolution(int index)
    {
        currentResolutionIndex = index;
        ApplyResolutionSetting();
        SaveSettings();
    }

    void ApplyResolutionSetting()
    {
        Debug.Log("CurrResIndex" + currentResolutionIndex + resolutions.Length);
        if(currentResolutionIndex < resolutions.Length){
        Resolution resolution = resolutions[currentResolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        }
    }

    // Public method to get SFX level (0 to 3)
    public int GetSFXLevel()
    {
        return sfxLevel;
    }

    // Public method to get Music level (0 or 1)
    public int GetMusicLevel()
    {
        return musicLevel;
    }
    void BackToHome()
    {
        SaveSettings();
        sceneTransitionManager.FadeToNextScene();
    }

    void ExitGame()
    {
        SaveSettings();
        StartCoroutine(sceneTransitionManager.FadeOutAndExit());
    }
}