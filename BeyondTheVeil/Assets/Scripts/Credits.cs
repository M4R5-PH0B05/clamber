using UnityEngine;

public class ScrollingCredits : MonoBehaviour
{
    public RectTransform viewport;
    public float speed = 60f;

    public float startPadding = 50f; // smaller = less gap at the start
    public float endPadding = 50f;

    RectTransform content;
    Vector2 startPos;
    float endY;

    void Awake()
    {
        content = GetComponent<RectTransform>();
    }

    void Start()
    {
        // Start just below the viewport (not miles away)
        startPos = content.anchoredPosition;
        startPos.y = -startPadding;
        content.anchoredPosition = startPos;

        // End when content has fully cleared the top
        endY = content.rect.height + viewport.rect.height + endPadding;
    }

    void Update()
    {
        content.anchoredPosition += Vector2.up * speed * Time.deltaTime;

        // optional: loop
        // if (content.anchoredPosition.y >= endY) content.anchoredPosition = startPos;
    }
}
