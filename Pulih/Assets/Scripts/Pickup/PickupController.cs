using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PickupController : MonoBehaviour
{
    public Transform rightHandTransform;
    public Transform leftHandTransform;
    public Transform bothHandTransform;

    public Animator animator;

    GameObject rightHandItem;
    GameObject leftHandItem;
    GameObject bothHandItem;

    AliceController aliceController;

    void Start()
    {
        aliceController = Object.FindFirstObjectByType <AliceController>();
    }

    void Update()
    {
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

    IEnumerator DelayedPickup(PickupItem item, GameObject target)
    {
        yield return new WaitForSeconds(item.delayPickup);

        if (item.useBothHands)
        {
            bothHandItem = target;

            target.transform.SetParent(bothHandTransform);

            target.transform.localPosition = Vector3.zero;
            target.transform.localRotation = Quaternion.identity;

            ApplyHoldTransform(target, item.rightPositionOffset, item.rightRotationOffset, item.rightScaleOffset);
        }
        else if (item.leftFirst)
        {
            if (leftHandItem == null)
            {
                leftHandItem = target;

                target.transform.SetParent(leftHandTransform);

                target.transform.localPosition = Vector3.zero;
                target.transform.localRotation = Quaternion.identity;

                ApplyHoldTransform(target, item.leftPositionOffset, item.leftRotationOffset, item.leftScaleOffset);
            }
            else if (rightHandItem == null)
            {
                rightHandItem = target;

                target.transform.SetParent(rightHandTransform);

                target.transform.localPosition = Vector3.zero;
                target.transform.localRotation = Quaternion.identity;

                ApplyHoldTransform(target, item.rightPositionOffset, item.rightRotationOffset, item.rightScaleOffset);
            }
        }
        else
        {
            if (rightHandItem == null)
            {
                rightHandItem = target;

                target.transform.SetParent(rightHandTransform);

                target.transform.localPosition = Vector3.zero;
                target.transform.localRotation = Quaternion.identity;

                ApplyHoldTransform(target, item.rightPositionOffset, item.rightRotationOffset, item.rightScaleOffset);
            }
            else if (leftHandItem == null)
            {
                leftHandItem = target;

                target.transform.SetParent(leftHandTransform);

                target.transform.localPosition = Vector3.zero;
                target.transform.localRotation = Quaternion.identity;

                ApplyHoldTransform(target, item.leftPositionOffset, item.leftRotationOffset, item.leftScaleOffset);
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
        }

        if (aliceController != null)
        {
            aliceController.enabled = true;
        }
    }

    void PickupObject()
    {
        if (CrosshairAim.currentTarget == null)
            return;

        PickupItem item =
            CrosshairAim.currentTarget.GetComponent<PickupItem>();

        if (item == null)
            return;

        GameObject target = CrosshairAim.currentTarget;

        Rigidbody rb = target.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.isKinematic = true;
        }

        if (animator != null)
        {
            string triggerName = string.IsNullOrEmpty(item.animationTrigger)
                ? "Pickup"
                : item.animationTrigger;

            if (aliceController != null)
            {
                aliceController.enabled = false;
            }

            animator.SetTrigger(triggerName);
        }

        if (item.useBothHands)
        {
            if (
                rightHandItem != null ||
                leftHandItem != null ||
                bothHandItem != null
            )
            {
                Debug.Log("tidak dapat diambil");
                return;
            }

            StartCoroutine(DelayedPickup(item, target));
            return;
        }

        if (item.leftFirst)
        {
            if (leftHandItem == null || rightHandItem == null)
            {
                StartCoroutine(DelayedPickup(item, target));
                return;
            }
        }
        else
        {
            if (rightHandItem == null || leftHandItem == null)
            {
                StartCoroutine(DelayedPickup(item, target));
                return;
            }
        }

        Debug.Log("kedua tangan penuh");
    }
}