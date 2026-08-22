using UnityEngine;

public class PercentFiller : MonoBehaviour
{
    public RectTransform rangeUI;
    public RectTransform thickUI; 
    
    [Range(0f, 1f)] public float startPercent = 0f;
    [Range(0f, 1f)] public float endPercent = 0f;
    
    public float speed = 1f;

void OnValidate()
{
    if (rangeUI != null)
    {
        rangeUI.anchorMin = new Vector2(startPercent, rangeUI.anchorMin.y);
        rangeUI.anchorMax = new Vector2(endPercent, rangeUI.anchorMax.y);
        
        rangeUI.offsetMin = new Vector2(0f, rangeUI.offsetMin.y);
        rangeUI.offsetMax = new Vector2(0f, rangeUI.offsetMax.y);
    }
}

void Update()
{
if (thickUI != null)
    {
        float pingPong = Mathf.PingPong(Time.time * speed, 1f);
        
        thickUI.anchorMin = new Vector2(pingPong, thickUI.anchorMin.y);
        thickUI.anchorMax = new Vector2(pingPong, thickUI.anchorMax.y);
        
        thickUI.anchoredPosition = new Vector2(0f, thickUI.anchoredPosition.y); 
    }
}
}