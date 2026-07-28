using UnityEngine;

public class InventoryItem : MonoBehaviour
{
    [System.Serializable]
    public class ItemData
    {
        public string itemName;
        public string description;
        public Sprite thumbnail;
        public Sprite cardImage;
        public Sprite cardImageHover;
        [HideInInspector] public GameObject worldObject;
    }

    public ItemData itemData;
}
