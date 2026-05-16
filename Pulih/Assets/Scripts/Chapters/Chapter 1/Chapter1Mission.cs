using UnityEngine;

/*
CHAPTER 1 - MISSIONS
- Go to the object
- Pick up trash [
    - pickup a trash
    - find a plastic bag
    - pickup a trash with plastic bag
]
- Find a trash grabber
- Pick trash from the river
*/

public class Chapter1Mission : BaseChapterMission 
{
    public InteractUiPosition interactUI;
    public ChapterProgress chapterProgress;
    public ObjectiveZone objectiveZone; 
    
    public AuraController auraController; 

    void Start()
    {
        if (interactUI != null) interactUI.SetUI(true);

        if (objectiveZone != null)
        {
            objectiveZone.onObjectiveReached.AddListener(OnObjectiveHit);
        }
    }

    private void OnObjectiveHit()
    {
        if (interactUI != null) interactUI.SetUI(false);
        foreach (var mission in missions)
        {
            if (!mission.isCompleted)
            {
                mission.isCompleted = true;
                
                if (auraController != null)
                {
                    auraController.AddAura(mission.auraPoint);
                }
                
                break; 
            }
        }

        if (chapterProgress != null)
        {
            chapterProgress.GenerateChapterUI();
        }
    }
}