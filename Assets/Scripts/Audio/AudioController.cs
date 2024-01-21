using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioController : MonoBehaviour
{

    public AudioMixerGroup mixerGroup;

    public Sound[] sounds;

    void Awake()
    {

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

        s.source.volume = s.volume * (1f + UnityEngine.Random.Range(-s.volumeVariance / 2f, s.volumeVariance / 2f));
        s.source.pitch = s.pitch * (1f + UnityEngine.Random.Range(-s.pitchVariance / 2f, s.pitchVariance / 2f));
        s.source.spatialBlend = s.spatialBlend;
        s.source.Play();
    }

    //creo nuova istanza per traccia audio per evitare interruzione suono
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


    public void StopPlayingAll()
    {
        foreach (Sound s in sounds)
        {
            s.source.Stop();
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

    //metodo chiamato durante animazione camminata e corsa
    public void footStep()
    {
        PlayOverlappingSound("footstep");
    }

}
