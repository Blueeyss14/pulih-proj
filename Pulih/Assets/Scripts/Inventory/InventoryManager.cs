using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryManager : MonoBehaviour
{
    public GameObject inventoryMenu;
    private AliceController aliceController;
    private CamerController cameraController;
    private InventoryController inventoryController;
    private PickupController pickupController;
    private DropController dropController;

    void Start()
    {
        if (inventoryMenu != null) inventoryMenu.SetActive(false);
        aliceController     = Object.FindFirstObjectByType<AliceController>();
        cameraController    = Object.FindFirstObjectByType<CamerController>();
        pickupController    = Object.FindFirstObjectByType<PickupController>();

        var invControllers = Resources.FindObjectsOfTypeAll<InventoryController>();
        if (invControllers.Length > 0) inventoryController = invControllers[0];

        var dropControllers = Resources.FindObjectsOfTypeAll<DropController>();
        if (dropControllers.Length > 0) dropController = dropControllers[0];
    }

    void Update() 
    {
        if (Keyboard.current.tabKey.wasPressedThisFrame) 
        {
            if (inventoryMenu != null)
            {
                if (inventoryMenu.activeSelf)
                    CloseInventoryMenu();
                else
                    OpenInventoryMenu();
            }
        }
    }

    public void OpenInventoryMenu()
    {
        if (inventoryController == null || inventoryController.savedItems.Count == 0) return;
        if (inventoryMenu != null) inventoryMenu.SetActive(true);
        if (aliceController != null) aliceController.enabled = false;
        if (cameraController != null) cameraController.enabled = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        inventoryController?.SelectFirstItem();
    }

    public void CloseInventoryMenu()
    {
        if (inventoryMenu != null) inventoryMenu.SetActive(false);
        if (aliceController != null) aliceController.enabled = true;
        if (cameraController != null) cameraController.enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void UseItem()
    {
        if (inventoryController == null || pickupController == null) return;

        InventoryItem.ItemData data = inventoryController.selectedItem;
        if (data == null || data.worldObject == null) return;

        GameObject obj       = data.worldObject;
        PickupItem pickupItem = obj.GetComponent<PickupItem>();
        if (pickupItem == null) return;

        bool isFull =
            (pickupItem.useBothHands && (pickupController.rightHandItem != null || pickupController.leftHandItem != null || pickupController.bothHandItem != null)) ||
            (!pickupItem.useBothHands && pickupItem.leftFirst  && (pickupController.leftHandItem  != null && pickupController.rightHandItem != null)) ||
            (!pickupItem.useBothHands && !pickupItem.leftFirst && (pickupController.rightHandItem != null && pickupController.leftHandItem  != null));

        if (isFull) { Debug.Log("[Inventory] Tangan penuh."); return; }

        obj.SetActive(true);

        Collider col = obj.GetComponent<Collider>();
        if (col != null) col.enabled = false;

        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb != null) { rb.isKinematic = true; rb.linearVelocity = Vector3.zero; }

        if (pickupItem.useBothHands)
        {
            pickupController.AttachItem(ref pickupController.bothHandItem, obj, pickupController.bothHandTransform,
                pickupItem.rightPositionOffset, pickupItem.rightRotationOffset, pickupItem.rightScaleOffset);
        }
        else if (pickupItem.leftFirst)
        {
            if (pickupController.leftHandItem == null)
                pickupController.AttachItem(ref pickupController.leftHandItem, obj, pickupController.leftHandTransform,
                    pickupItem.leftPositionOffset, pickupItem.leftRotationOffset, pickupItem.leftScaleOffset);
            else
                pickupController.AttachItem(ref pickupController.rightHandItem, obj, pickupController.rightHandTransform,
                    pickupItem.rightPositionOffset, pickupItem.rightRotationOffset, pickupItem.rightScaleOffset);
        }
        else
        {
            if (pickupController.rightHandItem == null)
                pickupController.AttachItem(ref pickupController.rightHandItem, obj, pickupController.rightHandTransform,
                    pickupItem.rightPositionOffset, pickupItem.rightRotationOffset, pickupItem.rightScaleOffset);
            else
                pickupController.AttachItem(ref pickupController.leftHandItem, obj, pickupController.leftHandTransform,
                    pickupItem.leftPositionOffset, pickupItem.leftRotationOffset, pickupItem.leftScaleOffset);
        }

        RemoveSelectedFromInventory();
    }

    public void DropItem()
    {
        if (inventoryController == null || pickupController == null || dropController == null) return;

        InventoryItem.ItemData data = inventoryController.selectedItem;
        if (data == null || data.worldObject == null) return;

        GameObject obj      = data.worldObject;
        PickupItem pickupItem = obj.GetComponent<PickupItem>();
        if (pickupItem == null) return;

        obj.SetActive(true);

        Collider col = obj.GetComponent<Collider>();
        if (col != null) col.enabled = false;

        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb != null) { rb.isKinematic = true; rb.linearVelocity = Vector3.zero; }

        if (pickupItem.useBothHands)
            pickupController.AttachItem(ref pickupController.bothHandItem, obj, pickupController.bothHandTransform,
                pickupItem.rightPositionOffset, pickupItem.rightRotationOffset, pickupItem.rightScaleOffset);
        else if (pickupItem.leftFirst)
            pickupController.AttachItem(ref pickupController.leftHandItem, obj, pickupController.leftHandTransform,
                pickupItem.leftPositionOffset, pickupItem.leftRotationOffset, pickupItem.leftScaleOffset);
        else
            pickupController.AttachItem(ref pickupController.rightHandItem, obj, pickupController.rightHandTransform,
                pickupItem.rightPositionOffset, pickupItem.rightRotationOffset, pickupItem.rightScaleOffset);

        RemoveSelectedFromInventory();
        dropController.TryDrop();
    }

    void RemoveSelectedFromInventory()
    {
        inventoryController.savedItems.Remove(inventoryController.selectedItem);
        if (inventoryController.selectedCard != null)
            Destroy(inventoryController.selectedCard);
        inventoryController.selectedItem = null;
        inventoryController.selectedCard = null;
        inventoryController.itemDetail?.UpdateDetail(null, "", "");
        inventoryController.SelectFirstItem();
    }
}
