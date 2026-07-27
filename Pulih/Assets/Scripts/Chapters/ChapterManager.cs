using System.Collections.Generic;
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
            if (allMissions[i].chapterNumber == currentChapter)
            {
                chapterProgress.currentChapterIndex = i;
                found = true;
            }

            if (allMissions[i].chapterNumber < currentChapter)
            {
                allMissions[i].ForceComplete();
            }
        }

        if (found)
        {
            chapterProgress.InitializeChapters();
        }
    }

    public void CompleteCurrentMission(List<BaseChapterMission.Mission> missions, AuraController auraController, ChapterProgress chapterProgress)
    {
        var mission = missions.Find(x => !x.isCompleted);
        if (mission != null)
        {
            mission.isCompleted = true;
            if (auraController != null) auraController.AddAura(mission.auraPoint);
        }
        chapterProgress?.GenerateChapterUI();
    }

    public void CompleteCurrentSubMission(List<BaseChapterMission.Mission> missions, AuraController auraController)
    {
        var mission = missions.Find(x => !x.isCompleted);
        var subMission = mission?.subMissions.Find(x => !x.isCompleted);
        if (subMission != null)
        {
            subMission.isCompleted = true;
            if (auraController != null) auraController.AddAura(subMission.auraPoint);
        }

        if (mission != null && mission.subMissions.TrueForAll(x => x.isCompleted))
        {
            mission.isCompleted = true;
        }
    }
}