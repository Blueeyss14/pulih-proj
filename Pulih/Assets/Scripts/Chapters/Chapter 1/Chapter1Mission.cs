using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.Playables;

/*
CHAPTER 1 - MISSIONS

- Mission 1: Go to the object
- Mission 2 : Pick up trash [
    - find a trash a can
    - pickup a trash with trash can
]
- Mission 3: Move all the oil barrels
*/

public enum Chapter1Step { Mission1, Mission2Sub1, Mission2Sub2, Mission3, Completed }

public class Chapter1Mission : BaseChapterMission
{

    public Chapter1Step currentStep = Chapter1Step.Mission1;

    public ChapterProgress chapterProgress;
    public AuraController auraController;
    private ChapterManager chapterManager;

    [Header("Mission 1")]
    public ObjectiveZone objectiveZone;
    public PlayableDirector mission1CutsceneDirector;
    public GameObject cutsceneTimelineObject;
    public GameObject playerController;
    public Camera gameplayCamera;
    public GameObject allTrash;

    [Header("Mission 2")]
    public int requiredTrash = 10;
    public TMP_Text requiredTrashText;
    private bool trashCanFound;
    private bool trashPickupDone;
    public bool hasReachedRequiredTrash;

    private int totalCollectedTrash = 0;
    private Dictionary<PickupUsingPlastic, int> trashCanTracker = new Dictionary<PickupUsingPlastic, int>();

    [Header("Mission 3")]
    public int requiredOilBarrels = 6;
    public Collider oilBarrelPlace;
    public PickupController pickupController;
    public TMP_Text oilBarrelProgressText;
    private bool oilBarrelPulled;

    void Start()
    {
        chapterManager = FindObjectOfType<ChapterManager>();
        ActiveObjectUi.SetCurrentActiveNumber(1);
        if (objectiveZone != null) objectiveZone.onObjectiveReached.AddListener(OnObjectiveHit);


        if (requiredTrashText != null)
            requiredTrashText.text = "Required Trash: " + "0/" + requiredTrash;

        if (oilBarrelProgressText != null)
            oilBarrelProgressText.text = "0/" + requiredOilBarrels;
    }

    void Update()
    {
        FindTrashCanMission();
        PickupTrashMission();
        MoveAllOilBarrels();
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
        currentStep = Chapter1Step.Completed;
        foreach (var mission in missions)
        {
            mission.isCompleted = true;
            foreach (var sub in mission.subMissions)
                sub.isCompleted = true;
        }
    }

    /// MISSION 1: go to object
    private void OnObjectiveHit()
    {
        if (currentStep != Chapter1Step.Mission1) return;
        ActiveObjectUi.SetCurrentActiveNumber(0);

        if (playerController != null) playerController.SetActive(false);
        if (gameplayCamera != null) gameplayCamera.gameObject.SetActive(false);

        if (cutsceneTimelineObject != null) cutsceneTimelineObject.SetActive(true);

        if (mission1CutsceneDirector != null)
        {
            mission1CutsceneDirector.stopped += OnMission1CutsceneEnd;
            mission1CutsceneDirector.Play();
        }
        else
        {
            OnMission1CutsceneEnd(null);
        }
    }

    public void OnMission1CutsceneEnd(PlayableDirector director)
    {
        if (director != null) director.stopped -= OnMission1CutsceneEnd;

        if (cutsceneTimelineObject != null) cutsceneTimelineObject.SetActive(false);

        if (playerController != null) playerController.SetActive(true);
        if (gameplayCamera != null) gameplayCamera.gameObject.SetActive(true);

        ActiveObjectUi.SetCurrentActiveNumber(2);
        CompleteCurrentMission();

        currentStep = Chapter1Step.Mission2Sub1;
    }

    /* MISSION 2
    Submission 1: find a trash can
    */
    private void FindTrashCanMission()
    {
        if (trashCanFound) return;
        if (currentStep != Chapter1Step.Mission2Sub1) return;

        PickupUsingPlastic[] trashCans = FindObjectsOfType<PickupUsingPlastic>();

        foreach (PickupUsingPlastic trashCan in trashCans)
        {
            if (!trashCan.CompareTag("Trash Can")) continue;

            if (trashCan.transform.parent != null &&
                trashCan.GetComponentInParent<PickupController>() != null)
            {
                CompleteCurrentSubMission();
                trashCanFound = true;
                ActiveObjectUi.SetCurrentActiveNumber(0);
                chapterProgress?.GenerateChapterUI();

                currentStep = Chapter1Step.Mission2Sub2;
                return;
            }
        }
    }

    /* MISSION 2
    Submission 2:
    pickup a trash with trash can
    */
    private void PickupTrashMission()
    {
        if (trashPickupDone || hasReachedRequiredTrash || !trashCanFound) return;
        if (currentStep != Chapter1Step.Mission2Sub2) return;

        PickupUsingPlastic[] trashCans = FindObjectsOfType<PickupUsingPlastic>();

        foreach (PickupUsingPlastic trashCan in trashCans)
        {
            if (!trashCan.CompareTag("Trash Can")) continue;

            if (!trashCanTracker.ContainsKey(trashCan))
            {
                trashCanTracker[trashCan] = 0;
            }

            int difference = trashCan.currentTrash - trashCanTracker[trashCan];
            if (difference > 0)
            {
                totalCollectedTrash += difference;
            }

            trashCanTracker[trashCan] = trashCan.currentTrash;
        }

        if (requiredTrashText != null)
        {
            requiredTrashText.text = "Required Trash: " + totalCollectedTrash + "/" + requiredTrash;
        }

        if (totalCollectedTrash >= requiredTrash)
        {
            hasReachedRequiredTrash = true;
            ActiveObjectUi.SetCurrentActiveNumber(3);
        }
    }

    public void OnTrashDumped()
    {
        if (trashPickupDone) return;
        if (currentStep != Chapter1Step.Mission2Sub2) return;

        CompleteCurrentSubMission();
        Destroy(allTrash);
        trashPickupDone = true;
        ActiveObjectUi.SetCurrentActiveNumber(4);
        chapterProgress?.GenerateChapterUI();

        currentStep = Chapter1Step.Mission3;
    }

    /// MISSION 3: Move all the oil barrels
    private void MoveAllOilBarrels()
    {
        if (currentStep != Chapter1Step.Mission3) return;
        if (oilBarrelPlace == null) return;

        GameObject[] barrels = GameObject.FindGameObjectsWithTag("Oil Barrel");
        int count = 0;

        foreach (GameObject barrel in barrels)
        {
            bool isHeld = pickupController != null && (pickupController.bothHandItem == barrel || pickupController.rightHandItem == barrel || pickupController.leftHandItem == barrel);

            if (isHeld && !oilBarrelPulled)
            {
                oilBarrelPulled = true;
                ActiveObjectUi.SetCurrentActiveNumber(5);
            }

            if (!isHeld && oilBarrelPlace.bounds.Contains(barrel.transform.position))
            {
                count++;
            }
        }

        if (oilBarrelProgressText != null)
        {
            oilBarrelProgressText.text = count + "/" + requiredOilBarrels;
        }

        if (count >= requiredOilBarrels)
        {
            CompleteCurrentMission();
            ActiveObjectUi.SetCurrentActiveNumber(6);
            currentStep = Chapter1Step.Completed;
        }
    }
}