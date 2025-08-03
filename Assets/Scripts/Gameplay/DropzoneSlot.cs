using UnityEngine;

public class DropzoneSlot : MonoBehaviour
{
    public bool IsOccupied => currentItem != null;

    private GameObject currentItem;

    public void PlaceItem(GameObject itemPrefab)
    {
        currentItem = Instantiate(itemPrefab, transform.position, Quaternion.identity);
    }

    public void ClearSlot()
    {
        if (currentItem != null)
        {
            Destroy(currentItem);
            currentItem = null;
        }
    }
}
