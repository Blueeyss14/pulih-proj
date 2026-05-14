using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChapterProgress : MonoBehaviour
{
    public int chapterNumber; 
    public Sprite checklistIcon;
    public Sprite questionmarkIcon;

    [System.Serializable]
    public class Mission {
        public string missionTitle;
        public bool isCompleted;
    }

    public List<Mission> missions = new List<Mission>();

    [Header("UI References")]
    public TextMeshProUGUI chapterText;
    public TextMeshProUGUI progressCounterText;
    public GameObject contentPrefab;
    public Transform container;

    void Start()
    {
        GenerateChapterUI();
    }

    public void GenerateChapterUI()
    {
        if (chapterText != null) chapterText.text = "Chapter " + chapterNumber;

        foreach (Transform child in container) {
            if (child.name != "HEAD") Destroy(child.gameObject);
        }

        int completedCount = 0;

        foreach (Mission m in missions)
        {
            GameObject newRow = Instantiate(contentPrefab, container);
            
            TextMeshProUGUI txt = newRow.GetComponentInChildren<TextMeshProUGUI>();
            if (txt != null) txt.text = m.missionTitle;

            Transform iconObj = newRow.transform.Find("icons");
            if (iconObj != null) {
                Image img = iconObj.GetComponent<Image>();
                img.sprite = m.isCompleted ? checklistIcon : questionmarkIcon;
            }

            if (m.isCompleted) completedCount++;
        }

        if (progressCounterText != null) {
            progressCounterText.text = completedCount + "/" + missions.Count;
        }
    }
}