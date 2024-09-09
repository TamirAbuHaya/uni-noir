using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class EndScnScript : MonoBehaviour
{
    // Start is called before the first frame update

        public SceneTransitionManager sceneTransitionManager;
        public PanelAnimatorController settingsPanelAnimator;
        public Image BadEnding;
        public Image GoodEnding;
        public float buttonCooldown = 0.2f;

        public Button clsSettingsButton;
        public Button settingsButton;

        public float sceneDuration = 65.0f;
        private bool canPressButton = true;

         
        [Header("Audio")]
        public AudioSource backgroundMusicSource;
        public AudioClip backgroundMusic;
        public AudioSource sfxSource;
        public AudioClip buttonClick;
        public AudioClip goodEndClip;
        public AudioClip badEndClip;
        public AudioClip carPassing;
        public AudioClip gunshot;
        private float timer = 0;
         
         private bool done1 = false;
        private bool done2 = false;



    void Start()
    {
        settingsPanelAnimator.gameObject.SetActive(false);
        settingsButton.onClick.AddListener(() => StartCoroutine(ButtonPressWithDelay(ToggleSettingsPanel)));
        clsSettingsButton.onClick.AddListener(() => StartCoroutine(ButtonPressWithDelay(ToggleSettingsPanel)));

        if(GameScnScript.goodEnd){
            sceneTransitionManager.backgroundMusic = goodEndClip;
            BadEnding.gameObject.SetActive(false);
        }
        else{
            sceneTransitionManager.backgroundMusic = badEndClip;
            sceneDuration = 45.0f;
            GoodEnding.gameObject.SetActive(false);
        }
        
        if (sceneTransitionManager != null)
        {
            StartCoroutine(sceneTransitionManager.FadeIn());
        }

        StartCoroutine(WaitSceneDuration());

    }

    private void Update(){

         if(Input.GetKey(KeyCode.Escape)){
            StartCoroutine(ButtonPressWithDelay(ToggleSettingsPanel));
        }
        if(Mathf.FloorToInt(timer) == 26 && !GameScnScript.goodEnd && !done1){
             if (sfxSource != null &&  carPassing != null)
            {
                sfxSource.clip = carPassing;
                sfxSource.Play();
            }
            done1 = true;
        }
        if(Mathf.FloorToInt(timer) == 35 && !GameScnScript.goodEnd && !done2){
             if (sfxSource != null &&  gunshot != null)
            {
                sfxSource.clip = gunshot;
                sfxSource.Play();
            }
            done2 = true;
        }
        timer +=  Time.deltaTime;
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
    private IEnumerator WaitSceneDuration( )
    {
        yield return new WaitForSeconds(sceneDuration);
        if (sceneTransitionManager != null)
        {
            StartCoroutine(sceneTransitionManager.FadeOutAndExit());
        }
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
        }
        else
        {
            settingsPanelAnimator.ShowPanel();
        }
    }
    
}
