using UnityEngine;

/*
CHAPTER 3 - MISSIONS
- Mission 1: Open Map
- Mission 2: Go To Seed Seller [
    - Buy
    - ??????
]
- Mission 3: Starts planting
*/
public enum Chapter3Step { Mission1, Mission2Sub1, Completed }

public class Chapter3Mission : BaseChapterMission
{
    public Chapter3Step currentStep = Chapter3Step.Mission1;
    public GameObject openMapButton;

    public ChapterProgress chapterProgress;
    public AuraController auraController;
    private ChapterManager chapterManager;
    public Chapter2Mission chapter2Mission;
    public MapController mapController;

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
        if (openMapButton != null) openMapButton.SetActive(false);
    }

    void Update()
    {
        if (chapterManager.currentChapter == 3)
        {
            if (openMapButton != null) openMapButton.SetActive(true);
        }
        else
        {
            if (openMapButton != null) openMapButton.SetActive(false);
        }

        CheckChapterStatus();
        Buyy();
    }

    private void CheckChapterStatus()
    {
        if (currentStep == Chapter3Step.Completed && mapController != null)
        {
            mapController.canOpenMap = false;
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
        if (mapController != null) mapController.canOpenMap = false;
        foreach (var mission in missions)
        {
            mission.isCompleted = true;
            foreach (var sub in mission.subMissions)
                sub.isCompleted = true;
        }
    }

    //Mission 1: Open Map
    private void OnMapOpened()
    {
        if (currentStep != Chapter3Step.Mission1) return;
        if (chapter2Mission != null && chapter2Mission.currentStep != Chapter2Step.Completed) return;

        CompleteCurrentMission();
        currentStep = Chapter3Step.Mission2Sub1;
    }

    private void Buyy()
    {
        if (currentStep != Chapter3Step.Mission2Sub1) return;
    }
}
