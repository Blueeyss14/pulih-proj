using UnityEngine;

/*
CHAPTER 1 - MISSIONS

- Go to the object
- Pick up trash [
    - find a trash a can
    - pickup a trash with trash can
]
- Move all the oil barrels
*/

public enum Chapter1Step { Mission1, Mission2Sub1, Mission2Sub2, Mission3, Mission4, Completed }

public class Chapter1Mission : BaseChapterMission
{
    public Chapter1Step currentStep = Chapter1Step.Mission1;

    public ChapterProgress chapterProgress;
    public ObjectiveZone objectiveZone;
    public AuraController auraController;
    // public PickupUsingTrashGrabber pickupUsingGrabber;
    public int requiredTrash = 2;
    public int requiredTrashInGrabber = 2;

    private bool trashCanFound;
    private bool trashPickupDone;
    public bool hasReachedRequiredTrash;

    private bool trashGrabberFound;
    private bool trashPickupDoneByGrabber;
    public bool hasReachedReqTrashByGrabber;

    void Start()
    {
        ActiveObjectUi.SetCurrentActiveNumber(1);
        if (objectiveZone != null) objectiveZone.onObjectiveReached.AddListener(OnObjectiveHit);
    }

    void Update()
    {
        FindTrashCanMission();
        PickupTrashMission();
        FindTrashGrabber();
        // PickupTrashUsingGrabber();
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

    /// MISSION 3: find a trash grabber
    private void FindTrashGrabber()
    {
        if (trashGrabberFound) return;
        if (currentStep != Chapter1Step.Mission3) return;

        GameObject trashGrabber = GameObject.FindGameObjectWithTag("Trash Grabber");
        if (trashGrabber == null || trashGrabber.transform.parent == null || trashGrabber.GetComponentInParent<PickupController>() == null) return;

        CompleteCurrentMission();
        trashGrabberFound = true;
        ActiveObjectUi.SetCurrentActiveNumber(0);

        currentStep = Chapter1Step.Mission4;
    }

    /// MISSION 4: Pickup Trash from the river
    // private void PickupTrashUsingGrabber()
    // {
    //     if (trashPickupDoneByGrabber || hasReachedReqTrashByGrabber || !trashGrabberFound) return;
    //     if (currentStep != Chapter1Step.Mission4) return;
    //     if (pickupUsingGrabber == null || pickupUsingGrabber.currentTrash < requiredTrashInGrabber) return;

    //     hasReachedReqTrashByGrabber = true;
    //     CompleteCurrentMission();
    //     currentStep = Chapter1Step.Completed;
    // }
}