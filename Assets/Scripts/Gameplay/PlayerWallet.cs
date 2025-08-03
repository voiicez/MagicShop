using UnityEngine;
using System;

public class PlayerWallet : MonoBehaviour
{
    [SerializeField] private int _currentGold = 100;

    public int currentGold => _currentGold;

    public static event Action<int> OnGoldChanged;

    void Start()
    {
        OnGoldChanged?.Invoke(_currentGold);
    }

    public void AddGold(int amount)
    {
        _currentGold += Mathf.Max(0, amount);
        OnGoldChanged?.Invoke(_currentGold);
        Debug.Log($"Gold eklendi: +{amount}. Toplam: {_currentGold}");
    }

    public bool SpendGold(int amount)
    {
        if (_currentGold >= amount)
        {
            _currentGold -= amount;
            OnGoldChanged?.Invoke(_currentGold);
            Debug.Log($"Gold harcandý: -{amount}. Kalan: {_currentGold}");
            return true;
        }

        Debug.Log($"Yetersiz gold! Gerekli: {amount}, Mevcut: {_currentGold}");
        return false;
    }
}