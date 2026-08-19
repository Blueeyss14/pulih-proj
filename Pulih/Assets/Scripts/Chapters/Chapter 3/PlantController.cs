using UnityEngine;

public class PlantInteract : MonoBehaviour
{
    public RectTransform uiElement;
    public Vector3 offset;

    private Camera mainCamera;
    private bool manuallyHidden = false;

    void Awake()
    {
        mainCamera = Camera.main;
        if (uiElement != null) uiElement.gameObject.SetActive(true);
    }

    public void SetUI(bool state)
    {
        manuallyHidden = !state;
        if (uiElement != null) uiElement.gameObject.SetActive(state);
    }

    void LateUpdate()
    {
        if (uiElement == null || manuallyHidden) return;

        Vector3 screenPos = mainCamera.WorldToScreenPoint(transform.position + offset);
        screenPos.z = 0;
        uiElement.position = screenPos;
    }
}
