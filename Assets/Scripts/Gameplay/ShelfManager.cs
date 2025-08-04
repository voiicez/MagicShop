using UnityEngine;

public class ShelfManager : MonoBehaviour
{
    public ShelfSlot[] slots;


public ShelfSlot GetNearestAvailableSlot(Vector3 position)
    {
        ShelfSlot closest = null;
        float minDist = Mathf.Infinity;

        foreach (ShelfSlot slot in slots)
        {
            if (slot.IsOccupied) continue;

            float dist = Vector3.Distance(position, slot.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = slot;
            }
        }

        return closest;
    }

    // En yakın boş slota item yerleştirir ve yerleştirilen GameObject’i döndürür
    public GameObject PlaceItemAtNearestSlot(GameObject itemPrefab, ItemData itemData, Vector3 hitPoint)
    {
        ShelfSlot slot = GetNearestAvailableSlot(hitPoint);
        if (slot != null)
        {
            return slot.PlaceItem(itemPrefab, itemData);
        }
        return null;
    }
}