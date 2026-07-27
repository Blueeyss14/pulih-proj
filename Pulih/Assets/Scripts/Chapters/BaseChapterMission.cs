using System.Collections.Generic;
using UnityEngine;

public abstract class BaseChapterMission : MonoBehaviour 
{
    public int chapterNumber;
    public string objectiveHint; 
    
    [System.Serializable]
    public class SubMission {
        public string subMissionTitle;
        public bool isCompleted;
        public float auraPoint;
    }

    [System.Serializable]
    public class Mission {
        public string missionTitle;
        public bool isCompleted;
        public float auraPoint;
        public List<SubMission> subMissions = new List<SubMission>();
    }

    public List<Mission> missions = new List<Mission>();

    // Dipanggil ChapterManager saat chapter ini di-skip (testing)
    public virtual void ForceComplete() { }
}