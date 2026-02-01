using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFader : MonoBehaviour
{
    private Image FadeImage;

    private void Awake()
    {
        FadeImage = GetComponent<Image>();
    }

    public IEnumerator FadeInCoroutine(float duration)
    {
        Color startColour = new Color(FadeImage.color.r, FadeImage.color.g, FadeImage.color.b, 1);
        Color endColour = new Color(FadeImage.color.r, FadeImage.color.g, FadeImage.color.b, 0);

        yield return FadeCoroutine(startColour, endColour, duration);

        gameObject.SetActive(false);
    }

    public IEnumerator FadeOutCoroutine(float duration)
    {
        Color startColour = new Color(FadeImage.color.r, FadeImage.color.g, FadeImage.color.b, 0);
        Color endColour = new Color(FadeImage.color.r, FadeImage.color.g, FadeImage.color.b, 1);

        gameObject.SetActive(true);
        yield return FadeCoroutine(startColour, endColour, duration);       
    }

    private IEnumerator FadeCoroutine(Color startColour, Color endColour, float duration)
    {
        float elapsedTime = 0;
        float elapsedPercentage = 0;

        while (elapsedPercentage < 1)
        {
            elapsedPercentage = elapsedTime /duration;
            FadeImage.color = Color.Lerp(startColour, endColour, elapsedPercentage);

            yield return null;
            elapsedTime += Time.deltaTime;
        }
    }


}
