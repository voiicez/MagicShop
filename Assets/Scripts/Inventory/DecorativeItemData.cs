using UnityEngine;

[CreateAssetMenu(fileName = "NewDecorItem", menuName = "Shop/Decor Item")]
public class DecorativeItemData : ScriptableObject
{
    public string itemName;
    public int cost;
    public GameObject prefab;
    public Sprite icon;
}
