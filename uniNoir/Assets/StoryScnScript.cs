using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;



public class StoryScnScript : MonoBehaviour
{
    public SceneTransitionManager sceneTransitionManager;
    public PanelAnimatorController settingsPanelAnimator;
    public Button settingsButton;
    public Button clsSettingsButton;
    public TextMeshProUGUI convText;
    public Button textButton;

    [Header("Button Settings")]
    private bool canPressButton = true;
    public float buttonCooldown = 1.0f;

    [Header("Text Settings")]
    private string[] texts = new string[10];

    private int textCounter = -1;

    public float textBoxDelay = 1.0f;

    private int tgl = 0;

    private bool settingsOpen = false;

    public Image[] suspectsImages = new Image[8];

     
    [Header("Audio")]
    public AudioSource backgroundMusicSource;
    public AudioSource sfxSource;
    public AudioClip buttonClick;



    // Start is called before the first frame update

    void Start()
    {
        InitializeUI();
        SetupButtonListeners();
        InitTexts();
        StartCoroutine(ShowTextBarAfterDelay());

    }

     private void InitializeUI()
    {
        settingsPanelAnimator.gameObject.SetActive(false);
        textButton.gameObject.SetActive(false);
        if (sceneTransitionManager != null)
        {
            StartCoroutine(sceneTransitionManager.FadeIn());
        }

    }
    private void Update(){

        StartCoroutine(RunTextsWithDelay());
        if(Input.GetKey(KeyCode.Escape)){
            StartCoroutine(ButtonPressWithDelay(ToggleSettingsPanel));
        }
        if(Input.GetKey(KeyCode.Space)){
            StartCoroutine(ButtonPressWithDelay(TextManager));
        }
        if(Input.GetKey(KeyCode.Return)){
            StartCoroutine(ButtonPressWithDelay(TextManager));
        }
        

    }


    private void SetupButtonListeners()
    {
        
        settingsButton.onClick.AddListener(() => StartCoroutine(ButtonPressWithDelay(ToggleSettingsPanel)));
        textButton.onClick.AddListener(() => StartCoroutine(ButtonPressWithDelay(TextManager)));
        clsSettingsButton.onClick.AddListener(() => StartCoroutine(ButtonPressWithDelay(ToggleSettingsPanel)));
    }

    private void InitTexts(){
        texts[0] = "The well respected Prof. Shimi Tavori has been found dead in his office.\nWe brought you here to find who killed him. ";
        texts[1] = "We've gathered some information about suspects while waiting for you.\nContinue to see the suspects. ";
        texts[2] = "#1 Suspect : Prof. Gal Gumbler \nShimi debunked her lifework, making it all go to waste. ";
        texts[3] = "#2 Suspect : Secretary Oshrit Yeffeti \nShimi and Oshrit have been skimming funds from university grants together.\n They got caught and Shimi tried to scapegoat the fiasco onto Oshrit. ";
        texts[4] = "#3 Suspect : Martinez Despadas \nA troubled student with a history of anger issues.\n Blames Shimi for failing his courses, resulting in the loss of his scholarship. ";
        texts[5] = "#4 Suspect : Dr. Michael Coddler \nA long-time colleague of Professor Shimi who feels overshadowed by his  success. ";
        texts[6] = "#5 Suspect : Marina the lab assistant \nA graduate student rumored to have had an affair with Shimi.\n She became obsessed with him, but their relationship ended badly. ";
        texts[7] = "#6 Suspect : Prof. Semion Kogan \nTurns out Prof. kogan here is a quite a big spender.\n Shimi discovered semions' misuse of funds and was going to report him. ";
        texts[8] = "#7 Suspect : Janitor Yonas \nResented him for constantly criticizing his work ethic and feared losing his job. ";
        texts[9] = "Well ,that's all we could find ,from here it's up to you Goldy, \n Are you up for the challenge? ";
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
    
    private IEnumerator RunTextsWithDelay()
    {
        if(!settingsOpen){
        int tmp = textCounter;
        yield return new WaitForSeconds(20.0f);
        if(tmp == textCounter){
            TextManager();
        }
        }
    }

     private IEnumerator ShowTextBarAfterDelay()
    {
        yield return new WaitForSeconds(textBoxDelay);
        textButton.gameObject.SetActive(true);
    }

    private void ToggleSettingsPanel()
    {
        
        if (sfxSource != null &&  buttonClick != null)
        {
            sfxSource.clip = buttonClick;
            //backgroundMusicSource.loop = true;
            sfxSource.Play();
        }

        if (settingsPanelAnimator.gameObject.activeSelf)
        {
            settingsPanelAnimator.HidePanel();
            settingsOpen = false;
        }
        else
        {
            settingsPanelAnimator.ShowPanel();
            settingsOpen = true;
        }
    }

  

    private void TextManager()
    {

        convText.gameObject.SetActive(false);
    
        if(textCounter == 9){
            if (settingsPanelAnimator.gameObject.activeSelf)
            {
              settingsPanelAnimator.HidePanel();
            }

            textButton.gameObject.SetActive(false);
            settingsButton.gameObject.SetActive(false);

            if (sceneTransitionManager != null)
            {
             sceneTransitionManager.FadeToNextScene();
            }
            
        }
        else{
            if(textCounter >= 1){
                suspectsImages[textCounter - 1].gameObject.SetActive(false);
                suspectsImages[textCounter % suspectsImages.Length].gameObject.SetActive(true);
            }
            textCounter++;
            convText.SetText(texts[textCounter]);
            convText.gameObject.SetActive(true);
            tgl++;
            }   

         }
       
  }

