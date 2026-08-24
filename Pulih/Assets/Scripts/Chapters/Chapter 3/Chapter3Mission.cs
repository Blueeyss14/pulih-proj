using UnityEngine;

/*
CHAPTER 3 - MISSIONS
- Mission 1: Open Map
- Mission 2: Go To Seed Seller [
    - Buy plant
]
- Mission 3: Starts planting [
    - Go to object
    - Completed restoration
]
*/
public enum Chapter3Step { Mission1, Mission2Sub1, Mission2Sub2, Mission3Sub1, Completed }

public class Chapter3Mission : BaseChapterMission
{
    public Chapter3Step currentStep = Chapter3Step.Mission1;
    public GameObject openMapButton;

    public ChapterProgress chapterProgress;
    public AuraController auraController;
    private ChapterManager chapterManager;
    public Chapter2Mission chapter2Mission;
    public MapController mapController;
    public ObjectiveZone objectiveZone;
    public ObjectiveZone objectiveZone2;
    public SeedSellerController seedSellerController;

    private InventoryController inventoryController;
    private bool seedBought = false;

    void OnEnable()
    {
        MapController.OnMapOpened += OnMapOpened;
    }

    void OnDisable()
    {
        MapController.OnMapOpened -= OnMapOpened;
    }

    void Start()
    {
        chapterManager = FindObjectOfType<ChapterManager>();
        if (mapController == null) mapController = FindObjectOfType<MapController>();
        if (seedSellerController == null) seedSellerController = FindObjectOfType<SeedSellerController>();

        var invControllers = Resources.FindObjectsOfTypeAll<InventoryController>();
        if (invControllers.Length > 0) inventoryController = invControllers[0];

        if (openMapButton != null) openMapButton.SetActive(false);
        if (objectiveZone != null) objectiveZone.onObjectiveReached.AddListener(OnObjectiveHit);
        if (objectiveZone2 != null) objectiveZone2.onObjectiveReached.AddListener(GoToObject);
    }

    void Update()
    {
        if (chapterManager != null && chapterManager.currentChapter == 3)
        {
            if (openMapButton != null) openMapButton.SetActive(true);
        }
        else
        {
            if (openMapButton != null) openMapButton.SetActive(false);
        }

        CheckChapterStatus();
        BuySeed();
    }

    private void CheckChapterStatus()
    {
        if (mapController != null)
        {
            mapController.canOpenMap = true;
        }
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
        currentStep = Chapter3Step.Completed;
        PlantController.showActiveObjectiveUI = true;
        foreach (var mission in missions)
        {
            mission.isCompleted = true;
            foreach (var sub in mission.subMissions)
                sub.isCompleted = true;
        }
    }

    ///Mission 1: Open Map
    private void OnMapOpened()
    {
        if (currentStep != Chapter3Step.Mission1) return;
        if (chapter2Mission != null && chapter2Mission.currentStep != Chapter2Step.Completed) return;

        CompleteCurrentMission();
        ActiveObjectUi.SetCurrentActiveNumber(8);
        currentStep = Chapter3Step.Mission2Sub1;
    }

    /* MISSION 2
    Submission 1: go to object
    */
    private void OnObjectiveHit()
    {
        if (currentStep != Chapter3Step.Mission2Sub1) return;
        CompleteCurrentSubMission();
        ActiveObjectUi.SetCurrentActiveNumber(0);
        chapterProgress?.GenerateChapterUI();
        currentStep = Chapter3Step.Mission2Sub2;
    }

    /* MISSION 2
    Submission 2: buy seeds
    */
    private void BuySeed()
    {
        if (seedBought) return;
        if (currentStep != Chapter3Step.Mission2Sub2) return;

        if (inventoryController == null)
        {
            var invControllers = Resources.FindObjectsOfTypeAll<InventoryController>();
            if (invControllers.Length > 0) inventoryController = invControllers[0];
        }
        if (seedSellerController == null)
        {
            seedSellerController = FindObjectOfType<SeedSellerController>();
        }

        if (inventoryController != null && seedSellerController != null)
        {
            bool hasBoughtFromSeller = false;
            foreach (var invItem in inventoryController.savedItems)
            {
                if (invItem.worldObject != null)
                {
                    foreach (var sellerItem in seedSellerController.items)
                    {
                        if (sellerItem.itemPrefab != null && invItem.worldObject.name.StartsWith(sellerItem.itemPrefab.name))
                        {
                            hasBoughtFromSeller = true;
                            break;
                        }
                    }
                }
                if (hasBoughtFromSeller) break;
            }

            if (hasBoughtFromSeller)
            {
                seedBought = true;
                CompleteCurrentSubMission();
                chapterProgress?.GenerateChapterUI();
                ActiveObjectUi.SetCurrentActiveNumber(9);
                currentStep = Chapter3Step.Mission3Sub1;
            }
        }
    }

    /* MISSION 3
    Submission 1: Go to object
    */

    private void GoToObject() {
        if (currentStep != Chapter3Step.Mission3Sub1) return;
        CompleteCurrentSubMission();
        ActiveObjectUi.SetCurrentActiveNumber(0);
        PlantController.showActiveObjectiveUI = true;
        chapterProgress?.GenerateChapterUI();
        currentStep = Chapter3Step.Completed;
    }

    /* MISSION 3
    Submission 2: Completed Restoration
    */

    private void CompletedRestoration() {
        
    }

}
