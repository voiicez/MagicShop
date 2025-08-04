using UnityEngine;
using System.Collections.Generic;

public class WarehouseManager : MonoBehaviour
{
    [Header("Warehouse Settings")]
    public Transform warehouseSpawnArea;
    public Vector3 spawnAreaSize = new Vector3(5f, 0f, 5f);
    public float itemSpacing = 1.5f;

    [Header("Visual")]
    public bool showWarehouseArea = true;

    private List<GameObject> warehouseItems = new List<GameObject>();
    private int nextSpawnIndex = 0;

    void Start()
    {
        if (warehouseSpawnArea == null)
        {
            Debug.LogError("Warehouse spawn area tanımlanmamış!");
        }
    }

    public void AddItemToWarehouse(ItemData itemData)
    {
        if (itemData == null || itemData.prefab == null)
        {
            Debug.LogError("Item data veya prefab null!");
            return;
        }

        Vector3 spawnPosition = GetNextSpawnPosition();
        GameObject spawnedItem = Instantiate(itemData.prefab, spawnPosition, Quaternion.identity);

        // Layer'ı güvenli şekilde ata
        int pickableLayer = GetSafeLayer("Pickable", 0);
        spawnedItem.layer = pickableLayer;

        // Child object'lerin layer'ını da ayarla
        SetLayerRecursively(spawnedItem, pickableLayer);

        // WarehouseItem component'i ekle
        WarehouseItem warehouseItem = spawnedItem.GetComponent<WarehouseItem>();
        if (warehouseItem == null)
        {
            warehouseItem = spawnedItem.AddComponent<WarehouseItem>();
        }

        warehouseItem.Initialize(itemData, this);
        warehouseItems.Add(spawnedItem);

        Debug.Log($"{itemData.itemName} depoya eklendi! Layer: {LayerMask.LayerToName(pickableLayer)} (index: {pickableLayer})");
    }

    public void RemoveItemFromWarehouse(GameObject item)
    {
        if (warehouseItems.Contains(item))
        {
            warehouseItems.Remove(item);
            Debug.Log($"Item depodan çıkarıldı: {item.name}");
        }
    }

    private Vector3 GetNextSpawnPosition()
    {
        if (warehouseSpawnArea == null)
            return Vector3.zero;

        // Grid sistemine göre pozisyon hesapla
        int itemsPerRow = Mathf.FloorToInt(spawnAreaSize.x / itemSpacing);
        if (itemsPerRow <= 0) itemsPerRow = 1;

        int row = nextSpawnIndex / itemsPerRow;
        int col = nextSpawnIndex % itemsPerRow;

        Vector3 localPos = new Vector3(
            (col * itemSpacing) - (spawnAreaSize.x / 2f) + (itemSpacing / 2f),
            0.5f, // Y offset
            (row * itemSpacing) - (spawnAreaSize.z / 2f) + (itemSpacing / 2f)
        );

        nextSpawnIndex++;
        return warehouseSpawnArea.position + localPos;
    }

    // Güvenli layer alma
    private int GetSafeLayer(string layerName, int defaultLayer)
    {
        int layer = LayerMask.NameToLayer(layerName);

        if (layer == -1)
        {
            Debug.LogWarning($"Layer '{layerName}' bulunamadı! Default layer ({defaultLayer}) kullanılıyor.");
            return defaultLayer;
        }

        return layer;
    }

    // Recursive layer setting
    private void SetLayerRecursively(GameObject obj, int layer)
    {
        if (obj == null) return;

        obj.layer = layer;

        foreach (Transform child in obj.transform)
        {
            if (child != null)
            {
                SetLayerRecursively(child.gameObject, layer);
            }
        }
    }

    void OnDrawGizmos()
    {
        if (showWarehouseArea && warehouseSpawnArea != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(warehouseSpawnArea.position, spawnAreaSize);

            // Grid çizgileri
            Gizmos.color = Color.green;
            int itemsPerRow = Mathf.FloorToInt(spawnAreaSize.x / itemSpacing);
            if (itemsPerRow > 0)
            {
                for (int i = 0; i < itemsPerRow; i++)
                {
                    for (int j = 0; j < 3; j++) // 3 sıra göster
                    {
                        Vector3 pos = GetSpawnPositionAt(i, j);
                        Gizmos.DrawWireCube(pos, Vector3.one * 0.5f);
                    }
                }
            }
        }
    }

    private Vector3 GetSpawnPositionAt(int col, int row)
    {
        if (warehouseSpawnArea == null) return Vector3.zero;

        Vector3 localPos = new Vector3(
            (col * itemSpacing) - (spawnAreaSize.x / 2f) + (itemSpacing / 2f),
            0.5f,
            (row * itemSpacing) - (spawnAreaSize.z / 2f) + (itemSpacing / 2f)
        );

        return warehouseSpawnArea.position + localPos;
    }
}