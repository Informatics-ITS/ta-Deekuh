using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HomeConfirmation : MonoBehaviour
{
    // public GameObject homeButton;
    // public GameObject confirmButton;
    // public GameObject cancelButton;
    // public GameObject confirmationText;
    // public GameObject hintButton;

    public GameObject buttonsToHide;
    public GameObject buttonsToShow;

    public string sceneToLoad = "MainMenu";

    public void OnHomePressed()
    {
        // homeButton.SetActive(false);
        // hintButton.SetActive(false);
        buttonsToHide.SetActive(false);
        // confirmButton.SetActive(true);
        // cancelButton.SetActive(true);
        // confirmationText.SetActive(true);
        buttonsToShow.SetActive(true);
        Time.timeScale = 0; // Pause the game
    }

    public void OnCancelPressed()
    {
        // confirmButton.SetActive(false);
        // cancelButton.SetActive(false);
        // confirmationText.SetActive(false);
        buttonsToShow.SetActive(false);
        // homeButton.SetActive(true);
        // hintButton.SetActive(true);
        buttonsToHide.SetActive(true);
        Time.timeScale = 1; // Resume the game
    }

    public void OnConfirmPressed()
    {
        Time.timeScale = 1; // Resume the game
        SceneManager.LoadScene(sceneToLoad);
    }
}
