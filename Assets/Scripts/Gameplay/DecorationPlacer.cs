using UnityEngine;
using UnityEngine.InputSystem;

public class DecorationPlacer : MonoBehaviour
{
    public GameObject ghostPrefab;
    public GameObject actualPrefab;
    public LayerMask groundLayer;

    private GameObject ghostInstance;
    private Quaternion currentRotation = Quaternion.identity;
    private bool canPlace = false;

    void Start()
    {
        ghostInstance = Instantiate(ghostPrefab);
        ghostInstance.SetActive(false); // Ba�ta devre d���
    }

    void Update()
    {
        if (!canPlace) return; // Se�im yap�lmad�ysa i�lem yapma.

        UpdateGhostPosition();

        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            currentRotation *= Quaternion.Euler(0, 45, 0);
        }

        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            PlaceObject();
        }
    }

    void UpdateGhostPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        if (Physics.Raycast(ray, out RaycastHit hit, 10f, groundLayer))
        {
            ghostInstance.transform.position = hit.point;
            ghostInstance.transform.rotation = currentRotation;
        }
    }

    void PlaceObject()
    {
        Instantiate(actualPrefab, ghostInstance.transform.position, ghostInstance.transform.rotation);
        canPlace = false;
        ghostInstance.SetActive(false); // yerle�tirildi�inde ghost kapan�r
    }

    // D��ar�dan �a�r�l�r (�rne�in UI butonu)
    public void SelectObjectToPlace(GameObject ghost, GameObject actual)
    {
        ghostPrefab = ghost;
        actualPrefab = actual;

        if (ghostInstance != null)
            Destroy(ghostInstance);

        ghostInstance = Instantiate(ghostPrefab);
        ghostInstance.SetActive(true);

        currentRotation = Quaternion.identity;
        canPlace = true;
    }
}
