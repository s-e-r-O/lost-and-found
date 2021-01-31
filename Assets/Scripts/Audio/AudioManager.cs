using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviourSingleton<AudioManager>
{
    public Sound[] sounds;
    public float lowVolume = 0.1f;
    private string background;
    private Stack<float> _originalVolume;
    private Stack<float> OriginalVolume {
        get { 
        if (_originalVolume == null)
            {
                _originalVolume = new Stack<float>();
            }
            return _originalVolume;
        }
    }
    private new void Awake()
    {
        base.Awake();
        foreach (var sound in sounds)
        {
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;
            sound.source.volume = sound.volume;
            sound.source.pitch = sound.pitch;
            sound.source.loop = sound.loop;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Play(string name, bool follow = false, float delay = 0f)
    {
        var sound = Array.Find(sounds, s => s.name == name);
        if (sound == null)
        {
            Debug.LogWarning("Sound: " + name + " doesn't exist");
            return;
        }
        if (sound.source == null || (follow && sound.source.isPlaying)) {
            return;
        }
        if (sound.pitchMinRange != 0 && sound.pitchMaxRange != 0)
        {
            sound.source.pitch = UnityEngine.Random.Range(sound.pitchMinRange, sound.pitchMaxRange);
        }
        sound.source.volume = 0f;
        sound.source.Play();
        sound.source.DOFade(sound.volume, delay);
    }
    public void Stop(string name, float delay)
    {
        var sound = Array.Find(sounds, s => s.name == name);
        if (sound == null)
        {
            Debug.LogWarning("Sound: " + name + " doesn't exist");
            return;
        }
        StartCoroutine(StopDelay(sound.source, delay));
    }

    public void Reverse()
    {
        var sound = Array.Find(sounds, s => s.name == background);
        if (sound == null)
        {
            Debug.LogWarning("Sound: " + name + " doesn't exist");
            return;
        }
        sound.source.DOPitch(-1f, 0.5f);
    }

    public void Resume()
    {
        var sound = Array.Find(sounds, s => s.name == background);
        if (sound == null)
        {
            Debug.LogWarning("Sound: " + name + " doesn't exist");
            return;
        }
        sound.source.DOPitch(1f, 0.5f);
    }
    private IEnumerator StopDelay(AudioSource source, float delay)
    {
        source.DOFade(0, delay);
        yield return new WaitForSeconds(delay);
        source.Stop();
    }

    public void ChangeBackgroundMusic(string name, float delay = 1f)
    {
        if (background != name)
        {
            if (background != null)
            {
                Stop(background, delay);
            }
            background = name;
            Play(background, true, delay);
                OriginalVolume.Clear();
            
        }
    }

    public void ReduceBackgroundVolume(float? vol = null)
    {

        var sound = Array.Find(sounds, s => s.name == background);
        if (sound == null)
        {
            Debug.LogWarning("There's no background sound");
            return;
        }
        OriginalVolume.Push(sound.source.volume);
        sound.source.volume = vol.HasValue? vol.Value : lowVolume;
    }
    
    public void RestoreBackgroundVolume()
    {
        var sound = Array.Find(sounds, s => s.name == background);
        if (sound == null)
        {
            Debug.LogWarning("There's no background sound");
            return;
        }
        if (OriginalVolume.Count > 0)
        {
            var vol = OriginalVolume.Pop();
            if (OriginalVolume.Count == 0)
            {
                sound.source.volume = sound.volume;
            } 
            else
            {
                sound.source.volume = vol;
            }
        }
    }
}

[Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
    [Range(0,3f)]
    public float volume;
    [Range(-1f,3f)]
    public float pitch;
    [Range(0.1f, 3f)]
    public float pitchMinRange = 0;
    [Range(0.1f, 3f)]
    public float pitchMaxRange = 0;
    public bool loop;
    [HideInInspector]
    public AudioSource source;
}
