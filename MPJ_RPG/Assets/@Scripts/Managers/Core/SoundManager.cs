using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager
{
    private AudioSource[] _audioSources = new AudioSource[(int)Define.ESound.Max];
    private Dictionary<string, AudioClip> _audioClips = new Dictionary<string, AudioClip>();
    private GameObject _soundRoot = null;

    public void Init()
    {
        if(_soundRoot = null)
        {
            _soundRoot = GameObject.Find("@SoundRoot");

            if(_soundRoot == null)
            {
                _soundRoot = new GameObject("@SoundRoot");
                UnityEngine.Object.DontDestroyOnLoad(_soundRoot);

                string[] soundTypeNames = Enum.GetNames(typeof(Define.ESound));

                for(int i = 0; i < soundTypeNames.Length - 1; i++)
                {
                    GameObject go = new GameObject(soundTypeNames[i]);
                    _audioSources[i] = go.AddComponent<AudioSource>();
                    go.transform.parent = _soundRoot.transform;
                }

                _audioSources[(int)Define.ESound.Bgm].loop = true;
            }
        }
    }

    public void Clear()
    {
        foreach (AudioSource audioSource in _audioSources)
            audioSource.Stop();

        _audioClips.Clear();
    }

    public void Play(Define.ESound type)
    {
        _audioSources[(int)type].Play();
    }

    /// <summary>
    /// key에 해당하는 Audio Clip을 재생한다.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="key">Audio clip</param>
    /// <param name="pitch"></param>
    public void Play(Define.ESound type, string key, float pitch = 1.0f)
    {
        AudioSource audioSource = _audioSources[(int)type];

        if(type == Define.ESound.Bgm)
        {
            LoadAudioClip(key, (clip) =>
            {
                if (audioSource.isPlaying)
                    audioSource.Stop();

                audioSource.clip = clip;
                audioSource.Play();
            });
        }
        else
        {
            LoadAudioClip(key, (clip) =>
            {
                audioSource.pitch = pitch;
                audioSource.PlayOneShot(clip);
            });
        }
    }

    public void Play(Define.ESound type, AudioClip clip, float pitch = 1.0f)
    {
        AudioSource audioSource = _audioSources[(int)type];

        if(type == Define.ESound.Bgm)
        {
            if (audioSource.isPlaying)
                audioSource.Stop();

            audioSource.clip = clip;
            audioSource.Play();
        }
        else
        {
            audioSource.pitch = pitch;
            audioSource.PlayOneShot(clip);
        }
    }

    private void LoadAudioClip(string key, Action<AudioClip> action)
    {
        AudioClip clip = null;
        if(_audioClips.TryGetValue(key, out clip))
        {
            action?.Invoke(clip);
            return;
        }

        clip = Managers.Resource.Load<AudioClip>(key);

        if(_audioClips.ContainsKey(key) == false)
            _audioClips.Add(key, clip);

        action?.Invoke(clip);
    }

    public void Stop(Define.ESound type)
    {
        _audioSources[(int)type].Stop();
    }
}
