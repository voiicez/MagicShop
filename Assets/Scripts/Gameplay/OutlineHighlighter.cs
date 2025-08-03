using UnityEngine;
using System.Collections.Generic;

public class OutlineHighlighter : MonoBehaviour
{
    [Header("Materials")]
    public Material highlightMaterial;

    private Dictionary<Renderer, Material[]> originalMaterials;
    private Renderer[] renderers;
    private bool isHighlighted = false;

    void Start()
    {
        InitializeRenderers();
    }

    private void InitializeRenderers()
    {
        renderers = GetComponentsInChildren<Renderer>();
        originalMaterials = new Dictionary<Renderer, Material[]>();

        foreach (Renderer rend in renderers)
        {
            if (rend != null)
            {
                originalMaterials[rend] = rend.materials;
            }
        }

        // E�er highlight material yoksa, varsay�lan bir tane olu�tur
        if (highlightMaterial == null)
        {
            CreateDefaultHighlightMaterial();
        }
    }

    private void CreateDefaultHighlightMaterial()
    {
        highlightMaterial = new Material(Shader.Find("Standard"));
        highlightMaterial.color = Color.yellow;
        highlightMaterial.SetFloat("_Metallic", 0f);
        highlightMaterial.SetFloat("_Glossiness", 0.5f);
    }

    public void Highlight()
    {
        if (isHighlighted || highlightMaterial == null) return;

        foreach (Renderer rend in renderers)
        {
            if (rend != null)
            {
                Material[] newMaterials = new Material[rend.materials.Length];
                for (int i = 0; i < newMaterials.Length; i++)
                {
                    newMaterials[i] = highlightMaterial;
                }
                rend.materials = newMaterials;
            }
        }
        isHighlighted = true;
    }

    public void RemoveHighlight()
    {
        if (!isHighlighted) return;

        foreach (Renderer rend in renderers)
        {
            if (rend != null && originalMaterials.ContainsKey(rend))
            {
                rend.materials = originalMaterials[rend];
            }
        }
        isHighlighted = false;
    }

    void OnDestroy()
    {
        // Memory leak'i �nlemek i�in temizlik
        originalMaterials?.Clear();
    }
}