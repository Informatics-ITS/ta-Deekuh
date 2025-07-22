using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LetterGlowAnimator : MonoBehaviour
{
    public TextMeshPro tmpText;
    public float glowSpeed = 2f;
    public float minGlow = 0.03f;
    public float maxGlow = 0.07f;

    private Material tmpMaterial;
    private float timer;

    void Start()
    {
        // Clone the material so we don't affect other letters
        tmpMaterial = Instantiate(tmpText.fontSharedMaterial);
        tmpText.fontSharedMaterial = tmpMaterial;
    }

    void Update()
    {
        timer += Time.deltaTime * glowSpeed;
        float width = Mathf.Lerp(minGlow, maxGlow, (Mathf.Sin(timer) + 1f) / 2f);
        tmpMaterial.SetFloat(ShaderUtilities.ID_OutlineWidth, width);
    }
}
