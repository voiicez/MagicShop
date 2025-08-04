using StarterAssets;
using System;
using UnityEngine;

public class SellableItem : MonoBehaviour
{
    [Header("Item Settings")]
    public ItemData itemData;
[Header("Visual Feedback")]
private OutlineHighlighter highlighter;

    private Transform playerTransform;
    private PickableItem pickableItem;

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

        // Oyuncu referansını al
        var playerController = FindObjectOfType<FirstPersonController>();
        if (playerController != null)
        {
            playerTransform = playerController.transform;
        }

        // PickableItem referansını al
        pickableItem = GetComponent<PickableItem>();
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

    // Mouse hover başladı
    void OnMouseEnter()
    {
        CheckHighlight();
    }

    // Mouse hover boyunca her frame çağrılır
    void OnMouseOver()
    {
        CheckHighlight();
    }

    // Mouse hover bitti
    void OnMouseExit()
    {
        if (highlighter != null)
        {
            highlighter.RemoveHighlight();
        }
    }

    // Mesafe kontrolü yaparak highlight durumunu güncelle
    private void CheckHighlight()
    {
        if (highlighter != null && playerTransform != null && pickableItem != null)
        {
            float distance = Vector3.Distance(transform.position, playerTransform.position);
            if (distance <= pickableItem.pickupRange)
            {
                highlighter.Highlight();
            }
            else
            {
                highlighter.RemoveHighlight();
            }
        }
    }
}