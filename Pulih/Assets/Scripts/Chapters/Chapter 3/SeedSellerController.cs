using UnityEngine;
using UnityEngine.InputSystem;

public class SeedSellerController : MonoBehaviour
{
    [Header("UI")]
    public GameObject interactUi;
    public GameObject seedSellerUi;
    private ChapterManager chapterManager;
    

    void Start()
    {
        if (interactUi != null) interactUi.SetActive(false);
        if (seedSellerUi != null) seedSellerUi.SetActive(false);
        chapterManager = FindObjectOfType<ChapterManager>();
    }

    void Update()
    {
        if (interactUi == null) return;

        if (chapterManager.currentChapter == 3) {        
                
            bool isAimed = CrosshairAim.currentTarget == gameObject;
            interactUi.SetActive(isAimed);

            if (isAimed && Keyboard.current.eKey.wasPressedThisFrame) {
                isAimed = false;
                OpenMenu();
            }
        }
    }

    private void OpenMenu() {
        if (seedSellerUi != null) seedSellerUi.SetActive(true);
        Time.timeScale = 0f; 
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void CloseMenu() {
        if (seedSellerUi != null) seedSellerUi.SetActive(false);
        Time.timeScale = 1f; 
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}

