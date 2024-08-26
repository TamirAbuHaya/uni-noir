using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class BookPanelController : MonoBehaviour
{
    [Header("UI Elements")]
    public PanelAnimatorController bookPanelAnimator;
    public PanelAnimatorController settingsPanelAnimator;
    public Animator letterAnimator;
    public TextMeshProUGUI messageText;
    //public TextMeshProUGUI downHereIndicator;
    public Button backgroundLetterButton;
    public Button closePanelButton;
    public Button settingsButton;
    public Button goButton;
    public Button quitButton;
    public TextMeshProUGUI text1;

    [Header("Button Delay Settings")]
    public float buttonCooldown = 1.0f;

    [Header("Scene Transition")]
    public SceneTransitionManager sceneTransitionManager;

    private bool canPressButton = true;

    
    [Header("Audio")]
    public AudioSource backgroundMusicSource;
    public AudioClip backgroundMusic;

/* Start background music
        if (backgroundMusic != null && backgroundMusicSource != null)
        {
            backgroundMusicSource.clip = backgroundMusic;
            backgroundMusicSource.loop = true;
            backgroundMusicSource.Play();
        }
        */

    private void Start()
    {
        InitializeUI();
        SetupButtonListeners();
        //StartIndicatorCoroutine();
        if (sceneTransitionManager != null)
        {
            StartCoroutine(sceneTransitionManager.FadeIn());
        }
    }

    private void InitializeUI()
    {
        bookPanelAnimator.gameObject.SetActive(false);
        settingsPanelAnimator.gameObject.SetActive(false);
        messageText.gameObject.SetActive(false);
        goButton.gameObject.SetActive(false);
        letterAnimator.gameObject.SetActive(false);
    }

    private void SetupButtonListeners()
    {
        closePanelButton.onClick.AddListener(() => StartCoroutine(ButtonPressWithDelay(ClosePanel)));
        backgroundLetterButton.onClick.AddListener(() => StartCoroutine(ButtonPressWithDelay(ShowPanel)));
        settingsButton.onClick.AddListener(() => StartCoroutine(ButtonPressWithDelay(ToggleSettingsPanel)));
        goButton.onClick.AddListener(() => StartCoroutine(TransitionToNextScene()));
        quitButton.onClick.AddListener(()=> StartCoroutine(ButtonPressWithDelay(QuitGame)));
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

     /* private IEnumerator PlayLetterAnimationAndTransition()
    {
        // Play the letter animation
        letterAnimator.Play("letter_animation");

        // Wait for the animation to complete
        yield return new WaitForSeconds(letterAnimator.GetCurrentAnimatorStateInfo(0).length);

        // Transition to the next scene
        TransitionToNextScene();
    }
    */
    private IEnumerator TransitionToNextScene()
    {
        //PlayLetterAnimationAndTransition();
        
        if (sceneTransitionManager != null)
        {
            bookPanelAnimator.HidePanel();
            letterAnimator.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            text1.gameObject.SetActive(false);
            yield return new WaitForSeconds(1.5f);
            letterAnimator.gameObject.SetActive(false);
            sceneTransitionManager.FadeToNextScene();
        }
        else
        {
            yield return new WaitForSeconds(3.0f);
            Debug.LogError("SceneTransitionManager is not assigned!");
        }
        
    }

    private void ShowPanel()
    {
        backgroundLetterButton.interactable = false;
        bookPanelAnimator.ShowPanel();
        messageText.gameObject.SetActive(true);
        goButton.gameObject.SetActive(true);
         //Start background music
        if (backgroundMusic != null && backgroundMusicSource != null)
        {
            backgroundMusicSource.clip = backgroundMusic;
            backgroundMusicSource.loop = true;
            backgroundMusicSource.Play();
        }
        

        Debug.Log("Book Panel set to active");
        
       
    }

    private void ClosePanel()
    {
        backgroundMusicSource.Pause();
        bookPanelAnimator.HidePanel();
        StartCoroutine(ActivateBackgroundElementsAfterDelay());
        Debug.Log("Book Panel deactivated");
    }

    private IEnumerator ActivateBackgroundElementsAfterDelay()
    {
        yield return new WaitForSeconds(bookPanelAnimator.animationDuration);
        backgroundLetterButton.interactable = true;
  
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

    private void QuitGame(){
        if(sceneTransitionManager != null){
            StartCoroutine(sceneTransitionManager.FadeOutAndExit());
        }
        Debug.Log("Exited");
    }
}

