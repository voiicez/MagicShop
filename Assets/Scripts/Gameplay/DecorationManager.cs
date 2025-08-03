using UnityEngine;

public class DecorationManager : MonoBehaviour
{
    [Header("References")]
    public PlayerWallet playerWallet;

    [Header("Decoration Settings")]
    public Transform decorationSpawnPoint;
    public float spawnOffset = 2f;

    void Start()
    {
        if (playerWallet == null)
        {
            playerWallet = FindObjectOfType<PlayerWallet>();
        }

        if (decorationSpawnPoint == null)
        {
            // Varsay�lan spawn point olu�tur
            GameObject spawnPoint = new GameObject("DecorationSpawnPoint");
            spawnPoint.transform.parent = transform;
            spawnPoint.transform.localPosition = Vector3.forward * spawnOffset;
            decorationSpawnPoint = spawnPoint.transform;
        }
    }

    public void PurchaseDecor(DecorativeItemData decorItem)
    {
        if (decorItem == null)
        {
            Debug.LogError("DecorativeItemData null!");
            return;
        }

        if (playerWallet == null)
        {
            Debug.LogError("PlayerWallet referans� bulunamad�!");
            return;
        }

        // Gold kontrol� ve harcama
        if (playerWallet.SpendGold(decorItem.cost))
        {
            // Decorasyon sat�n al�nd�, spawn et
            SpawnDecoration(decorItem);
        }
        else
        {
            Debug.Log($"Yetersiz gold! Gerekli: {decorItem.cost}, Mevcut: {playerWallet.currentGold}");
        }
    }

    private void SpawnDecoration(DecorativeItemData decorItem)
    {
        if (decorItem.prefab == null)
        {
            Debug.LogError($"{decorItem.itemName} i�in prefab atanmam��!");
            return;
        }

        Vector3 spawnPosition = decorationSpawnPoint.position;
        GameObject spawnedDecor = Instantiate(decorItem.prefab, spawnPosition, Quaternion.identity);

        // DecorInteraction component'i ekle
        DecorInteraction decorInteraction = spawnedDecor.GetComponent<DecorInteraction>();
        if (decorInteraction == null)
        {
            decorInteraction = spawnedDecor.AddComponent<DecorInteraction>();
        }

        // PickableItem component'i ekle (oyuncu alabilsin)
        PickableItem pickableItem = spawnedDecor.GetComponent<PickableItem>();
        if (pickableItem == null)
        {
            pickableItem = spawnedDecor.AddComponent<PickableItem>();
        }

        // ItemData olu�tur ve ata (DecorativeItemData'dan ItemData'ya �evir)
        ItemData itemData = CreateItemDataFromDecor(decorItem);
        pickableItem.Initialize(itemData);

        Debug.Log($"{decorItem.itemName} sat�n al�nd� ve spawn edildi!");
    }

    private ItemData CreateItemDataFromDecor(DecorativeItemData decorItem)
    {
        // Runtime'da ItemData olu�tur
        ItemData itemData = ScriptableObject.CreateInstance<ItemData>();
        itemData.itemName = decorItem.itemName;
        itemData.price = decorItem.cost;
        itemData.icon = decorItem.icon;
        itemData.prefab = decorItem.prefab;
        itemData.itemType = ItemType.Decorative;

        return itemData;
    }

    // UI Button'lar�ndan �a�r�labilir
    public void PurchaseDecorByIndex(int index)
    {
        // E�er decorItem listesi varsa index ile sat�n al
        // Bu metod UI button'lar�ndan kullan�labilir
        Debug.Log($"Decor index {index} sat�n al�nmaya �al���l�yor...");
    }
}