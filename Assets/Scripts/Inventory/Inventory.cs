using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public List<ItemData> items = new List<ItemData>();
    public ItemData testItem; // Inspector’dan atanacak

    private void Start()
    {
        if (testItem != null)
        {
            AddItem(testItem);
        }
    }

    public void AddItem(ItemData item)
    {
        items.Add(item);
        Debug.Log("Item added: " + item.itemName);
    }

    public void RemoveItem(ItemData item)
    {
        items.Remove(item);
        Debug.Log("Item removed: " + item.itemName);
    }
}
