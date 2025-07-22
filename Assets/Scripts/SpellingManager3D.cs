using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Oculus.Interaction;

public class SpellingManager3D : MonoBehaviour
{

    [Header("Pose Checking")]
    public List<ActiveStateGroup> letterPoses; // A–Z
    public AudioSource correctSound;

    [Header("Letter UI")]
    public Transform letterParent;
    public GameObject letterPrefab;
    public float letterSpacing = 0.5f;
    public Material glowBlueMaterial;
    public Material glowGreenMaterial;
    public GameObject correctWordPrefab;

    [Header("Enemy & Level Setup")]
    public Transform[] enemySpawnPoints;
    public GameObject enemyPrefab;
    public EnemyAI enemyAI;
    // public int killsForRasenShuriken = 5;

    [Header("Shuriken Setup")]
    public GameObject shurikenPrefab;
    public Transform shurikenSpawnParent;
    public int shurikenCount = 5;

    [Header("Spelling Settings")]
    public float letterCooldown = 0.5f; // Cooldown in seconds between letter checks
    private float letterCooldownTimer = 0f;

    public enum GamePhase { Spelling, Shooting }
    public GamePhase currentPhase = GamePhase.Spelling;
    public HintManager hintManager;

    private LevelData currentLevelData;
    private int currentLetterIndex = 0;
    private List<GameObject> currentLetters = new();
    private List<GameObject> currentEnemies = new();
    private List<GameObject> spawnedShurikens = new();
    // private int killCount = 0;

    public System.Action OnWordSpelled;
    public System.Action OnRasenShurikenReady;
    public System.Action OnLevelComplete; // To notify LevelManager

    private void Update()
    {
        if (currentPhase != GamePhase.Spelling || currentLetterIndex >= currentLevelData.word.Length)
            return;

        if (letterCooldownTimer > 0f)
        {
            letterCooldownTimer -= Time.deltaTime;
            return;
        }

        // if (Input.GetKeyDown(KeyCode.Space))
        // {
        //     MarkLetterAsSpelled(currentLetterIndex);
        // }

        char currentChar = currentLevelData.word[currentLetterIndex];
        int charIndex = char.ToUpper(currentChar) - 'A';
        if (charIndex < 0 || charIndex >= letterPoses.Count) return;

        if (letterPoses[charIndex].Active || Input.GetKeyDown(KeyCode.Space))
        {
            hintManager.TryHideHintIfSolved();
            MarkLetterAsSpelled(currentLetterIndex);
            currentLetterIndex++;
            correctSound?.Play();

            // Reset cooldown timer
            letterCooldownTimer = letterCooldown;

            if (currentLetterIndex >= currentLevelData.word.Length)
            {
                Instantiate(correctWordPrefab, letterParent.position, Quaternion.identity);
                OnWordSpelled?.Invoke();
                StartCoroutine(DelayedShootingMode());
                // StartShootingMode();
            }
        }
    }

    public void LoadLevel(LevelData levelData)
    {
        currentLevelData = levelData;
        // killCount = 0;
        currentLetterIndex = 0;
        hintManager.ResetHints();
        currentPhase = GamePhase.Spelling;

        LoadWord(currentLevelData.word);
        SpawnEnemies(currentLevelData.enemyCount);

        // ScoreManager scoreManager = FindObjectOfType<ScoreManager>();
        // if (scoreManager != null)
        // {
        //     scoreManager.OnScoreBarFilled += HandleScoreBarFilled;
        // }
    }

    private void HandleScoreBarFilled()
    {
        // EndShootingMode(); // same as when all enemies are dead
        // StartCoroutine(DelayedEndShootingMode(2f));
        // ClearSpawnedShurikens();
    }

    void OnDestroy()
    {
        // var scoreManager = FindObjectOfType<ScoreManager>();
        // if (scoreManager != null)
        //     scoreManager.OnScoreBarFilled -= HandleScoreBarFilled;
    }

    private void LoadWord(string word)
    {
        foreach (Transform child in letterParent)
            Destroy(child.gameObject);

        currentLetters.Clear();

        float startX = -((word.Length - 1) * letterSpacing) / 2f;

        letterParent.gameObject.SetActive(true);

        for (int i = 0; i < word.Length; i++)
        {
            char c = word[i];
            Vector3 pos = new Vector3(startX + i * letterSpacing, 0, 0);
            GameObject letterObj = Instantiate(letterPrefab, letterParent);
            letterObj.transform.localPosition = pos;

            var tmp = letterObj.GetComponent<TextMeshPro>();
            tmp.text = c.ToString().ToUpper();
            tmp.fontSharedMaterial = glowBlueMaterial;

            var pulser = letterObj.GetComponent<GrowShrink>();
            if (pulser != null) pulser.enabled = (i == 0);

            currentLetters.Add(letterObj);
        }
    }

