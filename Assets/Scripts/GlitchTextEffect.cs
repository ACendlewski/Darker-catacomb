using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class MultiGlitchTextEffect : MonoBehaviour
{
    [System.Serializable]
    public class GlitchText
    {
        public TextMeshProUGUI textComponent;
        public float intensity = 2f; // Jak bardzo litery mają się trząść
        public float speed = 2f; // Jak szybko się trzęsą

        [HideInInspector] public TMP_TextInfo textInfo;
        [HideInInspector] public Vector3[][] originalVertices;
    }

    public List<GlitchText> glitchTexts = new List<GlitchText>();

    void Start()
    {
        foreach (var glitchText in glitchTexts)
        {
            if (glitchText.textComponent == null) continue;

            glitchText.textInfo = glitchText.textComponent.textInfo;
            glitchText.textComponent.ForceMeshUpdate();

            // Zapamiętaj oryginalne pozycje wierzchołków dla każdego znaku
            glitchText.originalVertices = new Vector3[glitchText.textInfo.meshInfo.Length][];
            for (int i = 0; i < glitchText.textInfo.meshInfo.Length; i++)
            {
                glitchText.originalVertices[i] = new Vector3[glitchText.textInfo.meshInfo[i].vertices.Length];
                glitchText.textInfo.meshInfo[i].vertices.CopyTo(glitchText.originalVertices[i], 0);
            }
        }
    }

    void Update()
    {
        foreach (var glitchText in glitchTexts)
        {
            if (glitchText.textComponent == null) continue;

            glitchText.textComponent.ForceMeshUpdate();
            TMP_MeshInfo[] meshInfo = glitchText.textInfo.meshInfo;

            for (int i = 0; i < glitchText.textInfo.characterCount; i++)
            {
                TMP_CharacterInfo charInfo = glitchText.textInfo.characterInfo[i];
                if (!charInfo.isVisible) continue;

                Vector3 shakeOffset = new Vector3(
                    Mathf.PerlinNoise(Time.time * glitchText.speed, i) * glitchText.intensity - glitchText.intensity / 2,
                    Mathf.PerlinNoise(i, Time.time * glitchText.speed) * glitchText.intensity - glitchText.intensity / 2,
                    0f
                );

                int vertexIndex = charInfo.vertexIndex;
                for (int j = 0; j < 4; j++)
                {
                    meshInfo[charInfo.materialReferenceIndex].vertices[vertexIndex + j] =
                        glitchText.originalVertices[charInfo.materialReferenceIndex][vertexIndex + j] + shakeOffset;
                }
            }

            for (int i = 0; i < glitchText.textInfo.meshInfo.Length; i++)
            {
                glitchText.textComponent.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
            }
        }
    }
}
