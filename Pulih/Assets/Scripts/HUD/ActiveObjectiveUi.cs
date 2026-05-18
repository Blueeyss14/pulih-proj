using UnityEngine;

public class ActiveObjectUi : MonoBehaviour
{
    public RectTransform uiElement;
    public Vector3 offset;
    public float marginX = 100f;
    public float marginY = 100f;
    private Camera mainCamera;
    public int activeNumber = 0;
    static int currentActiveNumber = 0;

    void Awake()
    {
        mainCamera = Camera.main;

        if (uiElement != null)
        {
            uiElement.gameObject.SetActive(false);
        }
    }

    public static void SetCurrentActiveNumber(int number)
    {
        currentActiveNumber = number;
    }

    void Update()
    {
        if (uiElement == null) return;

        if (currentActiveNumber <= 0)
        {
            uiElement.gameObject.SetActive(false);
            return;
        }

        if (activeNumber != currentActiveNumber)
        {
            return;
        }
        else
        {
            uiElement.gameObject.SetActive(true);
        }

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