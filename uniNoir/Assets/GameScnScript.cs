using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using System.Collections;



public class GameScnScript : MonoBehaviour
{
    public SceneTransitionManager sceneTransitionManager;
    public PanelAnimatorController settingsPanelAnimator;
    public Button settingsButton;
    private bool canPressButton = true;
    public TextMeshProUGUI timerText;
    public float totalTime = 360f; // 6 minutes in seconds
    private float timeRemaining;

    private bool[] suspects = new bool[7];

    [Header("Button Delay Settings")]
    public float buttonCooldown = 1.0f;
    

    private void Start()
    {
        InitializeUI();
        SetupButtonListeners();
        timeRemaining = totalTime;
        UpdateTimerDisplay();
        
    }
     private void InitializeUI()
    {
       
        settingsPanelAnimator.gameObject.SetActive(false);

    }

    private void SetupButtonListeners()
    {
      
        settingsButton.onClick.AddListener(() => StartCoroutine(ButtonPressWithDelay(ToggleSettingsPanel)));

    }

    private void Update()
    {
        if (timeRemaining > 0)
        {
            if(timeRemaining % 60 == 0 && timeRemaining != 360){ // Choose 1 suspect to remove every minute

            }
            timeRemaining -= Time.deltaTime;
            UpdateTimerDisplay();
        }
        else
        {
            timeRemaining = 0;
            UpdateTimerDisplay();
            // Timer has finished, add your logic here
            Debug.Log("Timer finished!");
        }
    }


    private IEnumerator ButtonPressWithDelay(System.Action action)
    {
        if (canPressButton)
        {
            canPressButton = false;
            action.Invoke();
            yield return new WaitForSeconds(buttonCooldown);
            canPressButton = true;
        }
    }
    private void UpdateTimerDisplay()
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(timeRemaining);
        timerText.text = string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
    }

    
    private void ToggleSettingsPanel()
    {
        if (settingsPanelAnimator.gameObject.activeSelf)
        {
            settingsPanelAnimator.HidePanel();
        }
        else
        {
            settingsPanelAnimator.ShowPanel();
        }
    }
}