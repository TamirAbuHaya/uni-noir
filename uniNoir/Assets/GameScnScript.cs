using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using System.Collections;
using UnityEditor;

public class GameScnScript : MonoBehaviour
{
    public SceneTransitionManager sceneTransitionManager;

    [Header("Pop-up Panels")]
    public PanelAnimatorController settingsPanelAnimator;
    public PanelAnimatorController evidencePanelAnimator;
    public PanelAnimatorController startingPanelAnimator;
    public PanelAnimatorController closeUpSusPanelAnimator;
    public PanelAnimatorController removingSusPanelAnimator;

    [Header("Panels - groups")]
    public GameObject suspectPanel;
    public Button settingsButton;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI timerTextRemovSus;

    [Header("Timer Settings")]
    public float totalTime = 3f; // 6 minutes in seconds
    public float removingTime = 15f; // 15 seconds for removingSusPanel

    [Header("Buttons")]
    public Button startButton;
    public Button phoneButton;
    public Button caseFileButton;
    public Button goBackButtonClsup;
    public Button closeCaseFileButton;

    [Header("Buttons/Images arrays")]
    public Image[] suspectsCloseUp = new Image[7];
    public Button[] suspectsButtons = new Button[7];
    public Button[] remove_SusButtons = new Button[7];
    public Image[] remove_BigXs = new Image[7];

    [Header("Evidence related")]

    public Button survFootButton;
    public Button forensicRepButton;
    public Button directLinksButton;
    public Button directStatmentsButton;
    public PanelAnimatorController survFootPanelAnimator;
    public PanelAnimatorController forensicRepPanelAnimator;
    public PanelAnimatorController directLinksPanelAnimator;
    public PanelAnimatorController directStatmentsPanelAnimator;

    public Button survFootClosePnl;
    public Button forensicRepClosePnl;
    public Button directLinksClosePnl;
    public Button directStatmentsClosePnl;

    // index 0 - left (back) , index 1 - right (next)
    public Button[] arrowsSurvFoot = new Button[2];
    public Button[] arrowsForensicRep = new Button[2];
    public Button[] arrowsDirectLinks = new Button[2];

    public Image[] survFootImages = new Image[5];
    public Image[] forensicRepImages = new Image[5];
    public Image[] directLinksImages = new Image[7];

    private int currPgSurvFoot = 0;
    private int currPgForensicRep = 0;
    private int currPgDirectLinks = 0;


    [Header("Button Delay Settings")]
    public float buttonCooldown = 1.0f;
    private bool canPressButton = true;
    private float timeRemaining;
    private float remove_timeRemaining;
    private bool settings = false;
    private bool started = false;
    private bool removing = false;
    private bool removed = false;
    private int num_of_removed = -1;
    private bool[] suspectRemoved = new bool[7];
    public static bool goodEnd = false;

    private void Start()
    {
        InitializeUI();
        startingPanelAnimator.ShowPanel();   
        SetupButtonListeners();
        timeRemaining = totalTime;
        remove_timeRemaining = removingTime;
        UpdateTimerDisplay();
    }

    private void InitializeUI()
    {
        for (int i = 0; i < 7; i++)
        {
            suspectRemoved[i] = false;
            remove_BigXs[i].gameObject.SetActive(false);
        }

        removingSusPanelAnimator.gameObject.SetActive(false);
        settingsPanelAnimator.gameObject.SetActive(false);
        evidencePanelAnimator.gameObject.SetActive(false);
        suspectPanel.SetActive(false);
        goBackButtonClsup.gameObject.SetActive(false);
        closeUpSusPanelAnimator.gameObject.SetActive(false);
        survFootPanelAnimator.gameObject.SetActive(false);
        forensicRepPanelAnimator.gameObject.SetActive(false);
        directLinksPanelAnimator.gameObject.SetActive(false);
        directStatmentsPanelAnimator.gameObject.SetActive(false);

        if (sceneTransitionManager != null)
        {
            StartCoroutine(sceneTransitionManager.FadeIn());
        }
    }

