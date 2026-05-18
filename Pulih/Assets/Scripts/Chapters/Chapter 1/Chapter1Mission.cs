using UnityEngine;

/*
CHAPTER 1 - MISSIONS

- Go to the object
- Pick up trash [
    - find a trash a can
    - pickup a trash with trash can
]
- Find a trash grabber
- Pick trash from the river
*/

public class Chapter1Mission : BaseChapterMission
{
    public ChapterProgress chapterProgress;
    public ObjectiveZone objectiveZone;

    public AuraController auraController;
    public PickupUsingPlastic pickupUsingPlastic;
    public int requiredTrash = 2;

    private bool trashCanFound;
    private bool trashPickupDone;
    private bool trashGrabberFound;
    public bool hasReachedRequiredTrash;

    void Start()
    {
        ActiveObjectUi.SetCurrentActiveNumber(1);

        if (objectiveZone != null)
        {
            objectiveZone.onObjectiveReached.AddListener(OnObjectiveHit);
        }
    }

    void Update()
    {
        FindTrashCanMission();
        PickupTrashMission();
        FindTrashGrabber();
    }

    /// MISSION 1: go to object
    private void OnObjectiveHit()
    {
        ActiveObjectUi.SetCurrentActiveNumber(2);

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

    /*
    MISSION 2:
    Submission 1: find a trash can
    */
    private void FindTrashCanMission()
    {
        if (trashCanFound) return;

        GameObject trashCan = GameObject.FindGameObjectWithTag("Trash Can");

        if (trashCan == null) return;
        if (trashCan.transform.parent == null) return;
        if (trashCan.GetComponentInParent<PickupController>() == null) return;

        SubMission subMission = missions.Find(x => !x.isCompleted)?.subMissions.Find(x => !x.isCompleted);
        if (subMission == null) return;
        subMission.isCompleted = true;

        trashCanFound = true;

        ActiveObjectUi.SetCurrentActiveNumber(0);

        if (chapterProgress != null)
        {
            chapterProgress.GenerateChapterUI();
        }
    }

    /*
    MISSION 2:
    Submission 2: pickup a trash with trash can & throw to dumpster
    */
    private void PickupTrashMission()
    {
        if (trashPickupDone) return;
        if (hasReachedRequiredTrash) return;
        if (!trashCanFound) return;
        if (pickupUsingPlastic == null) return;
        if (pickupUsingPlastic.currentTrash < requiredTrash) return;

        hasReachedRequiredTrash = true;

        ActiveObjectUi.SetCurrentActiveNumber(3);
    }

    public void OnTrashDumped()
    {
        if (trashPickupDone) return;


        SubMission subMission = missions.Find(x => !x.isCompleted)?.subMissions.Find(x => !x.isCompleted);
        if (subMission == null) return;
        subMission.isCompleted = true;

        trashPickupDone = true;

        ActiveObjectUi.SetCurrentActiveNumber(4);

        if (chapterProgress != null)
        {
            chapterProgress.GenerateChapterUI();
        }
    }

    /// MISSION 3: find a trash grabber
    private void FindTrashGrabber()
    {
        if (trashGrabberFound) return;

        GameObject trashGrabber = GameObject.FindGameObjectWithTag("Trash Grabber");

        if (trashGrabber == null) return;
        if (trashGrabber.transform.parent == null) return;
        if (trashGrabber.GetComponentInParent<PickupController>() == null) return;

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

        trashGrabberFound = true;

        ActiveObjectUi.SetCurrentActiveNumber(0);

        if (chapterProgress != null)
        {
            chapterProgress.GenerateChapterUI();
        }
    }

}