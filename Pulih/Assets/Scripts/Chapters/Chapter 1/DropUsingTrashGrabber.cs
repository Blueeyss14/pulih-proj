// using UnityEngine;
// using UnityEngine.InputSystem;
// using System.Collections;

// public class DropTrashUsingGrabber : MonoBehaviour
// {
//     [Header("Animator")]
//     public Animator animator;
//     // public Animator grabberAnimator;
//     [Header("Character Position & Rotation")]
//     public Vector3 targetPosition;
//     public Vector3 targetRotation;

//     [Header("Settings")]
//     public float delayDestroy = 0.5f;
//     public float moveSpeed = 5f;

//     private PickupUsingTrashGrabber trashGrabber;
//     private AliceController aliceController;
//     private DropController dropController;

//     void Awake()
//     {
//         trashGrabber = GetComponent<PickupUsingTrashGrabber>();
//         aliceController = Object.FindFirstObjectByType<AliceController>();
//         dropController = Object.FindFirstObjectByType<DropController>();
//     }

//     void Start()
//     {
//         if (animator == null) return;
//         // if (grabberAnimator == null) return;
//         animator.SetBool("DropTrashFromGrabber", false);
//         // grabberAnimator.SetBool("TrashGrabber", false);
//     }

//     void Update()
//     {
//         if (aliceController == null) aliceController = Object.FindFirstObjectByType<AliceController>();
//         if (dropController == null) dropController = Object.FindFirstObjectByType<DropController>();

//         if (Keyboard.current == null) return;

//         bool isAimingDumpster = CrosshairAim.currentTarget != null && CrosshairAim.currentTarget.CompareTag("Dumpster");

//         if (dropController != null)
//         {
//             dropController.enabled = !isAimingDumpster;
//         }

//         if (isAimingDumpster && trashGrabber != null && trashGrabber.isHeld && trashGrabber.currentTrash > 0)
//         {
//             if (Keyboard.current.gKey.wasPressedThisFrame)
//             {
//                 StartCoroutine(MoveAndDropRoutine());
//             }
//         }
//     }

//     private IEnumerator MoveAndDropRoutine()
//     {
//         if (aliceController != null)
//         {
//             CharacterController cc = aliceController.GetComponent<CharacterController>();

//             Vector3 worldTarget = aliceController.transform.parent != null
//                 ? aliceController.transform.parent.TransformPoint(targetPosition)
//                 : targetPosition;

//             Quaternion targetRot = Quaternion.Euler(targetRotation);

//             aliceController.isTeleporting = true;
//             if (cc != null) cc.enabled = false;

//             while (Vector3.Distance(aliceController.transform.position, worldTarget) > 0.01f)
//             {
//                 aliceController.PlayerWalk();
//                 aliceController.transform.position = Vector3.MoveTowards(aliceController.transform.position, worldTarget, moveSpeed * Time.deltaTime);
//                 aliceController.transform.localRotation = Quaternion.Lerp(aliceController.transform.localRotation, targetRot, moveSpeed * Time.deltaTime);
//                 yield return null;
//             }

//             aliceController.transform.position = worldTarget;
//             aliceController.transform.localRotation = targetRot;

//             if (cc != null) cc.enabled = true;
//             aliceController.isTeleporting = false;

//             animator.SetBool("DropTrashFromGrabber", true);
//             // grabberAnimator.SetBool("TrashGrabber", true);
//         }

//         yield return new WaitForSeconds(delayDestroy);

//         if (trashGrabber.grabberCollider != null)
//         {
//             foreach (Transform child in trashGrabber.grabberCollider.transform)
//             {
//                 if (child.CompareTag("Trash"))
//                 {
//                     Destroy(child.gameObject);
//                 }
//             }
//         }

//         trashGrabber.currentTrash = 0;

//         if (trashGrabber.trashCounterText != null)
//         {
//             trashGrabber.trashCounterText.text = trashGrabber.currentTrash + "/" + trashGrabber.maxTrash;
//         }
//     }
// }