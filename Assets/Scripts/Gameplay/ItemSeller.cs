using UnityEngine;
using UnityEngine.InputSystem;

public class ItemSeller : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float interactionRange = 3f;

    [Header("Layer Masks")]
    [SerializeField] private LayerMask sellableLayerMask = -1; // Everything başlangıçta

    [Header("References")]
    public PlayerWallet wallet;
    private Camera playerCamera;

    void Start()
    {
        playerCamera = Camera.main;
        if (playerCamera == null)
        {
            Debug.LogError("Ana kamera bulunamadı!");
        }

        if (wallet == null)
        {
            wallet = FindObjectOfType<PlayerWallet>();
        }

        Debug.Log($"ItemSeller başlatıldı. Sellable Layer Mask: {sellableLayerMask.value}");
    }

    void Update()
    {
        if (Keyboard.current?.fKey?.wasPressedThisFrame == true)
        {
            TrySellItem();
        }
    }

    private void TrySellItem()
    {
        if (playerCamera == null || wallet == null) return;

        Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0);
        Ray ray = playerCamera.ScreenPointToRay(screenCenter);

        Debug.Log("F tuşuna basıldı, satış raycast yapılıyor...");

        // Raycast yap (layer mask kullan ama component kontrolü de yap)
        if (Physics.Raycast(ray, out RaycastHit hit, interactionRange))
        {
            Debug.Log($"Hit object: {hit.collider.name}, Layer: {LayerMask.LayerToName(hit.collider.gameObject.layer)}");

            // SellableItem component kontrolü (layer bağımsız)
            SellableItem sellable = hit.collider.GetComponent<SellableItem>();
            if (sellable != null)
            {
                Debug.Log($"SellableItem bulundu: {sellable.itemData?.itemName ?? "Unknown"}");
                sellable.Sell(wallet);
            }
            else
            {
                Debug.Log($"Hit object'de SellableItem component yok: {hit.collider.name}");
            }
        }
        else
        {
            Debug.Log("Hiçbir object hit edilmedi veya menzil dışında");
        }
    }
}