using UnityEngine;

public class PlantController : MonoBehaviour
{
    public RectTransform uiElement;
    public Vector3 offset;

    private Camera mainCamera;

    void Awake()
    {
        mainCamera = Camera.main;
    }

    void LateUpdate()
    {
        if (uiElement == null) return;

        if (PlayerInZone.isInZone) {
            if (CrosshairAim.currentTarget == gameObject) {
                uiElement.gameObject.SetActive(false);
            } else {
                uiElement.gameObject.SetActive(true);
            }
        }
        else {
            uiElement.gameObject.SetActive(false);
        }
        Vector3 screenPos = mainCamera.WorldToScreenPoint(transform.position + offset);
        screenPos.z = 0;
        uiElement.position = screenPos;
    }
}