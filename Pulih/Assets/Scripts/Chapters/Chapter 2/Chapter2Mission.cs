using UnityEngine;
using UnityEngine.InputSystem;

/*
CHAPTER 2 - MISSIONS
- Mission 1: Open crafting menu
- Mission 2: Find something to craft [
    - pickup item
    - save item to inventory
]
- Mission 3: Starts crafting
*/

public enum Chapter2Step { Mission1, Mission2Sub1, Mission2Sub2, Mission3, Completed }

public class Chapter2Mission : BaseChapterMission
{
    public Chapter2Step currentStep = Chapter2Step.Mission1;

    public ChapterProgress chapterProgress;
    public AuraController auraController;
    private ChapterManager chapterManager;
    public CraftManager craftManager;
    public Chapter1Mission chapter1Mission;

    public PickupController pickupController;
    private InventoryController inventoryController;
    private bool itemPickedUp = false;
    private bool itemSaved = false;

    public CraftItemDetail craftItemDetail;
    private bool craftStarted = false;
    private bool craftMenuUnlocked = false;

    void OnEnable()
    {
        CraftItemDetail.OnItemCrafted += OnCraftSuccess;
    }

    void OnDisable()
    {
        CraftItemDetail.OnItemCrafted -= OnCraftSuccess;
    }

    void Start()
    {
        chapterManager = FindObjectOfType<ChapterManager>();
        pickupController = FindObjectOfType<PickupController>();
        var invControllers = Resources.FindObjectsOfTypeAll<InventoryController>();
        if (invControllers.Length > 0) inventoryController = invControllers[0];
    }

    void Update()
    {
        OpenCraftingMenuMission();
        PickupItemToCraft();
        SaveItemToInventory();
        StartsCrafting();
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

    ///Mission 1: Open Crafting Menu
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
        {
            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                targeted.OpenCraftingMenu();

                if (!craftMenuUnlocked)
                {
                    craftMenuUnlocked = true;
                    CompleteCurrentMission();
                    ActiveObjectUi.SetCurrentActiveNumber(7);
                    currentStep = Chapter2Step.Mission2Sub1;
                }
            }
        }
    }

    /* MISSION 2
    Submission 1: pickup item
    */
    private void PickupItemToCraft() 
    {
        if (itemPickedUp) return;
        if (currentStep != Chapter2Step.Mission2Sub1) return;

        if (pickupController != null)
        {
            bool hasItem = false;
            if (pickupController.bothHandItem != null && pickupController.bothHandItem.CompareTag("Item")) hasItem = true;
            if (pickupController.rightHandItem != null && pickupController.rightHandItem.CompareTag("Item")) hasItem = true;
            if (pickupController.leftHandItem != null && pickupController.leftHandItem.CompareTag("Item")) hasItem = true;

            if (hasItem)
            {
                CompleteCurrentSubMission();
                itemPickedUp = true;
                ActiveObjectUi.SetCurrentActiveNumber(0);
                chapterProgress?.GenerateChapterUI();
                currentStep = Chapter2Step.Mission2Sub2;
            }
        }
    }

    /* MISSION 2
    Submission 2: save item to inventory
    */
    private void SaveItemToInventory() 
    {
        if (itemSaved) return;
        if (currentStep != Chapter2Step.Mission2Sub2) return;

        if (inventoryController != null)
        {
            bool hasSavedItem = false;
            foreach (var item in inventoryController.savedItems)
            {
                if (item.worldObject != null && item.worldObject.CompareTag("Item"))
                {
                    hasSavedItem = true;
                    break;
                }
            }

            if (hasSavedItem)
            {
                CompleteCurrentSubMission();
                itemSaved = true;
                chapterProgress?.GenerateChapterUI();
                
                currentStep = Chapter2Step.Mission3;
            }
        }
    }

    ///Mission 3: Starts crafting
    private void StartsCrafting() 
    {
        if (craftStarted) return;
        if (currentStep != Chapter2Step.Mission3) return;
    }

    private void OnCraftSuccess()
    {
        if (craftStarted) return;
        if (currentStep != Chapter2Step.Mission3) return;

        craftStarted = true;
        CompleteCurrentMission();
        chapterProgress?.GenerateChapterUI();
        currentStep = Chapter2Step.Completed;
    }
}