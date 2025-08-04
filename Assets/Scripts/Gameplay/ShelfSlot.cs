using UnityEngine;

public class ShelfSlot : MonoBehaviour
{
    // Slotta çocuk nesne varsa dolu kabul edilir
    public bool IsOccupied => transform.childCount > 0;

// Item’ı bu slota instantiate eder ve gerekli bileşenleri ekler
public GameObject PlaceItem(GameObject itemPrefab, ItemData itemData)
    {
        // Nesneyi slotun child’ı olarak oluştur
        GameObject item = Instantiate(itemPrefab, transform.position, transform.rotation, transform);

        // Layer’ı “Sellable” olarak ayarla (varsa)
        int sellableLayer = LayerMask.NameToLayer("Sellable");
        if (sellableLayer != -1)
        {
            item.layer = sellableLayer;
        }

        // SellableItem bileşeni ekle veya güncelle
        SellableItem sellable = item.GetComponent<SellableItem>();
        if (sellable == null) sellable = item.AddComponent<SellableItem>();
        sellable.itemData = itemData;

        // PickableItem bileşeni ekle veya güncelle
        PickableItem pickable = item.GetComponent<PickableItem>();
        if (pickable == null) pickable = item.AddComponent<PickableItem>();
        pickable.Initialize(itemData);

        return item;
    }

    // Slotu tamamen temizler
    public void ClearSlot()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    // Raftan item alınırken slotu boşaltmak için kullanılır; nesneyi slotun child’ı olmaktan çıkarır
    public void DetachItem(GameObject item)
    {
        if (item != null && item.transform.parent == transform)
        {
            item.transform.SetParent(null, true);
        }
    }
}