using UnityEngine.Audio;
using System;
using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{

    public static AudioManager instance;

    public AudioMixerGroup mixerGroup;

    public Sound[] sounds;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.loop = s.loop;

            if (s.mixerGroup == null)
                s.source.outputAudioMixerGroup = mixerGroup;
            else
                s.source.outputAudioMixerGroup = s.mixerGroup;
        }
    }

    public void Play(string sound)
    {
        Sound s = Array.Find(sounds, item => item.name == sound);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }


        //creo nuova istanza per traccia audio se effettuo sparo, per evitare interruzione suono
        if (sound.Contains("Shoot"))
        {
            StartCoroutine(shootSound(s));
        }
        else
        {
            s.source.volume = s.volume * (1f + UnityEngine.Random.Range(-s.volumeVariance / 2f, s.volumeVariance / 2f));
            s.source.pitch = s.pitch * (1f + UnityEngine.Random.Range(-s.pitchVariance / 2f, s.pitchVariance / 2f));
            //s.source.spatialBlend = s.spatialBlend;
            s.source.Play();
        }

    }

    IEnumerator shootSound(Sound s)
    {
        AudioSource audioSource = gameObject.AddComponent(typeof(AudioSource)) as AudioSource;
        audioSource.clip = s.clip;
        audioSource.outputAudioMixerGroup = s.mixerGroup;
        audioSource.Play();
        yield return new WaitForSeconds(1f);
        Destroy(audioSource);
    }

    public bool IsPlaying(string sound)
    {
        Sound s = Array.Find(sounds, item => item.name == sound);
        return s.source.isPlaying;
    }


    public void StopPlaying(string sound)
    {
        Sound s = Array.Find(sounds, item => item.name == sound);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

        s.source.volume = s.volume * (1f + UnityEngine.Random.Range(-s.volumeVariance / 2f, s.volumeVariance / 2f));
        s.source.pitch = s.pitch * (1f + UnityEngine.Random.Range(-s.pitchVariance / 2f, s.pitchVariance / 2f));

        s.source.Stop();
    }

    public void PlayOverlappingSound(string sound)
    {
        StartCoroutine(playOverlappingSound(sound));
    }

    IEnumerator playOverlappingSound(string sound)
    {
        Sound s = Array.Find(sounds, item => item.name == sound);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            yield break;
        }

        AudioSource audioSource = gameObject.AddComponent(typeof(AudioSource)) as AudioSource;
        audioSource.clip = s.clip;
        audioSource.outputAudioMixerGroup = s.mixerGroup;
        audioSource.volume = s.volume * (1f + UnityEngine.Random.Range(-s.volumeVariance / 2f, s.volumeVariance / 2f));
        audioSource.pitch = s.pitch * (1f + UnityEngine.Random.Range(-s.pitchVariance / 2f, s.pitchVariance / 2f));
        audioSource.spatialBlend = s.spatialBlend;
        audioSource.Play();
        yield return new WaitForSeconds(1f);
        Destroy(audioSource);
    }


    public void StopPlayingAll()
    {
        foreach (Sound s in sounds)
        {
            s.source.Stop();
        }
    }

    public void Resume(string soundName)
    {

        Sound s = Array.Find(sounds, item => item.name == soundName);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

        foreach (Sound sound in sounds)
        {
            if (sound.clip.name.Equals(s.clip.name))
            {
                sound.source.UnPause();
                return;
            }
        }
    }

    public void PauseAll()
    {
        foreach (Sound s in sounds)
        {
            s.source.Pause();
        }
    }

    public void ResumeAll()
    {
        foreach (Sound s in sounds)
        {
            s.source.UnPause();
        }
    }
}
