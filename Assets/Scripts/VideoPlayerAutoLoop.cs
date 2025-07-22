using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

[RequireComponent(typeof(VideoPlayer))]
public class VideoPlayerAutoLoop : MonoBehaviour
{
    private VideoPlayer videoPlayer;

    private void Awake()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        videoPlayer.isLooping = true; // Ensure looping is on
    }

    private void OnEnable()
    {
        // Optional safety: restart from beginning
        videoPlayer.frame = 0;
        videoPlayer.Play();
    }
}
