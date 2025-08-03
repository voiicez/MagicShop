using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Shop/Item Data")]
public class ItemData : ScriptableObject
{
    [Header("Basic Info")]
    public string itemName;
    public int price;
    public Sprite icon;

    [Header("Prefabs")]
    public GameObject prefab;
    public GameObject ghostPrefab;

    [Header("Item Properties")]
    public ItemType itemType = ItemType.Sellable;
    public bool isStackable = false;
    public int maxStackSize = 1;
}

public enum ItemType
{
    Sellable,
    Decorative,
    Consumable
}