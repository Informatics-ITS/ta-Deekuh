using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    [Header("UI")]
    public Slider scoreSlider;
    // public TMP_Text scoreText;
    public RectTransform handleTransform; // Assign in Inspector or find via script
    public float rotationSpeed = 180f;    // Degrees per second

    public Image fillImage;
    public Color minGlowColor, maxGlowColor;
    private Color currentGlowColor;

    public float glowFadeSpeed = 5f;



    [Header("Progress Settings")]
    public int maxKills = 5;
    public int currentKills;

    private float displayedSliderValue = 0f;
    public float fillSpeed = 2f; // Adjust speed to your liking

    public LevelManager levelManager; 
    public System.Action OnScoreBarFilled;

    private void Start()
    {
        if (scoreSlider == null)
        {
            Debug.LogError("Score Slider not assigned in the Inspector!");
        }

        if (handleTransform == null && scoreSlider != null)
        {
            handleTransform = scoreSlider.transform.Find("Handle Slide Area/Handle")?.GetComponent<RectTransform>();
            if (handleTransform == null)
            {
                Debug.LogError("Handle not found. Check your slider hierarchy.");
            }
        }

        ColorUtility.TryParseHtmlString("#5ABFFF", out minGlowColor); // Cyan
        ColorUtility.TryParseHtmlString("#7CFCFF", out maxGlowColor); // Green

        currentGlowColor = minGlowColor;
        fillImage.color = currentGlowColor;

        ResetScore();
    }

    private void Update()
    {
        if (scoreSlider != null)
        {
            scoreSlider.value = Mathf.Lerp(scoreSlider.value, displayedSliderValue, Time.deltaTime * fillSpeed);
        }

        if (handleTransform != null)
        {
            handleTransform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
        }
    }

    public void AddKill()
    {
        currentKills++;
        currentKills = Mathf.Clamp(currentKills, 0, maxKills);
        UpdateSlider();

        if (currentKills >= maxKills)
        {
            Debug.Log("Rasengan Ready!");
            OnScoreBarFilled?.Invoke(); // Trigger Rasengan or similar effect
        }
    }

    public void ResetScore()
    {
        currentKills = 0;
        UpdateSlider();
    }

    private void UpdateSlider()
    {
        float sliderValue = (float)currentKills / maxKills;
        displayedSliderValue = sliderValue;

        // if (scoreText != null)
        // {
        //     scoreText.text = $"Level {levelManager.currentLevelIndex + 1}";
        //     Debug.Log($"Current Level: {levelManager.currentLevelIndex + 1}");
        // }

        if (fillImage != null)
        {
            // fillImage.color = Color.Lerp(minGlowColor, maxGlowColor, sliderValue);

            Color targetGlowColor = Color.Lerp(minGlowColor, maxGlowColor, sliderValue);
            currentGlowColor = Color.Lerp(currentGlowColor, targetGlowColor, Time.deltaTime * glowFadeSpeed);
            fillImage.color = currentGlowColor;
        }
    }
}
