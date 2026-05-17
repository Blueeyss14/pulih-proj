using UnityEngine;

public class ActiveObjectUi : MonoBehaviour
{
    // public int targetChapter = 1;
    // public int targetMission = 1;
    // [Tooltip("0 kalau ga punya submission")]
    // public int targetSubmission = 0; 

    // [Header("Referensi")]
    // public ChapterProgress chapterProgress;
    // private InteractUiPosition interactUiPosition;

    // void Start()
    // {
    //     interactUiPosition = GetComponent<InteractUiPosition>();
    // }

    // void Update()
    // {
    //     if (chapterProgress == null || interactUiPosition == null || chapterProgress.dataHolder == null) return;

    //     bool shouldBeActive = CheckIfActive();

    //     interactUiPosition.enabled = shouldBeActive;
    //     interactUiPosition.SetUI(shouldBeActive);
    // }

    // private bool CheckIfActive()
    // {
    //     int chapterIndex = targetChapter - 1;
    //     int missionIndex = targetMission - 1;

    //     if (chapterProgress.currentChapterIndex != chapterIndex) return false;

    //     BaseChapterMission[] chapters = chapterProgress.dataHolder.GetComponents<BaseChapterMission>();
    //     if (chapterIndex >= chapters.Length || chapterIndex < 0) return false;

    //     BaseChapterMission currentChapter = chapters[chapterIndex];

    //     int activeMissionIndex = currentChapter.missions.FindIndex(m => !m.isCompleted);
        
    //     if (activeMissionIndex != missionIndex) return false;

    //     if (targetSubmission != 0)
    //     {
    //         int subIndex = targetSubmission - 1;
    //         var activeMission = currentChapter.missions[activeMissionIndex];
            
    //         if (activeMission.subMissions != null)
    //         {
    //             int activeSubIndex = activeMission.subMissions.FindIndex(s => !s.isCompleted);
    //             if (activeSubIndex != subIndex) return false;
    //         }
    //         else
    //         {
    //             return false;
    //         }
    //     }

    //     return true;
    // }
}