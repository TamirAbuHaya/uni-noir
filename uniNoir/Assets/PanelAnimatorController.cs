using UnityEngine;
using System.Collections;

public class PanelAnimatorController : MonoBehaviour
{
    [Header("Animation Settings")]
    public float scaleMultiplier = 1.1f;
    public float animationDuration = 0.3f;
    public AnimationCurve animationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private RectTransform rectTransform;
    private Vector3 originalScale;
    private Coroutine animationCoroutine;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originalScale = rectTransform.localScale;
    }

    public void ShowPanel()
    {
        StopAndStartAnimation(true);
    }

    public void HidePanel()
    {
        StopAndStartAnimation(false);
    }

    private void StopAndStartAnimation(bool show)
    {
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
        }
        gameObject.SetActive(true);
        animationCoroutine = StartCoroutine(AnimateScale(show));
    }

    private IEnumerator AnimateScale(bool show)
    {
        Vector3 startScale = show ? Vector3.zero : originalScale * scaleMultiplier;
        Vector3 endScale = show ? originalScale * scaleMultiplier : Vector3.zero;
    
        float elapsedTime = 0f;

        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / animationDuration;
            float curveValue = animationCurve.Evaluate(t);
            rectTransform.localScale = Vector3.Lerp(startScale, endScale, curveValue);
            yield return null;
        }

        rectTransform.localScale = endScale;

        if (!show)
        {
            gameObject.SetActive(false);
        }
    }
}