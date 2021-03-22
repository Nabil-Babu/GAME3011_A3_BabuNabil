using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    public AudioSource source;
    public AudioClip ClearMatchSound;

    public void PlayClearSFX()
    {
        source.PlayOneShot(ClearMatchSound, 0.1f);
    }
}
