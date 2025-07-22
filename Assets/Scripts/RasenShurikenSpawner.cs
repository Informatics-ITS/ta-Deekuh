using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RasenShurikenSpawner : MonoBehaviour
{
    public GameObject rasenShurikenPrefab;
    public GameObject explosionPrefab;
    public Transform spawnPoint; // A child transform on your right hand
    public float spawnDuration = 1f;
    public ScoreManager scoreManager;
    public SpellingManager3D spellingManager;

    public AudioSource spawnSound;
    public AudioSource throwSound;
    public AudioSource explosionSound;

    private bool isSpawning = false;
    private GameObject currentInstance;
    private Transform currentTarget;

    public void SpawnRasengan()
    {
        if (isSpawning || currentInstance != null) return;

        if (scoreManager == null || scoreManager.currentKills < scoreManager.maxKills || spellingManager.currentPhase == SpellingManager3D.GamePhase.Spelling)
        {
            Debug.Log("Cannot spawn Rasengan: Score bar not full.");
            return;
        }

        currentInstance = Instantiate(rasenShurikenPrefab, spawnPoint.position, spawnPoint.rotation, spawnPoint);
        currentInstance.transform.SetParent(spawnPoint);
        currentInstance.transform.localPosition = new Vector3(-0.0032f, -0.0852f, 0.1171f);
        spawnSound.Play();
        Debug.LogWarning("Spawning Rasen Shuriken at: " + spawnPoint.position);

        StartCoroutine(FadeInAndScale(currentInstance));
        scoreManager.ResetScore(); // Reset score after spawning
    }

    private IEnumerator FadeInAndScale(GameObject rasengan)
    {
        isSpawning = true;

        Renderer rend = rasengan.GetComponentInChildren<Renderer>();
        Material mat = rend.material;
        Color startColor = mat.color;
        startColor.a = 0;
        mat.color = startColor;

        Vector3 startScale = Vector3.zero;
        Vector3 endScale = new Vector3(0.17f, 0.17f, 0.17f); // Adjust if needed

        float time = 0f;

        while (time < spawnDuration)
        {
            float t = time / spawnDuration;
            // Lerp scale and opacity
            rasengan.transform.localScale = Vector3.Lerp(startScale, endScale, t);

            Color c = mat.color;
            c.a = Mathf.Lerp(0, 1, t);
            mat.color = c;

            time += Time.deltaTime;
            yield return null;
        }

        // Final snap
        rasengan.transform.localScale = endScale;
        Color finalColor = mat.color;
        finalColor.a = 1;
        mat.color = finalColor;

        isSpawning = false;
    }

    public void ClearRasengan()
    {
        if (currentInstance != null)
            Destroy(currentInstance);
    }

    private Vector3 lastHandPosition;
    private Vector3 handVelocity;

    void Update()
    {
        if (currentInstance != null)
        {
            Vector3 currentHandPosition = spawnPoint.position;
            handVelocity = (currentHandPosition - lastHandPosition) / Time.deltaTime;
            lastHandPosition = currentHandPosition;
        }
    }

    public void ThrowRasengan()
    {
        if (currentInstance == null || isSpawning) return;

        currentInstance.transform.SetParent(null);

        // Disable Rigidbody if you're going to animate path
        Rigidbody rb = currentInstance.GetComponent<Rigidbody>();
        rb.isKinematic = true;

        // Find nearest enemy
        currentTarget = FindNearestEnemy();
        if (currentTarget != null)
        {
            Debug.Log("Throwing Rasengan towards " + currentTarget.name);
            // Start coroutine to move rasengan toward enemy
            StartCoroutine(MoveTowardsTarget(currentInstance, currentTarget));
            throwSound.Play();
        }
        else
        {
            Debug.LogWarning("No enemies found — Rasengan will do nothing.");
            Destroy(currentInstance, 2f); // auto-destroy if no target
        }

        currentInstance = null;
    }


    private Transform FindNearestEnemy()
    {
        float minDist = Mathf.Infinity;
        Transform nearest = null;
        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            float dist = Vector3.Distance(spawnPoint.position, enemy.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = enemy.transform;
            }
        }
        return nearest;
    }

    private IEnumerator MoveTowardsTarget(GameObject rasengan, Transform target)
    {
        float speed = 15f; // adjust as needed
        float rotateSpeed = 5f; // for homing turn effect

        while (rasengan != null && target != null)
        {
            // Move forward
            Vector3 aimPosition = target.position + new Vector3(0, 1.0f, 0); // 1m upward
            Vector3 direction = (aimPosition - rasengan.transform.position).normalized;
            rasengan.transform.position += direction * speed * Time.deltaTime;

            // Optional: rotate to face target
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            rasengan.transform.rotation = Quaternion.Slerp(rasengan.transform.rotation, lookRotation, rotateSpeed * Time.deltaTime);

            // Check for hit distance
            float distanceToTarget = Vector3.Distance(rasengan.transform.position, aimPosition);
            if (distanceToTarget < 0.5f) // hit threshold
            {
                Debug.Log("Rasengan hit target!");

                // Spawn explosion effect at enemy position
                // if (explosionPrefab != null)
                // {
                //     GameObject explosionInstance = Instantiate(explosionPrefab, target.position, Quaternion.identity);
                //     Destroy(explosionInstance, 3f); // Destroy explosion after 3 seconds
                // }

                explosionSound.Play();
                StartCoroutine(RasenganExplosionEffect(rasengan));
                explosionPrefab.SetActive(true);

                // Destroy(rasengan);
                spellingManager.ClearEnemiesWithPoof();
                StartCoroutine(spellingManager.DelayedEndShootingMode(4f));
                spellingManager.ClearSpawnedShurikens();

                yield break;
            }

            yield return null;
        }

        // If target destroyed early, cleanup
        if (rasengan != null)
            Destroy(rasengan, 2f);
    }

    private IEnumerator RasenganExplosionEffect(GameObject rasengan)
    {
        float duration = 1.5f; // scaling phase
        float fadeDuration = 1f; // fade out phase
        float time = 0f;

        Vector3 startScale = rasengan.transform.localScale;
        Vector3 endScale = startScale * 80f; // adjust explosion size

        // Disable WindEffect child if found
        Transform windEffect = rasengan.transform.Find("Shuriken");
        if (windEffect != null)
        {
            windEffect.gameObject.SetActive(false);
        }

        // Get material
        Renderer rend = rasengan.GetComponentInChildren<Renderer>();
        Material mat = rend.material;

        // Optional: initialize fully visible
        mat.SetFloat("_Alpha", 1f);
        mat.SetFloat("_EmissiveIntensity", 1f);

        // Phase 1: scale up + brighten + more solid
        while (time < duration)
        {
            float t = time / duration;

            float scaleT = Mathf.Pow(t, 0.7f);
            rasengan.transform.localScale = Vector3.Lerp(startScale, endScale, scaleT);

            // Emissive flash up
            float emissive = Mathf.Lerp(1f, 1.8f, t); // bigger boost for "solid" look
            mat.SetFloat("_EmissiveIntensity", emissive);

            // Alpha slightly more dense (solid)
            float alpha = Mathf.Lerp(1f, 1.2f, t);
            mat.SetFloat("_Alpha", Mathf.Min(alpha, 1f)); // don't go over 1

            time += Time.deltaTime;
            yield return null;
        }

        // Phase 2: fade out alpha + emissive
        time = 0f;
        while (time < fadeDuration)
        {
            float t = time / fadeDuration;

            rasengan.transform.localScale = endScale;

            float alpha = Mathf.Lerp(1f, 0f, t);
            float emissive = Mathf.Lerp(1f, 0f, t);

            mat.SetFloat("_Alpha", alpha);
            mat.SetFloat("_EmissiveIntensity", emissive);

            time += Time.deltaTime;
            yield return null;
        }

        // Final snap
        mat.SetFloat("_Alpha", 0f);
        mat.SetFloat("_EmissiveIntensity", 0f);

        Destroy(rasengan);
        explosionPrefab.SetActive(false);
    }
}
