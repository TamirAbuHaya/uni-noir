using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using System.Runtime.ExceptionServices;
using Unity.VisualScripting;

public class OpeningScene : MonoBehaviour {
        // These are the script variables.
        // For more character images or buttons, duplicate the ArtChar ones listed here and renumber.
        public int primeInt = 1;         // This integer drives game progress!

// Initial visibility settings. Any new images or buttons need to also be SetActive(false);
void Start(){  
        SceneManager.LoadScene("intro");
      
        
   }

// Use the spacebar as a faster "Next" button:
void Update(){        
   
   }

//Story Units! The main story function. Players hit [NEXT] to progress to the next primeInt:
public void Next(){
        primeInt = primeInt + 1;
        if (primeInt == 1){
                // audioSource1.Play();
        }      
}
}