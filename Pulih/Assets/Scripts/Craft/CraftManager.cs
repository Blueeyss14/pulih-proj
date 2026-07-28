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

    public void OpenCraftingMenu()
    {
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
