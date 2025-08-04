using UnityEngine;

public class WarehouseItem : MonoBehaviour
{
    [Header("Item Data")]
    public ItemData itemData;

    [Header("Visual")]
    public GameObject highlightEffect;

    private WarehouseManager warehouseManager;
    private OutlineHighlighter outliner;
    private bool isHighlighted = false;

    void Start()
    {
        outliner = GetComponent<OutlineHighlighter>();
        if (outliner == null)
        {
            outliner = gameObject.AddComponent<OutlineHighlighter>();
        }
    }

    public void Initialize(ItemData data, WarehouseManager manager)
    {
        itemData = data;
        warehouseManager = manager;

        // Item'ı pickable yap
        if (GetComponent<PickableItem>() == null)
        {
            PickableItem pickable = gameObject.AddComponent<PickableItem>();
            pickable.Initialize(itemData);
        }
    }

    void OnMouseEnter()
    {
        if (!isHighlighted)
        {
            Highlight();
        }
    }

    void OnMouseExit()
    {
        if (isHighlighted)
        {
            RemoveHighlight();
        }
    }

    private void Highlight()
    {
        if (outliner != null)
        {
            outliner.Highlight();
            isHighlighted = true;
        }
    }

    private void RemoveHighlight()
    {
        if (outliner != null)
        {
            outliner.RemoveHighlight();
            isHighlighted = false;
        }
    }

    public void OnPickedUp()
    {
        // Warehouse'dan çıkar
        if (warehouseManager != null)
        {
            warehouseManager.RemoveItemFromWarehouse(gameObject);
        }
    }
}