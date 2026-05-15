using UnityEngine;
using UnityEngine.InputSystem;

public class PickupController : MonoBehaviour
{
    public Transform rightHandTransform;
    public Transform leftHandTransform;
    public Transform bothHandTransform;

    public Animator animator;

    GameObject rightHandItem;
    GameObject leftHandItem;
    GameObject bothHandItem;

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
            animator.SetTrigger("Pickup");
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

            bothHandItem = target;

            target.transform.SetParent(bothHandTransform);

            target.transform.localPosition = Vector3.zero;
            target.transform.localRotation = Quaternion.identity;

            ApplyHoldTransform(target, item.rightPositionOffset, item.rightRotationOffset, item.rightScaleOffset);

            return;
        }

        if (item.leftFirst)
        {
            if (leftHandItem == null)
            {
                leftHandItem = target;

                target.transform.SetParent(leftHandTransform);

                target.transform.localPosition = Vector3.zero;
                target.transform.localRotation = Quaternion.identity;

                ApplyHoldTransform(target, item.leftPositionOffset, item.leftRotationOffset, item.leftScaleOffset);

                return;
            }

            if (rightHandItem == null)
            {
                rightHandItem = target;

                target.transform.SetParent(rightHandTransform);

                target.transform.localPosition = Vector3.zero;
                target.transform.localRotation = Quaternion.identity;

                ApplyHoldTransform(target, item.rightPositionOffset, item.rightRotationOffset, item.rightScaleOffset);

                return;
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

                return;
            }

            if (leftHandItem == null)
            {
                leftHandItem = target;

                target.transform.SetParent(leftHandTransform);

                target.transform.localPosition = Vector3.zero;
                target.transform.localRotation = Quaternion.identity;

                ApplyHoldTransform(target, item.leftPositionOffset, item.leftRotationOffset, item.leftScaleOffset);

                return;
            }
        }

        Debug.Log("kedua tangan penuh");
    }
}