using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class EndScnScript : MonoBehaviour
{
    // Start is called before the first frame update

        public SceneTransitionManager sceneTransitionManager;
        public Image BadEnding;
        public Image GoodEnding;

        public float sceneDuration = 61.0f;

    void Start()
    {
        if(GameScnScript.goodEnd){
            BadEnding.gameObject.SetActive(false);
        }
        else{
            GoodEnding.gameObject.SetActive(false);
        }
        
        if (sceneTransitionManager != null)
        {
            StartCoroutine(sceneTransitionManager.FadeIn());
        }

        StartCoroutine(WaitSceneDuration());

        

    }

    private IEnumerator WaitSceneDuration( )
    {
        yield return new WaitForSeconds(sceneDuration);
        if (sceneTransitionManager != null)
        {
            StartCoroutine(sceneTransitionManager.FadeOutAndExit());
        }
    }
    
}
