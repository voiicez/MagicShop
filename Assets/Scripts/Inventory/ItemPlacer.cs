using System;
using UnityEngine;

public class ItemPlacer : MonoBehaviour
{
    [Header("References")]
    public Transform itemHoldPoint;
    public Inventory inventory;

[Header("Placement Settings")]
public float placementRange = 5f;

    [Header("Layer Masks")]
    [SerializeField] private LayerMask shelfLayerMask = -1;  // Sadece raflara yerleþtirme yapýlacak

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

        if (Physics.Raycast(ray, out RaycastHit hit, placementRange))
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

        // Orijinal item'ý gizle
        originalItem.SetActive(false);

        // Ghost item oluþtur
        CreateGhostItem();
    }

    private void CreateGhostItem()
    {
        if (currentItemData == null)
        {
            Debug.LogError("CreateGhostItem: currentItemData null!");
            return;
        }

        if (currentItemData.ghostPrefab != null)
        {
            currentGhostItem = Instantiate(currentItemData.ghostPrefab);
            Collider[] colliders = currentGhostItem.GetComponentsInChildren<Collider>();
            foreach (Collider col in colliders)
            {
                col.enabled = false;
            }
            MakeTransparent(currentGhostItem);
        }
        else if (currentItemData.prefab != null)
        {
            currentGhostItem = Instantiate(currentItemData.prefab);
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
    }

    private void UpdateGhostItem()
    {
        if (currentGhostItem == null) return;

        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));

        // Rafýn layer mask'ine göre raycast yap
        if (Physics.Raycast(ray, out RaycastHit hit, placementRange, shelfLayerMask))
        {
            ShelfManager shelfManager = hit.collider.GetComponentInParent<ShelfManager>();
            if (shelfManager != null)
            {
                ShelfSlot slot = shelfManager.GetNearestAvailableSlot(hit.point);
                if (slot != null)
                {
                    currentGhostItem.transform.position = slot.transform.position;
                    currentGhostItem.SetActive(true);
                    SetGhostColor(Color.green);
                }
                else
                {
                    currentGhostItem.SetActive(false);
                }
            }
            else
            {
                currentGhostItem.SetActive(false);
            }
        }
        else
        {
            currentGhostItem.SetActive(false);
        }
    }

    private void TryPlaceItem()
    {
        if (!isHoldingItem || currentGhostItem == null) return;

        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));

        // Raflara doðru raycast yap
        if (Physics.Raycast(ray, out RaycastHit hit, placementRange, shelfLayerMask))
        {
            ShelfManager shelfManager = hit.collider.GetComponentInParent<ShelfManager>();
            if (shelfManager != null)
            {
                // En yakýn boþ slota item yerleþtir
                GameObject placedItem = shelfManager.PlaceItemAtNearestSlot(currentItemData.prefab, currentItemData, hit.point);
                if (placedItem != null)
                {
                    // Eski nesneyi yok et ve temizle
                    if (originalPickedItem != null)
                    {
                        Destroy(originalPickedItem);
                    }
                    ClearCurrentItem();
                }
                else
                {
                    Debug.Log("Rafýn tüm slotlarý dolu!");
                }
            }
        }
        else
        {
            Debug.Log("Raf üzerine yerleþtirmek için uygun bir alan seçilmedi!");
        }
    }

    private void DropItem()
    {
        if (!isHoldingItem) return;

        if (originalPickedItem != null)
        {
            originalPickedItem.SetActive(true);
        }

        ClearCurrentItem();
    }

    private void ClearCurrentItem()
    {
        if (currentGhostItem != null)
        {
            Destroy(currentGhostItem);
        }

        currentItemData = null;
        originalPickedItem = null;
        isHoldingItem = false;
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