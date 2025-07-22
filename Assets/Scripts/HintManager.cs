using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HintManager : MonoBehaviour
{
    [Header("Hint Settings")]
    [SerializeField] private GameObject scrollHint;
    [SerializeField] private GameObject[] handPoseObjects;        // One pose object per letter (A–Z)
    [SerializeField] private TMP_Text hintCounterText;
    [SerializeField] private int maxHintsPerLevel = 3;

    [Header("Dependencies")]
    [SerializeField] private SpellingManager3D spellingManager; // <-- Add your manager that knows the current letter index

    private int hintsRemaining;
    private bool hintActive = false;
    private int activeHintIndex = -1;

    void Start()
    {
        ResetHints();
    }

    public void UseHint()
    {
        if (hintsRemaining <= 0 || hintActive || spellingManager.currentPhase == SpellingManager3D.GamePhase.Shooting) return;

        int currentIndex = spellingManager.GetCurrentLetterIndex(); // e.g. 0 = A, 25 = Z
        ShowHint(currentIndex);
        hintsRemaining--;
        UpdateHintUI();
    }

    private void ShowHint(int index)
    {
        hintActive = true;
        activeHintIndex = index;
        scrollHint.SetActive(true);

        for (int i = 0; i < handPoseObjects.Length; i++)
        {
            handPoseObjects[i].SetActive(i == index);
        }
    }

    public void TryHideHintIfSolved()
    {
        if (!hintActive) return;

        int currentIndex = spellingManager.GetCurrentLetterIndex();
        if (currentIndex != activeHintIndex) return;

        if (spellingManager.IsCurrentPoseCorrect())
        {
            HideHint();
        }
    }

    private void HideHint()
    {
        hintActive = false;
        activeHintIndex = -1;
        scrollHint.SetActive(false);

        for (int i = 0; i < handPoseObjects.Length; i++)
        {
            handPoseObjects[i].SetActive(false);
        }
    }

    public void ResetHints()
    {
        hintsRemaining = maxHintsPerLevel;
        UpdateHintUI();
    }

    private void UpdateHintUI()
    {
        if (hintCounterText != null)
        {
            hintCounterText.text = "Hints: " + hintsRemaining;
        }
    }
}
