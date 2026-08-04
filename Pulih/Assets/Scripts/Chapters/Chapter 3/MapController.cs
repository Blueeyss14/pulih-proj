using UnityEngine;
using UnityEngine.InputSystem;

public class MapController : MonoBehaviour
{
    public GameObject mapUi;
    public bool canOpenMap = true;
    public Chapter2Mission chapter2Mission;

    public static System.Action OnMapOpened;

    private AliceController aliceController;
    private CamerController cameraController;

    void Start()
    {
        if (chapter2Mission == null) chapter2Mission = FindObjectOfType<Chapter2Mission>();
        aliceController = FindObjectOfType<AliceController>();
        cameraController = FindObjectOfType<CamerController>();

        if (mapUi != null) mapUi.SetActive(false);
    }

    void Update()
    {
        if (mapUi != null && mapUi.activeInHierarchy)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        HandleMapInput();
    }

    private bool IsOtherMenuActive()
    {
        var resumeControllers = Resources.FindObjectsOfTypeAll<ResumeMenuController>();
        foreach (var rm in resumeControllers)
        {
            if (rm != null && rm.resumeMenu != null && rm.resumeMenu.activeInHierarchy)
                return true;
        }

        var invManagers = Resources.FindObjectsOfTypeAll<InventoryManager>();
        foreach (var inv in invManagers)
        {
            if (inv != null && inv.inventoryMenu != null && inv.inventoryMenu.activeInHierarchy)
                return true;
        }

        var craftManagers = Resources.FindObjectsOfTypeAll<CraftManager>();
        foreach (var cm in craftManagers)
        {
            if (cm != null && cm.craftingMenu != null && cm.craftingMenu.activeInHierarchy)
                return true;
        }

        return false;
    }

    private void HandleMapInput()
    {
        if (Keyboard.current == null || !Keyboard.current.mKey.wasPressedThisFrame) return;
        if (mapUi == null) return;

        if (mapUi.activeSelf)
        {
            CloseMap();
        }
        else
        {
            if (!canOpenMap) return;
            if (chapter2Mission != null && chapter2Mission.currentStep != Chapter2Step.Completed) return;
            if (IsOtherMenuActive()) return;

            OpenMap();
            OnMapOpened?.Invoke();
        }
    }

    public void OpenMap()
    {
        if (mapUi != null) mapUi.SetActive(true);
        if (aliceController != null) aliceController.enabled = false;
        if (cameraController != null) cameraController.enabled = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void CloseMap()
    {
        if (mapUi != null) mapUi.SetActive(false);
        if (aliceController != null) aliceController.enabled = true;
        if (cameraController != null) cameraController.enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
