using UnityEngine;
using UnityEngine.InputSystem;

public class DecorationPlacer : MonoBehaviour
{
    public GameObject ghostPrefab;
    public GameObject actualPrefab;
    public LayerMask groundLayer;
    //
    private GameObject ghostInstance;
    private Quaternion currentRotation = Quaternion.identity;

    void Start()
    {
        ghostInstance = Instantiate(ghostPrefab);
    }

    void Update()
    {
        UpdateGhostPosition();

        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            currentRotation *= Quaternion.Euler(0, 45, 0); // 45 derece döndür
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
        Instantiate(actualPrefab, ghostInstance.transform.position, currentRotation);
        Destroy(ghostInstance);
        Destroy(this); // bu yerleþtirme iþi bitti
    }
}
