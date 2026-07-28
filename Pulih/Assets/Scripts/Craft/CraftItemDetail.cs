using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CraftItemDetail : MonoBehaviour
{
    public Image thumbnail;
    public TMP_Text itemName;
    public TMP_Text description;

    public void UpdateDetail(Sprite thumb, string name, string desc)
    {
        if (thumbnail != null) thumbnail.sprite = thumb;
        if (itemName != null) itemName.text = name;
        if (description != null) description.text = desc;
    }
}
