using UnityEngine;
using UnityEngine.InputSystem;

public class CamerController : MonoBehaviour
{
    public Transform target;

    [Header("Camera Position")]
    public float distance = 2.2f; 
    public float heightOffset = 3.3f; 
    public float sideOffset = 0.43f;

    [Header("Mouse Settings")]
    public float sensitivity = 1.5f;
    public float minYAngle = -40f;
    public float maxYAngle = 70;

    private float mouseX;
    private float mouseY;

    private Plane[] frustumPlanes;
    public Plane[] FrustumPlanes => frustumPlanes;

    private Camera cachedCamera;
    private Vector3 lastCameraPosition;
    private Quaternion lastCameraRotation;

    void Awake()
    {
        cachedCamera = GetComponent<Camera>();
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        if (target == null || Mouse.current == null) return;

        mouseX += Mouse.current.delta.x.ReadValue() * sensitivity * Time.deltaTime;
        mouseY -= Mouse.current.delta.y.ReadValue() * sensitivity * Time.deltaTime;
        mouseY = Mathf.Clamp(mouseY, minYAngle, maxYAngle);

        Quaternion rotation = Quaternion.Euler(mouseY, mouseX, 0f);

        Vector3 focusPoint = target.position + Vector3.up * heightOffset;

        Vector3 finalPosition = focusPoint - (rotation * Vector3.forward * distance) + (rotation * Vector3.right * sideOffset);

        transform.position = finalPosition;
        transform.rotation = rotation;

        if (lastCameraPosition != finalPosition || lastCameraRotation != rotation)
        {
            frustumPlanes = GeometryUtility.CalculateFrustumPlanes(cachedCamera);
            lastCameraPosition = finalPosition;
            lastCameraRotation = rotation;
        }
    }
}