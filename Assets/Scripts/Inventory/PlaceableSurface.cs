using UnityEngine;

public class PlaceableSurface : MonoBehaviour
{
    public Transform[] itemSlots;

    public Transform GetClosestSlot(Vector3 position)
    {
        Transform closest = null;
        float minDist = Mathf.Infinity;

        foreach (var slot in itemSlots)
        {
            float dist = Vector3.Distance(position, slot.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = slot;
            }
        }

        return closest;
    }
}
