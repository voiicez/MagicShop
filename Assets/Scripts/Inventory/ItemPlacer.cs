using UnityEngine;

public class ItemPlacer : MonoBehaviour
{
    [Header("References")]
    public Transform itemHoldPoint;
    public Inventory inventory;

    [Header("Placement Settings")]
    public float placementRange = 5f;

    [Header("Layer Masks")]
    [SerializeField] private LayerMask groundLayerMask = -1;
    [SerializeField] private LayerMask pickableLayerMask = -1;
    [SerializeField] private LayerMask obstacleLayerMask = 0;
    [SerializeField] private LayerMask sellableLayerMask = -1;

    [Header("Current Item")]
    private ItemData currentItemData;
    private GameObject currentGhostItem;
    private GameObject originalPickedItem;
    private bool isHoldingItem = false;

    void Update()
    {
        HandleInput();

        if (isHoldingItem)
        {
            UpdateGhostItem();
        }
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (isHoldingItem)
            {
                TryPlaceItem();
            }
            else
            {
                TryPickupItem();
            }
        }

        if (Input.GetKeyDown(KeyCode.Q) && isHoldingItem)
        {
            DropItem();
        }
    }

    private void TryPickupItem()
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));

        if (Physics.Raycast(ray, out RaycastHit hit, placementRange, pickableLayerMask))
        {
            PickableItem pickableItem = hit.collider.GetComponent<PickableItem>();

            if (pickableItem != null && pickableItem.CanPickup(transform))
            {
                pickableItem.Pickup(this);
            }
        }
    }

    public void PickupItem(ItemData itemData, GameObject originalItem)
    {
        if (isHoldingItem) return;

        currentItemData = itemData;
        originalPickedItem = originalItem;
        isHoldingItem = true;

        // Debug log
        Debug.Log($"PickupItem called - itemData: {itemData?.itemName ?? "NULL"}, originalItem: {originalItem?.name ?? "NULL"}");

        // Orijinal item'� gizle
        originalItem.SetActive(false);

        // Ghost item olu�tur
        CreateGhostItem();

        Debug.Log($"{itemData.itemName} eline al�nd�!");
    }

    private void CreateGhostItem()
    {
        Debug.Log("CreateGhostItem ba�lad�...");

        if (currentItemData == null)
        {
            Debug.LogError("CreateGhostItem: currentItemData null!");
            return;
        }

        Debug.Log($"CreateGhostItem: itemData = {currentItemData.itemName}");
        Debug.Log($"CreateGhostItem: ghostPrefab = {(currentItemData.ghostPrefab != null ? "VAR" : "YOK")}");
        Debug.Log($"CreateGhostItem: prefab = {(currentItemData.prefab != null ? "VAR" : "YOK")}");

        if (currentItemData.ghostPrefab != null)
        {
            currentGhostItem = Instantiate(currentItemData.ghostPrefab);
            Debug.Log("Ghost prefab'dan ghost olu�turuldu");

            // Ghost item'�n collider'lar�n� kapat
            Collider[] colliders = currentGhostItem.GetComponentsInChildren<Collider>();
            foreach (Collider col in colliders)
            {
                col.enabled = false;
            }

            // Transparent material ekle
            MakeTransparent(currentGhostItem);
        }
        else if (currentItemData.prefab != null)
        {
            // ghostPrefab yoksa normal prefab'dan olu�tur
            currentGhostItem = Instantiate(currentItemData.prefab);
            Debug.Log("Normal prefab'dan ghost olu�turuldu");

            // Collider'lar� kapat
            Collider[] colliders = currentGhostItem.GetComponentsInChildren<Collider>();
            foreach (Collider col in colliders)
            {
                col.enabled = false;
            }

            MakeTransparent(currentGhostItem);
        }
        else
        {
            Debug.LogError("Ne ghost prefab ne de normal prefab var!");
        }

        Debug.Log($"CreateGhostItem tamamland�. Ghost object: {(currentGhostItem != null ? "OLU�TU" : "NULL")}");
    }

    private void UpdateGhostItem()
    {
        if (currentGhostItem == null) return;

        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));

        if (Physics.Raycast(ray, out RaycastHit hit, placementRange, groundLayerMask))
        {
            currentGhostItem.transform.position = hit.point;
            currentGhostItem.SetActive(true);

            bool canPlace = CanPlaceAt(hit.point);
            SetGhostColor(canPlace ? Color.green : Color.red);
        }
        else
        {
            currentGhostItem.SetActive(false);
        }
    }

    private bool CanPlaceAt(Vector3 position)
    {
        Collider[] overlapping = Physics.OverlapSphere(position, 0.8f);

        foreach (Collider col in overlapping)
        {
            if (col.gameObject == currentGhostItem) continue;
            if (IsInLayerMask(col.gameObject.layer, groundLayerMask)) continue;

            if (col.GetComponent<SellableItem>() != null ||
                col.GetComponent<PickableItem>() != null)
            {
                return false;
            }
        }

        return true;
    }

    private bool IsInLayerMask(int layer, LayerMask layerMask)
    {
        return (layerMask.value & (1 << layer)) != 0;
    }

    private void TryPlaceItem()
    {
        Debug.Log("TryPlaceItem �a�r�ld�...");
        Debug.Log($"isHoldingItem: {isHoldingItem}");
        Debug.Log($"currentGhostItem: {(currentGhostItem != null ? "VAR" : "NULL")}");
        Debug.Log($"currentItemData: {(currentItemData != null ? currentItemData.itemName : "NULL")}");

        if (!isHoldingItem || currentGhostItem == null) return;

        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));

        if (Physics.Raycast(ray, out RaycastHit hit, placementRange, groundLayerMask))
        {
            if (CanPlaceAt(hit.point))
            {
                PlaceItem(hit.point);
            }
            else
            {
                Debug.Log("Buraya yerle�tirilemez!");
            }
        }
        else
        {
            Debug.Log("Ground layer hit edilmedi!");
        }
    }

    private void PlaceItem(Vector3 position)
    {
        Debug.Log("=== PlaceItem ba�lad� ===");
        Debug.Log($"Position: {position}");

        // NULL KONTROLLERI
        if (currentItemData == null)
        {
            Debug.LogError("PlaceItem: currentItemData null! Yerle�tirme iptal edildi.");
            return;
        }
        Debug.Log($"currentItemData OK: {currentItemData.itemName}");

        // ITEM NAME'� KAYDET (ClearCurrentItem null yapacak)
        string itemName = currentItemData.itemName;

        if (currentItemData.prefab == null)
        {
            Debug.LogError($"PlaceItem: {currentItemData.itemName} i�in prefab null!");
            return;
        }
        Debug.Log($"currentItemData.prefab OK: {currentItemData.prefab.name}");

        // ITEM OLU�TURMA
        Debug.Log("Instantiate edilmeye �al���l�yor...");
        GameObject placedItem = null;

        try
        {
            placedItem = Instantiate(currentItemData.prefab, position, Quaternion.identity);
            Debug.Log($"Instantiate ba�ar�l�: {placedItem?.name ?? "NULL"}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Instantiate hatas�: {e.Message}");
            return;
        }

        if (placedItem == null)
        {
            Debug.LogError("placedItem null! Instantiate ba�ar�s�z.");
            return;
        }

        // LAYER ATAMA
        Debug.Log("Layer atan�yor...");
        int sellableLayer = GetSafeLayer("Sellable", 0);
        placedItem.layer = sellableLayer;
        Debug.Log($"Item layer atand�: {LayerMask.LayerToName(sellableLayer)} (index: {sellableLayer})");

        // SELLABLE ITEM COMPONENT
        Debug.Log("SellableItem component ekleniyor...");
        SellableItem sellableItem = null;

        try
        {
            sellableItem = placedItem.GetComponent<SellableItem>();
            if (sellableItem == null)
            {
                Debug.Log("SellableItem component yok, ekleniyor...");
                sellableItem = placedItem.AddComponent<SellableItem>();
            }
            Debug.Log($"SellableItem component OK: {sellableItem != null}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"SellableItem component hatas�: {e.Message}");
            return;
        }

        if (sellableItem == null)
        {
            Debug.LogError("sellableItem null! Component eklenemedi.");
            return;
        }

        // ITEM DATA ATAMA
        Debug.Log("ItemData atan�yor...");
        try
        {
            sellableItem.itemData = currentItemData;
            Debug.Log("ItemData atama ba�ar�l�");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"ItemData atama hatas�: {e.Message}");
            return;
        }

        // OR�J�NAL ITEM DESTROY
        Debug.Log("Orijinal item destroy ediliyor...");
        if (originalPickedItem != null)
        {
            try
            {
                Destroy(originalPickedItem);
                Debug.Log("Orijinal item destroy edildi");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Destroy hatas�: {e.Message}");
            }
        }
        else
        {
            Debug.Log("originalPickedItem null, destroy edilmedi");
        }

        // TEM�ZL�K
        Debug.Log("Temizlik yap�l�yor...");
        try
        {
            ClearCurrentItem(); // Bu currentItemData'y� null yap�yor
            Debug.Log("Temizlik ba�ar�l�");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Temizlik hatas�: {e.Message}");
        }

        // LOCAL VARIABLE KULLAN
        Debug.Log($"=== {itemName} yerle�tirildi! ===");
    }

    private int GetSafeLayer(string layerName, int defaultLayer)
    {
        int layer = LayerMask.NameToLayer(layerName);

        if (layer == -1)
        {
            Debug.LogWarning($"Layer '{layerName}' bulunamad�! Default layer ({defaultLayer}) kullan�l�yor.");
            return defaultLayer;
        }

        return layer;
    }

    private void DropItem()
    {
        if (!isHoldingItem) return;

        if (originalPickedItem != null)
        {
            originalPickedItem.SetActive(true);
        }

        ClearCurrentItem();
        Debug.Log("Item b�rak�ld�!");
    }

    private void ClearCurrentItem()
    {
        Debug.Log("ClearCurrentItem ba�lad�...");

        if (currentGhostItem != null)
        {
            try
            {
                Destroy(currentGhostItem);
                Debug.Log("Ghost item destroy edildi");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Ghost destroy hatas�: {e.Message}");
            }
        }

        currentItemData = null;
        originalPickedItem = null;
        isHoldingItem = false;

        Debug.Log("ClearCurrentItem tamamland�");
    }

    private void MakeTransparent(GameObject obj)
    {
        if (obj == null) return;

        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();

        foreach (Renderer renderer in renderers)
        {
            if (renderer != null && renderer.materials != null)
            {
                foreach (Material mat in renderer.materials)
                {
                    if (mat != null && mat.HasProperty("_Color"))
                    {
                        Color color = mat.color;
                        color.a = 0.5f;
                        mat.color = color;
                    }
                }
            }
        }
    }

    private void SetGhostColor(Color color)
    {
        if (currentGhostItem == null) return;

        Renderer[] renderers = currentGhostItem.GetComponentsInChildren<Renderer>();

        foreach (Renderer renderer in renderers)
        {
            if (renderer != null && renderer.materials != null)
            {
                foreach (Material mat in renderer.materials)
                {
                    if (mat != null && mat.HasProperty("_Color"))
                    {
                        Color newColor = color;
                        newColor.a = 0.5f;
                        mat.color = newColor;
                    }
                }
            }
        }
    }
}