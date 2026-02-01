using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFader : MonoBehaviour
{
    public static ScreenFader m_Instance;

    [SerializeField] private Image fadeImage;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] public float fadeTime = 0.6f;

    void Awake()
    {
        if (m_Instance != null && m_Instance != this)
        {
            Destroy(transform.root.gameObject);
            return;
        }
        m_Instance = this;

        DontDestroyOnLoad(transform.root.gameObject);

        if (!fadeImage) fadeImage = GetComponent<Image>();
        if (!canvasGroup) canvasGroup = GetComponent<CanvasGroup>();
    }

    void Start()
    {
        SetAlpha(1f);
        SetBlocking(true);
        StartCoroutine(FadeTo(0f));
    }

    public IEnumerator FadeTo(float targetAlpha)
    {
        SetBlocking(true);

        float startAlpha = fadeImage.color.a;
        float t = 0f;

        while (t < fadeTime)
        {
            t += Time.unscaledDeltaTime; // works even if timeScale = 0
            float a = Mathf.Lerp(startAlpha, targetAlpha, t / fadeTime);
            SetAlpha(a);
            yield return null;
        }

        SetAlpha(targetAlpha);

        if (targetAlpha <= 0.001f)
            SetBlocking(false);
    }

    void SetBlocking(bool block)
    {
        if (canvasGroup)
        {
            canvasGroup.blocksRaycasts = block;
            canvasGroup.interactable = block;
        }
        else if (fadeImage)
        {
            fadeImage.raycastTarget = block;
        }
    }

    void SetAlpha(float a)
    {
        var c = fadeImage.color;
        c.a = a;
        fadeImage.color = c;
    }
}
