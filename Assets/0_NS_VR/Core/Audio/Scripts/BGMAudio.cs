using NS.Util;
using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// BGM　管理はSoundManager
/// </summary>
public class BGMAudio : MonoBehaviour
{
    [NonSerialized] public float volume = 1f;
    [NonSerialized] public float fadeTimeStart = 1f;
    [NonSerialized] public float fadeTimeStop = 0.25f;
    [SerializeField] AudioSource _audioSource;
    bool isPlay = true;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        _audioSource.volume = volume;
    }

    public void SetAudioClip(AudioClip audioClip)
    {
        _audioSource.clip = audioClip;
    }
    public void Play()
    {
        isPlay = true;
        StartCoroutine(FadeStart());
    }
    public void Stop(bool isDestory = false)
    {
        isPlay = false;
        if (_audioSource.volume != 1 && isDestory)
        {
            Destroy(gameObject);
            return;
        }

        CoroutineRunner.Instance.Run(FadeStop(isDestory));
    }

    public void ChangeBGM(AudioClip audioClip)
    {
        //if (Models.Instance != null)
        //{
        //    if (_modelSoundActive.ActiveStateBGM != SoundState.On)
        //    {
        //        //BGMがオフの場合は、音源のみ変える
        //        _audioSource.clip = audioClip;
        //        return;
        //    }
        //}

        StartCoroutine(ChangeBGMIE(audioClip));
    }
    public IEnumerator ChangeBGMIE(AudioClip audioClip)
    {
        if (isPlay)
        {
            yield return FadeStop(false);
        }
        _audioSource.clip = audioClip;
        yield return FadeStart();
    }

    private IEnumerator FadeStart()
    {
        _audioSource.Play();
        float timer = 0;
        _audioSource.volume = timer;
        while (timer < 1)
        {
            _audioSource.volume = timer * volume;
            yield return null;
            timer += Time.deltaTime * (1 / (fadeTimeStart));
        }
        _audioSource.volume = volume;
    }

    private IEnumerator FadeStop(bool isDestory)
    {
        float timer = 1;
        while (timer > 0)
        {
            _audioSource.volume = timer * volume;
            yield return null;
            timer -= Time.deltaTime * (1 / fadeTimeStop);
        }
        _audioSource.volume = 0;
        if (isDestory)
        {
            Destroy(gameObject);
        }
    }
}
