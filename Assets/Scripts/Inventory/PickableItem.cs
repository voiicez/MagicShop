using System;
using UnityEngine;

public class PickableItem : MonoBehaviour
{
    [Header("Item Settings")]
    public ItemData itemData;

[Header("Pickup Settings")]
public float pickupRange = 3f;

    private OutlineHighlighter outliner;

    void Start()
    {
        outliner = GetComponent<OutlineHighlighter>();
        if (outliner == null)
        {
            outliner = gameObject.AddComponent<OutlineHighlighter>();
        }
    }

    public void Initialize(ItemData data)
    {
        itemData = data;
    }

    public bool CanPickup(Transform player)
    {
        if (itemData == null) return false;

        float distance = Vector3.Distance(transform.position, player.position);
        return distance <= pickupRange;
    }

    public void Pickup(ItemPlacer itemPlacer)
    {
        if (itemPlacer == null || itemData == null) return;

        // Eğer bu nesne bir ShelfSlot’un child’ı ise, slotu boşalt
        ShelfSlot shelfSlot = transform.parent != null ? transform.parent.GetComponent<ShelfSlot>() : null;
        if (shelfSlot != null)
        {
            shelfSlot.DetachItem(gameObject);
        }

        itemPlacer.PickupItem(itemData, gameObject);
        Debug.Log($"{itemData.itemName} alındı!");
    }
}