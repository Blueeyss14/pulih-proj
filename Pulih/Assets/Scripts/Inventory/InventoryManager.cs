using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryManager : MonoBehaviour
{
    public GameObject inventoryMenu;
    private AliceController aliceController;
    private CamerController cameraController;

    void Start()
    {
        if (inventoryMenu != null) inventoryMenu.SetActive(false);
        aliceController = FindObjectOfType<AliceController>();
        cameraController = FindObjectOfType<CamerController>();
    }

    void Update() 
    {
        if (Keyboard.current.tabKey.wasPressedThisFrame) 
        {
            if (inventoryMenu != null)
            {
                if (inventoryMenu.activeSelf)
                {
                    CloseInventoryMenu();
                }
                else
                {
                    OpenInventoryMenu();
                }
            }
        }
    }

    public void OpenInventoryMenu()
    {
        if (inventoryMenu != null) inventoryMenu.SetActive(true);

        if (aliceController != null) aliceController.enabled = false;
        if (cameraController != null) cameraController.enabled = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void CloseInventoryMenu()
    {
        if (inventoryMenu != null) inventoryMenu.SetActive(false);

        if (aliceController != null) aliceController.enabled = true;
        if (cameraController != null) cameraController.enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
