using UnityEngine;

public class DropzoneManager : MonoBehaviour
{
    public DropzoneSlot[] slots;

    // ilk boş slota yerleştirme metodu
    public bool PlaceItemToFirstEmptySlot(GameObject itemPrefab)
    {
        foreach (var slot in slots)
        {
            if (!slot.IsOccupied)
            {
                slot.PlaceItem(itemPrefab);
                return true; // Yerleşti
            }
        }

        Debug.Log("Tüm slotlar dolu! Ürün yerleştirilemedi.");
        return false; // Yerleşemedi
    }

    // Tüm slotları temizlemek istersen kullanırsın
    public void ClearAllSlots()
    {
        foreach (var slot in slots)
        {
            slot.ClearSlot();
        }
    }
}
