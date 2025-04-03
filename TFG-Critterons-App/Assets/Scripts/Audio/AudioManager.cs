using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager m;
    [SerializeField]
    AudioSource clickSound, winSound, defeatSound, criterronStopSound, combatSound, newLevelSound, upgradeSound, changeSceneSound, hitSoundBase;
    [SerializeField]
    int hitInstances;
    [SerializeField]
    float pitchVariationHit = 0.2f;


    AudioSource[] hitSounds;

    private void Awake()
    {
        if (m == null)
        {
            m = this;
            DontDestroyOnLoad(gameObject);
            LoadMultiSounds();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void LoadMultiSounds()
    {
        hitSounds = new AudioSource[hitInstances];
        hitSounds[0] = hitSoundBase;
        for (int i = 1; i < hitInstances; i++)
        {
            hitSounds[i] = Instantiate(hitSoundBase.gameObject, hitSoundBase.transform).GetComponent<AudioSource>();
            hitSounds[i].pitch = hitSoundBase.pitch + Random.Range(-pitchVariationHit / 2, pitchVariationHit / 2);
        }
    }

    public void PlaySound(string soundName, bool lowPriority = false)
    {
        AudioSource sound = null;

        switch (soundName.ToLower())
        {
            case "click": sound = clickSound; break;
            case "win": sound = winSound; break;
            case "defeat": sound = defeatSound; break;
            case "combat": sound = combatSound; break;
            case "stop": sound = criterronStopSound; break;
            case "newlevel": sound = newLevelSound; break;
            case "upgrade": sound = upgradeSound; break;
            case "changescene": sound = changeSceneSound; break;
        }

        if (sound != null && (!lowPriority || !sound.isPlaying))
        {
            sound.Play();
        }
    }


    public void Hit()
    {
        foreach (var item in hitSounds)
        {
            if (!item.isPlaying)
            {
                item.Play();
                break;
            }
        }
    }
}
