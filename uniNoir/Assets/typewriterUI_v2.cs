// Script for having a typewriter effect for UI - Version 2
// Prepared by Nick Hwang (https://www.youtube.com/nickhwang)
// Want to get creative? Try a Unicode leading character(https://unicode-table.com/en/blocks/block-elements/)
// Copy Paste from page into Inspector

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class typewriterUI_v2 : MonoBehaviour
{
	[SerializeField] Text text;
	[SerializeField] TMP_Text tmpProText;
	string writer;
	[SerializeField] private Coroutine coroutine;

	[SerializeField] float delayBeforeStart = 0f;
	[SerializeField] float timeBtwChars = 0.1f;
	[SerializeField] string leadingChar = "";
	[SerializeField] bool leadingCharBeforeDelay = false;
	[Space(10)] [SerializeField] private bool startOnEnable = false;
	
	[Header("Collision-Based")]
	[SerializeField] private bool clearAtStart = false;
	[SerializeField] private bool startOnCollision = false;
	enum options {clear, complete}
	[SerializeField] options collisionExitOptions;

	[Header("Sound")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip typewriterSound;
    [SerializeField] private float pitchVariation = 0.1f;

	// Use this for initialization
	 void Awake()
    {
        if (text != null)
        {
            writer = text.text.Substring(0,text.text.Length-1);
        }

        if (tmpProText != null)
        {
//            writer = tmpProText.text.Substring(0,tmpProText.text.Length-1);;
        }

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

	void Start()
	{
		if (!clearAtStart ) return;
		if(text != null)
		{
			text.text = "";
		}
		
		if (tmpProText != null)
		{
			tmpProText.text = "";
		}
	}

	private void OnEnable()
	{
		print("On Enable!");
		if(tmpProText.text.Length > 0){
			writer = tmpProText.text.Substring(0,tmpProText.text.Length-1);
		}
		if(startOnEnable) StartTypewriter();
	}

	private void OnCollisionEnter2D(Collision2D col)
	{
		print("Collision!");
		if (startOnCollision)
		{
			StartTypewriter();
		}
	}

	private void OnCollisionExit2D(Collision2D other)
	{
		if (collisionExitOptions == options.complete)
		{
			if (text != null)
			{
				text.text = writer;
			}

			if (tmpProText != null)
			{
				tmpProText.text = writer;
			}
		}
		// clear
		else
		{
			if (text != null)
			{
				text.text = "";
			}

			if (tmpProText != null)
			{
				tmpProText.text = "";
			}
		}
		
		StopAllCoroutines();
	}


	private void StartTypewriter()
	{
		StopAllCoroutines();

		if(text != null)
		{
			text.text = "";

			StartCoroutine("TypeWriterText");
		}
		
		if (tmpProText != null)
		{
			tmpProText.text = "";

			StartCoroutine("TypeWriterTMP");
		}
	}

	private void OnDisable()
	{
		StopAllCoroutines();
		
	}

	
    IEnumerator TypeWriterText()
    {
        text.text = leadingCharBeforeDelay ? leadingChar : "";

        yield return new WaitForSeconds(delayBeforeStart);

        foreach (char c in writer)
        {
            if (text.text.Length > 0)
            {
                text.text = text.text.Substring(0, text.text.Length - leadingChar.Length);
            }
            text.text += c;
            text.text += leadingChar;
			
            yield return new WaitForSeconds(timeBtwChars);
        }

        if (leadingChar != "")
        {
            text.text = text.text.Substring(0, text.text.Length - leadingChar.Length);
        }

        yield return null;
    }

    IEnumerator TypeWriterTMP()
    {
        tmpProText.text = leadingCharBeforeDelay ? leadingChar : "";

        yield return new WaitForSeconds(delayBeforeStart);

        foreach (char c in writer)
        {
            if (tmpProText.text.Length > 0)
            {
                tmpProText.text = tmpProText.text.Substring(0, tmpProText.text.Length - leadingChar.Length);
            }
			float prevpitch = audioSource.pitch;
			if(c != '.' && c != ' '){
				PlayTypewriterSound();
			}
            tmpProText.text += c;
            tmpProText.text += leadingChar;
            yield return new WaitForSeconds(timeBtwChars);
			audioSource.pitch = prevpitch;
        }

        if (leadingChar != "")
        {
            tmpProText.text = tmpProText.text.Substring(0, tmpProText.text.Length - leadingChar.Length);
        }
    }

    private void PlayTypewriterSound()
    {
        if (audioSource != null && typewriterSound != null)
        {
            audioSource.pitch = UnityEngine.Random.Range(1 - pitchVariation, 1 + pitchVariation);
            audioSource.PlayOneShot(typewriterSound);
        }
    }
}
