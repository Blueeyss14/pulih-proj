using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

[System.Serializable]
public class ObjectiveData
{
    public GameObject objective;
    public Sprite image; 
    public Sprite imageComplete;
    public GameObject objectiveInfo;
    public bool isCompleted;
}

public class MapController : MonoBehaviour
{
    public GameObject mapUi;
    public bool canOpenMap = true;
    public Chapter2Mission chapter2Mission;

    [Header("Objectives List")]
    public List<ObjectiveData> objectives = new List<ObjectiveData>();

    [Header("Hover Colors")]
    public Color color = Color.white;
    public Color colorHover = Color.yellow;

    public static System.Action OnMapOpened;

    private AliceController aliceController;
    private CamerController cameraController;
    private int selectedIndex = -1;

    void Start()
    {
        if (chapter2Mission == null) chapter2Mission = FindObjectOfType<Chapter2Mission>();
        aliceController = FindObjectOfType<AliceController>();
        cameraController = FindObjectOfType<CamerController>();

        if (mapUi != null) mapUi.SetActive(false);

        SetupObjectiveListeners();
        ResetObjectiveInfos();
        ResetObjectiveColors();
        UpdateObjectiveVisuals();
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

    private void SetupObjectiveListeners()
    {
        for (int i = 0; i < objectives.Count; i++)
        {
            int index = i;
            if (objectives[index] == null) continue;

            GameObject targetObj = objectives[index].objective;

            if (targetObj != null)
            {
                EventTrigger trigger = targetObj.GetComponent<EventTrigger>();
                if (trigger == null) trigger = targetObj.AddComponent<EventTrigger>();

                EventTrigger.Entry entryEnter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
                entryEnter.callback.AddListener((data) => OnPointerEnterObjective(index));
                trigger.triggers.Add(entryEnter);

                EventTrigger.Entry entryExit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
                entryExit.callback.AddListener((data) => OnPointerExitObjective(index));
                trigger.triggers.Add(entryExit);

                EventTrigger.Entry entryClick = new EventTrigger.Entry { eventID = EventTriggerType.PointerClick };
                entryClick.callback.AddListener((data) => OnClickObjective(index));
                trigger.triggers.Add(entryClick);
            }
        }
    }

    public void ResetObjectiveInfos()
    {
        for (int i = 0; i < objectives.Count; i++)
        {
            if (objectives[i] != null && objectives[i].objectiveInfo != null)
            {
                objectives[i].objectiveInfo.SetActive(false);
            }
        }
    }

    public void ResetObjectiveColors()
    {
        for (int i = 0; i < objectives.Count; i++)
        {
            if (objectives[i] == null || objectives[i].objective == null) continue;
            Image img = objectives[i].objective.GetComponentInChildren<Image>();
            if (img != null) img.color = (i == selectedIndex) ? colorHover : color;
        }
    }

    public void UpdateObjectiveVisuals()
    {
        for (int i = 0; i < objectives.Count; i++)
        {
            if (objectives[i] == null || objectives[i].objective == null) continue;

            Image img = objectives[i].objective.GetComponentInChildren<Image>();
            if (img == null) continue;

            bool completed = objectives[i].isCompleted;
            if (completed && objectives[i].imageComplete != null)
                img.sprite = objectives[i].imageComplete;
            else if (!completed && objectives[i].image != null)
                img.sprite = objectives[i].image;

            img.color = (i == selectedIndex) ? colorHover : color;
        }
    }

    public void OnPointerEnterObjective(int index)
    {
        if (index < 0 || index >= objectives.Count || objectives[index] == null) return;
        if (objectives[index].objective == null) return;

        Image img = objectives[index].objective.GetComponentInChildren<Image>();
        if (img != null) img.color = colorHover;
    }

    public void OnPointerExitObjective(int index)
    {
        if (index < 0 || index >= objectives.Count || objectives[index] == null) return;
        if (objectives[index].objective == null) return;

        Image img = objectives[index].objective.GetComponentInChildren<Image>();
        if (img != null) img.color = (index == selectedIndex) ? colorHover : color;
    }

    public void OnClickObjective(int index)
    {
        selectedIndex = index;

        for (int i = 0; i < objectives.Count; i++)
        {
            if (objectives[i] == null) continue;

            if (objectives[i].objectiveInfo != null)
                objectives[i].objectiveInfo.SetActive(i == index);

            if (objectives[i].objective != null)
            {
                Image img = objectives[i].objective.GetComponentInChildren<Image>();
                if (img != null) img.color = (i == index) ? colorHover : color;
            }
        }
    }

    public void SetObjectiveComplete(int index, bool complete)
    {
        if (index >= 0 && index < objectives.Count && objectives[index] != null)
        {
            objectives[index].isCompleted = complete;
            UpdateObjectiveVisuals();
        }
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

        ResetObjectiveInfos();
        ResetObjectiveColors();
        UpdateObjectiveVisuals();
    }

    public void CloseMap()
    {
        if (mapUi != null) mapUi.SetActive(false);
        if (aliceController != null) aliceController.enabled = true;
        if (cameraController != null) cameraController.enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        ResetObjectiveInfos();
        ResetObjectiveColors();
    }
}
