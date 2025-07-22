using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowShrink : MonoBehaviour
{
    public float speed = 2f;
    public float scaleAmount = 0.1f;
    private Vector3 originalScale;

    void Awake()
    {
        originalScale = transform.localScale;
    }

    void OnEnable()
    {
        // When this script is enabled, reset to original size
        transform.localScale = originalScale;
    }

    void OnDisable()
    {
        // When this script is disabled, reset to original size
        transform.localScale = originalScale;
    }

    void Update()
    {
        float scale = 1 + Mathf.Sin(Time.time * speed) * scaleAmount;
        transform.localScale = originalScale * scale;
    }
}
