using UnityEngine;

public class UIAction : MonoBehaviour
{
    [Header("UI Drop")]
    public GameObject uiPressGToDrop;
    public GameObject uiHoldGToDrop;
    public GameObject tabInventory;

    private PickupController pickupController;

    void Start()
    {
        pickupController = FindObjectOfType<PickupController>();
        if (tabInventory != null) tabInventory.SetActive(true);
    }

    void Update()
    {
        if (pickupController == null) return;

        GameObject currentItem = pickupController.rightHandItem;
        if (currentItem == null) currentItem = pickupController.leftHandItem;
        if (currentItem == null) currentItem = pickupController.bothHandItem;

        if (currentItem != null)
        {
            if (currentItem.GetComponent<PickupUsingPlastic>() != null)
            {
                if (uiPressGToDrop != null) uiPressGToDrop.SetActive(false);
                if (uiHoldGToDrop != null) uiHoldGToDrop.SetActive(true);
                if (tabInventory != null) tabInventory.SetActive(false);
            }
            else
            {
                if (uiPressGToDrop != null) uiPressGToDrop.SetActive(true);
                if (uiHoldGToDrop != null) uiHoldGToDrop.SetActive(false);
                if (tabInventory != null) tabInventory.SetActive(false);
            }
        }
        else
        {
            if (uiPressGToDrop != null) uiPressGToDrop.SetActive(false);
            if (uiHoldGToDrop != null) uiHoldGToDrop.SetActive(false);
            if (tabInventory != null) tabInventory.SetActive(true);
        }
    }
}
