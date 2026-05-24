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

    [Header("Pickup Range Settings")]
    public float pickupDistance = 1.5f; 
    public float approachSpeed = 2f;    
    public float rotationSpeed = 10f;   

    [HideInInspector] public float targetBothWeight = 1f;
    [HideInInspector] public float targetLeftWeight = 0f;

    [HideInInspector] public GameObject rightHandItem;
    [HideInInspector] public GameObject leftHandItem;
    [HideInInspector] public GameObject bothHandItem;

    AliceController aliceController;
    CharacterController playerController;
    bool isPickupPlaying;
    bool isApproaching;

    void Start()
    {
        aliceController = Object.FindFirstObjectByType<AliceController>();
        playerController = GetComponent<CharacterController>();

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

        if (col != null)
            col.enabled = false;

        if (item.useBothHands)
        {
            AttachItem(ref bothHandItem, target, bothHandTransform, item.rightPositionOffset, item.rightRotationOffset, item.rightScaleOffset);

            if (animator != null && !string.IsNullOrEmpty(item.holdAnimation))
            {
                animator.SetBool(item.holdAnimation, true);
            }
            else
            {
                animator.SetBool(item.holdAnimation, false);
            }
        }
        else if (item.leftFirst)
        {
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

    IEnumerator ApproachAndPickup(PickupItem item, GameObject target)
    {
        isApproaching = true;

        if (aliceController != null)
        {
            aliceController.enabled = false;      
        }

        Vector3 flatTargetPos = new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z);
        float distance = Vector3.Distance(transform.position, flatTargetPos);

        while (distance > pickupDistance)
        {
            Vector3 direction = (flatTargetPos - transform.position).normalized;

            if (direction.magnitude > 0.1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            }

            if (playerController != null)
            {
                playerController.Move(direction * approachSpeed * Time.deltaTime);
            }

            if (animator != null)
            {
                animator.SetFloat("Move", 0.5f, 0.1f, Time.deltaTime);
            }

            distance = Vector3.Distance(transform.position, flatTargetPos);
            yield return null;
        }

        if (animator != null)
        {
            animator.SetFloat("Move", 0f, 0.1f, Time.deltaTime);
        }

        ExecutePickupAnimation(item, target);
        isApproaching = false;
    }

    void ExecutePickupAnimation(PickupItem item, GameObject target)
    {
        Rigidbody rb = target.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
            rb.interpolation = RigidbodyInterpolation.None;
        }

        if (animator != null)
        {
            string triggerName = string.IsNullOrEmpty(item.pickupAnimation) ? "Pickup" : item.pickupAnimation;
            animator.SetTrigger(triggerName);
        }

        StartCoroutine(DelayedPickup(item, target));
    }

    void PickupObject()
    {
        if (CrosshairAim.currentTarget == null)
            return;

        if (isPickupPlaying || isApproaching)
            return;

        PickupItem item = CrosshairAim.currentTarget.GetComponent<PickupItem>();

        if (item == null)
            return;

        GameObject target = CrosshairAim.currentTarget;

        bool isFull =
            (item.useBothHands && (rightHandItem != null || leftHandItem != null || bothHandItem != null)) ||
            (!item.useBothHands && item.leftFirst && (leftHandItem != null && rightHandItem != null)) ||
            (!item.useBothHands && !item.leftFirst && (rightHandItem != null && leftHandItem != null));

        if (isFull)
        {
            Debug.Log("kedua tangan penuh");
            return;
        }

        Vector3 flatTargetPos = new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z);
        float distance = Vector3.Distance(transform.position, flatTargetPos);

        if (distance > pickupDistance)
        {
            StartCoroutine(ApproachAndPickup(item, target));
        }
        else
        {
            if (aliceController != null)
            {
                aliceController.enabled = false;
            }
            ExecutePickupAnimation(item, target);
        }
    }
}