using System.Collections;
using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject loadingScene;
    public GameObject game;

    [SerializeField] private CamerController cameraController;

    void Start()
    {
        if (mainMenu != null) mainMenu.SetActive(true);
        if (loadingScene != null) loadingScene.SetActive(false);
        if (game != null) game.SetActive(false);
        if (cameraController != null) cameraController.enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
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
}