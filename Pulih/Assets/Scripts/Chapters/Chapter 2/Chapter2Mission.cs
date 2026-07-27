using UnityEngine;
using UnityEngine.InputSystem;

/*
CHAPTER 2 - MISSIONS

- Mission 1: Open Crafting Menu
*/

public enum Chapter2Step { Mission1, Completed }

public class Chapter2Mission : BaseChapterMission
{
    public Chapter2Step currentStep = Chapter2Step.Mission1;

    public ChapterProgress chapterProgress;
    public AuraController auraController;
    public CraftManager craftManager;
    public Chapter1Mission chapter1Mission;

    // private ChapterManager chapterManager;
    private bool craftMenuUnlocked = false;

    void Start()
    {
        // chapterManager = FindObjectOfType<ChapterManager>();
        // ActiveObjectUi.SetCurrentActiveNumber(1);
    }

    void Update()
    {
        OpenCraftingMenuMission();
    }

    private void CompleteCurrentMission()
    {
        var mission = missions.Find(x => !x.isCompleted);
        if (mission != null)
        {
            mission.isCompleted = true;
            if (auraController != null) auraController.AddAura(mission.auraPoint);
        }
        chapterProgress?.GenerateChapterUI();
    }

    private void CompleteCurrentSubMission()
    {
        var mission = missions.Find(x => !x.isCompleted);
        var subMission = mission?.subMissions.Find(x => !x.isCompleted);
        if (subMission != null) subMission.isCompleted = true;

        if (mission != null && mission.subMissions.TrueForAll(x => x.isCompleted))
        {
            mission.isCompleted = true;
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
    // || (chapterManager != null && chapterManager.currentChapter == 2)

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
}