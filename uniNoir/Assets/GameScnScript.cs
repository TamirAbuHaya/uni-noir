using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using System.Collections;
using UnityEditor;
using Unity.VisualScripting;
using System.Collections.Generic;

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
    public Button clsSettingsButton;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI timerTextRemovSus;

    [Header("Timer Settings")]
    public float totalTime = 360f; // 6 minutes in seconds
    public float removingTime = 15f; // 15 seconds for removingSusPanel

    [Header("Buttons")]
    public Button startButton;
    public Button caseFileButton;
    public Button goBackButtonClsup;
    public Button closeCaseFileButton;

    [Header("Start related")]

    public TextMeshProUGUI text1;
    public TextMeshProUGUI text2;




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

    [Header("Dialogue related")]

    public PanelAnimatorController dialGSPanelAnimator;

    public PanelAnimatorController dialgQuestionsPanelAnimator;


    private bool[] seenEvidence = new bool[7];
    private bool[] seenWitnesses = new bool[7];
    private bool[] seenDirectLinks = new bool[7];

    public Button investigateButton;
    public Button goodCopButton;
    public Button badCopButton;
    public Button quitQuestionsButton;
    public Button[] questionsButtons = new Button[7];
    public Button dialgTextButton;
    public TextMeshProUGUI dialgText;   
    private bool goodCop = false;
    private int currSus = -1;
    private int textIndex = 0;
    private List<string>[,,] allSuspectDialogues;
    private List<string> currDialog;

    [Header("Telephone related")]

    private List<string>[] telePhoneTexts = new List<string>[4];
    public PanelAnimatorController telePhonePanelAnimator;
    public Button phoneButton;
    public Button phoneTextButton;
    public TextMeshProUGUI phoneText;   
    public Button[] telePhoneButtons = new Button[4]; 
    public Button quitTelephone; //wish I could ffs
    private int phoneTextIndex = 0;
    private int curr_phone = -1;//Look into this


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

    private int startingProg = 0;

     
    [Header("Audio")]
    public AudioSource backgroundMusicSource;
    public AudioClip backgroundMusic;
    public AudioSource sfxSource;
    public AudioClip buttonClick1;
    public AudioClip buttonClick2;
    public AudioClip paperflip;
    //public AudioClip left_right_exit;
    public AudioClip casefilepress;
    public AudioClip suspectLeaving;
    public AudioClip phonePress;
  

    private void Start()
    {
        InitializeUI();
        startingPanelAnimator.ShowPanel();   
        SetupButtonListeners();
        timeRemaining = totalTime;
        remove_timeRemaining = removingTime;
        InitializeDialogues();
        UpdateTimerDisplay();
        initPhoneTexts();
    }

    private void InitializeUI()
    {
        for (int i = 0; i < 7; i++)
        {
            suspectRemoved[i] = false;
            seenDirectLinks[i] = false;
            seenEvidence[i] = false;
            seenWitnesses[i] = false;
            remove_BigXs[i].gameObject.SetActive(false);
        }
        text2.gameObject.SetActive(false);
        removingSusPanelAnimator.gameObject.SetActive(false);
        settingsPanelAnimator.gameObject.SetActive(false);
        evidencePanelAnimator.gameObject.SetActive(false);
        suspectPanel.SetActive(false);
        goBackButtonClsup.gameObject.SetActive(false);
        investigateButton.gameObject.SetActive(false);
        closeUpSusPanelAnimator.gameObject.SetActive(false);
        survFootPanelAnimator.gameObject.SetActive(false);
        forensicRepPanelAnimator.gameObject.SetActive(false);
        directLinksPanelAnimator.gameObject.SetActive(false);
        directStatmentsPanelAnimator.gameObject.SetActive(false);
        dialGSPanelAnimator.gameObject.SetActive(false);
        dialgQuestionsPanelAnimator.gameObject.SetActive(false);
        telePhonePanelAnimator.gameObject.SetActive(false);


        if (sceneTransitionManager != null)
        {
            StartCoroutine(sceneTransitionManager.FadeIn());
        }
    }

    private void UpdateQuestionsListeners(){
        Debug.Log("updatingListeners"+goodCop);
         for (int i = 0; i < 7; i++)
        {
            int index = i;
            questionsButtons[i].onClick.AddListener(() => StartCoroutine(ButtonPressWithDelay(() => ActivateQuestionDialouge(currSus,goodCop?0:1,index))));
        }
    }

    private void SetupButtonListeners()
    {
        startButton.onClick.AddListener(() => StartCoroutine(ButtonPressWithDelay(ToggleStartingPanel)));
        settingsButton.onClick.AddListener(() => StartCoroutine(ButtonPressWithDelay(ToggleSettingsPanel)));
        caseFileButton.onClick.AddListener(() => StartCoroutine(ButtonPressWithDelay(ToggleEvidencePanel)));
        goBackButtonClsup.onClick.AddListener(() => StartCoroutine(ButtonPressWithDelay(ToggleCloseUpSuspectPanel)));
        closeCaseFileButton.onClick.AddListener(() => StartCoroutine(ButtonPressWithDelay(ToggleEvidencePanel)));
        clsSettingsButton.onClick.AddListener(() => StartCoroutine(ButtonPressWithDelay(ToggleSettingsPanel)));
        goodCopButton.onClick.AddListener(() => StartCoroutine(ButtonPressWithDelay(() => ToggleDiagQuestionPanel(true))));
        badCopButton.onClick.AddListener(() => StartCoroutine(ButtonPressWithDelay(() => ToggleDiagQuestionPanel(false))));
        investigateButton.onClick.AddListener(() => StartCoroutine(ButtonPressWithDelay(ToggleDiagPanel)));
        dialgTextButton.onClick.AddListener(() => StartCoroutine(ButtonPressWithDelay(ContinueQuestionDialouge)));
        quitQuestionsButton.onClick.AddListener(() => StartCoroutine(ButtonPressWithDelay(() => ToggleDiagQuestionPanel(false))));
        phoneButton.onClick.AddListener(() => StartCoroutine(ButtonPressWithDelay(ToggleTelePhonePanel)));
        quitTelephone.onClick.AddListener(() => StartCoroutine(ButtonPressWithDelay(ToggleTelePhonePanel)));
        phoneTextButton.onClick.AddListener(() => StartCoroutine(ButtonPressWithDelay(() => PhoneTextManager(curr_phone))));

        for (int i = 0; i < 7; i++)
        {
            int index = i;
            if(index < 4){
                telePhoneButtons[i].onClick.AddListener(() => StartCoroutine(ButtonPressWithDelay(() => PhoneTextManager(index))));
            }
            suspectsButtons[i].onClick.AddListener(() => StartCoroutine(ButtonPressWithDelay(() => ShowSuspectCloseUp(index))));
            remove_SusButtons[i].onClick.AddListener(() => StartCoroutine(ButtonPressWithDelay(() => RemoveSuspect(index))));
        }   
        survFootButton.onClick.AddListener(() => StartCoroutine(ButtonPressWithDelay(ToggleSurvFootagePanel)));
        forensicRepButton.onClick.AddListener(() => StartCoroutine(ButtonPressWithDelay(ToggleForensicRepPanel)));
        directLinksButton.onClick.AddListener(() => StartCoroutine(ButtonPressWithDelay(ToggleDirectLinksPanel)));
        directStatmentsButton.onClick.AddListener(() => StartCoroutine(ButtonPressWithDelay(ToggleDirectStatmentsPanel)));


        survFootClosePnl.onClick.AddListener(() => StartCoroutine(ButtonPressWithDelay(ToggleSurvFootagePanel)));
        forensicRepClosePnl.onClick.AddListener(() => StartCoroutine(ButtonPressWithDelay(ToggleForensicRepPanel)));
        directLinksClosePnl.onClick.AddListener(() => StartCoroutine(ButtonPressWithDelay(ToggleDirectLinksPanel)));
        directStatmentsClosePnl.onClick.AddListener(() => StartCoroutine(ButtonPressWithDelay(ToggleDirectStatmentsPanel)));

        
        SetupArrowListeners(ref arrowsSurvFoot, NavigateSurvFootage);
        SetupArrowListeners(ref arrowsForensicRep, NavigateForensicRep);
        SetupArrowListeners(ref arrowsDirectLinks, NavigateDirectLinks);
    }
   
    private void Update()
    {
         if(Input.GetKey(KeyCode.Escape)){
            StartCoroutine(ButtonPressWithDelay(ToggleSettingsPanel));
        }
        else if(Input.GetKey(KeyCode.Home) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)){
            if (survFootPanelAnimator.gameObject.activeSelf)
                StartCoroutine(ButtonPressWithDelay(() => NavigateSurvFootage(-1)));
            if (forensicRepPanelAnimator.gameObject.activeSelf)
                StartCoroutine(ButtonPressWithDelay(() => NavigateForensicRep(-1)));
            if (directLinksPanelAnimator.gameObject.activeSelf)
                StartCoroutine(ButtonPressWithDelay(() => NavigateDirectLinks(-1)));
        }
        else if(Input.GetKey(KeyCode.End) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow) ){
           if (survFootPanelAnimator.gameObject.activeSelf)
                StartCoroutine(ButtonPressWithDelay(() => NavigateSurvFootage(1)));
            if (forensicRepPanelAnimator.gameObject.activeSelf)
                StartCoroutine(ButtonPressWithDelay(() => NavigateForensicRep(1)));
            if (directLinksPanelAnimator.gameObject.activeSelf)
                StartCoroutine(ButtonPressWithDelay(() => NavigateDirectLinks(1)));
        }
        else if(Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.Return) ){
            if (dialgTextButton.gameObject.activeSelf){
                StartCoroutine(ButtonPressWithDelay(ContinueQuestionDialouge));
            }
            else if (phoneTextButton.gameObject.activeSelf){
                Debug.Log("Hello");
                StartCoroutine(ButtonPressWithDelay(() => PhoneTextManager(curr_phone)));
            }
            
           
        }
        

    
        if (started && !settings)
        {             
            if (!removing)
            {          
                if (timeRemaining > 0)
                {
                    timeRemaining -= Time.deltaTime;
                    UpdateTimerDisplay();

                    // Check if a minute has passed
                    if (Mathf.FloorToInt(timeRemaining) % 120 == 0 && timeRemaining < totalTime - 1)
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
                    if(!suspectRemoved[4]){
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
                    if (sfxSource != null &&  suspectLeaving != null)
                        {
                            Debug.Log("heresus");
                            sfxSource.clip = suspectLeaving;
                            sfxSource.Play();
                        }
                    Debug.Log("heresus1");
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

       // FinishRemovingSuspect();
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
        Debug.Log("removing now : "+numOfSuspect + suspectRemoved[numOfSuspect]);
        if (!suspectRemoved[numOfSuspect] )
        {
            if (sfxSource != null &&  buttonClick1 != null)
            {
                sfxSource.clip = buttonClick1;
                sfxSource.Play();
            }
            if(num_of_removed!= -1)
            {
               remove_BigXs[num_of_removed].gameObject.SetActive(false);
            }
            Debug.Log("removing"+numOfSuspect);
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
         if (sfxSource != null &&  buttonClick1 != null)
            {
                sfxSource.clip = buttonClick1;
                sfxSource.Play();
            }

        foreach (var closeUp in suspectsCloseUp)
        {
            closeUp.gameObject.SetActive(false);
        }
       
        currSus = suspectNumber; // For Dialgs
        suspectsCloseUp[suspectNumber].gameObject.SetActive(true);
        closeUpSusPanelAnimator.ShowPanel();
        goBackButtonClsup.gameObject.SetActive(true);
        investigateButton.gameObject.SetActive(true);
        


    }

    private void ToggleSettingsPanel()
    {
        
        if (sfxSource != null &&  buttonClick1 != null)
        {
            sfxSource.clip = buttonClick1;
            sfxSource.Play();
        }

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
    private void ToggleTelePhonePanel()
    {
        if (sfxSource != null &&  buttonClick1 != null)
            {
                sfxSource.clip = buttonClick1;
                sfxSource.Play();
            }
        if (telePhonePanelAnimator.gameObject.activeSelf)
        {
            telePhonePanelAnimator.HidePanel();
            phoneButton.gameObject.SetActive(true);
            caseFileButton.gameObject.SetActive(true);
            
        }
        else
        {
            HideAll();
            phoneButton.gameObject.SetActive(false);
            phoneTextButton.gameObject.SetActive(false);
            telePhonePanelAnimator.ShowPanel();
        }
    }
    private void PhoneTextManager(int index){
            // if(phoneTextIndex == 0){
            //     phoneTextButton.gameObject.SetActive(true);
                curr_phone = index;
            //     phoneTextIndex++;
            // }
            // else{
                phoneTextButton.gameObject.SetActive(false);
                List<string> currList = telePhoneTexts[index];
                if(phoneTextIndex <= (currList.Count - 1)){
                    if(phoneTextIndex <= 2){
                         if (sfxSource != null &&  phonePress != null)
                        {
                            sfxSource.clip = phonePress;
                            sfxSource.Play();
                        }
                    }
                    phoneText.SetText(currList[phoneTextIndex]);
                    phoneTextButton.gameObject.SetActive(true);
                    phoneTextIndex++;
                }  
                else{
                    phoneTextIndex = 0;
                    curr_phone = -1;
                    phoneTextButton.gameObject.SetActive(false);
                    telePhoneButtons[index].interactable = false;
                } 
            //}

    }
    

     private void ToggleDiagPanel(){
        if (dialGSPanelAnimator.gameObject.activeSelf)
        {
            dialGSPanelAnimator.HidePanel();
            
            
        }
        else
        {
            if (sfxSource != null &&  buttonClick2 != null)
            {
                sfxSource.clip = buttonClick2;
                sfxSource.Play();
            }
            textIndex = 0;
            dialgTextButton.gameObject.SetActive(false);
            dialgQuestionsPanelAnimator.gameObject.SetActive(false);
            dialGSPanelAnimator.ShowPanel();
            badCopButton.gameObject.SetActive(true);
            goodCopButton.gameObject.SetActive(true);
            
        }
     }

     private void ToggleDiagQuestionPanel(bool goodCopbadcop)
    {
        if (sfxSource != null &&  buttonClick2 != null)
            {
                sfxSource.clip = buttonClick2;
                sfxSource.Play();
            }

        if (dialgQuestionsPanelAnimator.gameObject.activeSelf)
        {
          
            dialgQuestionsPanelAnimator.HidePanel();
            investigateButton.gameObject.SetActive(true);
            goBackButtonClsup.gameObject.SetActive(true);
            //ToggleDiagPanel();
            //goodCopButton.gameObject.SetActive(true);
            //badCopButton.gameObject.SetActive(true);
        }
        else
        {
           
            //dialgTextButton.gameObject.SetActive(false);
            goodCop = goodCopbadcop; 
            UpdateQuestionsListeners();
            dialgQuestionsPanelAnimator.ShowPanel();
            dialgTextButton.gameObject.SetActive(false);
            goodCopButton.gameObject.SetActive(false);
            badCopButton.gameObject.SetActive(false);
        }
    }
    private void ActivateQuestionDialouge(int suspectIndex,int copType, int dialgIndex){

        dialgText.gameObject.SetActive(true);
        ToggleDiagQuestionPanel(copType == 0);
        dialgTextButton.gameObject.SetActive(false);// set this to false by def
        currDialog = GetDialogue(suspectIndex,copType,dialgIndex); 
        int len = currDialog.Count-1;
        if(textIndex <= len){
            dialgText.SetText(currDialog[textIndex]);
            dialgTextButton.gameObject.SetActive(true);
            textIndex++;
        }
      
            //wait 10 seconds or not ? change if needed and mby also add support for space or enter. 
            //yield return new WaitForSeconds(10.0f);
        
    }
     private void ContinueQuestionDialouge(){

        dialgTextButton.gameObject.SetActive(false);// set this to false by def 
        if(textIndex <= currDialog.Count-1){
            dialgText.SetText(currDialog[textIndex]);
            dialgTextButton.gameObject.SetActive(true);
            textIndex++;
        }
        else{
            dialgText.gameObject.SetActive(false);
            dialgQuestionsPanelAnimator.ShowPanel();
            
            textIndex = 0;
        }
            //wait 10 seconds or not ? change if needed and mby also add support for space or enter. 
            //yield return new WaitForSeconds(10.0f);
        
    }

          private void SetupArrowListeners(ref Button[] arrows, Action<int> navigateAction)
    {
        arrows[0].onClick.AddListener(() => StartCoroutine(ButtonPressWithDelay(() => navigateAction(-1))));
        arrows[1].onClick.AddListener(() => StartCoroutine(ButtonPressWithDelay(() => navigateAction(1))));
    }
     private void ToggleEvidencePanel(PanelAnimatorController panelAnimator, ref int currentPage, Image[] images)
    {

        if (panelAnimator.gameObject.activeSelf)
        {
            if (sfxSource != null &&  buttonClick2 != null)
            {
                sfxSource.clip = buttonClick2;
                sfxSource.Play();
            }

            panelAnimator.HidePanel();
            ToggleEvidencePanel();

        }
        else
        {
            ToggleEvidencePanel();
            caseFileButton.gameObject.SetActive(false);
            panelAnimator.ShowPanel();
            UpdateEvidenceDisplay(currentPage, images);
        }
    }
      private void ToggleDirectStatmentsPanel()
    {
        if (sfxSource != null &&  buttonClick2 != null)
            {
                sfxSource.clip = buttonClick2;
                sfxSource.Play();
            }
        if (directStatmentsPanelAnimator.gameObject.activeSelf)
        {
            directStatmentsPanelAnimator.HidePanel();
            ToggleEvidencePanel();

        }
        else
        {
            ToggleEvidencePanel();
            caseFileButton.gameObject.SetActive(false);
            directStatmentsPanelAnimator.ShowPanel();
        }
    }
    private void ToggleSurvFootagePanel() => ToggleEvidencePanel(survFootPanelAnimator, ref currPgSurvFoot, survFootImages);
    private void ToggleForensicRepPanel() => ToggleEvidencePanel(forensicRepPanelAnimator, ref currPgForensicRep, forensicRepImages);
    private void ToggleDirectLinksPanel() => ToggleEvidencePanel(directLinksPanelAnimator, ref currPgDirectLinks, directLinksImages);

    private void NavigateEvidence(ref int currentPage, int direction, Image[] images, Button[] arrows)
    {
        if (sfxSource != null &&  buttonClick2 != null) //arrow keys sound
        {
            sfxSource.clip = buttonClick2;
            sfxSource.Play();
        }
        Debug.Log("direction: " + direction);
        if((currentPage != (images.Length-1) && direction == 1) || (currentPage != 0 && direction == -1))
            currentPage += direction ;
        UpdateEvidenceDisplay(currentPage, images);
        UpdateArrowStates(currentPage, arrows, images.Length);
    }

    private void ToggleEvidencePanel()
    {
        if (evidencePanelAnimator.gameObject.activeSelf)
        {
            if(sfxSource != null &&  buttonClick2 != null){
                sfxSource.clip = buttonClick2;
                sfxSource.Play();

            }
            evidencePanelAnimator.HidePanel();
            caseFileButton.gameObject.SetActive(true);
            phoneButton.gameObject.SetActive(true);
        }
        else
        {
             if (sfxSource != null &&  casefilepress != null &&  caseFileButton.gameObject.activeSelf)
            {
                sfxSource.clip = casefilepress;
                sfxSource.Play();
            }
            else if(sfxSource != null &&  buttonClick2 != null){
                sfxSource.clip = buttonClick2;
                sfxSource.Play();

            }
            HideAll();
            caseFileButton.gameObject.SetActive(false);
            evidencePanelAnimator.ShowPanel();
        }
    }
    
     private void NavigateSurvFootage(int direction) => NavigateEvidence(ref currPgSurvFoot, direction, survFootImages, arrowsSurvFoot);
    private void NavigateForensicRep(int direction) => NavigateEvidence(ref currPgForensicRep, direction, forensicRepImages, arrowsForensicRep);
    private void NavigateDirectLinks(int direction) => NavigateEvidence(ref currPgDirectLinks, direction, directLinksImages, arrowsDirectLinks);

  private void UpdateEvidenceDisplay(int currentPage, Image[] images)
{
    if (sfxSource != null && buttonClick2 != null && currentPage != 0)
    {
        sfxSource.clip = buttonClick2;
        sfxSource.Play();
    }

   
    StartCoroutine(UpdateEvidenceDisplayCoroutine(currentPage, images));
}

private IEnumerator UpdateEvidenceDisplayCoroutine(int currentPage, Image[] images)
{
    if(currentPage!=0)
    yield return new WaitForSeconds(0.3f);

     if (sfxSource != null && paperflip != null && currentPage != 0)
    {
        sfxSource.clip = paperflip;
        sfxSource.Play();
    }

    Debug.Log("currentPage: " + currentPage);
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
            if (sfxSource != null &&  buttonClick2 != null)
            {
                sfxSource.clip = buttonClick2;
                sfxSource.Play();
            }
            
            closeUpSusPanelAnimator.HidePanel();
            goBackButtonClsup.gameObject.SetActive(false);
            investigateButton.gameObject.SetActive(false);
            currSus = -1;
            if (dialGSPanelAnimator.gameObject.activeSelf)
                dialGSPanelAnimator.HidePanel();        
        }
        else
        {
             if (sfxSource != null &&  buttonClick1 != null)
            {
                sfxSource.clip = buttonClick1;
                sfxSource.Play();
            }
            closeUpSusPanelAnimator.ShowPanel();
           // caseFileButton.gameObject.SetActive(false);
            //dialgTextButton.gameObject.SetActive(false);
            goBackButtonClsup.gameObject.SetActive(true);
            investigateButton.gameObject.SetActive(true);


        }
    }

    private void ToggleRemovingSuspectPanel()
    {
        if (removingSusPanelAnimator.gameObject.activeSelf)
        {
            removingSusPanelAnimator.HidePanel();
            caseFileButton.gameObject.SetActive(true);
            phoneButton.gameObject.SetActive(true);
        }
        else
        {
            HideAll();
            removingSusPanelAnimator.ShowPanel();
        }
    }

    private void ToggleStartingPanel()
    {
        if (sfxSource != null &&  buttonClick2 != null)
            {
                sfxSource.clip = buttonClick2;
                sfxSource.Play();
            }

        if(startingProg == 0){
            text1.gameObject.SetActive(false);
            text2.gameObject.SetActive(true);
            startingProg++;
        }
        else{
            suspectPanel.SetActive(true);
            startingPanelAnimator.HidePanel();
            started = true;
        }
    }

    private void HideAll()
    {
        if (closeUpSusPanelAnimator.gameObject.activeSelf)
        closeUpSusPanelAnimator.HidePanel();
        if (evidencePanelAnimator.gameObject.activeSelf)
        evidencePanelAnimator.HidePanel();
        if (removingSusPanelAnimator.gameObject.activeSelf)
        removingSusPanelAnimator.HidePanel();
        if (settingsPanelAnimator.gameObject.activeSelf)
        settingsPanelAnimator.HidePanel();
        if (survFootPanelAnimator.gameObject.activeSelf)
        survFootPanelAnimator.HidePanel();
        if (forensicRepPanelAnimator.gameObject.activeSelf)
        forensicRepPanelAnimator.HidePanel();
        if (directLinksPanelAnimator.gameObject.activeSelf)
        directLinksPanelAnimator.HidePanel();
        if (directStatmentsPanelAnimator.gameObject.activeSelf)
        directStatmentsPanelAnimator.HidePanel();
        caseFileButton.gameObject.SetActive(false);
         if (telePhonePanelAnimator.gameObject.activeSelf)
        telePhonePanelAnimator.HidePanel();
         if (dialGSPanelAnimator.gameObject.activeSelf)
        dialGSPanelAnimator.HidePanel();
        caseFileButton.gameObject.SetActive(false);
        phoneButton.gameObject.SetActive(false);
        investigateButton.gameObject.SetActive(false);
        goBackButtonClsup.gameObject.SetActive(false);
        
    }

     
 private void InitializeDialogues()
    {
        allSuspectDialogues = new List<string>[7, 2, 7];

        // Gal Gumbler (Pride)
        InitializeSuspectDialogues(0, sus1Dialogues);

        // Sec Oshrit Yeffeti (Greed)
        InitializeSuspectDialogues(1, sus2Dialogues);

        // Martinez Despadas (Wrath)
        InitializeSuspectDialogues(2, sus3Dialogues);

        // Dr. Michael Coddler (Envy)
        InitializeSuspectDialogues(3, sus4Dialogues);

        // Marina the Lab Assistant (Lust)
        InitializeSuspectDialogues(4, sus5Dialogues);

        // Prof. Semion Kogan (Gluttony)
        InitializeSuspectDialogues(5, sus6Dialogues);

        // Janitor Yonas (Sloth)
        InitializeSuspectDialogues(6, sus7Dialogues);
    }

    private void InitializeSuspectDialogues(int suspectIndex, List<string>[,] suspectDialogues)
    {
        for (int copType = 0; copType < 2; copType++)
        {
            for (int dialogueType = 0; dialogueType < 7; dialogueType++)
            {
                allSuspectDialogues[suspectIndex, copType, dialogueType] = suspectDialogues[copType, dialogueType];
            }
        }
    }

    public List<string> GetDialogue(int suspectIndex, int copType, int dialogueType)
    {
        return allSuspectDialogues[suspectIndex, copType, dialogueType];
    }

    // Suspect dialogues
    private List<string>[,] sus1Dialogues = new List<string>[2, 7]
    {
        { // Good Cop
            new List<string> { "Can you tell us where you were the night of Professor Shimi's murder? ", "<b>Gumbler</b>: I was in my office, preparing for my lecture.\nIt's important to stay ahead in academia, you know. " },
            new List<string> { "We found your fingerprints on Professor Shimi's desk.\nI'm sure there's a reasonable explanation. ", "<b>Gumbler</b>: Of course, Detective. We had a meeting earlier that day.\nI often visit his office for academic discussions. " },
            new List<string> { "Witnesses saw you arguing with Shimi the day before the murder.\nCan you explain what that was about? ", "<b>Gumbler</b>: It was just a professional disagreement.\nAcademic debates can get heated, but it was nothing out of the ordinary. " },
            new List<string> { "I understand critiques can be harsh.\nDid Shimi's comments on your latest paper upset you? ", "<b>Gumbler</b>: Well, no one likes criticism, but it's part of the process.\nI can handle it. " },
            new List<string> { "Professor Gumbler, I know working relationships can be complex.\nCan you tell me about your relationship with Professor Shimi? ", "<b>Gumbler</b>: We had a professional relationship.\nWe didn't always see eye to eye, but I respected his intellect. " },
            new List<string> { "Professor Gumbler, I understand that academic environments can be highly competitive.\nDid Professor Shimi ever make any threats about your academic career? ", "<b>Gumbler</b>: He could be harsh, but I never felt threatened smirks.\nI've always been confident in my abilities. " },
            new List<string> { "Professor Gumbler, considering everything that's happened,\ndo you have any thoughts on who might be responsible for Professor Shimi's murder? ", "<b>Gumbler</b>: It's hard to say. Shimi had his share of disagreements with many people, not just me.\nBesides,isn't that your job as a detective to find out? Why are you trying to make me do your work? " }
        },
        { // Bad Cop
            new List<string> { "Gumbler, stop pretending.\nWhere were you the night of Professor Shimi's murder? Or are you too scared to admit it? ", "<b>Gumbler</b>: I was in my office, working on my lecture.\nI'm not scared to admit anything.\n I don't need to hide behind lies. " },
            new List<string> { "Your fingerprints are all over Shimi's desk.\nWhat were you doing there, trying to prove you're important? ", "<b>Gumbler</b>: I was there for a meeting, discussing academic matters.\nAnd I am important! My work speaks for itself, unlike some others who may be dead. " },
            new List<string> { "Witnesses saw you arguing with Shimi.\nWhat was it about, Gumbler? Couldn't handle someone being smarter than you? ", "<b>Gumbler</b>: Smarter? Shimi had his moments, but he wasn't superior to me.\nOur argument was just a professional disagreement, nothing more. " },
            new List<string> { "Shimi tore your latest paper apart, didn't he?\nDid that hurt your fragile ego enough to kill him? ", "<b>Gumbler</b>: My ego is just fine, Detective! Critiques are part of the academic process.\nI'm used to them, and I certainly wouldn't kill over one bad review. In what world do you live in, lala land? " },
            new List<string> { "Gumbler, cut the nonsense. What was your real relationship with Shimi?\nWere you just jealous of his success? ", "<b>Gumbler</b>: Jealous? Hardly. We had our differences, but I respected his intellect.\nHe was no threat to my career or my accomplishments. " },
            new List<string> { "You think you're better than everyone else, don't you?\nDid Shimi threatening your career make you realize you're not as brilliant as you think? ", "<b>Gumbler</b>: Shimi never threatened my career.\nI know my worth, and I don't need his validation. If anything, he was the one who felt threatened by my achievements. " },
            new List<string> { "You better start talking, Gumbler. Who do you think killed Shimi?\nOr are you too busy protecting your own pride to see the truth? ", "<b>Gumbler</b>: With that attitude of yours I don't even want to cooperate with you. " }
        }
    };
     private List<string>[,] sus2Dialogues = new List<string>[2, 7]
    {
        { // Good Cop (Sec Oshrit Yeffeti - Greed)
            new List<string> { "Ms. Yeffeti, we're just trying to understand what happened.\nWhere were you on the night of the murder? ", "<b>Yeffeti</b>: I was at the office, working late as usual.\nThere's always so much to do, and I take my job very seriously. " },
            new List<string> { "Your fingerprints were found on the bookend used as the murder weapon.\nHow do you explain that? ", "<b>Yeffeti</b>: I work as a secretary, my fingerprints are on all the books in his office.\nIt's part of my job to keep things in order. " },
            new List<string> { "A witness saw you leaving Shimi's office in a hurry.\nWhy were you so nervous? ", "<b>Yeffeti</b>: *<i>nervously</i>* I was just late for another meeting.\nIt's a busy job, and I was probably worried about being late. " },
            new List<string> { "Financial records show substantial discrepancies.\nWere you afraid of what Shimi might uncover? ", "<b>Yeffeti</b>: I don't know what you are talking about, I never stole funds. " },
            new List<string> { "Ms. Yeffeti, working closely with Professor Shimi, you must have known him well.\nCan you tell me about your relationship with him? ", "<b>Yeffeti</b>: We worked together often.\nHe was demanding, but I tried my best to meet his expectations.\n I knew how important his work was. " },
            new List<string> { "Ms. Yeffeti, handling financial matters can be stressful.\nDid you ever feel threatened by Professor Shimi's scrutiny over the finances? ", "<b>Yeffeti</b>: *<i>defensive</i>* Threatened? No, I just wanted to do my job well.\nMaybe I felt pressured, but I never felt like I was in danger. " },
            new List<string> { "Ms. Yeffeti, considering everything that's happened,\ndo you have any thoughts on who might be responsible for Professor Shimi's murder? ", "<b>Yeffeti</b>: It's hard to say.\nHe had a lot of demands and not everyone could keep up.\n Maybe someone else felt the pressure more than I did. " }
        },
        { // Bad Cop
            new List<string> { "Yeffeti, quit yapping. Where were you the night Shimi was murdered?\nOr were you too busy counting your ill-gotten gains?\n You think we don't know that? ", "<b>Yeffeti</b>: I don't know what you're talking about. " },
            new List<string> { "Your fingerprints are on the murder weapon.\nWere you trying to cover up your tracks after siphoning off money? ", "<b>Yeffeti</b>: *<i>nervous</i>* I handle all the items in his office regularly.\nIt's part of my job to keep things in order.\n And I didn't siphon any money, I don't know what you are talking about! " },
            new List<string> { "A witness saw you bolting out of Shimi's office.\nWhat were you running from?\n The mess you created trying to protect your precious money? ", "<b>Yeffeti</b>: *<i>panicked</i>* I wasn't running from anything! I was just late for another meeting.\nI had no reason to harm him. Besides, what precious money? " },
            new List<string> { "Financial discrepancies in your records, Yeffeti.\nDid you kill Shimi to keep your greedy hands in the university's coffers? ", "<b>Yeffeti</b>: I don't know what you are talking about. ", "It is very clear you siphoned university funds to yourself. ", "<b>Yeffeti</b>: The court will decide if that is true or not, not you. " },
            new List<string> { "Tell me the truth, Yeffeti.\nWere you using your position close to Shimi to manipulate funds? How deep does your greed go? ", "<b>Yeffeti</b>: *<i>defensive</i>* Excuse me?! I manipulated funds? I am greedy?\nIn what world do you live in detective? We had a professional relationship.\n I respected him, and I tried to meet his high standards. There was nothing more to it. " },
            new List<string> { "You handled the finances, and Shimi was onto you.\nDid you kill him to cover up your embezzlement? ", "<b>Yeffeti</b>: I plead the fifth. ", "Huh, there's no fifth here we aren't in America. ", "<b>Yeffeti</b>: I said I plead the fifth. " },
            new List<string> { "Stop the charade, Yeffeti.\nWho do you think killed Shimi? Or are you too busy trying to save your own skin? ", "<b>Yeffeti</b>: *<i>desperately</i>* I don't know! \nMaybe someone else who felt the pressure or couldn't keep up with his demands.\n But it wasn't me, I swear! " }
        }
    };
     private List<string>[,] sus3Dialogues = new List<string>[2, 7]
    {
        { // Good Cop (Martinez Despadas - Wrath)
            new List<string> { "On the night of the murder, you were seen leaving the gym, looking angry.\nDid you go to Shimi's office afterward? ", "<b>Despadas</b>: *<i>calmly</i>* No, I went straight home.\nI was angry, yes, but I needed to cool off.\n Confronting him wouldn't have helped. " },
            new List<string> { "Skin cells matching your DNA were found under Shimi's fingernails.\nCan you explain that? ", "<b>Despadas</b>: *<i>sighs</i>* We had a fight a while back.\nMaybe he scratched me then.\n It wasn't from that night. " },
            new List<string> { "Witnesses heard you muttering about confronting Shimi.\nWhat were you planning to do? ", "<b>Despadas</b>: *<i>defensive</i>* I was just venting.\nI had no real intention of confronting him that night.\n I was frustrated, that's all. " },
            new List<string> { "Martinez, we've come across some records that suggest you had an altercation with Shimi in the past,\nyou lost your scholarship because of him? Can you tell us about that incident? ", "<b>Despadas</b>: *<i>angrily</i>* Yeah, Shimi's actions cost me my scholarship.\nHe thought I wasn't good enough, but he never gave me a fair chance.\nThat still stings. " },
            new List<string> { "Martinez, what was your relationship with Professor Shimi like?\nWe know he gave you low grades on purpose, is there a reason? ", "<b>Despadas</b>: *<i>bitterly</i>* He had it in for me from the start.\nI don't know why, but he never saw my potential.\n It was always a struggle with him. " },
            new List<string> { "Martinez, we know you had a rough history with Shimi.\nHelp us understand what happened. ", "<b>Despadas</b>: *<i>resigned</i>* Shimi was tough on me, harsher than on others.\nI guess he saw me as a threat or just didn't like me.\n It wasn't fair, but I had to deal with it. " },
            new List<string> { "Martinez, given everything that's happened,\ndo you have any idea who might have wanted to harm Professor Shimi? ", "<b>Despadas</b>: *<i>thoughtfully</i>* There were many who didn't like him.\nHe was a hard man to get along with.\n But honestly, I can't say who would go that far. " }
        },
        { // Bad Cop
            new List<string> { "Despadas, you were seen leaving the gym angry.\nDid you go to Shimi's office to finally settle the score? Were you that mad? ", "<b>Despadas</b>: *<i>angrily</i>* I went home to cool off!\nI was mad, but not enough to do something stupid like that. " },
            new List<string> { "Your DNA was found under Shimi's fingernails.\nWhat did you do, get into another fight with him? ", "<b>Despadas</b>: *<i>defensive</i>* We fought months ago. Maybe he scratched me then.\nI didn't touch him that night! ", "What? I know you have a thick skull but are you that stupid?\nYou really think I am gonna believe that? ", "<b>Despadas</b>: Whatever man, believe what you want " },
            new List<string> { "Witnesses heard you muttering about confronting Shimi.\nWhat was your plan, Despadas? Were you going to beat him to a pulp? ", "<b>Despadas</b>: Everyone has fantasies, no? I had fantasies of wanting to beat him to a pulp, yes.\nBut the important thing to note is that they are fantasies, and I wouldn't do it in real life.", "Uh huh, sure buddy. " },
            new List<string> { "We know about your past altercation with Shimi.\nYou lost your scholarship because of him. Did you finally decide to get your revenge and end him? ", "<b>Despadas</b>: yelling Revenge? Sure, I was angry about losing my scholarship, but I wouldn't kill over it.\nI'm not a murderer! " },
            new List<string> { "I was a student too before, I know what it's like to go crazy after getting your bad grades and wanting to kill a professor.\nYou snapped, didn't you? ", "<b>Despadas</b>: Crazy? I was crazy once.\nThey put me in a room. A rubber room. A rubber room with rats. And rats make me crazy.\n Grades don't make crazy, I wouldn't kill Shimi after some stupid grade. " },
            new List<string> { "You had a rough history with Shimi, didn't you?\nHow badly did you want him out of the picture? Bad enough to kill? ", "<b>Despadas</b>: *<i>clenched fists</i>* I wanted him out of my life, sure. But killing him? No. That's not me. " },
            new List<string> { "I know you can think in that thick skull of yours Despadas.\nWho do you think wanted Shimi gone? ", "<b>Despadas</b>: Thick skull? You better pray I don't see you again after this is over. " }
        }
    };

    private List<string>[,] sus4Dialogues = new List<string>[2, 7]
    {
        { // Good Cop (Dr. Michael Coddler - Envy)
            new List<string> { "Dr. Coddler, where were you the night Professor Shimi was murdered? ", "<b>Coddler</b>: *<i>calmly</i>* I was at home, working on my research.\nI needed some quiet time away from the university. ", "Home? You were at the university. ", "<b>Coddler</b>: I consider my office my safe space, my home, I don't see it part of the university compound. " },
            new List<string> { "Your blood was found near Shimi's office door.\nCan you explain how it got there? ", "<b>Coddler</b>: *<i>hesitant</i>* I cut myself earlier in the day.\nI must have accidentally brushed against something when I was near his office." },
            new List<string> { "A witness saw you exiting the building laughing hysterically.\nWhat did you do or see? ", "<b>Coddler</b>: *<i>awkwardly</i>* I remember feeling relieved after finishing some work.\nI have the joker laugh syndrome where I laugh on random occasions. ", "okay... " },
            new List<string> { "Emails from Shimi belittling your work were found on his laptop.\nDid that make you angry? ", "<b>Coddler</b>: *<i>sighs</i>* Yes, it did.\nShimi was often critical of my work, but I tried to take it in stride.\n Anger is part of the process, but it didn't drive me to harm him. " },
            new List<string> { "Dr. Coddler, can you tell me about your professional relationship with Professor Shimi? ", "<b>Coddler</b>: *<i>reflective</i>* We had a complicated relationship.\nThere was mutual respect, but also a lot of tension.\n He could be very critical, which made things difficult at times. " },
            new List<string> { "Dr. Coddler, it's clear you and Shimi had some professional disagreements.\nDid you ever feel threatened by him? ", "<b>Coddler</b>: *<i>thoughtfully</i>* Threatened? Not exactly.\nIntimidated, perhaps.\n He had a way of making you doubt yourself, but I wouldn't say I felt physically threatened. " },
            new List<string> { "Dr. Coddler, considering everything that's happened, do you have any thoughts on who might be responsible for Professor Shimi's murder? ", "<b>Coddler</b>: *<i>hesitant</i>* I don't know for sure.\nShimi had a lot of enemies, given how harsh he could be.\n It's possible someone finally snapped, but I can't point fingers without evidence. " }
        },
        { // Bad Cop
            new List<string> { "Coddler, you were in Shimi's building on the night he was murdered, weren't you? \nCouldn't stand seeing him succeed while you floundered? ", "<b>Coddler</b>: First of all, I'm definitely not floundering in any way,\nsecond of all, yes my office is in that building as well, it is pretty close to his office. ", "And you didn't hear anything come out of Shimi's office that night? ", "<b>Coddler</b>: I heard a heated argument between Shimi and someone else but other than that nothing else out of the ordinary. " }, //might need checking
            new List<string> { "Your blood was found at the scene.\nDid Shimi manage to hurt you before you tried to take him down out of jealousy? ", "<b>Coddler</b>: *<i>nervous</i>* I have regular nose bleeds, it could be from the night before his murder when I was at his office. " },
            new List<string> { "A witness saw you leaving the building laughing like the Joker.\nWhat were you so happy about? Finally getting rid of Shimi? ", "<b>Coddler</b>: *<i>awkwardly</i>* I laugh when I'm stressed! It was just a reaction after a long day.\nThere was nothing sinister about it. " },
            new List<string> { "Shimi's emails tore your work apart, didn't they?\nDid that wound you enough to make you snap? ", "<b>Coddler</b>: *<i>angrily</i>* Yes, they humiliated me! But I didn't let it drive me to murder.\nCriticism is part of academia, even if it stings. " },
            new List<string> { "Cut the act, Coddler. What was your real relationship with Shimi?\nWere you consumed by envy, always trying to outdo him? ", "<b>Coddler</b>: Wouldn't you like to know weatherboy? This is none of your business. " },
            new List<string> { "Shimi humiliated you and made your work look like a joke, didn't he?\nHow badly did you want him out of your way? ", "<b>Coddler</b>: *<i>frustrated</i>* He did belittle me, made my life hell at times.\nBut I didn't kill him. I wanted him gone, but not like that. " },
            new List<string> { "Who do you think killed Shimi? That nerdy head of yours can think no?\nDon't try to pin it on someone else to save your own skin 'cause I'll see through it. ", "<b>Coddler</b>: My nerdy head of mine knows this can be used against me so I won't be answering anything.\nI know what to answer and what not to answer detective. " }
        }
    };

    private List<string>[,] sus5Dialogues = new List<string>[2, 7]
    {
        { // Good Cop (Marina the Lab Assistant - Lust)
            new List<string> { "Marina, can you tell us where you were the night of Professor Shimi's murder? ", "<b>Marina</b>: *<i>tearfully</i>* I was in the lab, working late as usual.\nI left around midnight. " },
            new List<string> { "Your hair was found in his office and photos of you two were found torn and scattered on the ground in his office,\ncan you explain to me what happened? ", "<b>Marina</b>: *<i>Flustered</i>* Do you really want me to answer this question?\nIt is kind of personal.. I mean every couple has its ups and downs, right detective? " },
            new List<string> { "A witness saw you crying near the lab that night.\nCan you tell me why you were so upset? ", "<b>Marina</b>: *<i>sniffling</i>* We had a small argument, didn't expect it to be my last with him." },
            new List<string> { "We found some lipstick on Shimi's neck, is it yours or someone else's? ", "<b>Marina</b>: *<i>nervously</i>* It's mine… " },
            new List<string> { "Marina, we understand you had a personal relationship with Professor Shimi.\nCan you tell us more about it? ", "<b>Marina</b>: *<i>sadly</i>* We were close, but it was complicated.\nHe was charming but also very demanding.\n It was hard to keep up with him. " },
            new List<string> { "Marina, relationships can be complicated.\nDid you ever feel threatened or betrayed by Shimi? ", "<b>Marina</b>: *<i>hesitant</i>* Yes, sometimes.\nHe could be very controlling, and I felt trapped at times.\n But I never wanted to hurt him. " },
            new List<string> { "Marina, considering everything that's happened,\ndo you have any thoughts on who might be responsible for Professor Shimi's murder? ", "<b>Marina</b>: *<i>thoughtfully</i>* I don't know.\nHe had a lot of enemies, and he could be very harsh.\n I wished I could help you to bring the perpetrator to justice. " }
        },
        { // Bad Cop
            new List<string> { "Where were you the night Shimi was killed?\nI don't believe you just stayed in the lab all night. ", "<b>Marina</b>: *<i>defensive</i>* I was in the lab, working.\nI didn't go anywhere else.\n I needed to be alone to think. " },
            new List<string> { "Your hair and torn photos of you two were found in his office.\nWhat really happened? ", "<b>Marina</b>: *<i>deadfaced</i>* I never knew a detective would be so fixated on someone's relationships.\nWe had an argument and that's it. " },
            new List<string> { "A witness saw you crying near the lab.\nWhat did Shimi do to make you that upset? Did he end things with you? ", "<b>Marina</b>: *<i>tearfully</i>* Yes, he did. He wanted to end our relationship abruptly. " },
            new List<string> { "Lipstick was found on Shimi's neck.\nIs it yours? Or is it someone else's and that is why you killed him? ", "<b>Marina</b>: It is mine yes, we were together in the morning.\nIs there an issue with that detective? ", "No no not at all. " },
            new List<string> { "Don't lie to me, Marina.\nWe know your relationship with Shimi was more than just professional,\nyou managed to hide it long enough from his wife,\n what was it really like at the end? Was there another woman in the scene? ", "<b>Marina</b>: *<i>bitterly</i>* It was complicated. He was charming but also very demanding.\nThere was no other woman, other than his wife that is. \nAt least I hope there isn't. " },
            new List<string> { "You felt threatened or betrayed by Shimi, didn't you? Did you decide to get even? ", "<b>Marina</b>: *<i>sobbing</i>* I felt betrayed, yes. But I didn't want to hurt him.\nI just wanted him to stay. " },
            new List<string> { "Who do you think killed Shimi?\nOr are you too busy covering up your own involvement to see the truth? ", "<b>Marina</b>: *<i>as coldly as humanly possible with death in the eyes and some souls twirling in her eyes</i>*\nIf I knew who killed Shimi they would be dead already, don't you think? " }
        }
    };
    private List<string>[,] sus6Dialogues = new List<string>[2, 7]
    {
        { // Good Cop (Prof. Semion Kogan - Gluttony)
            new List<string> { "Professor Kogan, can you tell us where you were the night of Professor Shimi's murder? ", "<b>Kogan</b>: *<i>calmly</i>* I was at home, enjoying a quiet evening.\nI needed some time to unwind after a long day. " },
            new List<string> { "Your coffee cup was found in Shimi's office.\nWere you there the night of the murder? ", "<b>Kogan</b>: *<i>nonchalantly</i>* I often visit Shimi's office for a chat and a coffee. It must have been from an earlier visit. " },
            new List<string> { "Witnesses mentioned your excessive spending habits.\nCare to explain? ", "<b>Kogan</b>: *<i>jovially</i>* I enjoy the finer things in life, Detective.\nI work hard and like to treat myself. Is that a crime? " },
            new List<string> { "Financial documents found in Shimi's office link you to significant misuse of funds.\nWere you worried Shimi would expose you? ", "<b>Kogan</b>: *<i>shrugging</i>* Shimi was meticulous, always going through the accounts.\nI wasn't worried.\n Everything I did was within the rules. " },
            new List<string> { "Professor Kogan, how was your relationship with Professor Shimi? Did you two get along? ", "<b>Kogan</b>: *<i>thoughtfully</i>* We had our differences, but we managed to work together.\nHe was a stickler for rules, but we found common ground. " },
            new List<string> { "Professor Kogan, there are rumors about your misuse of university funds.\nCan you explain? ", "<b>Kogan</b>: *<i>smiling</i>* Rumors, Detective. Just rumors.\nI might have bent the rules a bit, but nothing serious. " },
            new List<string> { "Professor Kogan, considering everything that's happened,\ndo you have any thoughts on who might be responsible for Professor Shimi's murder? ", "<b>Kogan</b>: *<i>pensive</i>* Shimi was strict and had many enemies.\nIt could have been anyone who felt wronged by him. " }
        },
        { // Bad Cop
            new List<string> { "Kogan, don't waste my time.\nYou were chatting with Shimi on the night of the murder, weren't you? Did you kill him then? ", "<b>Kogan</b>: too perfect of an alibi, don't you think? " },
            new List<string> { "Your coffee mug was found in his office,\ndid you get a caffeine boost and decide to do it right then and there? ", "<b>Kogan</b>: *<i>scoffs</i>* I often visit his office for coffee.\nIt's not unusual.\n I didn't kill him. " },
            new List<string> { "I get it,\nwitnesses say you love an extravagant lifestyle and heard you at some occasions mention that Shimi threatened to end it,\n it's the perfect murder, don't you think? ", "<b>Kogan</b>: *<i>defensive</i>* I enjoy the finer things, yes.\nBut I wouldn't kill to keep them.\n Besides, Shimi was just being his usual self, making threats he couldn't follow through on. " },
            new List<string> { "The financial documents tie you to misused funds.\nWere you scared Shimi would expose you? ", "<b>Kogan</b>: *<i>nervously</i>* I wasn't scared.\nShimi was meticulous, but everything I did was within the rules.\n Maybe bending them a bit, but not breaking them. " },
            new List<string> { "What was your relationship with Shimi really like? ", "<b>Kogan</b>: *<i>bitterly</i>* Shimi was a stickler for rules, and we had our differences.\nBut it wasn't worth killing over. " },
            new List<string> { "You were misusing university funds, weren't you?\nDid you kill Shimi to keep it quiet? ", "<b>Kogan</b>: *<i>frustrated</i>* I may have bent some rules, but I didn't kill him.\nI wouldn't risk everything for that.\n I'm not stupid. " },
            new List<string> { "That foie gras filled body of yours can still properly think, no?\nWho do you think did it if it weren't you,\n because you seem to be the perfect suspect? ", "<b>Kogan</b>: *<i>angrily</i>* Plenty of people had issues with Shimi.\nYou should be looking at them, not me.\n I didn't do it. " }
        }
    };

    private List<string>[,] sus7Dialogues = new List<string>[2, 7]
    {
        { // Good Cop (Janitor Yonas - Sloth)
            new List<string> { "Yonas, can you tell us where you were the night of Professor Shimi's murder? ", "<b>Yonas</b>: *<i>slowly</i>* I was doing my rounds, cleaning the halls and emptying trash bins.\nI finished my shift late and went straight home. " },
            new List<string> { "Some cleaning supplies stains were found in Shimi's office and Shimi's hands showed clear signs of chemical burns.\nCare to explain why? ", "<b>Yonas</b>: *<i>confused</i>* I might have cleaned his office earlier, but I don't know about any burns.\nMaybe he touched something he shouldn't have. " },
            new List<string> { "A witness saw you near the office cleaning your hands for 20 minutes nervously.\nWhat were you doing? ", "<b>Yonas</b>: *<i>defensively</i>* I was just washing up. The chemicals can be harsh, and I wanted to make sure my hands were clean. " },
            new List<string> { "We found some questionable photos of Marina and Shimi in your janitor closet with some other questionable stolen documents,\nwhat were they doing there? ", "<b>Yonas</b>: Uhhh potato tomato. ", "What? ", "<b>Yonas</b>: I don't get paid enough to answer these questions. " },
            new List<string> { "Yonas, we know Professor Shimi treated you very harshly.\nAny thoughts on that? ", "<b>Yonas</b>: Ehm it is what it is, I still get my paycheck at least. " },
            new List<string> { "There are multiple harsh maintenance complaints from Shimi about your work and \nit seems that there was an incident where he threatened to splash you with some harsh chemicals.\n Did that make you scared of him? ", "<b>Yonas</b>: *<i>nervously</i>* Yes, it did.\nHe was always threatening me, making me feel like I couldn't do anything right. " },
            new List<string> { "Yonas, considering everything that's happened,\ndo you have any thoughts on who might be responsible for Professor Shimi's murder? ", "<b>Yonas</b>: Do I get a bonus on my paycheck if I answer? ", "Uhhh no? ", "<b>Yonas</b>: Then no I don't know. " }
        },
        { // Bad Cop
            new List<string> { "Yonas I know you are kinda slow but stop stalling.\nWhere were you the night Shimi was killed? ", "<b>Yonas</b>: Cleaning here and there, I'm not slow you classist. " },
            new List<string> { "Some of your cleaning supplies were found spilled in Shimi's office and Shimi's arms showed signs of chemical burns.\nDid you splash him with the chemicals before he did the same to you and then finished the job? ", "<b>Yonas</b>: Shimi probably took some to end his miserable life and then spilled them on himself by accident for all I care.\nI wouldn't go out of my way and hurt him for some stupid remarks about some stupid job. " },
            new List<string> { "A witness saw you washing your hands for 20 minutes, looking nervous.\nThat isn't normal behavior, what were you up to? ", "<b>Yonas</b>:I wanted to make sure there were no harsh chemicals remaining on my hands after cleaning,they can ruin your skin pretty bad you know?\n I'm still young and I want to cherish my skin. " },
            new List<string> { "We found questionable photos and stolen documents in your janitor closet.\nOther than it being weird why would you keep them with you?\n Did you want to threaten Shimi with something or are you just a weirdo? ", "<b>Yonas</b>: Whaaaa, how did they get in there lmao, pffttt. ", "<b>Marina</b>: Omg why are you so weird? " },
            new List<string> { "Tell me the truth, Yonas.\nWhat was your real relationship with Shimi? Did his harsh treatment push your crazy buttons? ", "<b>Yonas</b>: I honestly don't get paid enough to care for how he treats me,\nhe doesn't really phase me I just want to go home. " },
            new List<string> { "Shimi's complaints about your work were pretty harsh,\nnot to mention the time he threatened to spill harsh chemicals on you that one time.\n Did that make you angry enough to kill him? ", "<b>Yonas</b>:Nuh Uh. ", "Uh huh. ", "<b>Yonas</b>: It would hurt my resume if I had a murder charge on me,\nI am still applying for jobs you know, I wouldn't murder someone. " },
            new List<string> { "Let that small brain of yours think for a bit and tell me who it is if it isn't you. ", "<b>Yonas</b>: I'll have you know I'm pretty smart, asshole, I have a master's in computer science! ", "Why are you working as a janitor then? ", "<b>Yonas</b>:The market is oversaturated nowadays :( \nI think it's Professor Kogan, he had too much to lose if Shimi leaked the documents. ", "<b>Kogan</b>: What the fuck,\n detective this is obviously defamation,\n he is trying revenge for the fact that I got him fired from my brother's company! ", "<b>Yonas</b>: Nuh uh, shut up Kogan, you pompous clown. " }
        }
    };


    private void initPhoneTexts(){

        telePhoneTexts[0] = new List<string> { "  . . . . . . " , "  . . . . .  " ,"  . . . .  " , "  <b>M. Goldie</b>: Dexter, you’re tellin’ me that bookend was the murder weapon?\n C’mon, it don’t sit right. ","  <b>Dexter M.:</b> Listen, Goldie, I’m tellin’ ya, that bookend’s got nothin’ to do\n with it. Sure, it left a mark, but that ain't what did the guy in.\nThere’s somethin’ else here we’re missin’."};
        telePhoneTexts[1] = new List<string> { "  . . . . . . " , "  . . . . .  " ,"  . . . .   " , "  <b>M. Goldie</b>: Alex, you heard about Shimi, right?\n Word is, he was into some deep stuff.\nWhat do ya know?. ","  <b>Alex Cooper:</b> Goldie, lemme tell ya, Shimi wasn’t no saint.\n He had enemies, sure, but I didn’t think anyone had the stones to take him out.\nThen again, you don’t swim with sharks without riskin’ a bite.."};
        telePhoneTexts[2] = new List<string> { "  . . . . . . " , "  . . . . .  " ,"  . . . .  " , "  <b>M. Goldie</b>: Greta, you’re the eyes and ears around here.\n What’s the scoop on who came and went that night? ","  <b>Greta Limbhall:</b> Goldie, you know I don’t miss a thing.\n But that night? Things were a little too quiet, if ya know what I mean.\nAlmost like someone knew how to stay outta sight."};
        telePhoneTexts[3] = new List<string> { "  . . . . . . " , "  . . . . .  " ,"  . . . .  " , "  <b>M. Goldie</b>: Yeah, this is M. Goldie.\n I need a rush job on some files.\nYou can handle that, right? ","  <b>Lee Office Printing Services:</b>: Rush job? Sure, Goldie, we can print you a confession if you want!\nJust don’t ask where the ink comes from, capisce? "};
    }

}