    private void SetupButtonListeners()
    {
        startButton.onClick.AddListener(() => StartCoroutine(ButtonPressWithDelay(ToggleStartingPanel)));
        settingsButton.onClick.AddListener(() => StartCoroutine(ButtonPressWithDelay(ToggleSettingsPanel)));
        caseFileButton.onClick.AddListener(() => StartCoroutine(ButtonPressWithDelay(ToggleEvidencePanel)));
        goBackButtonClsup.onClick.AddListener(() => StartCoroutine(ButtonPressWithDelay(ToggleCloseUpSuspectPanel)));
        closeCaseFileButton.onClick.AddListener(() => StartCoroutine(ButtonPressWithDelay(ToggleEvidencePanel)));
       

        for (int i = 0; i < 7; i++)
        {
            int index = i;
            suspectsButtons[i].onClick.AddListener(() => StartCoroutine(ButtonPressWithDelay(() => ShowSuspectCloseUp(index))));
            remove_SusButtons[i].onClick.AddListener(() => StartCoroutine(ButtonPressWithDelay(() => RemoveSuspect(index))));
        }   
         survFootButton.onClick.AddListener(() => StartCoroutine(ButtonPressWithDelay(ToggleSurvFootagePanel)));
        forensicRepButton.onClick.AddListener(() => StartCoroutine(ButtonPressWithDelay(ToggleForensicRepPanel)));
        directLinksButton.onClick.AddListener(() => StartCoroutine(ButtonPressWithDelay(ToggleDirectLinksPanel)));

        survFootClosePnl.onClick.AddListener(() => StartCoroutine(ButtonPressWithDelay(ToggleSurvFootagePanel)));
        forensicRepClosePnl.onClick.AddListener(() => StartCoroutine(ButtonPressWithDelay(ToggleForensicRepPanel)));
        directLinksClosePnl.onClick.AddListener(() => StartCoroutine(ButtonPressWithDelay(ToggleDirectLinksPanel)));

        SetupArrowListeners(arrowsSurvFoot, NavigateSurvFootage);
        SetupArrowListeners(arrowsForensicRep, NavigateForensicRep);
        SetupArrowListeners(arrowsDirectLinks, NavigateDirectLinks);
    }

    private void Update()
    {
        if (started && !settings)
        {             
            if (!removing)
            {          
                if (timeRemaining > 0)
                {
                    timeRemaining -= Time.deltaTime;
                    UpdateTimerDisplay();

                    // Check if a minute has passed
                    if (Mathf.FloorToInt(timeRemaining) % 60 == 0 && timeRemaining < totalTime - 1)
                    {
                        StartCoroutine(RemoveSuspectRoutine());
                        timeRemaining -= 1.0f;
                    }
                }
                else
                {
                    timeRemaining = 0;
                    UpdateTimerDisplay();
                    Debug.Log("Timer finished!");
                    if(!suspectRemoved[5]){
                        goodEnd = true;
                    }
                    if (sceneTransitionManager != null)
                    {
                        sceneTransitionManager.FadeToNextScene();
                    }
                    
                }
            }
            else
            {
                if (remove_timeRemaining > 0)
                {
                    remove_timeRemaining -= Time.deltaTime;
                    UpdateRemoveSusTimerDisplay();
                }
                else
                {
                    FinishRemovingSuspect();
                }
            }
        }
    }

    private IEnumerator RemoveSuspectRoutine()
    {
        removing = true;
        remove_timeRemaining = removingTime;
        ToggleRemovingSuspectPanel();

        yield return new WaitForSeconds(removingTime);

        if (!removed)
        {
            RemoveRandomSuspect();
        }

        FinishRemovingSuspect();
    }

    private void RemoveRandomSuspect()
    {
        int rand = UnityEngine.Random.Range(0, 6);
        while (suspectRemoved[rand])
        {
            rand = UnityEngine.Random.Range(0, 6);
        }
        Debug.Log("Random generated"+rand);
        RemoveSuspect(rand);
    }

    private void RemoveSuspect(int numOfSuspect)
    {
        Debug.Log("removing"+numOfSuspect);
        if (!suspectRemoved[numOfSuspect] )
        {
            if(num_of_removed!= -1)
            {
               remove_BigXs[num_of_removed].gameObject.SetActive(false);
            }
            remove_BigXs[numOfSuspect].gameObject.SetActive(true);
            num_of_removed = numOfSuspect;
            removed = true;
        }
    }

    private void FinishRemovingSuspect()
    {
        
        Debug.Log("num_of_removed"+num_of_removed);
        suspectRemoved[num_of_removed] = true;
        suspectsButtons[num_of_removed].gameObject.SetActive(false);
        removing = false;
        num_of_removed = -1;
        removed = false;
        ToggleRemovingSuspectPanel();
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

    private void UpdateRemoveSusTimerDisplay()
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(remove_timeRemaining);
        timerTextRemovSus.text = string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
    }

    private void ShowSuspectCloseUp(int suspectNumber)
    {
        foreach (var closeUp in suspectsCloseUp)
        {
            closeUp.gameObject.SetActive(false);
        }
        goBackButtonClsup.gameObject.SetActive(false);

        suspectsCloseUp[suspectNumber].gameObject.SetActive(true);
        closeUpSusPanelAnimator.ShowPanel();
        goBackButtonClsup.gameObject.SetActive(true);
    }

