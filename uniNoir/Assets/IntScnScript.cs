using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class IntroClass : MonoBehaviour
{
    public SceneFader sceneFader;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) // Example trigger
        {
            sceneFader.FadeToScene("OpeningScene");
        }
    }
     void Awake()
    {
    DontDestroyOnLoad(sceneFader);
    }
}
