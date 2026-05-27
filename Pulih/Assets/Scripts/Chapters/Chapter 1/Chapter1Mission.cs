using UnityEngine;
using TMPro;

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

    [Header("Mission 1")]
    public ObjectiveZone objectiveZone;
    [Header("Mission 2")]
    public int requiredTrash = 2;
    private bool trashCanFound;
    private bool trashPickupDone;
    public bool hasReachedRequiredTrash;

    [Header("Mission 3")]
    public int requiredOilBarrels = 3;
    public Collider oilBarrelPlace;
    public PickupController pickupController;
    public TMP_Text oilBarrelProgressText;


    void Start()
    {
        ActiveObjectUi.SetCurrentActiveNumber(1);
        if (objectiveZone != null) objectiveZone.onObjectiveReached.AddListener(OnObjectiveHit);

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

    /// MISSION 1: go to object
    private void OnObjectiveHit()
    {
        if (currentStep != Chapter1Step.Mission1) return;

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

            if (trashCan.currentTrash >= requiredTrash)
            {
                hasReachedRequiredTrash = true;
                ActiveObjectUi.SetCurrentActiveNumber(3);
                return;
            }
        }
    }

    public void OnTrashDumped()
    {
        if (trashPickupDone) return;
        if (currentStep != Chapter1Step.Mission2Sub2) return;

        CompleteCurrentSubMission();
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
            ActiveObjectUi.SetCurrentActiveNumber(0);
            currentStep = Chapter1Step.Completed;
        }
    }
}