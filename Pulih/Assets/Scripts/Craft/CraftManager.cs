using UnityEngine;

public class CraftManager : MonoBehaviour
{
    public GameObject craftingMenu;
    private AliceController aliceController;
    private CamerController cameraController;

    void Start()
    {
        if (craftingMenu != null) craftingMenu.SetActive(false);
        aliceController = FindObjectOfType<AliceController>();
        cameraController = FindObjectOfType<CamerController>();
    }

    void Update()
    {
        if (craftingMenu != null && craftingMenu.activeInHierarchy)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    private bool IsInventoryMenuActive()
    {
        var invManagers = Resources.FindObjectsOfTypeAll<InventoryManager>();
        foreach (var inv in invManagers)
        {
            if (inv != null && inv.inventoryMenu != null && inv.inventoryMenu.activeInHierarchy)
            {
                return true;
            }
        }
        return false;
    }

    public void OpenCraftingMenu()
    {
        if (IsInventoryMenuActive()) return;

        if (craftingMenu != null) craftingMenu.SetActive(true);

        if (aliceController != null) aliceController.enabled = false;
        if (cameraController != null) cameraController.enabled = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void CloseCraftingMenu()
    {
        if (craftingMenu != null) craftingMenu.SetActive(false);

        if (aliceController != null) aliceController.enabled = true;
        if (cameraController != null) cameraController.enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
