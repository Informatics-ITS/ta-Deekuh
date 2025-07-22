using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction;
using TMPro;

public class LetterPoseValidator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ScrollController scrollController;      //  Your existing script
    [SerializeField] private ActiveStateGroup[] letterStates;        //  26 groups, A‑Z
    [SerializeField] private GameObject feedbackObject;              //  e.g. green tick
    [SerializeField] private AudioSource feedbackAudio;              //  optional “ding”

    [Header("Feedback Timing")]
    [SerializeField] private float feedbackDuration = 1f;            //  seconds the tick stays on‑screen

    private bool _cooldown;                                          //  prevents spamming
    private float _timer;

    private void Update()
    {
        //  1. Which letter is on‑screen?
        int idx = scrollController.GetCurrentLetterIndex();          //  0 = A, 25 = Z
        if (idx < 0 || idx >= letterStates.Length) return;           //  safety

        //  2. Is its recogniser group active?
        bool poseCorrect = letterStates[idx].Active;

        //  3. If correct & not already showing feedback … give feedback
        if (poseCorrect && !_cooldown)
        {
            ShowFeedback();
        }

        //  4. Auto‑hide feedback after the duration
        if (_cooldown)
        {
            _timer += Time.deltaTime;
            if (_timer >= feedbackDuration)
            {
                HideFeedback();
            }
        }
    }

    private void ShowFeedback()
    {
        if (feedbackObject)  feedbackObject.SetActive(true);
        if (feedbackAudio)   feedbackAudio.Play();

        _cooldown = true;
        _timer    = 0f;
    }

    private void HideFeedback()
    {
        if (feedbackObject)  feedbackObject.SetActive(false);

        _cooldown = false;
        _timer    = 0f;
    }
}
