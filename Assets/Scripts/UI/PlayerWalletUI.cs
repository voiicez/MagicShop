using TMPro;
using UnityEngine;

public class PlayerWalletUI : MonoBehaviour
{
    private PlayerWallet wallet;
    public TextMeshProUGUI goldText;

    void Start()
    {
        wallet = FindObjectOfType<PlayerWallet>();
    }

    void Update()
    {
        if (wallet != null && goldText != null)
        {
            goldText.text = "Gold: " + wallet.currentGold.ToString();
        }
    }
}
