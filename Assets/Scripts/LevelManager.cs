using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelManager : MonoBehaviour
{
    [Header("Levels")]
    public List<LevelData> levels;
    public int currentLevelIndex = 0;

    [Header("References")]
    public SpellingManager3D spellingManager;
    public ScoreManager scoreManager;
    public TMP_Text scoreText;


    private void Start()
    {
        spellingManager.OnLevelComplete += AdvanceToNextLevel;
        if (levels.Count == 0)
        {
            Debug.LogWarning("No levels defined!");
            return;
        }
        LoadLevel(currentLevelIndex);
        Debug.LogWarning("Current level: " + currentLevelIndex);
    }

    public void LoadLevel(int index)
    {
        if (index < 0 || index >= levels.Count)
        {
            Debug.LogWarning("Invalid level index: " + index);
            return;
        }

        if (scoreText != null)
        {
            scoreText.text = $"Level {currentLevelIndex + 1}";
            Debug.Log($"Current Level: {currentLevelIndex + 1}");
        }

        spellingManager.LoadLevel(levels[index]);
    }

    public void AdvanceToNextLevel()
    {
        currentLevelIndex++;

        if (currentLevelIndex >= levels.Count)
        {
            // currentLevelIndex = 0; // Loop or end
            Debug.LogWarning("All levels completed!");
            FindObjectOfType<EndGameManager>().TriggerVictory();
        }

        LoadLevel(currentLevelIndex);
    }

    // New method to update level words from GPT JSON array string
    public void UpdateLevelsFromGPT(string jsonArrayString)
    {
        try
        {
            // Parse the JSON array string into a list of strings
            List<string> words = JsonUtilityWrapper.ParseJsonArray(jsonArrayString);

            // Make sure you have enough levels to update
            int count = Mathf.Min(words.Count, levels.Count);

            for (int i = 0; i < count; i++)
            {
                levels[i].word = words[i];
                Debug.Log($"Updated Level {i} word from GPT: {levels[i].word}");
            }

            Debug.Log("Updated LevelData words from GPT response.");
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to parse GPT response or update levels: " + e.Message);
        }

        LoadLevel(currentLevelIndex);
    }
}
