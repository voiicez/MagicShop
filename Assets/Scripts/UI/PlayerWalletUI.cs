using TMPro;
using UnityEngine;

public class PlayerWalletUI : MonoBehaviour
{
    public TextMeshProUGUI goldText;

    void OnEnable()
    {
        PlayerWallet.OnGoldChanged += UpdateGoldDisplay;
    }

    void OnDisable()
    {
        PlayerWallet.OnGoldChanged -= UpdateGoldDisplay;
    }

    void Start()
    {
        var wallet = FindObjectOfType<PlayerWallet>();
        if (wallet != null)
        {
            UpdateGoldDisplay(wallet.currentGold);
        }
    }

    private void UpdateGoldDisplay(int goldAmount)
    {
        if (goldText != null)
        {
            goldText.text = $"Gold: {goldAmount}";
        }
    }
}