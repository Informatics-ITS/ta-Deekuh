using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public int health;
    public float animationSpeed = 1f; // Speed of the enemy's animations
    public float rotationSpeed = 5f; // Or even 7–10 for snappier turning
    public float stopDistance = 2f; // Enemy will stop this far from the player
    private Animator animator;
    public float explodeDistance = 2f;
    private bool hasSpawned = false;
    private bool isExploding = false;

    public GameObject smokeEffectPrefab;
    public GameObject explodeEffectPrefab;
    public GameObject wordCanvas;

    public Transform player;
    public ShurikenBehaviour shurikenBehaviour;
    public SpellingManager3D spellingManager;
    public ScoreManager scoreManager;

    [Header("UI")]
    [SerializeField] private GameObject healthUIPrefab;
    [SerializeField] private GameObject heartPrefab;

    [Header("Audio")]
    public AudioSource poofSound;
    public AudioSource boomSound;
    public AudioSource hitSound;

    private GameObject healthUIInstance;
    private List<GameObject> heartIcons = new();
    private Transform heartContainer;

    private Renderer enemyRenderer;
    private Color originalColor;
    public Color hitColor = Color.red;
    public float flashDuration = 0.2f;


    public void Initialize(int hp)
    {
        health = hp;
    }

    void Start()
    {
        // InitHealthUI();

        animator = GetComponentInChildren<Animator>();
        // if (animator != null)
        // {
        //     animator.SetTrigger("Spawn");
        // }

        enemyRenderer = GetComponentInChildren<Renderer>();
        if (enemyRenderer != null)
        {
            originalColor = enemyRenderer.material.color;
        }

    }

    void InitHealthUI()
    {
        if (healthUIPrefab == null || heartPrefab == null) return;

        healthUIInstance = Instantiate(healthUIPrefab);
        healthUIInstance.transform.SetParent(transform);
        healthUIInstance.transform.localPosition = Vector3.up * 2f;

        // Find the container inside the prefab by name or tag
        heartContainer = healthUIInstance.transform.Find("HeartContainer");
        if (heartContainer == null)
        {
            Debug.LogError("HeartContainer not found in healthUIPrefab!");
            return;
        }

        InitHearts();
    }

    void InitHearts()
    {
        for (int i = 0; i < health - 1; i++)
        {
            GameObject heart = Instantiate(heartPrefab, heartContainer);
            heartIcons.Add(heart);
        }
    }

    void Update()
    {
        if (player == null || isExploding)
        {
            Debug.LogWarning("Player not assigned or enemy is exploding. Exiting Update.");
            return;
        }

        if (!hasSpawned)
        {
            Debug.LogWarning("EnemyAI has not spawned yet. Exiting Update.");
            return;
        }

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= explodeDistance)
        {
            TriggerExplosion();
            return;
        }

        // Move on ground only
        Vector3 moveDirection = player.position - transform.position;
        moveDirection.y = 0f; // Prevent vertical movement

        Debug.LogWarning("Direction to player: " + moveDirection);

        // float distanceToPlayer = moveDirection.magnitude;
        // if (distanceToPlayer > stopDistance && hasSpawned)
        // {
        //     // transform.position += moveDirection.normalized * speed * Time.deltaTime;
        //     LookAt();
        // }
        // else
        // {
        //     LookAt(); // Even when stopped, still face player
        //     // Optionally: Trigger idle or attack animation here
        // }

        LookAt();

        if (healthUIInstance != null)
            healthUIInstance.transform.rotation = Quaternion.LookRotation(healthUIInstance.transform.position - Camera.main.transform.position); // Face camera
    }

    void LookAt()
    {
        if (player == null) return;

        Vector3 direction = player.position - transform.position;
        direction.y = 0f; // Keep horizontal

        if (direction.sqrMagnitude > 0.001f) // Prevents NaN when very close
        {
            Debug.LogWarning("Enemy is moving and should rotate");
            Quaternion rotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * rotationSpeed);
        }
    }

    public void OnHit()
    {
        health -= 1;
        Debug.LogWarning("Enemy hit! Remaining health: " + health);

        if (hitSound != null && health > 0)
        {
            hitSound.Play();
        }

        StartCoroutine(FlashRed());

        if (heartIcons.Count > 0)
        {
            // Remove the last heart
            GameObject heart = heartIcons[heartIcons.Count - 1];
            heart.SetActive(false);
            heartIcons.RemoveAt(heartIcons.Count - 1);
        }

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // Spawn smoke effect
        if (smokeEffectPrefab != null)
        {
            poofSound.Play();
            Instantiate(smokeEffectPrefab, transform.position, Quaternion.identity);
        }

        // Track kills
        ScoreManager scoreManager = FindObjectOfType<ScoreManager>();
        if (scoreManager != null)
        {
            scoreManager.AddKill();
        }

        // Notify manager and destroy self
        FindObjectOfType<SpellingManager3D>().OnEnemyDefeated(gameObject);
        Destroy(gameObject);
    }

    IEnumerator FlashRed()
    {
        if (enemyRenderer != null)
        {
            enemyRenderer.material.color = hitColor;
            yield return new WaitForSeconds(flashDuration);
            enemyRenderer.material.color = originalColor;
        }
    }

    public void OnSpawnFinished()
    {
        hasSpawned = true;
        InitHealthUI();
    }

    void TriggerExplosion()
    {
        isExploding = true;
        animator.SetTrigger("Explode");

        // Stop movement
        // animationSpeed = 0f;
    }

    public void ExplodeAnimationFinished()
    {
        if (explodeEffectPrefab != null)
        {
            boomSound.Play();
            Instantiate(explodeEffectPrefab, wordCanvas.transform.position + new Vector3(0f, -1f, 0f), Quaternion.identity);
        }

        // FindObjectOfType<SpellingManager3D>().OnEnemyDefeated(gameObject);
        // Destroy(gameObject);

        spellingManager.ClearEnemies();
        spellingManager.ClearSpawnedShurikens();
        spellingManager.letterParent.gameObject.SetActive(false);

        FindObjectOfType<EndGameManager>().TriggerGameOver();
    }
}
