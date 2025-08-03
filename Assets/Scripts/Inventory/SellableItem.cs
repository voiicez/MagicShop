using UnityEngine;

public class SellableItem : MonoBehaviour
{
    [Header("Item Settings")]
    public ItemData itemData;

    [Header("Visual Feedback")]
    private OutlineHighlighter highlighter;

    void Start()
    {
        highlighter = GetComponent<OutlineHighlighter>();
        if (highlighter == null)
        {
            highlighter = gameObject.AddComponent<OutlineHighlighter>();
        }

        if (itemData == null)
        {
            Debug.LogWarning($"SellableItem on {gameObject.name} has no ItemData assigned!");
        }
    }

    public bool CanSell()
    {
        return itemData != null && itemData.price > 0;
    }

    public void Sell(PlayerWallet wallet)
    {
        if (!CanSell())
        {
            Debug.LogWarning("Bu item satılamaz!");
            return;
        }

        if (wallet == null)
        {
            Debug.LogError("PlayerWallet referansı null!");
            return;
        }

        wallet.AddGold(itemData.price);

        // Satış efekti
        PlaySellEffect();

        Destroy(gameObject);
    }

    private void PlaySellEffect()
    {
        // Particle effect, ses efekti vb. eklenebilir
        Debug.Log($"{itemData.itemName} satıldı! +{itemData.price} gold");

        // Basit visual feedback
        if (highlighter != null)
        {
            highlighter.Highlight();
        }
    }

    // Mouse hover için
    void OnMouseEnter()
    {
        if (highlighter != null)
        {
            highlighter.Highlight();
        }
    }

    void OnMouseExit()
    {
        if (highlighter != null)
        {
            highlighter.RemoveHighlight();
        }
    }
}