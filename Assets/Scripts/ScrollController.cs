using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScrollController : MonoBehaviour
{
    [SerializeField]
    public TMP_Text letterText; // reference to the text that shows current letter
    public GameObject[] handPoseObjects; // one per letter (A–Z)
    public AudioSource scrollAudio;

    private int currentIndex = 0;

    void Start()
    {
        UpdateDisplay();
    }

    // void Update()
    // {
    //     // Check for input to go to the next or previous letter
    //     if (Input.GetKeyDown(KeyCode.RightArrow))
    //     {
    //         GoToNextLetter();
    //     }
    //     else if (Input.GetKeyDown(KeyCode.LeftArrow))
    //     {
    //         GoToPreviousLetter();
    //     }
    // }

    public void GoToNextLetter()
    {
        currentIndex = (currentIndex + 1) % 26;
        Debug.Log("Next button pressed");
        UpdateDisplay();
    }

    public void GoToPreviousLetter()
    {
        currentIndex = (currentIndex - 1 + 26) % 26;
        Debug.Log("Prev button pressed");
        UpdateDisplay();
    }

    void UpdateDisplay()
    {
        char currentLetter = (char)('A' + currentIndex);
        letterText.text = currentLetter.ToString();

        for (int i = 0; i < handPoseObjects.Length; i++)
            handPoseObjects[i].SetActive(i == currentIndex);

        scrollAudio.Play(); // Play the scroll sound
    }

    public int GetCurrentLetterIndex()
    {
        return currentIndex;
    }
}
