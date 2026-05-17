    using UnityEngine;

    public class DropItem : MonoBehaviour
    {
        [Header("Animation")]
        public string dropAnimation = "Drop";
        public float delayDrop = 0.3f;

        [Header("Throw Settings")]
        public float throwForce = 3f;
        public float throwUpwardForce = 2f;
    }