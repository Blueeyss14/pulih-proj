using UnityEngine;

public class PickupItem : MonoBehaviour
{

    ///Dell: kalau useBothHand nya true yg dipake cuma Right aja bg, sengaja bikin gini biar gampang ntar nyesuain rotasi beberapa item
    public bool useBothHands;

    [Header("Right Hand / Both Hand Offset")]
    public Vector3 rightPositionOffset = Vector3.zero;
    public Vector3 rightRotationOffset = Vector3.zero;
    public Vector3 rightScaleOffset = Vector3.zero;

    [Header("Left Hand Offset")]
    public Vector3 leftPositionOffset = Vector3.zero;
    public Vector3 leftRotationOffset = Vector3.zero;
    public Vector3 leftScaleOffset = Vector3.zero;
}