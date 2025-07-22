using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using OpenAI;

public class GameModeConfirmation : MonoBehaviour
{
    public enum GameMode { Training, Jutsu }

    [System.Serializable]
    public struct GameModeData
    {
        public GameObject confirmationText;
        public GameObject canvas;
        public string sceneName;
    }

    public GameObject trainingButton;
    public GameObject jutsuButton;
    public GameObject confirmButton;
    public GameObject cancelButton;
    public VideoPlayer trainingVideoPlayer;
    public VideoPlayer jutsuVideoPlayer;

    public GameModeData trainingMode;
    public GameModeData jutsuMode;
    public ChatGPT chatGPT;

    private GameMode? selectedMode = null;

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            chatGPT.SendReply();
            SceneManager.LoadScene(jutsuMode.sceneName, LoadSceneMode.Single);
        }
    }

    public void OnTrainingPressed()
    {
        ShowConfirmation(GameMode.Training);
        trainingVideoPlayer.frame = 0; // Reset video to start
        trainingVideoPlayer.Play(); // Start playing the training video
    }

    public void OnJutsuPressed()
    {
        ShowConfirmation(GameMode.Jutsu);
        jutsuVideoPlayer.frame = 0; // Reset video to start
        jutsuVideoPlayer.Play(); // Start playing the jutsu video
    }

    private void ShowConfirmation(GameMode mode)
    {
        selectedMode = mode;

        // Hide buttons
        trainingButton.SetActive(false);
        jutsuButton.SetActive(false);

        // Show common UI
        confirmButton.SetActive(true);
        cancelButton.SetActive(true);

        // Show selected mode content
        if (mode == GameMode.Training)
        {
            trainingMode.confirmationText.SetActive(true);
            trainingMode.canvas.SetActive(true);
        }
        else
        {
            jutsuMode.confirmationText.SetActive(true);
            jutsuMode.canvas.SetActive(true);
        }
    }

    public void OnCancelPressed()
    {
        confirmButton.SetActive(false);
        cancelButton.SetActive(false);

        trainingMode.confirmationText.SetActive(false);
        jutsuMode.confirmationText.SetActive(false);
        trainingMode.canvas.SetActive(false);
        jutsuMode.canvas.SetActive(false);

        jutsuButton.SetActive(true);
        trainingButton.SetActive(true);

        selectedMode = null;
    }

    public void OnConfirmPressed()
    {
        if (selectedMode == GameMode.Training)
        {
            SceneManager.LoadScene(trainingMode.sceneName, LoadSceneMode.Single);
        }
        else if (selectedMode == GameMode.Jutsu)
        {
            chatGPT.SendReply();
            SceneManager.LoadScene(jutsuMode.sceneName, LoadSceneMode.Single);
        }
    }
}
