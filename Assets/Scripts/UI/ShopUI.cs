using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ShopUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject shopPanel;
    public Transform shopItemContainer;
    public GameObject shopItemPrefab;

    [Header("Shop Items")]
    public List<ItemData> shopItems = new List<ItemData>();

    [Header("References")]
    public PlayerWallet playerWallet;
    public DropzoneManager dropzoneManager; // bu satırı class başında tanımla

    private bool isShopOpen = false;
    private bool itemsSetup = false;

    void Start()
    {
        Debug.Log("ShopUI Start çalışıyor...");

        if (playerWallet == null)
        {
            playerWallet = FindObjectOfType<PlayerWallet>();
            Debug.Log(playerWallet != null ? "PlayerWallet bulundu" : "PlayerWallet bulunamadı!");
        }

       

        // Referans kontrolleri
        Debug.Log($"shopPanel: {(shopPanel != null ? "OK" : "NULL")}");
        Debug.Log($"shopItemContainer: {(shopItemContainer != null ? "OK" : "NULL")}");
        Debug.Log($"shopItemPrefab: {(shopItemPrefab != null ? "OK" : "NULL")}");
        Debug.Log($"shopItems count: {shopItems.Count}");

        // Shop panel'i kapat ama item'ları setup etme
        if (shopPanel != null)
        {
            shopPanel.SetActive(false);
        }
        else
        {
            Debug.LogError("ShopPanel referansı NULL! Inspector'dan atayın.");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            Debug.Log("B tuşuna basıldı!");
            ToggleShop();
        }

        if (Input.GetKeyDown(KeyCode.Escape) && isShopOpen)
        {
            CloseShop();
        }
    }

    void SetupShopItems()
    {
        Debug.Log("SetupShopItems başladı...");

        if (shopItemContainer == null)
        {
            Debug.LogError("shopItemContainer NULL! Inspector'dan atayın.");
            return;
        }

        if (shopItemPrefab == null)
        {
            Debug.LogError("shopItemPrefab NULL! Inspector'dan atayın.");
            return;
        }

        // Mevcut item'ları temizle
        int childCount = shopItemContainer.childCount;
        Debug.Log($"Container'da {childCount} child var, temizleniyor...");

        for (int i = childCount - 1; i >= 0; i--)
        {
            if (Application.isPlaying)
            {
                Destroy(shopItemContainer.GetChild(i).gameObject);
            }
            else
            {
                DestroyImmediate(shopItemContainer.GetChild(i).gameObject);
            }
        }

        // Shop items kontrolü
        if (shopItems.Count == 0)
        {
            Debug.LogWarning("shopItems listesi boş! Test item'ları ekleniyor...");
            CreateTestItems();
        }

        // Yeni item'ları oluştur
        Debug.Log($"{shopItems.Count} item oluşturuluyor...");
        foreach (ItemData item in shopItems)
        {
            if (item != null)
            {
                CreateShopItemUI(item);
            }
            else
            {
                Debug.LogWarning("shopItems listesinde NULL item var!");
            }
        }

        // Layout'u zorla güncelle
        Canvas.ForceUpdateCanvases();
        if (shopItemContainer.GetComponent<RectTransform>() != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(shopItemContainer.GetComponent<RectTransform>());
        }

        Debug.Log("SetupShopItems tamamlandı!");
        itemsSetup = true;

        // DEBUG: Container durumu kontrol
        Debug.Log($"Final container child count: {shopItemContainer.childCount}");
    }

    void CreateTestItems()
    {
        // Test için basit item'lar oluştur
        for (int i = 0; i < 3; i++)
        {
            ItemData testItem = ScriptableObject.CreateInstance<ItemData>();
            testItem.itemName = $"Test Item {i + 1}";
            testItem.price = (i + 1) * 10;
            testItem.itemType = ItemType.Sellable;

            shopItems.Add(testItem);
        }

        Debug.Log("Test item'ları oluşturuldu!");
    }

    void CreateShopItemUI(ItemData item)
    {
        Debug.Log($"Item UI oluşturuluyor: {item.itemName}");

        GameObject itemUI = Instantiate(shopItemPrefab, shopItemContainer);
        itemUI.name = $"ShopItem_{item.itemName}";

        // Item'ı aktif yap
        itemUI.SetActive(true);

        ShopItemUI shopItemUI = itemUI.GetComponent<ShopItemUI>();

        if (shopItemUI != null)
        {
            shopItemUI.SetupItem(item, this);
            Debug.Log($"Item UI başarıyla kuruldu: {item.itemName}");
        }
        else
        {
            Debug.LogError($"ShopItemUI component bulunamadı! Prefab'da ShopItemUI script'i var mı?");
        }

        // DEBUG: Item pozisyon bilgisi
        RectTransform itemRect = itemUI.GetComponent<RectTransform>();
        Debug.Log($"Item {item.itemName} position: {itemRect.anchoredPosition}, size: {itemRect.sizeDelta}");
    }

   

    public void PurchaseItem(ItemData item)
    {
        Debug.Log($"Satın alma isteği: {item.itemName}, Fiyat: {item.price}");

        if (playerWallet != null && playerWallet.SpendGold(item.price))
        {
            Debug.Log($"{item.itemName} satın alındı!");

            // Dropzone’a yerleştir
            if (dropzoneManager != null)
            {
                bool success = dropzoneManager.PlaceItemToFirstEmptySlot(item.prefab);
                if (!success)
                {
                    Debug.LogWarning("Dropzone dolu! Ürün yerleştirilemedi.");
                }
            }
            else
            {
                Debug.LogWarning("DropzoneManager referansı atanmadı!");
            }
        }
        else
        {
            Debug.Log("Yetersiz gold!");
        }
    }


    public void ToggleShop()
    {
        Debug.Log($"Shop toggle - Current state: {isShopOpen}");

        isShopOpen = !isShopOpen;

        if (shopPanel != null)
        {
            shopPanel.SetActive(isShopOpen);
            Debug.Log($"Shop panel set to: {isShopOpen}");

            // Shop ilk açıldığında item'ları setup et
            if (isShopOpen && !itemsSetup)
            {
                Debug.Log("İlk kez açılıyor, item'lar setup ediliyor...");
                SetupShopItems();
            }
        }

        // Cursor'u unlock/lock yap
        if (isShopOpen)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Debug.Log("Cursor unlocked");

            // Layout'u tekrar güncelle
            if (itemsSetup)
            {
                Canvas.ForceUpdateCanvases();
                if (shopItemContainer.GetComponent<RectTransform>() != null)
                {
                    LayoutRebuilder.ForceRebuildLayoutImmediate(shopItemContainer.GetComponent<RectTransform>());
                }
            }
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Debug.Log("Cursor locked");
        }
    }

    public void CloseShop()
    {
        Debug.Log("Shop kapatılıyor...");

        isShopOpen = false;
        if (shopPanel != null)
        {
            shopPanel.SetActive(false);
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}