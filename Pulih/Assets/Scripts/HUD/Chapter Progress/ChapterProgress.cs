using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChapterProgress : MonoBehaviour
{
    [Header("Data Source")]
    public GameObject dataHolder;

    private List<BaseChapterMission> allChapters = new List<BaseChapterMission>();

    [HideInInspector]
    public int currentChapterIndex = 0;
    private int lastAnimatedChapter = -1;

    [Header("Icons")]
    public Sprite checklistIcon;
    public Sprite questionmarkIcon;

    [Header("Sub-Mission")]
    public SubmissionDialogue subMissionDialogue; 
    public GameObject dialoguePrefab;
    public Transform dialogueContainer;

    [Header("UI References")]
    public TextMeshProUGUI chapterText;
    public TextMeshProUGUI progressCounterText;

    public GameObject contentPrefab;
    public Transform container;

    [Header("Overlay Animation")]
    public OverlayAnimation overlayAnimation;

    void Start()
    {
        InitializeChapters();
    }

    public void InitializeChapters()
    {
        if (dataHolder != null)
        {
            allChapters.Clear();
            allChapters.AddRange(dataHolder.GetComponents<BaseChapterMission>());
        }

        GenerateChapterUI();
    }

    public void GenerateChapterUI()
    {
        if (allChapters.Count == 0 || currentChapterIndex >= allChapters.Count)
            return;

        BaseChapterMission currentData = allChapters[currentChapterIndex];

        string chapterTitle = "Chapter " + currentData.chapterNumber;

        if (chapterText != null)
            chapterText.text = chapterTitle;

        if (overlayAnimation != null && lastAnimatedChapter != currentChapterIndex)
        {
            overlayAnimation.Play(
                currentData.chapterNumber.ToString(),
                currentData.objectiveHint
            );
            lastAnimatedChapter = currentChapterIndex;
        }

        foreach (Transform child in container)
        {
            if (child.name != "HEAD")
                Destroy(child.gameObject);
        }

        if (dialogueContainer != null)
        {
            foreach (Transform child in dialogueContainer) Destroy(child.gameObject);
        }

        int completedCount = 0;
        bool foundActive = false;

        foreach (var m in currentData.missions)
        {
            if (m.subMissions != null && m.subMissions.Count > 0)
            {
                bool allSubDone = true;
                foreach (var s in m.subMissions) { if (!s.isCompleted) { allSubDone = false; break; } }
                m.isCompleted = allSubDone;
            }

            GameObject newRow = Instantiate(contentPrefab, container);

            TextMeshProUGUI txt = newRow.GetComponentInChildren<TextMeshProUGUI>();

            if (txt != null)
                txt.text = m.missionTitle;

            Transform iconObj = newRow.transform.Find("icons");

            if (iconObj != null)
            {
                Image img = iconObj.GetComponent<Image>();

                img.sprite = m.isCompleted
                    ? checklistIcon
                    : questionmarkIcon;
            }

            if (m.isCompleted)
            {
                completedCount++;
            }
            else if (!foundActive)
            {
                if (m.subMissions != null && m.subMissions.Count > 0 && subMissionDialogue != null)
                {
                    subMissionDialogue.GenerateDialogueUI(m.subMissions, dialoguePrefab, dialogueContainer);
                }
                foundActive = true;
            }
        }

        if (progressCounterText != null)
        {
            progressCounterText.text =
                completedCount + "/" + currentData.missions.Count;
        }

        if (completedCount == currentData.missions.Count &&
            currentChapterIndex < allChapters.Count - 1)
        {
            if (!IsInvoking(nameof(NextChapter)))
            {
                Invoke(nameof(NextChapter), 2f);
            }
        }
    }

    public void NextChapter()
    {
        currentChapterIndex++;
        
        ChapterManager manager = GetComponent<ChapterManager>();
        if(manager == null) manager = FindObjectOfType<ChapterManager>();
        if(manager != null) manager.currentChapter = allChapters[currentChapterIndex].chapterNumber;

        GenerateChapterUI();
    }
}