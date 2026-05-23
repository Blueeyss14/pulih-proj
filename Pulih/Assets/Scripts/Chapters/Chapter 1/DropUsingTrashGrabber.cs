using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class DropTrashUsingGrabber : MonoBehaviour
{
    [Header("Character Position & Rotation")]
    public Vector3 targetPosition;
    public Vector3 targetRotation;

    [Header("Settings")]
    public float delayDestroy = 0.5f;

    [Header("References")]
    public PickupUsingTrashGrabber trashGrabber;

    private AliceController aliceController;
    private DropController dropController;

    void Start()
    {
        aliceController = Object.FindFirstObjectByType<AliceController>();
        dropController = Object.FindFirstObjectByType<DropController>();
    }

    void Update()
    {
        if (Keyboard.current == null) return;

        bool isAimingDumpster = CrosshairAim.currentTarget != null &&
                                CrosshairAim.currentTarget.CompareTag("Dumpster");

        if (dropController != null)
        {
            dropController.enabled = !isAimingDumpster;
        }

        if (Keyboard.current.gKey.wasPressedThisFrame)
        {
            if (isAimingDumpster && trashGrabber != null && trashGrabber.currentTrash > 0)
            {
                StartCoroutine(DropTrashRoutine());
            }
        }
    }

    private IEnumerator DropTrashRoutine()
    {
        if (aliceController != null)
        {
            // aliceController.TeleportTo(targetPosition, Quaternion.Euler(targetRotation));
        }

        yield return new WaitForSeconds(delayDestroy);

        if (trashGrabber.grabberCollider != null)
        {
            foreach (Transform child in trashGrabber.grabberCollider.transform)
            {
                if (child.CompareTag("Trash"))
                {
                    Destroy(child.gameObject);
                }
            }
        }

        trashGrabber.currentTrash = 0;

        if (trashGrabber.trashCounterText != null)
        {
            trashGrabber.trashCounterText.text = trashGrabber.currentTrash + "/" + trashGrabber.maxTrash;
        }
    }
}