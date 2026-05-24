using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PickupController : MonoBehaviour
{
    public Transform rightHandTransform;
    public Transform leftHandTransform;
    public Transform bothHandTransform;

    public Animator animator;

    public float layerBlendSpeed = 5f;

    [Header("Auto Walk Settings")]
    public float pickupRange = 1.5f;
    public float autoWalkSpeed = 2f;

    [HideInInspector] public float targetBothWeight = 1f;
    [HideInInspector] public float targetLeftWeight = 0f;

    [HideInInspector] public GameObject rightHandItem;
    [HideInInspector] public GameObject leftHandItem;
    [HideInInspector] public GameObject bothHandItem;

    [HideInInspector] public string overridePickupAnimation = "";

    AliceController aliceController;

    bool isPickupPlaying;

    void Start()
    {
        aliceController = Object.FindFirstObjectByType<AliceController>();

        if (animator != null)
        {
            animator.SetLayerWeight(2, 1f);
            animator.SetLayerWeight(3, 0f);
        }
    }

    void Update()
    {
        if (animator != null)
        {
            float bothWeight = animator.GetLayerWeight(2);
            float leftWeight = animator.GetLayerWeight(3);

            float smoothBothWeight = Mathf.Lerp(bothWeight, targetBothWeight, Time.deltaTime * layerBlendSpeed);
            float smoothLeftWeight = Mathf.Lerp(leftWeight, targetLeftWeight, Time.deltaTime * layerBlendSpeed);

            animator.SetLayerWeight(2, smoothBothWeight);
            animator.SetLayerWeight(3, smoothLeftWeight);
        }

        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            PickupObject();
        }
    }

    void ApplyHoldTransform(GameObject target, Vector3 posOffset, Vector3 rotOffset, Vector3 scaleOffset)
    {
        target.transform.localPosition = posOffset;
        target.transform.localRotation = Quaternion.Euler(rotOffset);

        if (scaleOffset != Vector3.zero)
            target.transform.localScale = scaleOffset;
    }

    void AttachItem(ref GameObject handItem, GameObject target, Transform handTransform, Vector3 positionOffset, Vector3 rotationOffset, Vector3 scaleOffset)
    {
        handItem = target;

        target.transform.SetParent(handTransform);

        target.transform.localPosition = Vector3.zero;
        target.transform.localRotation = Quaternion.identity;

        ApplyHoldTransform(target, positionOffset, rotationOffset, scaleOffset);
    }

    IEnumerator DelayedPickup(PickupItem item, GameObject target)
    {
        isPickupPlaying = true;

        targetBothWeight = 0f;
        targetLeftWeight = 1f;

        yield return new WaitForSeconds(item.delayPickup);

        Collider col = target.GetComponent<Collider>();
        if (col != null) col.enabled = false;

        if (item.useBothHands)
        {
            AttachItem(ref bothHandItem, target, bothHandTransform, item.rightPositionOffset, item.rightRotationOffset, item.rightScaleOffset);
        }
        else if (item.leftFirst)
        {
            // animator.SetBool(item.holdAnimation, false);
            if (leftHandItem == null)
            {
                AttachItem(ref leftHandItem, target, leftHandTransform, item.leftPositionOffset, item.leftRotationOffset, item.leftScaleOffset);
            }
            else if (rightHandItem == null)
            {
                AttachItem(ref rightHandItem, target, rightHandTransform, item.rightPositionOffset, item.rightRotationOffset, item.rightScaleOffset);
            }
        }
        else
        {
            // animator.SetBool(item.holdAnimation, false);
            if (rightHandItem == null)
            {
                AttachItem(ref rightHandItem, target, rightHandTransform, item.rightPositionOffset, item.rightRotationOffset, item.rightScaleOffset);
            }
            else if (leftHandItem == null)
            {
                AttachItem(ref leftHandItem, target, leftHandTransform, item.leftPositionOffset, item.leftRotationOffset, item.leftScaleOffset);
            }
        }

        if (animator != null)
        {
            if (!string.IsNullOrEmpty(item.holdAnimation))
            {
                // animator.SetBool(item.holdAnimation, true);
                animator.SetTrigger(item.holdAnimation);
            }
            else
            {
                animator.SetTrigger("EmptyHold");
            }
        }

        if (animator != null)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(1);

            while (stateInfo.normalizedTime < 1f)
            {
                stateInfo = animator.GetCurrentAnimatorStateInfo(1);
                yield return null;
            }

            targetBothWeight = 1f;
            targetLeftWeight = 0f;
        }

        isPickupPlaying = false;

        if (aliceController != null)
        {
            aliceController.enabled = true;
        }
    }

    void PickupObject()
    {
        if (CrosshairAim.currentTarget == null)
            return;

        if (isPickupPlaying)
            return;

        PickupItem item = CrosshairAim.currentTarget.GetComponent<PickupItem>();

        if (item == null)
            return;

        GameObject target = CrosshairAim.currentTarget;

        Rigidbody rb = target.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
            rb.interpolation = RigidbodyInterpolation.None;
        }

        bool isFull =
            (item.useBothHands && (rightHandItem != null || leftHandItem != null || bothHandItem != null)) ||
            (!item.useBothHands && item.leftFirst && (leftHandItem != null && rightHandItem != null)) ||
            (!item.useBothHands && !item.leftFirst && (rightHandItem != null && leftHandItem != null));

        if (isFull)
        {
            Debug.Log("kedua tangan penuh");
            return;
        }

        StartCoroutine(WalkAndPickup(item, target));
    }

    IEnumerator WalkAndPickup(PickupItem item, GameObject target)
    {
        isPickupPlaying = true;

        if (aliceController == null)
        {
            isPickupPlaying = false;
            yield break;
        }

        aliceController.enabled = false;

        CharacterController cc = aliceController.GetComponent<CharacterController>();

        Vector3 playerPos = aliceController.transform.position;
        Vector3 targetPos = target.transform.position;
        float dist = Vector3.Distance(new Vector3(playerPos.x, 0f, playerPos.z), new Vector3(targetPos.x, 0f, targetPos.z));

        while (target != null && dist > pickupRange)
        {
            Vector3 direction = target.transform.position - aliceController.transform.position;
            direction.y = 0f;

            if (direction.sqrMagnitude > 0.001f)
            {
                aliceController.transform.rotation = Quaternion.LookRotation(direction);
            }

            aliceController.PlayerWalk();

            Vector3 moveVec = direction.normalized * autoWalkSpeed;

            if (cc != null)
            {
                cc.SimpleMove(moveVec);
            }
            else
            {
                aliceController.transform.position += moveVec * Time.deltaTime;
            }

            playerPos = aliceController.transform.position;
            dist = Vector3.Distance(new Vector3(playerPos.x, 0f, playerPos.z), new Vector3(targetPos.x, 0f, targetPos.z));

            yield return null;
        }

        if (target == null)
        {
            if (animator != null)
            {
                animator.SetFloat("Move", 0f);
            }
            aliceController.enabled = true;
            isPickupPlaying = false;
            yield break;
        }

        if (animator != null)
        {
            animator.SetFloat("Move", 0f);

            string triggerName = !string.IsNullOrEmpty(overridePickupAnimation)
                ? overridePickupAnimation
                : string.IsNullOrEmpty(item.pickupAnimation)
                    ? "Pickup"
                    : item.pickupAnimation;

            animator.SetTrigger(triggerName);
        }

        StartCoroutine(RotatePlayerToTarget(aliceController.transform, target.transform.position, 0.2f));
        StartCoroutine(DelayedPickup(item, target));
    }

    IEnumerator RotatePlayerToTarget(Transform playerTransform, Vector3 targetPosition, float duration)
    {
        float time = 0f;
        Quaternion startRotation = playerTransform.rotation;
        Vector3 direction = targetPosition - playerTransform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            while (time < duration)
            {
                playerTransform.rotation = Quaternion.Slerp(startRotation, targetRotation, time / duration);
                time += Time.deltaTime;
                yield return null;
            }
            playerTransform.rotation = targetRotation;
        }
    }
}