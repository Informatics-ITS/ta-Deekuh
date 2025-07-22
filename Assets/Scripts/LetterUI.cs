using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LetterUI : MonoBehaviour
{
    public TMP_Text letterText; // Assign the Text component in inspector
    public Color defaultColor = Color.white;
    public Color correctColor = Color.green;

    public void SetLetter(char letter)
    {
        letterText.text = letter.ToString();
        letterText.color = defaultColor;
    }

    public void MarkAsCorrect()
    {
        letterText.color = correctColor;
    }
}
