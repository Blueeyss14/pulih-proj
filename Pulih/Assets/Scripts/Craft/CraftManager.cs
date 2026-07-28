using UnityEngine;

public class CraftManager : MonoBehaviour
{
    public GameObject craftingMenu;

    void Start()
    {
        if (craftingMenu != null) craftingMenu.SetActive(false);
    }

    public void OpenCraftingMenu()
    {
        if (craftingMenu != null) craftingMenu.SetActive(true);

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void CloseCraftingMenu()
    {
        if (craftingMenu != null) craftingMenu.SetActive(false);

        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
