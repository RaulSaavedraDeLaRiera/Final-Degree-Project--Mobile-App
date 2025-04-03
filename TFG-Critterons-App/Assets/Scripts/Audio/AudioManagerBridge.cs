using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManagerBridge : MonoBehaviour
{
    public void PlaySound(string soundName)
    {
        if (AudioManager.m != null)
        {
            if (soundName.ToLower() == "hit")
            {
                AudioManager.m.Hit();
            }
            else
            {
                AudioManager.m.PlaySound(soundName, false);
            }
        }
    }

    public void PlaySoundLowPriority(string soundName)
    {
        if (AudioManager.m != null)
        {
            if (soundName.ToLower() == "hit")
            {
                AudioManager.m.Hit();
            }
            else
            {
                AudioManager.m.PlaySound(soundName, true);
            }
        }
    }
}
