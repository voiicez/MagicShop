using UnityEngine;

public class DecorInteraction : MonoBehaviour
{
    private GameObject selectedDecor;
    private OutlineHighlighter selectedOutline;

    void Update()
    {
        if (Input.GetMouseButtonDown(1)) // Sað týk ile obje seç
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 5f))
            {
                if (hit.collider.CompareTag("Decor"))
                {
                    // Önceki seçimi temizle
                    if (selectedOutline != null)
                        selectedOutline.RemoveHighlight();

                    // Yeni seçimi ayarla
                    selectedDecor = hit.collider.gameObject;
                    selectedOutline = selectedDecor.GetComponentInParent<OutlineHighlighter>();

                    if (selectedOutline != null)
                        selectedOutline.Highlight();

                    Debug.Log("Selected: " + selectedDecor.name);
                }
                else
                {
                    // Yanlýþ bir þey seçildiyse mevcut highlight'ý kapat
                    if (selectedOutline != null)
                        selectedOutline.RemoveHighlight();

                    selectedDecor = null;
                    selectedOutline = null;
                }
            }
        }

        if (selectedDecor != null)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                selectedDecor.transform.Rotate(Vector3.up, 45f);
            }

            if (Input.GetKeyDown(KeyCode.Delete))
            {
                Destroy(selectedDecor);
                selectedDecor = null;
                selectedOutline = null;
            }
        }
    }
}
