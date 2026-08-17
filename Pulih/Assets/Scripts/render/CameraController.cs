using UnityEngine;
using UnityEngine.InputSystem;

public class CamerController : MonoBehaviour
{
    public Transform target;

    [Header("Camera Position")]
    public float distance = 1.55f;
    public float rightMouseDistance = 0.8f;
    public float heightOffset = 3f;
    public float sideOffset = 0.43f;

    private const float LOD_BIAS         = 0.05f;
    private const float SHADOW_DISTANCE  = 0f;
    private const int   PIXEL_LIGHT_COUNT = 0;
    private const int   ANTI_ALIASING    = 0;
    private const int   TARGET_FRAMERATE = 60;

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

        QualitySettings.lodBias                  = LOD_BIAS;
        QualitySettings.shadowDistance           = SHADOW_DISTANCE;
        QualitySettings.shadows                  = ShadowQuality.Disable;
        QualitySettings.pixelLightCount          = PIXEL_LIGHT_COUNT;
        QualitySettings.antiAliasing             = ANTI_ALIASING;
        QualitySettings.anisotropicFiltering     = AnisotropicFiltering.Disable;
        QualitySettings.realtimeReflectionProbes = false;
        QualitySettings.softParticles            = false;
        Application.targetFrameRate              = TARGET_FRAMERATE;
        QualitySettings.vSyncCount               = 0;
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        if (target == null || Mouse.current == null) return;

        float currentDistance = distance;

        if (Mouse.current.rightButton.isPressed)
        {
            currentDistance -= rightMouseDistance;
        }

        mouseX += Mouse.current.delta.x.ReadValue() * sensitivity * Time.deltaTime;
        mouseY -= Mouse.current.delta.y.ReadValue() * sensitivity * Time.deltaTime;
        mouseY = Mathf.Clamp(mouseY, minYAngle, maxYAngle);

        Quaternion rotation = Quaternion.Euler(mouseY, mouseX, 0f);

        Vector3 focusPoint = target.position + Vector3.up * heightOffset;

        Vector3 finalPosition = focusPoint - (rotation * Vector3.forward * currentDistance) + (rotation * Vector3.right * sideOffset);

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