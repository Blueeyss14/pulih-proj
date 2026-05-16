using UnityEngine;

public class ScaleAnimation : MonoBehaviour
{
    [Header("Width Settings")]
    public float startWidth = 100f;
    public float endWidth = 150f;

    [Header("Animation Speed")]
    public float duration = 1f;

    private RectTransform rectTransform;
    private Vector2 originalSize;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            originalSize = rectTransform.sizeDelta;
        }
    }

    void Update()
    {
        if (rectTransform == null) return;

        float t = Mathf.PingPong(Time.time / duration, 1f);
        float currentWidth = Mathf.Lerp(startWidth, endWidth, t);
        rectTransform.sizeDelta = new Vector2(currentWidth, originalSize.y);
    }
}