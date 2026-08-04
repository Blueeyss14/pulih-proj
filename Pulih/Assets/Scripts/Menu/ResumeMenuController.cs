using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResumeMenuController : MonoBehaviour
{
    public GameObject resumeMenu;

    [SerializeField] private CamerController cameraController;
    [SerializeField] private Image[] hoverImages;

    private TextMeshProUGUI[] buttonTexts;
    private Color colorHover = new Color32(226, 214, 192, 255);
    private Color colorNormal = new Color32(152, 141, 122, 255);

    void Start()
    {
        buttonTexts = new TextMeshProUGUI[hoverImages.Length];
        for (int i = 0; i < hoverImages.Length; i++)
        {
            if (hoverImages[i] != null)
            {
                buttonTexts[i] = hoverImages[i].transform.parent.GetComponentInChildren<TextMeshProUGUI>();
            }
        }

        if (resumeMenu != null) resumeMenu.SetActive(false);
        if (cameraController != null) cameraController.enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        UpdateHoverAlpha(0);
    }

    private bool IsMapMenuActive()
    {
        var mapControllers = Resources.FindObjectsOfTypeAll<MapController>();
        foreach (var mc in mapControllers)
        {
            if (mc != null && mc.mapUi != null && mc.mapUi.activeInHierarchy)
            {
                return true;
            }
        }
        return false;
    }

    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (IsMapMenuActive()) return;

            if (resumeMenu != null)
            {
                bool isActive = !resumeMenu.activeSelf;
                resumeMenu.SetActive(isActive);

                if (isActive)
                {
                    CloseInventoryAndCraftMenus();
                    Time.timeScale = 0f; 
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                    UpdateHoverAlpha(0);
                }
                else
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                    OnResumeButton();
                }
            }
        }
    }

    private void CloseInventoryAndCraftMenus()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        var invManagers = Resources.FindObjectsOfTypeAll<InventoryManager>();
        foreach (var inv in invManagers)
        {
            if (inv != null) inv.CloseInventoryMenu();
        }

        var craftManagers = Resources.FindObjectsOfTypeAll<CraftManager>();
        foreach (var cm in craftManagers)
        {
            if (cm != null) cm.CloseCraftingMenu();
        }

        var mapControllers = Resources.FindObjectsOfTypeAll<MapController>();
        foreach (var mc in mapControllers)
        {
            if (mc != null) mc.CloseMap();
        }
    }

    public void UpdateHoverAlpha(int index)
    {
        for (int i = 0; i < hoverImages.Length; i++)
        {
            if (hoverImages[i] != null)
            {
                Color tempColor = hoverImages[i].color;
                tempColor.a = (i == index) ? 1f : 0f;
                hoverImages[i].color = tempColor;

                if (buttonTexts[i] != null)
                {
                    buttonTexts[i].color = (i == index) ? colorHover : colorNormal;
                }
            }
        }
    }

    public void OnResumeButton()
    {
        if (resumeMenu != null) resumeMenu.SetActive(false);
        if (cameraController != null) cameraController.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1f;
    }

    public void OnExitButton()
    {
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }


}
