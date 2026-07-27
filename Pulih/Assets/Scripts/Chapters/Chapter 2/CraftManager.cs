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
    }
}
