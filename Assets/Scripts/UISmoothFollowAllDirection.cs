using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISmoothFollowAllDirection : MonoBehaviour
{
    public Transform playerCamera;    // Assign your VR camera (CenterEyeAnchor or Camera.main.transform)
    public float smoothTime = 0.3f;   // Smoothing factor for movement
    public Vector3 offset = new Vector3(0, 0, 5f); // Offset in front of the player

    private Vector3 currentVelocity = Vector3.zero;

    void Update()
    {
        // Desired position is in front of the camera, with offset
        Vector3 targetPosition = playerCamera.position + playerCamera.forward * offset.z + playerCamera.up * offset.y + playerCamera.right * offset.x;

        // Smoothly follow the target position
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, smoothTime);

        // Optional: make the UI always face the camera
        transform.rotation = Quaternion.LookRotation(transform.position - playerCamera.position);
    }
}
