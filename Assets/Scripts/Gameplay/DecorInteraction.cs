using UnityEngine;

public class DecorInteraction : MonoBehaviour
{
    private GameObject selectedDecor;
    private OutlineHighlighter selectedOutline;

    void Update()
    {
        if (Input.GetMouseButtonDown(1)) // Sa� t�k ile obje se�
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 5f))
            {
                if (hit.collider.CompareTag("Decor"))
                {
                    // �nceki se�imi temizle
                    if (selectedOutline != null)
                        selectedOutline.RemoveHighlight();

                    // Yeni se�imi ayarla
                    selectedDecor = hit.collider.gameObject;
                    selectedOutline = selectedDecor.GetComponentInParent<OutlineHighlighter>();

                    if (selectedOutline != null)
                        selectedOutline.Highlight();

                    Debug.Log("Selected: " + selectedDecor.name);
                }
                else
                {
                    // Yanl�� bir �ey se�ildiyse mevcut highlight'� kapat
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
