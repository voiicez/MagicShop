using UnityEngine;

public class SellableItem : MonoBehaviour
{
    public ItemData itemData;

    public void Sell(PlayerWallet wallet)
    {
        wallet.AddGold(itemData.price);
        Destroy(gameObject);
    }
}
