using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenuController : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject loadingScene;
    public GameObject game;

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

        if (mainMenu != null) mainMenu.SetActive(true);
        if (loadingScene != null) loadingScene.SetActive(false);
        if (game != null) game.SetActive(false);
        if (cameraController != null) cameraController.enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        UpdateHoverAlpha(0);
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

    public void OnPlayButton()
    {
        if (mainMenu != null) mainMenu.SetActive(false);
        if (loadingScene != null) loadingScene.SetActive(true);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        StartCoroutine(StartGame());
    }

    IEnumerator StartGame()
    {
        yield return new WaitForSeconds(1f);
        
        if (game != null) game.SetActive(true);
        yield return new WaitForEndOfFrame();

        if (cameraController != null) cameraController.enabled = true;
        if (loadingScene != null) Destroy(loadingScene);
    }

    public void OnExitButton()
    {
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}