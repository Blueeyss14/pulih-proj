using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PullOilBarrel : MonoBehaviour
{
    [Header("Tag Settings")]
    public string oilBarrelTag = "Oil Barrel";

    [Header("Pulling Settings")]
    public float pullDistance = 1.2f;

    public float yOffset = 0f;

    public Vector3 rotationOffset = Vector3.zero;

    public float pullMoveSpeed = 1.0f;


    [Header("Ground Raycast Settings")]
    public float raycastStartOffset = 2.0f;

    public float raycastDistance = 5.0f;

    [Header("Rotation Settings")]
    public float rotationSmoothTime = 0.15f;

    private AliceController aliceController;
    private CharacterController playerController;
    private PickupController pickupController;
    private Animator playerAnime;
    private Transform cameraTransform;

    private bool wasHeldLastFrame = false;
    private float verticalVelocity = 0f;
    private float rotationVelocity = 0f;

    void Start()
    {
        FindReferences();
    }

    void FindReferences()
    {
        if (aliceController == null)
            aliceController = Object.FindFirstObjectByType<AliceController>();

        if (aliceController != null)
        {
            if (playerController == null)
                playerController = aliceController.GetComponent<CharacterController>();

            if (pickupController == null)
                pickupController = aliceController.GetComponent<PickupController>();

            if (playerAnime == null)
                playerAnime = aliceController.GetComponent<Animator>();
        }

        if (cameraTransform == null)
        {
            if (Camera.main != null)
            {
                cameraTransform = Camera.main.transform;
            }
            else
            {
                Camera camComponent = Object.FindFirstObjectByType<Camera>();
                if (camComponent != null)
                {
                    cameraTransform = camComponent.transform;
                }
            }
        }
    }

    void Update()
    {
        if (aliceController == null || playerController == null || pickupController == null)
        {
            FindReferences();
            return;
        }

        if (!gameObject.CompareTag(oilBarrelTag))
            return;

        bool isCurrentlyHeld = IsHeld();

        if (isCurrentlyHeld)
        {
            if (!wasHeldLastFrame)
            {
                verticalVelocity = 0f;
            }

            if (aliceController.enabled)
            {
                aliceController.enabled = false;
            }

            HandlePullingMovement();
            HandleRotation();

            wasHeldLastFrame = true;
        }
        else
        {
            if (wasHeldLastFrame)
            {
                if (playerAnime != null)
                {
                    playerAnime.SetBool("IdlePull", false);
                    playerAnime.SetBool("WalkBackward", false);
                    playerAnime.SetTrigger("EmptyHold");
                }

                if (aliceController != null)
                {
                    aliceController.enabled = true;
                }
                wasHeldLastFrame = false;
            }
        }
    }

    void LateUpdate()
    {
        if (!gameObject.CompareTag(oilBarrelTag))
            return;

        if (IsHeld())
        {
            UpdateBarrelPosition();
        }
    }

    bool IsHeld()
    {
        if (pickupController == null) return false;

        return pickupController.bothHandItem == gameObject ||
               pickupController.rightHandItem == gameObject ||
               pickupController.leftHandItem == gameObject;
    }

    void HandlePullingMovement()
    {
        if (playerController == null) return;

        bool isPressingBackward = false;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.sKey.isPressed)
            {
                isPressingBackward = true;
            }
        }

        Vector3 moveDir = Vector3.zero;

        if (playerController.isGrounded)
        {
            verticalVelocity = -0.1f;
        }
        else
        {
            verticalVelocity += -9.81f * Time.deltaTime;
        }

        if (isPressingBackward)
        {
            moveDir = -playerController.transform.forward;

            Vector3 velocity = moveDir * pullMoveSpeed;
            velocity.y = verticalVelocity;

            playerController.Move(velocity * Time.deltaTime);

            if (playerAnime != null)
            {
                playerAnime.SetBool("WalkBackward", true);
                playerAnime.SetBool("IdlePull", false);
            }
        }
        else
        {
            if (playerAnime != null)
            {
                playerAnime.SetBool("IdlePull", true);
                playerAnime.SetBool("WalkBackward", false);
            }
        }
    }

    void HandleRotation()
    {
        if (cameraTransform == null || playerController == null) return;

        float currentAngle = playerController.transform.eulerAngles.y;
        float targetAngle = cameraTransform.eulerAngles.y;

        float smoothAngle = Mathf.SmoothDampAngle(
            currentAngle,
            targetAngle,
            ref rotationVelocity,
            rotationSmoothTime
        );

        playerController.transform.rotation = Quaternion.Euler(0f, smoothAngle, 0f);
    }

    void UpdateBarrelPosition()
    {
        if (playerController == null) return;

        Transform playerTrans = playerController.transform;

        Vector3 targetPos = playerTrans.position + playerTrans.forward * pullDistance;

        float groundY = playerTrans.position.y;
        Ray ray = new Ray(targetPos + Vector3.up * raycastStartOffset, Vector3.down);
        RaycastHit hit;

        int playerLayer = playerTrans.gameObject.layer;
        int barrelLayer = gameObject.layer;
        int layerMask = ~((1 << playerLayer) | (1 << barrelLayer));

        if (Physics.Raycast(ray, out hit, raycastDistance, layerMask))
        {
            groundY = hit.point.y;
        }

        transform.position = new Vector3(targetPos.x, groundY + yOffset, targetPos.z);

        transform.rotation = Quaternion.LookRotation(playerTrans.forward) * Quaternion.Euler(rotationOffset);
    }
}