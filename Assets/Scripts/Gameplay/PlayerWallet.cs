using UnityEngine;

public class PlayerWallet : MonoBehaviour
{
    public int currentGold = 0;

    public void AddGold(int amount)
    {
        currentGold += amount;
        Debug.Log("Altın eklendi: " + amount + " | Toplam: " + currentGold);
    }

    public bool SpendGold(int amount)
    {
        if (currentGold >= amount)
        {
            currentGold -= amount;
            return true;
        }

        Debug.Log("Yetersiz altın!");
        return false;
    }
}
