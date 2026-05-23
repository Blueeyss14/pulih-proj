using UnityEngine;
using TMPro;

public class PickupUsingTrashGrabber : MonoBehaviour
{
    public TMP_Text trashCounterText;
    public int maxTrash = 5;
    public InteractUiPosition uiPosition;
    public SmokeAnimation smokeAnimation;
    public Collider grabberCollider;

    [Header("Crosshair")]
    public float extraPickupDistance = 10f;

    [Header("Animation")]
    public string pickupAnimation = "PickupUsingGrabber";

    [HideInInspector] public int currentTrash = 0;
    private PickupController pickupController;
    public bool isHeld = false;

    private Animator animator;

    void Start()
    {
        if (uiPosition != null) uiPosition.SetUI(false);

        animator = GetComponentInParent<Animator>();

        UpdateUI();
    }

    void Update()
    {
        if (transform.parent != null)
        {
            PickupController foundController = GetComponentInParent<PickupController>();

            if (foundController != null)
            {
                if (!isHeld)
                {
                    pickupController = foundController;
                    isHeld = true;

                    pickupController.overridePickupAnimation = pickupAnimation;

                    CrosshairAim crosshairAim = Object.FindFirstObjectByType<CrosshairAim>();

                    if (crosshairAim != null)
                    {
                        crosshairAim.distance += extraPickupDistance;
                    }

                    if (uiPosition != null)
                    {
                        uiPosition.SetUI(true);
                    }
                }
            }
            else
            {
                if (isHeld)
                {
                    CrosshairAim crosshairAim = Object.FindFirstObjectByType<CrosshairAim>();

                    if (crosshairAim != null)
                    {
                        crosshairAim.distance -= extraPickupDistance;
                    }

                    pickupController.overridePickupAnimation = "";
                }

                if (isHeld && uiPosition != null)
                {
                    uiPosition.SetUI(false);
                }

                isHeld = false;
                pickupController = null;
            }
        }
        else
        {
            if (isHeld)
            {
                CrosshairAim crosshairAim = Object.FindFirstObjectByType<CrosshairAim>();

                if (crosshairAim != null)
                {
                    crosshairAim.distance -= extraPickupDistance;
                }

                if (animator != null)
                {
                    animator.SetTrigger(pickupAnimation);
                }

                if (pickupController != null)
                {
                    pickupController.overridePickupAnimation = "";
                }
            }

            if (isHeld && uiPosition != null)
            {
                uiPosition.SetUI(false);
            }

            isHeld = false;
            pickupController = null;
        }

        if (isHeld && pickupController != null)
        {
            CheckHandForTrash(pickupController.rightHandTransform);
            CheckHandForTrash(pickupController.leftHandTransform);
        }
    }

    void CheckHandForTrash(Transform handTransform)
    {
        if (handTransform == null) return;

        for (int i = handTransform.childCount - 1; i >= 0; i--)
        {
            Transform child = handTransform.GetChild(i);

            if (child.CompareTag("Trash"))
            {
                if (currentTrash < maxTrash)
                {
                    currentTrash++;

                    UpdateUI();

                    if (smokeAnimation != null)
                    {
                        smokeAnimation.PlaySmoke();
                    }

                    if (grabberCollider != null)
                    {
                        child.SetParent(grabberCollider.transform);
                        child.localPosition = Vector3.zero;
                    }

                    if (pickupController.rightHandItem == child.gameObject) pickupController.rightHandItem = null;
                    if (pickupController.leftHandItem == child.gameObject) pickupController.leftHandItem = null;
                    if (pickupController.bothHandItem == child.gameObject) pickupController.bothHandItem = null;
                }
            }
        }
    }

    void UpdateUI()
    {
        if (trashCounterText != null)
        {
            trashCounterText.text = currentTrash + "/" + maxTrash;
        }
    }
}