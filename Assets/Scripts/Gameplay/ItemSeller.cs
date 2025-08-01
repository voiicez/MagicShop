using UnityEngine;
using UnityEngine.InputSystem;

public class ItemSeller : MonoBehaviour
{
    public PlayerWallet wallet;

    void Update()
    {
        if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
            if (Physics.Raycast(ray, out RaycastHit hit, 3f))
            {
                SellableItem sellable = hit.collider.GetComponent<SellableItem>();
                if (sellable != null)
                {
                    sellable.Sell(wallet);
                }
            }
        }
    }
}
