using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryItemDetail : MonoBehaviour
{
    public Image thumbnail;
    public TMP_Text itemName;
    public TMP_Text description;
    public TMP_Text itemCount;

    public void UpdateDetail(Sprite thumb, string name, string desc, int count = 0)
    {
        if (thumbnail != null) thumbnail.sprite = thumb;
        if (itemName != null) itemName.text = name;
        if (description != null) description.text = desc;
        if (itemCount != null)
        {
            if (count > 0)
                itemCount.text = $"Item Count: {count}";
            else
                itemCount.text = "";
        }
    }

    public void UpdateDetail(Sprite thumb, string name, string desc)
    {
        UpdateDetail(thumb, name, desc, 0);
    }
}
