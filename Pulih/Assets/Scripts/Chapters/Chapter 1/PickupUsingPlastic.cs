using UnityEngine;
using TMPro;

public class PickupUsingPlastic : MonoBehaviour
{
    public TMP_Text trashCounterText;
    public int maxTrash = 10;
    public InteractUiPosition uiPosition;

    public SmokeAnimation smokeAnimation;

    [HideInInspector] public int currentTrash = 0;
    private PickupController pickupController;
    private bool isHeld = false;

    private Animator animator;

    void Start()
    {
        if (animator != null) animator = GetComponent<Animator>();
        if (uiPosition != null) uiPosition.SetUI(false);
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
                    UpdateUI();
                }

                pickupController = foundController;
                isHeld = true;

                if (uiPosition != null) uiPosition.SetUI(true);
            }
            else
            {
                ResetPickupState();
            }
        }
        else
        {
            ResetPickupState();
        }

        if (isHeld && pickupController != null)
        {
            CheckHandForTrash(pickupController.rightHandTransform);
            CheckHandForTrash(pickupController.leftHandTransform);

            if (currentTrash >= maxTrash)
            {
                pickupController.rightHandItem = this.gameObject;
                pickupController.leftHandItem = this.gameObject;
            }
        }
    }

    void ResetPickupState()
    {
        if (isHeld && uiPosition != null)
        {
            uiPosition.SetUI(false);
        }

        isHeld = false;
        if (animator != null) animator.SetTrigger("EmptyHold");
        pickupController = null;
    }

    void CheckHandForTrash(Transform handTransform)
    {
        if (handTransform == null) return;

        for (int i = handTransform.childCount - 1; i >= 0; i--)
        {
            Transform child = handTransform.GetChild(i);

            if (child.CompareTag("Trash") && child.gameObject != this.gameObject)
            {
                if (currentTrash < maxTrash)
                {
                    currentTrash++;
                    UpdateUI();

                    if (smokeAnimation != null)
                    {
                        smokeAnimation.PlaySmoke();
                    }

                    Destroy(child.gameObject);
                }
            }
        }
    }

    void UpdateUI()
    {
        if (trashCounterText != null)
        {
            trashCounterText.text = "Current Trash: " + currentTrash + "/" + maxTrash;
        }
    }
}