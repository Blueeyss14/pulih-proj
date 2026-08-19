// using UnityEngine;

// public class PlantController : MonoBehaviour
// {
//     public RectTransform uiElement;
//     public GameObject plantUi;
//     public Vector3 offset;

//     private Camera mainCamera;

//     void Awake()
//     {
//         mainCamera = Camera.main;
//         if (uiElement != null) uiElement.gameObject.SetActive(true);
//     }

//     public void SetUI(bool state)
//     {
//         if (uiElement != null) uiElement.gameObject.SetActive(state);
//     }

//     void LateUpdate()
//     {
//         if (uiElement == null) return;

//         Vector3 screenPos = mainCamera.WorldToScreenPoint(transform.position + offset);
//         screenPos.z = 0;
//         uiElement.position = screenPos;
//     }
// }
