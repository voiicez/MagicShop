using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItemUI : MonoBehaviour
{
    [Header("UI Components")]
    public Image itemIcon;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemPriceText;
    public Button purchaseButton;

    private ItemData itemData;
    private ShopUI shopUI;

    void Start()
    {
        Debug.Log($"ShopItemUI Start - {gameObject.name}");

        // Component kontrolleri
        Debug.Log($"itemIcon: {(itemIcon != null ? "OK" : "NULL")}");
        Debug.Log($"itemNameText: {(itemNameText != null ? "OK" : "NULL")}");
        Debug.Log($"itemPriceText: {(itemPriceText != null ? "OK" : "NULL")}");
        Debug.Log($"purchaseButton: {(purchaseButton != null ? "OK" : "NULL")}");

        // Eğer button null ise otomatik bul
        if (purchaseButton == null)
        {
            purchaseButton = GetComponent<Button>();
            Debug.Log($"Button otomatik bulundu: {purchaseButton != null}");
        }

        // ZORLA BOYUT AYARLA
        RectTransform rect = GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(700, 80);
        Debug.Log($"Boyut zorla ayarlandı: {rect.sizeDelta}");

        // TEXT'LERİ BÜYÜT
        if (itemNameText != null)
        {
            itemNameText.fontSize = 24;
            itemNameText.color = Color.black;
        }

        if (itemPriceText != null)
        {
            itemPriceText.fontSize = 20;
            itemPriceText.color = Color.yellow;
        }
    }

    public void SetupItem(ItemData item, ShopUI shop)
    {
        Debug.Log($"SetupItem çağrıldı: {item?.itemName ?? "NULL"}");

        itemData = item;
        shopUI = shop;

        if (item == null)
        {
            Debug.LogError("SetupItem'a NULL ItemData gönderildi!");
            return;
        }

        // UI'yi güncelle
        if (itemIcon != null && item.icon != null)
        {
            itemIcon.sprite = item.icon;
            Debug.Log($"Icon set edildi: {item.icon.name}");
        }
        else if (itemIcon != null)
        {
            // Icon yoksa varsayılan renk
            itemIcon.color = Color.white;
            Debug.Log("Icon yok, varsayılan renk set edildi");
        }

        if (itemNameText != null)
        {
            itemNameText.text = item.itemName;
            itemNameText.fontSize = 24; // Büyük yap
            itemNameText.color = Color.black;
            Debug.Log($"Name text set edildi: {item.itemName}");
        }
        else
        {
            Debug.LogError("itemNameText NULL!");
        }

        if (itemPriceText != null)
        {
            itemPriceText.text = $"{item.price} Gold";
            itemPriceText.fontSize = 20; // Büyük yap
            itemPriceText.color = Color.yellow;
            Debug.Log($"Price text set edildi: {item.price} Gold");
        }
        else
        {
            Debug.LogError("itemPriceText NULL!");
        }

        // Button event'ini ayarla
        if (purchaseButton != null)
        {
            // Önce tüm listener'ları temizle
            purchaseButton.onClick.RemoveAllListeners();

            // Yeni listener ekle
            purchaseButton.onClick.AddListener(() => {
                Debug.Log($"BUTTON CLICKED: {item.itemName}");
                OnPurchaseClicked();
            });

            Debug.Log("Button event set edildi");

            // Button'ın interactable olduğundan emin ol
            purchaseButton.interactable = true;

            // Button rengini ayarla (görünür olsun)
            ColorBlock colors = purchaseButton.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = Color.cyan;
            colors.pressedColor = Color.green;
            purchaseButton.colors = colors;
        }
        else
        {
            Debug.LogError("purchaseButton NULL!");
        }

        Debug.Log($"SetupItem tamamlandı: {item.itemName}");
    }

    public void OnPurchaseClicked()
    {
        Debug.Log($"Purchase clicked: {itemData?.itemName ?? "NULL"}");

        if (shopUI != null && itemData != null)
        {
            shopUI.PurchaseItem(itemData);
        }
        else
        {
            Debug.LogError($"Purchase failed - shopUI: {shopUI != null}, itemData: {itemData != null}");
        }
    }

    // Manuel test için - Inspector'dan çağrılabilir
    public void TestButtonClick()
    {
        Debug.Log("TEST BUTTON CLICKED!");
        OnPurchaseClicked();
    }
}