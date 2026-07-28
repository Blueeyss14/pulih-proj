using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class InventoryController : MonoBehaviour
{
    [Header("Inventory Data")]
    public List<InventoryItem.ItemData> savedItems = new List<InventoryItem.ItemData>();

    [Header("UI Spawn")]
    public GameObject itemPrefab;
    public Transform content;
    public InventoryItemDetail itemDetail;

    private Image selectedBg;
    private Sprite selectedNormalSprite;
    private bool isFirstItem = true;

    public void AddItemToInventory(InventoryItem.ItemData newItem)
    {
        savedItems.Add(newItem);
        SpawnCard(newItem, isFirstItem);
        isFirstItem = false;
    }

    void SpawnCard(InventoryItem.ItemData item, bool isFirst)
    {
        GameObject card = Instantiate(itemPrefab, content);

        Image cardBg = card.GetComponent<Image>();
        Image thumb = card.transform.Find("Thumbnail")?.GetComponent<Image>();
        TMP_Text nameText = card.transform.Find("Item Name")?.GetComponent<TMP_Text>();

        if (cardBg != null) cardBg.sprite = item.cardImage;
        if (thumb != null) thumb.sprite = item.thumbnail;
        if (nameText != null) nameText.text = item.itemName;

        Button btn = card.GetComponent<Button>();
        if (btn != null) btn.transition = Selectable.Transition.None;

        EventTrigger trigger = card.AddComponent<EventTrigger>();
        Sprite normal = item.cardImage;
        Sprite hover = item.cardImageHover;
        InventoryItem.ItemData captured = item;

        if (isFirst)
        {
            selectedBg = cardBg;
            selectedNormalSprite = normal;
            if (cardBg) cardBg.sprite = hover;
            itemDetail?.UpdateDetail(captured.thumbnail, captured.itemName, captured.description);
        }

        AddTrigger(trigger, EventTriggerType.PointerEnter, _ => { 
            if (cardBg && selectedBg != cardBg) cardBg.sprite = hover; 
        });
        
        AddTrigger(trigger, EventTriggerType.PointerExit,  _ => { 
            if (cardBg && selectedBg != cardBg) cardBg.sprite = normal; 
        });
        
        AddTrigger(trigger, EventTriggerType.PointerClick, _ => {
            if (selectedBg != null && selectedBg != cardBg)
                selectedBg.sprite = selectedNormalSprite;
            
            selectedBg = cardBg;
            selectedNormalSprite = normal;
            if (cardBg) cardBg.sprite = hover;

            itemDetail?.UpdateDetail(captured.thumbnail, captured.itemName, captured.description);
        });
    }

    void AddTrigger(EventTrigger trigger, EventTriggerType type, UnityEngine.Events.UnityAction<BaseEventData> action)
    {
        var entry = new EventTrigger.Entry { eventID = type };
        entry.callback.AddListener(action);
        trigger.triggers.Add(entry);
    }
}
