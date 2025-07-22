using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISmoothFollow : MonoBehaviour
{
    public Transform playerCamera;    // Assign your VR camera here
    public float smoothTime = 0.3f;   // How smooth the movement is
    private float velocityY = 0f;     // For SmoothDamp velocity tracking
    private Vector3 initialOffset;    // Initial offset from camera

    void Start()
    {
        // Calculate initial vertical offset from camera to UI parent position
        initialOffset = playerCamera.position - transform.position;
    }

    void Update()
    {
        // Target Y position = camera Y + initial vertical offset Y
        float targetY = playerCamera.position.y + initialOffset.y;

        // Smoothly move UI on Y axis only
        float newY = Mathf.SmoothDamp(transform.position.y, targetY, ref velocityY, smoothTime);

        // Keep X and Z the same, only update Y
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}
