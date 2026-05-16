using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SubmissionDialogue : MonoBehaviour
{
    public Sprite dialogueDone;
    public Sprite dialogueSelected;
    public Sprite dialogueIdle;

    public void GenerateDialogueUI(List<BaseChapterMission.SubMission> subMissions, GameObject prefab, Transform container)
    {
        if (subMissions == null || subMissions.Count == 0) return;

        bool foundSelected = false;

        foreach (var sub in subMissions)
        {
            GameObject row = Instantiate(prefab, container);
            
            TextMeshProUGUI txt = row.GetComponentInChildren<TextMeshProUGUI>();
            if (txt != null) txt.text = sub.subMissionTitle;

            Image img = row.GetComponent<Image>();
            if (img != null)
            {
                if (sub.isCompleted)
                {
                    img.sprite = dialogueDone;
                }
                else if (!foundSelected)
                {
                    img.sprite = dialogueSelected;
                    foundSelected = true;
                }
                else
                {
                    img.sprite = dialogueIdle;
                }
            }
        }
    }
}