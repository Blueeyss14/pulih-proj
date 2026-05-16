using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChapterProgress : MonoBehaviour
{
    [Header("Data Source")]
    public GameObject dataHolder;

    private List<BaseChapterMission> allChapters = new List<BaseChapterMission>();

    public int currentChapterIndex = 0;
    private int lastAnimatedChapter = -1;

    [Header("Icons")]
    public Sprite checklistIcon;
    public Sprite questionmarkIcon;

    [Header("UI References")]
    public TextMeshProUGUI chapterText;
    public TextMeshProUGUI progressCounterText;

    public GameObject contentPrefab;
    public Transform container;

    [Header("Overlay Animation")]
    public OverlayAnimation overlayAnimation;

    void Start()
    {
        if (dataHolder != null)
        {
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

        int completedCount = 0;

        foreach (var m in currentData.missions)
        {
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
                completedCount++;
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

        GenerateChapterUI();
    }
}