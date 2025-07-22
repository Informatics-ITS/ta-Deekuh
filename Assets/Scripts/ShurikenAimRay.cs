using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShurikenAimRay : MonoBehaviour
{
    public Transform rayOrigin; // Assign RayOrigin child here
    public float rayDistance = 10f;
    public LayerMask enemyLayer;
    public GameObject aimIconPrefab; // Assign lock-on icon prefab
    private GameObject currentAimIcon;
    private Transform lockedEnemy;

    public float aimIconHeight = 1f;
    public float aimIconForwardOffset = 0.5f;
    public float iconRotationSpeed = 90f; // Degrees per second (counter-clockwise)

    private Transform iconVisual; // The spinning child
    private LineRenderer lineRenderer;

    public static Transform CurrentLockedEnemy { get; private set; }
    // public static ShurikenAimRay Instance { get; private set; }
    public static List<ShurikenAimRay> Instances { get; private set; } = new List<ShurikenAimRay>();

    void Awake()
    {
        Instances.Add(this);
        lineRenderer = GetComponent<LineRenderer>();

        if (lineRenderer != null)
        {
            lineRenderer.positionCount = 2;
            lineRenderer.enabled = false; // Start hidden
        }
    }

    void OnDestroy()
    {
        Instances.Remove(this);
    }

    void Update()
    {
        // Debug.Log("ShurikenAimRay active");
        if (rayOrigin == null)
        {
            Debug.LogWarning("Ray origin not assigned.");
            return;
        }

        // Raycast to detect enemy
        Ray ray = new Ray(rayOrigin.position, rayOrigin.forward);

        Vector3 rayStart = ray.origin;
        Vector3 rayEnd = ray.origin + ray.direction * rayDistance;

        // // Update LineRenderer to show the ray
        lineRenderer.SetPosition(0, rayStart);
        lineRenderer.SetPosition(1, rayEnd);

        if (Physics.Raycast(ray, out RaycastHit hit, rayDistance, enemyLayer))
        {
            // Debug.Log("Hit: " + hit.collider.name);
            // Hit an enemy
            Transform hitEnemy = hit.collider.transform;

            // Lock onto new enemy if it's different from the current
            if (lockedEnemy != hitEnemy)
            {
                lockedEnemy = hitEnemy;
                CurrentLockedEnemy = hitEnemy;

                if (currentAimIcon != null)
                {
                    Destroy(currentAimIcon);
                }

                currentAimIcon = Instantiate(aimIconPrefab);
                iconVisual = currentAimIcon.transform.Find("IconVisual");

                if (iconVisual == null)
                {
                    Debug.LogWarning("IconVisual child not found under aim icon prefab!");
                }
                else
                {
                    Debug.Log("IconVisual assigned successfully.");
                }
            }
        }

        // Show and update aim icon if locked onto an enemy
        if (lockedEnemy != null)
        {
            if (currentAimIcon == null)
            {
                currentAimIcon = Instantiate(aimIconPrefab);
                iconVisual = currentAimIcon.transform.Find("IconVisual");
            }

            currentAimIcon.SetActive(true);

            Vector3 position = lockedEnemy.position
                             + lockedEnemy.forward * aimIconForwardOffset
                             + Vector3.up * aimIconHeight;
            currentAimIcon.transform.position = position;

            // Face camera
            currentAimIcon.transform.rotation = Quaternion.LookRotation(
                currentAimIcon.transform.position - Camera.main.transform.position
            );

            // Spin child (IconVisual) counter-clockwise
            if (iconVisual != null)
            {
                iconVisual.Rotate(Vector3.forward, iconRotationSpeed * Time.deltaTime, Space.Self);

                // Debug: print current Z rotation
                float zAngle = iconVisual.localEulerAngles.z;
                // Debug.Log("IconVisual Z Rotation: " + zAngle);
            }
        }

        // Debug ray in Scene view
        Debug.DrawRay(ray.origin, ray.direction * rayDistance, Color.red);
    }

    public void ShowRay()
    {
        if (rayOrigin == null || lineRenderer == null)
            return;

        lineRenderer.enabled = true;

        // Ray ray = new Ray(rayOrigin.position, rayOrigin.forward);
        // Vector3 rayStart = ray.origin;
        // Vector3 rayEnd = ray.origin + ray.direction * rayDistance;

        // lineRenderer.SetPosition(0, rayStart);
        // lineRenderer.SetPosition(1, rayEnd);
    }

    public void HideRay()
    {
        if (lineRenderer != null)
            lineRenderer.enabled = false;
    }

    public static void ClearTargetIcon()
    {
        foreach (var instance in Instances)
        {
            if (instance.currentAimIcon != null)
            {
                Destroy(instance.currentAimIcon);
                instance.currentAimIcon = null;
            }

            instance.lockedEnemy = null;
        }

        CurrentLockedEnemy = null;
    }

    public Transform GetLockedEnemy()
    {
        return lockedEnemy;
    }
}
