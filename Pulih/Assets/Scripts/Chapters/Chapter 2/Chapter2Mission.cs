using UnityEngine;
using UnityEngine.InputSystem;

/*
CHAPTER 2 - MISSIONS
- Mission 1: Open crafting menu
- Mission 2: Find something to craft [
    - pickup item
    - save item to inventory
]
*/

public enum Chapter2Step { Mission1, Mission2, Completed }

public class Chapter2Mission : BaseChapterMission
{
    public Chapter2Step currentStep = Chapter2Step.Mission1;

    public ChapterProgress chapterProgress;
    public AuraController auraController;
    private ChapterManager chapterManager;
    public CraftManager craftManager;
    public Chapter1Mission chapter1Mission;

    private bool craftMenuUnlocked = false;

    void Start()
    {
        chapterManager = FindObjectOfType<ChapterManager>();
    }

    void Update()
    {
        OpenCraftingMenuMission();
    }

    private void CompleteCurrentMission()
    {
        chapterManager?.CompleteCurrentMission(missions, auraController, chapterProgress);
    }

    private void CompleteCurrentSubMission()
    {
        chapterManager?.CompleteCurrentSubMission(missions, auraController);
    }

    public override void ForceComplete()
    {
        currentStep = Chapter2Step.Completed;
        foreach (var mission in missions)
        {
            mission.isCompleted = true;
            foreach (var sub in mission.subMissions)
                sub.isCompleted = true;
        }
    }

    // Mission 1: Open Crafting Menu
    private void OpenCraftingMenuMission()
    {
        if (!craftMenuUnlocked)
        {
            if (currentStep != Chapter2Step.Mission1) return;
            if (chapter1Mission != null && chapter1Mission.currentStep != Chapter1Step.Completed) return;
        }

        if (CrosshairAim.currentTarget == null) return;

        CraftManager targeted = CrosshairAim.currentTarget.GetComponentInParent<CraftManager>();
        if (targeted == null) return;

        if (craftManager == null || targeted == craftManager)
    // 

        {
            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                targeted.OpenCraftingMenu();

                if (!craftMenuUnlocked)
                {
                    craftMenuUnlocked = true;
                    CompleteCurrentMission();
                    ActiveObjectUi.SetCurrentActiveNumber(0);
                    currentStep = Chapter2Step.Completed;
                }
            }
        }
    }

    /* MISSION 2
    Submission 1: pickup item
    */
    private void PickupItemToCraft() {
        
    }
    
    /* MISSION 2
    Submission 2: save item to inventory
    */
    private void SaveItemToInventory() {

    }
}