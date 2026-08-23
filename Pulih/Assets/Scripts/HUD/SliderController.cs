using UnityEngine;

public class SliderController : MonoBehaviour
{
    public RectTransform rangeUI;
    
    [Range(0f, 1f)] public float sliderValue = 0f;

    void OnValidate()
    {
        if (rangeUI != null)
        {
            float startPercent = 0f;
            float endPercent = sliderValue;

            rangeUI.anchorMin = new Vector2(startPercent, 0f);
            rangeUI.anchorMax = new Vector2(endPercent, 1f);
            rangeUI.offsetMin = Vector2.zero;
            rangeUI.offsetMax = Vector2.zero;
        }
    }
}