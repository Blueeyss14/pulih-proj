using UnityEngine;

public class ChapterManager : MonoBehaviour
{
    [Header("Settings")]
    public int currentChapter = 1;

    [Header("References")]
    public ChapterProgress chapterProgress;

    void Awake()
    {
        UpdateActiveChapter();
    }

    public void UpdateActiveChapter()
    {
        if (chapterProgress == null || chapterProgress.dataHolder == null) return;

        BaseChapterMission[] allMissions = chapterProgress.dataHolder.GetComponents<BaseChapterMission>();
        
        bool found = false;
        for (int i = 0; i < allMissions.Length; i++)
        {
            bool isActive = allMissions[i].chapterNumber == currentChapter;
            
            allMissions[i].enabled = isActive;

            if (isActive)
            {
                chapterProgress.currentChapterIndex = i;
                found = true;
            }
        }

        if (found)
        {
            chapterProgress.InitializeChapters();
        }
    }
}