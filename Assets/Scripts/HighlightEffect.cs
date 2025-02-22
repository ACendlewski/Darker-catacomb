using UnityEngine;

public class HighlightEffect : MonoBehaviour
{
    [Header("Highlight Settings")]
    public Color highlightColor = Color.yellow;
    public float highlightIntensity = 1.5f;

    private Material originalMaterial;
    private Renderer objectRenderer;

    void Start()
    {
        objectRenderer = GetComponent<Renderer>();
        if (objectRenderer != null)
        {
            originalMaterial = objectRenderer.material;
        }
    }

    public void SetHighlight(bool highlight)
    {
        if (objectRenderer == null) return;

        if (highlight)
        {
            // Create a new material instance for highlighting
            Material highlightMaterial = new Material(originalMaterial)
            {
                color = highlightColor * highlightIntensity
            };
            objectRenderer.material = highlightMaterial;
        }
        else
        {
            // Revert to original material
            objectRenderer.material = originalMaterial;
        }
    }

    void OnDestroy()
    {
        // Clean up material instances
        if (objectRenderer != null && objectRenderer.material != originalMaterial)
        {
            Destroy(objectRenderer.material);
        }
    }
}
