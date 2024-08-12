using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class BookPanelController : MonoBehaviour
{
    [Header("UI Elements")]
    public PanelAnimatorController bookPanelAnimator;
    public PanelAnimatorController settingsPanelAnimator;
    public TextMeshProUGUI messageText;
    public TextMeshProUGUI downHereIndicator;
    public Button viewMessagesButton;
    public Button backgroundBookButton;
    public Button closePanelButton;
    public Button settingsButton;

    [Header("Button Delay Settings")]
    public float buttonCooldown = 1.0f;
    
    [Header("Down Here Indicator Settings")]
    public float indicatorDelay = 5.0f; // Delay before showing the indicator

    private bool canPressButton = true;
    private bool hasOpenedBook = false;
    private Coroutine indicatorCoroutine;

    private void Start()
    {
        InitializeUI();
        SetupButtonListeners();
        StartIndicatorCoroutine();
    }

    private void InitializeUI()
    {
        downHereIndicator.gameObject.SetActive(false);
        bookPanelAnimator.gameObject.SetActive(false);
        settingsPanelAnimator.gameObject.SetActive(false);
        messageText.gameObject.SetActive(false);
    }

    private void SetupButtonListeners()
    {
        viewMessagesButton.onClick.AddListener(() => StartCoroutine(ButtonPressWithDelay(ToggleMessage)));
        closePanelButton.onClick.AddListener(() => StartCoroutine(ButtonPressWithDelay(ClosePanel)));
        backgroundBookButton.onClick.AddListener(() => StartCoroutine(ButtonPressWithDelay(ShowPanel)));
        settingsButton.onClick.AddListener(() => StartCoroutine(ButtonPressWithDelay(ToggleSettingsPanel)));
    }

    private void StartIndicatorCoroutine()
    {
        if (indicatorCoroutine != null)
        {
            StopCoroutine(indicatorCoroutine);
        }
        indicatorCoroutine = StartCoroutine(ShowIndicatorAfterDelay());
    }

    private IEnumerator ShowIndicatorAfterDelay()
    {
        yield return new WaitForSeconds(indicatorDelay);
        if (!hasOpenedBook)
        {
            downHereIndicator.gameObject.SetActive(true);
        }
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

    private void ToggleMessage()
    {
        messageText.gameObject.SetActive(!messageText.gameObject.activeSelf);
        Debug.Log($"Message Text set to {messageText.gameObject.activeSelf}");
    }

    private void ShowPanel()
    {
        hasOpenedBook = true;
        downHereIndicator.gameObject.SetActive(false);
        backgroundBookButton.gameObject.SetActive(false);
        bookPanelAnimator.ShowPanel();
        Debug.Log("Book Panel set to active");
        
        if (indicatorCoroutine != null)
        {
            StopCoroutine(indicatorCoroutine);
        }
    }

    private void ClosePanel()
    {
        bookPanelAnimator.HidePanel();
        StartCoroutine(ActivateBackgroundElementsAfterDelay());
        Debug.Log("Book Panel deactivated");
    }

    private IEnumerator ActivateBackgroundElementsAfterDelay()
    {
        yield return new WaitForSeconds(bookPanelAnimator.animationDuration);
        backgroundBookButton.gameObject.SetActive(true);
        hasOpenedBook = false;
        StartIndicatorCoroutine();
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
}