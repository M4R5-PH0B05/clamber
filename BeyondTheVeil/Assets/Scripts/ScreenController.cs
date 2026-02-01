using System.Collections;
using UnityEngine;

public class ScreenController : MonoBehaviour
{
    [SerializeField]
    private float fadeDuration;

    private ScreenFader screenFader;

    private void Awake()
    {
        screenFader = GetComponentInChildren<ScreenFader>();
    }

    private IEnumerator Start()
    {
        yield return screenFader.FadeInCoroutine(fadeDuration);
    }
}
