using UnityEngine;

public class UIAction : MonoBehaviour
{
    [Header("UI Drop")]
    public GameObject uiPressGToDrop;
    public GameObject uiSaveItem;
    public GameObject uiHoldGToDrop;
    public GameObject tabInventory;

    private PickupController pickupController;
    private InventoryController inventoryController;
    private Chapter2Mission chapter2Mission;

    void Start()
    {
        pickupController = FindObjectOfType<PickupController>();
        var invControllers = Resources.FindObjectsOfTypeAll<InventoryController>();
        if (invControllers.Length > 0) inventoryController = invControllers[0];
        if (tabInventory != null) tabInventory.SetActive(false);
        chapter2Mission = FindObjectOfType<Chapter2Mission>();
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
                if (uiSaveItem != null) uiSaveItem.SetActive(false);
                if (uiHoldGToDrop != null) uiHoldGToDrop.SetActive(true);
                if (tabInventory != null) tabInventory.SetActive(false);
            }
            else
            {
                if (uiPressGToDrop != null) uiPressGToDrop.SetActive(true);
                
                bool canSave = true;
                if (chapter2Mission != null)
                {
                    canSave = (int)chapter2Mission.currentStep >= (int)Chapter2Step.Mission2Sub2;
                }

                if (uiSaveItem != null) uiSaveItem.SetActive(canSave);
                
                if (uiHoldGToDrop != null) uiHoldGToDrop.SetActive(false);
                if (tabInventory != null) tabInventory.SetActive(false);
            }
        }
        else
        {
            if (uiPressGToDrop != null) uiPressGToDrop.SetActive(false);
            if (uiSaveItem != null) uiSaveItem.SetActive(false);
            if (uiHoldGToDrop != null) uiHoldGToDrop.SetActive(false);

            bool hasItems = inventoryController != null && inventoryController.savedItems.Count > 0;
            if (tabInventory != null) tabInventory.SetActive(hasItems);
        }
    }
}
