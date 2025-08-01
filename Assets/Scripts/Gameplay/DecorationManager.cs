using UnityEngine;

public class DecorationManager : MonoBehaviour
{
    private PlayerWallet wallet;

    private void Start()
    {
        wallet = FindObjectOfType<PlayerWallet>();
    }

    public void PurchaseDecor(DecorativeItemData item)
    {
        if (wallet == null) return;

        if (wallet.currentGold >= item.cost)
        {
            wallet.currentGold -= item.cost;

            // Ray ile bak�lan noktay� tespit et
            Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f)); // tam ortadan
            if (Physics.Raycast(ray, out RaycastHit hit, 5f))
            {
                Vector3 spawnPos = hit.point;
                Quaternion spawnRot = Quaternion.FromToRotation(Vector3.up, hit.normal);
                GameObject obj = Instantiate(item.prefab, spawnPos, spawnRot);

                // Dik durmas� i�in sadece Y eksenini koruyal�m
                obj.transform.rotation = Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0);
            }
        }
    }
}
