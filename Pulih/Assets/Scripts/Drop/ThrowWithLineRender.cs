using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

public class ThrowWithLineRender : MonoBehaviour
{
    [Header("Throw Settings")]
    public float minThrowForce = 3f;
    public float maxThrowForce = 20f;
    public float scrollSensitivity = 0.05f;

    [Header("Trajectory Settings")]
    public Color startColor = Color.cyan;
    public Color endColor = Color.green;
    public float lineWidth = 0.05f;
    public int trajectoryResolution = 45;
    public float timeStep = 0.05f;

    [Header("Landing Marker")]
    public GameObject customLandingMarker;
    public float markerScale = 0.25f;

    private float currentThrowForce;
    private bool isAiming = false;

    private LineRenderer lineRenderer;
    private GameObject landingMarker;

    private DropController dropController;
    private PickupController pickupController;
    private Camera playerCamera;
    private Animator animator;
    private Transform playerTransform;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }

        lineRenderer.enabled = false;
        lineRenderer.useWorldSpace = true;
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;

        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        UpdateLineRendererColors();
    }

    void Start()
    {
        dropController = Object.FindFirstObjectByType<DropController>();
        if (dropController != null)
        {
            pickupController = dropController.pickupController;
            playerCamera = dropController.playerCamera;
            animator = dropController.animator;
            playerTransform = dropController.transform;
        }

        DropItem dropItem = GetComponent<DropItem>();
        if (dropItem != null)
        {
            currentThrowForce = dropItem.throwForce;
        }
        else
        {
            currentThrowForce = 8f;
        }

        currentThrowForce = Mathf.Clamp(currentThrowForce, minThrowForce, maxThrowForce);

        CreateLandingMarker();
    }

    void Update()
    {
        if (!IsHeld())
        {
            if (isAiming)
            {
                CancelAiming();
            }
            return;
        }

        if (Keyboard.current != null)
        {
            if (Keyboard.current.gKey.wasPressedThisFrame)
            {
                StartAiming();
            }

            if (isAiming)
            {
                if (Keyboard.current.gKey.isPressed)
                {
                    UpdateAiming();
                }
                else if (Keyboard.current.gKey.wasReleasedThisFrame || !Keyboard.current.gKey.isPressed)
                {
                    ExecuteThrow();
                }
            }
        }
    }

    void LateUpdate()
    {
        if (isAiming && playerTransform != null && playerCamera != null)
        {
            Vector3 camForward = playerCamera.transform.forward;
            camForward.y = 0f;
            if (camForward.magnitude > 0.1f)
            {
                playerTransform.rotation = Quaternion.LookRotation(camForward);
            }
        }
    }

    bool IsHeld()
    {
        if (pickupController == null) return false;
        return pickupController.rightHandItem == gameObject ||
               pickupController.leftHandItem == gameObject ||
               pickupController.bothHandItem == gameObject;
    }

    string GetHeldHand()
    {
        if (pickupController == null) return null;
        if (pickupController.bothHandItem == gameObject) return "both";
        if (pickupController.rightHandItem == gameObject) return "right";
        if (pickupController.leftHandItem == gameObject) return "left";
        return null;
    }

    void StartAiming()
    {
        isAiming = true;
        
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        UpdateLineRendererColors();

        if (landingMarker != null)
        {
            landingMarker.SetActive(true);
        }
    }

    void UpdateAiming()
    {
        if (Mouse.current != null)
        {
            float scroll = Mouse.current.scroll.y.ReadValue();
            if (Mathf.Abs(scroll) > 0.01f)
            {
                currentThrowForce += scroll * scrollSensitivity;
                currentThrowForce = Mathf.Clamp(currentThrowForce, minThrowForce, maxThrowForce);
            }
        }

        DrawTrajectory();
    }

    void CancelAiming()
    {
        isAiming = false;
        if (lineRenderer != null) lineRenderer.enabled = false;
        if (landingMarker != null) landingMarker.SetActive(false);
    }

    void ExecuteThrow()
    {
        isAiming = false;
        if (lineRenderer != null) lineRenderer.enabled = false;
        if (landingMarker != null) landingMarker.SetActive(false);

        string hand = GetHeldHand();
        if (hand != null)
        {
            StartCoroutine(ThrowRoutine(hand));
        }
    }

    void UpdateLineRendererColors()
    {
        if (lineRenderer == null) return;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(startColor, 0.0f), new GradientColorKey(endColor, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(0.0f, 1.0f) }
        );
        lineRenderer.colorGradient = gradient;
    }

    void CreateLandingMarker()
    {
        if (customLandingMarker != null)
        {
            landingMarker = Instantiate(customLandingMarker);
        }
        else
        {
            landingMarker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            Destroy(landingMarker.GetComponent<Collider>());
            landingMarker.transform.localScale = Vector3.one * markerScale;

            Renderer renderer = landingMarker.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = new Material(Shader.Find("Sprites/Default"));
                renderer.material.color = endColor;
            }
        }
        landingMarker.SetActive(false);
    }

    Vector3 GetLaunchVelocity()
    {
        if (playerCamera == null) return transform.forward * currentThrowForce;

        DropItem dropItem = GetComponent<DropItem>();
        float upwardForce = (dropItem != null) ? dropItem.throwUpwardForce : 2f;

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        Vector3 targetPoint;

        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.GetPoint(100f);
        }

        Vector3 throwDir = (targetPoint - transform.position).normalized;
        Vector3 finalThrowDir = (throwDir + playerCamera.transform.up * upwardForce).normalized;

        return finalThrowDir * currentThrowForce;
    }

    void DrawTrajectory()
    {
        Vector3 startPosition = transform.position;
        Vector3 velocity = GetLaunchVelocity();

        List<Vector3> points = new List<Vector3>();
        points.Add(startPosition);

        Vector3 currentPosition = startPosition;
        bool hitSomething = false;
        Vector3 hitPoint = Vector3.zero;

        for (int i = 1; i < trajectoryResolution; i++)
        {
            float t = i * timeStep;
            Vector3 nextPosition = startPosition + velocity * t + 0.5f * Physics.gravity * (t * t);

            if (SafeLinecast(currentPosition, nextPosition, out RaycastHit hit))
            {
                points.Add(hit.point);
                hitSomething = true;
                hitPoint = hit.point;
                break;
            }

            points.Add(nextPosition);
            currentPosition = nextPosition;
        }

        lineRenderer.positionCount = points.Count;
        lineRenderer.SetPositions(points.ToArray());
        lineRenderer.enabled = true;

        if (landingMarker != null)
        {
            if (hitSomething)
            {
                landingMarker.transform.position = hitPoint;
                landingMarker.SetActive(true);
            }
            else
            {
                landingMarker.SetActive(false);
            }
        }
    }

    bool SafeLinecast(Vector3 start, Vector3 end, out RaycastHit hitInfo)
    {
        Vector3 direction = end - start;
        float distance = direction.magnitude;
        if (distance < 0.001f)
        {
            hitInfo = new RaycastHit();
            return false;
        }

        RaycastHit[] hits = Physics.RaycastAll(start, direction.normalized, distance);
        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        foreach (var hit in hits)
        {
            if (hit.collider.isTrigger) continue;
            if (playerTransform != null && hit.collider.transform.root == playerTransform.root) continue;
            if (hit.collider.transform.root == transform.root) continue;

            hitInfo = hit;
            return true;
        }

        hitInfo = new RaycastHit();
        return false;
    }

    IEnumerator ThrowRoutine(string hand)
    {
        DropItem dropItem = GetComponent<DropItem>();
        PickupItem pickupItem = GetComponent<PickupItem>();

        string dropAnim = (dropItem != null) ? dropItem.dropAnimation : "Drop";
        float delayDrop = (dropItem != null) ? dropItem.delayDrop : 0.3f;

        if (animator != null && !string.IsNullOrEmpty(dropAnim))
            animator.SetTrigger(dropAnim);

        if (pickupItem != null && animator != null && !string.IsNullOrEmpty(pickupItem.holdAnimation))
            animator.SetTrigger("EmptyHold");

        if (pickupController != null)
        {
            pickupController.targetBothWeight = 1f;
            pickupController.targetLeftWeight = 0f;
            ClearHandItem(hand);
        }

        Vector3 throwVelocity = GetLaunchVelocity();

        yield return new WaitForSeconds(delayDrop);

        transform.SetParent(null);

        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = true;
            if (col is MeshCollider meshCol)
            {
                meshCol.convex = true;
            }
        }

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null)
            rb = gameObject.AddComponent<Rigidbody>();

        rb.isKinematic = false;
        rb.useGravity = true;
        rb.constraints = RigidbodyConstraints.None;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        rb.AddForce(throwVelocity, ForceMode.Impulse);
        rb.AddTorque(Random.insideUnitSphere * throwVelocity.magnitude * 0.3f, ForceMode.Impulse);

        AliceController aliceController = Object.FindFirstObjectByType<AliceController>();
        if (aliceController != null)
            aliceController.enabled = true;
    }

    void ClearHandItem(string hand)
    {
        if (pickupController == null) return;

        switch (hand)
        {
            case "both":
                pickupController.bothHandItem = null;
                break;

            case "right":
                pickupController.rightHandItem = null;
                break;

            case "left":
                pickupController.leftHandItem = null;
                break;
        }

        if (pickupController.rightHandItem == null &&
            pickupController.leftHandItem == null &&
            pickupController.bothHandItem == null)
        {
            pickupController.targetBothWeight = 0f;
            pickupController.targetLeftWeight = 0f;
            if (animator != null) animator.SetTrigger("EmptyHold");
        }
    }

    void OnDestroy()
    {
        if (landingMarker != null)
        {
            Destroy(landingMarker);
        }
    }
}