    private void ToggleSettingsPanel()
    {
        if (settingsPanelAnimator.gameObject.activeSelf)
        {
            settingsPanelAnimator.HidePanel();
            settings = false;
        }
        else
        {
            settings = true;
            settingsPanelAnimator.ShowPanel();
        }
    }
      private void SetupArrowListeners(Button[] arrows, Action<int> navigateAction)
    {
        arrows[0].onClick.AddListener(() => StartCoroutine(ButtonPressWithDelay(() => navigateAction(-1))));
        arrows[1].onClick.AddListener(() => StartCoroutine(ButtonPressWithDelay(() => navigateAction(1))));
    }
     private void ToggleEvidencePanel(PanelAnimatorController panelAnimator, ref int currentPage, Image[] images)
    {
        if (panelAnimator.gameObject.activeSelf)
        {
            panelAnimator.HidePanel();
        }
        else
        {
            panelAnimator.ShowPanel();
            UpdateEvidenceDisplay(currentPage, images);
        }
    }
    private void ToggleSurvFootagePanel() => ToggleEvidencePanel(survFootPanelAnimator, ref currPgSurvFoot, survFootImages);
    private void ToggleForensicRepPanel() => ToggleEvidencePanel(forensicRepPanelAnimator, ref currPgForensicRep, forensicRepImages);
    private void ToggleDirectLinksPanel() => ToggleEvidencePanel(directLinksPanelAnimator, ref currPgDirectLinks, directLinksImages);
    //private void ToggleDirectStatmentsPanel() => ToggleEvidencePanel(directStatmentsPanelAnimator, ref currPgDirectStatments, directStatmentsImages);

    private void NavigateEvidence(ref int currentPage, int direction, Image[] images, Button[] arrows)
    {
        currentPage = Mathf.Clamp(currentPage + direction, 0, images.Length - 1);
        UpdateEvidenceDisplay(currentPage, images);
        UpdateArrowStates(currentPage, arrows, images.Length);
    }

    private void ToggleEvidencePanel()
    {
        if (evidencePanelAnimator.gameObject.activeSelf)
        {
            evidencePanelAnimator.HidePanel();
            caseFileButton.gameObject.SetActive(true);
        }
        else
        {
            caseFileButton.gameObject.SetActive(false);
            evidencePanelAnimator.ShowPanel();
        }
    }
     private void NavigateSurvFootage(int direction) => NavigateEvidence(ref currPgSurvFoot, direction, survFootImages, arrowsSurvFoot);
    private void NavigateForensicRep(int direction) => NavigateEvidence(ref currPgForensicRep, direction, forensicRepImages, arrowsForensicRep);
    private void NavigateDirectLinks(int direction) => NavigateEvidence(ref currPgDirectLinks, direction, directLinksImages, arrowsDirectLinks);

    private void UpdateEvidenceDisplay(int currentPage, Image[] images)
    {
        for (int i = 0; i < images.Length; i++)
        {
            images[i].gameObject.SetActive(i == currentPage);
        }
    }

      private void UpdateArrowStates(int currentPage, Button[] arrows, int totalPages)
    {
        arrows[0].interactable = currentPage > 0;
        arrows[1].interactable = currentPage < totalPages - 1;
    }


   

    private void ToggleCloseUpSuspectPanel()
    {
        if (closeUpSusPanelAnimator.gameObject.activeSelf)
        {
            closeUpSusPanelAnimator.HidePanel();
            goBackButtonClsup.gameObject.SetActive(false);
        }
        else
        {
            closeUpSusPanelAnimator.ShowPanel();
            goBackButtonClsup.gameObject.SetActive(true);


        }
    }

    private void ToggleRemovingSuspectPanel()
    {
        if (removingSusPanelAnimator.gameObject.activeSelf)
        {
            removingSusPanelAnimator.HidePanel();
        }
        else
        {
            removingSusPanelAnimator.ShowPanel();
        }
    }

    private void ToggleStartingPanel()
    {
        suspectPanel.SetActive(true);
        startingPanelAnimator.HidePanel();
        started = true;
    }

    private void HideAll()
    {
        closeUpSusPanelAnimator.HidePanel();
        evidencePanelAnimator.HidePanel();
        removingSusPanelAnimator.HidePanel();
        settingsPanelAnimator.HidePanel();
        survFootPanelAnimator.HidePanel();
        forensicRepPanelAnimator.HidePanel();
        directLinksPanelAnimator.HidePanel();
        directStatmentsPanelAnimator.HidePanel();
    }
}