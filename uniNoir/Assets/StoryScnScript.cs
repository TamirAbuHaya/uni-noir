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
    public TextMeshProUGUI convText;
    public Button textButton;

    [Header("Button Settings")]
    private bool canPressButton = true;
    public float buttonCooldown = 1.0f;

    [Header("Text Settings")]
    private string[] texts = new string[10];

    private int textCounter = -1;

    public float textBoxDelay = 1.0f;

    public Image[] suspectsImages = new Image[8];



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

    }

    private void SetupButtonListeners()
    {
        settingsButton.onClick.AddListener(() => StartCoroutine(ButtonPressWithDelay(ToggleSettingsPanel)));
        textButton.onClick.AddListener(() => StartCoroutine(ButtonPressWithDelay(TextManager)));
    }

    private void InitTexts(){
        texts[0] = "Prof. Shimmy has been found dead in his office.We brought you here to find who killed him. ";
        texts[1] = "We've gathered some information about suspects while waiting for you,Continue to see the suspects. ";
        texts[2] = "#1 Suspect : Prof. Gal Gumbler \nShimmy frequently belittled her life work in a theory she was about to publish.At the end, shimmy debunked her lifework, making it all go to waste. ";
        texts[3] = "#2 Suspect : Secretary Oshrit  Yeffeti \nshimmy and oshrit have been skimming funds from university grants together. They got caught and shimmy tried to scapegoat the fiasco onto oshrit. ";
        texts[4] = "#3 Suspect : Martinez Despadas \nA troubled student with a history of anger issues. He blames shimmy for failing his course, which resulted in the loss of his scholarship. ";
        texts[5] = "#4 Suspect : Dr. Michael Coddler \nA long-time colleague of Professor Shimmy who feels overshadowed by his success. Coddler's envy has been simmering for years, fueled by Tavori's constant praise from the academic community.  ";
        texts[6] = "#5 Suspect : Marina the lab assistant \nA graduate student rumored to have had an affair with Shimmy. She became obsessed with him, but their relationship ended badly. ";
        texts[7] = "#6 Suspect : Prof. Semion Kogan \nTurns out Prof. kogan here is a quite a big spender. Shimmy discovered semions' misuse of funds and was going to report him. ";
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

     private IEnumerator ShowTextBarAfterDelay()
    {
        yield return new WaitForSeconds(textBoxDelay);
        textButton.gameObject.SetActive(true);
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
            sceneTransitionManager.FadeToNextScene();
        }
        else{
            if(textCounter >= 1){
                suspectsImages[textCounter - 1].gameObject.SetActive(false);
                suspectsImages[textCounter % suspectsImages.Length].gameObject.SetActive(true);
            }
            textCounter++;
            convText.SetText(texts[textCounter]);
            convText.gameObject.SetActive(true);
        }


    }
}
