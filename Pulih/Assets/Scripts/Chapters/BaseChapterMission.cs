using System.Collections.Generic;
using UnityEngine;

public abstract class BaseChapterMission : MonoBehaviour 
{
    public int chapterNumber;
    public string objectiveHint; 
    
    [System.Serializable]
    public class Mission {
        public string missionTitle;
        public bool isCompleted;
        public float auraPoint;
    }

    public List<Mission> missions = new List<Mission>();
}