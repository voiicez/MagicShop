using UnityEngine;

public class PlayerWallet : MonoBehaviour
{
    public int currentGold = 0;

    public void AddGold(int amount)
    {
        currentGold += amount;
        Debug.Log("Alt�n eklendi: " + amount + " | Toplam: " + currentGold);
    }

    public bool SpendGold(int amount)
    {
        if (currentGold >= amount)
        {
            currentGold -= amount;
            return true;
        }

        Debug.Log("Yetersiz alt�n!");
        return false;
    }
}
