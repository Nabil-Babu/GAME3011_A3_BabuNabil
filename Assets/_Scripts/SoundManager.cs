using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    public AudioSource source;
    public AudioClip ClearMatchSound;
    public bool isAudioPlaying = false;
    [Range(0.1f,1.0f)]
    public float VolumeLevel = 0.1f;
    public void PlayClearSFX()
    {
        if (isAudioPlaying) return;
        StartCoroutine(PlayMatchSound());
    }

    IEnumerator PlayMatchSound()
    {
        isAudioPlaying = true; 
        source.PlayOneShot(ClearMatchSound, VolumeLevel);
        yield return new WaitForSeconds(0.1f);
        isAudioPlaying = false;
    }
}
