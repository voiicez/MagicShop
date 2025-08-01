using UnityEngine;
using UnityEngine.InputSystem;

public class ItemPlacer : MonoBehaviour
{
    public Transform itemHoldPoint;
    public Inventory inventory;

    private GameObject currentHeldItem;
    private GameObject ghostItem;

    void Update()
    {
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (currentHeldItem == null)
            {
                TryHoldFirstItem();
            }
            else
            {
                TryPlaceHeldItem();
            }
        }

        if (currentHeldItem != null)
        {
            UpdateGhostPreview();
        }
        else if (ghostItem != null)
        {
            Destroy(ghostItem);
        }
    }

    void TryHoldFirstItem()
    {
        if (inventory.items.Count == 0) return;

        ItemData itemToHold = inventory.items[0];
        currentHeldItem = Instantiate(itemToHold.prefab, itemHoldPoint.position, Quaternion.identity);
        currentHeldItem.transform.SetParent(itemHoldPoint);
        currentHeldItem.transform.localPosition = Vector3.zero;
        currentHeldItem.transform.localRotation = Quaternion.identity;
    }

    void TryPlaceHeldItem()
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        if (Physics.Raycast(ray, out RaycastHit hit, 3f))
        {
            if (hit.collider.CompareTag("Placeable"))
            {
                PlaceableSurface surface = hit.collider.GetComponent<PlaceableSurface>();
                if (surface != null)
                {
                    Transform slot = surface.GetClosestSlot(hit.point);
                    if (slot != null)
                    {
                        currentHeldItem.transform.SetParent(null);
                        currentHeldItem.transform.position = slot.position;
                        currentHeldItem.transform.rotation = slot.rotation;
                        currentHeldItem = null;
                    }
                }
            }
            else
            {
                Debug.Log("Burasý uygun bir yer deðil.");
            }
        }

        if (ghostItem != null)
        {
            Destroy(ghostItem);
        }
    }

    void UpdateGhostPreview()
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        if (Physics.Raycast(ray, out RaycastHit hit, 3f))
        {
            if (hit.collider.CompareTag("Placeable"))
            {
                if (inventory.items.Count > 0 && ghostItem == null)
                {
                    var data = inventory.items[0];
                    ghostItem = Instantiate(data.ghostPrefab);
                    ghostItem.SetActive(true);
                }

                if (ghostItem != null)
                {
                    PlaceableSurface surface = hit.collider.GetComponent<PlaceableSurface>();
                    if (surface != null)
                    {
                        Transform slot = surface.GetClosestSlot(hit.point);
                        if (slot != null)
                        {
                            ghostItem.transform.position = Vector3.Lerp(
                                ghostItem.transform.position,
                                slot.position,
                                Time.deltaTime * 15f
                            );

                            ghostItem.transform.rotation = Quaternion.Lerp(
                                ghostItem.transform.rotation,
                                slot.rotation,
                                Time.deltaTime * 15f
                            );

                            if (!ghostItem.activeSelf)
                                ghostItem.SetActive(true);
                        }
                    }
                }
            }
            else
            {
                if (ghostItem != null && ghostItem.activeSelf)
                {
                    ghostItem.SetActive(false);
                }
            }
        }
        else
        {
            if (ghostItem != null && ghostItem.activeSelf)
            {
                ghostItem.SetActive(false);
            }
        }
    }
}