    public void MarkLetterAsSpelled(int index)
    {
        if (index < 0 || index >= currentLetters.Count) return;

        var tmp = currentLetters[index].GetComponent<TextMeshPro>();
        tmp.fontSharedMaterial = glowGreenMaterial;

        var pulser = currentLetters[index].GetComponent<GrowShrink>();
        if (pulser != null) pulser.enabled = false;

        if (index + 1 < currentLetters.Count)
        {
            var nextPulser = currentLetters[index + 1].GetComponent<GrowShrink>();
            if (nextPulser != null) nextPulser.enabled = true;
        }
    }

    private void StartShootingMode()
    {
        currentPhase = GamePhase.Shooting;
        letterParent.gameObject.SetActive(false);
        SpawnShurikens();
    }

    IEnumerator DelayedShootingMode()
    {
        yield return new WaitForSeconds(1f); // Delay before starting shooting mode
        StartShootingMode();
    }

    private void SpawnEnemies(int count)
    {
        ClearEnemies();

        enemyPrefab.SetActive(true); // Ensure the enemy prefab is active

        for (int i = 0; i < count; i++)
        {
            Transform spawnPoint = enemySpawnPoints[i % enemySpawnPoints.Length];
            GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);

            EnemyAI enemyAI = enemy.GetComponent<EnemyAI>();
            if (enemyAI != null)
            {
                enemyAI.Initialize(currentLevelData.enemyHP);
            }
            else
            {
                Debug.LogWarning("EnemyAI component not found on enemy prefab!");
            }

            currentEnemies.Add(enemy);
        }

        enemyPrefab.SetActive(false); // Deactivate the prefab to avoid multiple instantiations
    }

    public void ClearEnemies()
    {
        // foreach (var e in currentEnemies)
        // {
        //     if (e != null)
        //     {
        //         currentEnemies.Clear();
        //     }
        // }

        for (int i = currentEnemies.Count - 1; i >= 0; i--)
        {
            Destroy(currentEnemies[i].gameObject);
            currentEnemies.RemoveAt(i);
        }
    }

    public void ClearEnemiesWithPoof()
    {
        foreach (var e in currentEnemies)
        {
            if (e != null)
            {
                enemyAI.poofSound?.Play();
                Instantiate(enemyAI.smokeEffectPrefab, e.transform.position, Quaternion.identity);

                Destroy(e);
            }
        }
        currentEnemies.Clear();
    }

    private void SpawnShurikens()
    {
        ClearSpawnedShurikens();

        for (int i = 0; i < shurikenCount; i++)
        {
            Vector3 spawnPos = shurikenSpawnParent.position;

            GameObject shuriken = Instantiate(shurikenPrefab, spawnPos, shurikenSpawnParent.rotation);
            shuriken.transform.SetParent(shurikenSpawnParent);
            spawnedShurikens.Add(shuriken);
        }
    }

    public void ClearSpawnedShurikens()
    {
        foreach (GameObject s in spawnedShurikens)
            if (s != null) Destroy(s);
        spawnedShurikens.Clear();
    }

    public void OnEnemyDefeated(GameObject enemy)
    {
        currentEnemies.Remove(enemy);
        Destroy(enemy);

        // killCount++;
        // if (killCount % killsForRasenShuriken == 0)
        // {
        //     OnRasenShurikenReady?.Invoke();
        // }

        if (currentEnemies.Count == 0)
        {
            // EndShootingMode();
            StartCoroutine(DelayedEndShootingMode(1f)); // Delay before ending shooting mode
            ClearSpawnedShurikens();
        }
    }

    private void EndShootingMode()
    {
        currentPhase = GamePhase.Spelling;
        OnLevelComplete?.Invoke(); // Notify LevelManager
    }

    
    public IEnumerator DelayedEndShootingMode(float delaySeconds)
    {
        yield return new WaitForSeconds(delaySeconds);
        EndShootingMode();
    }

    public int GetCurrentLetterIndex()
    {
        if (string.IsNullOrEmpty(currentLevelData.word) || currentLetterIndex >= currentLevelData.word.Length)
            return -1;

        char letter = currentLevelData.word[currentLetterIndex];  // e.g. 'F'
        return letter - 'A';                            // e.g. 5
    }

    public bool IsCurrentPoseCorrect()
    {
        int idx = GetCurrentLetterIndex();
        if (idx < 0 || idx >= letterPoses.Count) return false;

        return letterPoses[idx].Active;
    }
}
