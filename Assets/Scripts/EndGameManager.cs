using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGameManager : MonoBehaviour
{
    public GameObject gameOverUI;
    public GameObject victoryUI;
    public GameObject progressUI;
    public GameObject homeButton;
    public GameObject exitButton;
    public GameObject retryButton;
    public GameObject hintButton;
    public AudioSource victorySound;
    public GameObject victoryEffectPrefab;

    public void TriggerGameOver()
    {
        progressUI.SetActive(false);
        gameOverUI.SetActive(true);
        ShowButtons();
        ShurikenAimRay.ClearTargetIcon();
    }

    public void TriggerVictory()
    {
        progressUI.SetActive(false);
        victoryUI.SetActive(true);
        victorySound.Play();
        Instantiate(victoryEffectPrefab, victoryUI.transform.position, Quaternion.identity);
        ShowButtons();
        ShurikenAimRay.ClearTargetIcon();
    }

    public void ShowButtons()
    {
        homeButton.SetActive(false);
        exitButton.SetActive(true);
        retryButton.SetActive(true);
        hintButton.SetActive(false);
    }

    public void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitToMainMenu()
    {
        SceneManager.LoadScene("MainMenu"); // Replace with your menu scene name
    }
}
