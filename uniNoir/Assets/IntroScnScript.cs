using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroScnScript : MonoBehaviour
{
    // Start is called before the first frame update

        public SceneTransitionManager sceneTransitionManager;

        public float sceneDuration = 4.0f;

    void Start()
    {
        if (sceneTransitionManager != null)
        {
            StartCoroutine(sceneTransitionManager.FadeIn());
        }

        StartCoroutine(WaitForSceneDuration());

        

    }

    private IEnumerator WaitForSceneDuration( )
    {
        yield return new WaitForSeconds(sceneDuration);
        if (sceneTransitionManager != null)
        {
            sceneTransitionManager.FadeToNextScene();
        }
    }
    
}
