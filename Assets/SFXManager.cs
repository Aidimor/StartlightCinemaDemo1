using UnityEngine;
using System.Collections.Generic;

public class SFXManager : MonoBehaviour
{
    public static SFXManager instance;

    [Header("Audio Source Template")]
    public AudioSource audioSourcePrefab;

    [Header("Clips de Sonido")]
    public AudioClip[] soundClips;

    private Dictionary<string, AudioClip> soundDictionary;
    private Dictionary<string, AudioSource> activeSounds = new Dictionary<string, AudioSource>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }


        soundDictionary = new Dictionary<string, AudioClip>();
        foreach (AudioClip clip in soundClips)
        {
            if (clip != null && !soundDictionary.ContainsKey(clip.name))
            {
                soundDictionary.Add(clip.name, clip);
            }
        }
    }

    /// ?? Para sonidos cortos (no se pueden detener)
    public void PlaySound(string clipName)
    {
        if (soundDictionary.ContainsKey(clipName))
        {
            AudioSource src = CreateNewSource();
            src.loop = false;
            src.PlayOneShot(soundDictionary[clipName]);
            Destroy(src.gameObject, soundDictionary[clipName].length);
        }
        else
        {
            Debug.LogWarning("No se encontró el sonido: " + clipName);
        }
    }

    /// ?? Para sonidos largos (pueden detenerse)
    public void PlayStoppableSound(string clipName)
    {
        if (soundDictionary.ContainsKey(clipName))
        {
            // Si ya está sonando, no lo repitas
            if (activeSounds.ContainsKey(clipName))
                return;

            AudioSource src = CreateNewSource();
            src.clip = soundDictionary[clipName];
            src.loop = false; // se reproduce una sola vez
            src.Play();

            activeSounds.Add(clipName, src);
        }
        else
        {
            Debug.LogWarning("No se encontró el sonido: " + clipName);
        }
    }

    public void StopStoppableSound(string clipName)
    {
        if (activeSounds.ContainsKey(clipName))
        {
            AudioSource src = activeSounds[clipName];
            src.Stop();
            Destroy(src.gameObject);
            activeSounds.Remove(clipName);
        }
    }

    private AudioSource CreateNewSource()
    {
        AudioSource newSrc;
        if (audioSourcePrefab != null)
        {
            newSrc = Instantiate(audioSourcePrefab, transform);
        }
        else
        {
            newSrc = new GameObject("AudioSource_" + Random.value).AddComponent<AudioSource>();
            newSrc.transform.parent = transform;
        }
        return newSrc;
    }
}
