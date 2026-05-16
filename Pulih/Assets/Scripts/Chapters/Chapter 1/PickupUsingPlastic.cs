using UnityEngine;
using TMPro;

public class PickupUsingPlastic : MonoBehaviour
{
    public TMP_Text trashCounterText;
    public int maxTrash = 10;
    public InteractUiPosition uiPosition;

    public SmokeAnimation smokeAnimation;

    private int currentTrash = 0;
    private PickupController pickupController;
    private bool isHeld = false;

    void Start()
    {
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
                    pickupController = foundController;
                    isHeld = true;

                    if (uiPosition != null)
                    {
                        uiPosition.SetUI(true);
                    }
                }
            }
            else
            {
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

                    Destroy(child.gameObject);
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