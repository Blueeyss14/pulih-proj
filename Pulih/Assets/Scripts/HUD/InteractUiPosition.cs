using UnityEngine;
public class InteractUiPosition : MonoBehaviour
{
    public RectTransform uiElement;
    public Vector3 offset;
    public float marginX = 50f;
    public float marginY = 50f;

    private Camera mainCamera;

    void Awake()
    {
        mainCamera = Camera.main;
        if (uiElement != null) uiElement.gameObject.SetActive(false);
    }

    public void SetUI(bool state)
    {
        if (uiElement != null) uiElement.gameObject.SetActive(state);
    }

    void Update()
    {
        if (uiElement == null || !uiElement.gameObject.activeSelf) return;
        Vector3 screenPos = mainCamera.WorldToScreenPoint(transform.position + offset);
        Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0);
        if (screenPos.z < 0)
        {
            screenPos -= screenCenter;
            screenPos *= -1;
            screenPos = screenCenter + screenPos.normalized * 10000f;
        }
        screenPos.x = Mathf.Clamp(screenPos.x, marginX, Screen.width - marginX);
        screenPos.y = Mathf.Clamp(screenPos.y, marginY, Screen.height - marginY);
        screenPos.z = 0;
        uiElement.position = screenPos;
    }
}