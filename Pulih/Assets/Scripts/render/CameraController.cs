using UnityEngine;
using UnityEngine.InputSystem;

public class CamerController : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0, 2, -5);
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float zoomSpeed = 2f;
    [SerializeField] private float minZoom = 2f;
    [SerializeField] private float maxZoom = 10f;

    private float yawCamY = 0f;
    private float pitchCamX = 15f;
    private float distance;

    
    private Plane[] frustumPlanes;
    public Plane[] FrustumPlanes => frustumPlanes;
    private Renderer[] allRenderers;

    void Start()
    {
        distance = offset.magnitude;
        Cursor.lockState = CursorLockMode.Locked;
        allRenderers = FindObjectsByType<Renderer>(FindObjectsSortMode.None);
    }

    void Awake()
    {
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;
    }

    void LateUpdate()
    {
        Vector2 mouseDelta = Vector2.zero;
        if (Mouse.current != null)
            mouseDelta = Mouse.current.delta.ReadValue();

        yawCamY += mouseDelta.x * mouseSensitivity * 0.01f;
        pitchCamX -= mouseDelta.y * mouseSensitivity * 0.01f;
        pitchCamX = Mathf.Clamp(pitchCamX, -35f, 80f);

        float scroll = 0f;
        if (Mouse.current != null)
            scroll = Mouse.current.scroll.ReadValue().y * 0.01f;

        distance -= scroll * zoomSpeed;
        distance = Mathf.Clamp(distance, minZoom, maxZoom);

        Quaternion rotation = Quaternion.Euler(pitchCamX, yawCamY, 0);

        Vector3 targetPos = target.position + Vector3.up * offset.y;
        Vector3 desiredPos = targetPos - rotation * Vector3.forward * distance;

        transform.position = desiredPos;
        transform.rotation = rotation;

        frustumPlanes = GeometryUtility.CalculateFrustumPlanes(GetComponent<Camera>());
        foreach (var rend in allRenderers)
        {
            if (rend == null) continue;
            rend.gameObject.SetActive(
                GeometryUtility.TestPlanesAABB(frustumPlanes, rend.bounds)
            );
        }
    }
}