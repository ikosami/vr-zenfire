using NS.Util;
using System.Collections;
using UnityEngine;

public class SoundBgm : SingletonMonoBehaviour<SoundBgm>
{
    public float volume = 0.25f;
    const float fadeTime = 0.5f;
    [SerializeField] AudioSource _audioSource;

    // Start is called before the first frame update
    void Start()
    {
    }

    public void Play()
    {
        DontDestroyOnLoad(gameObject);
        StartCoroutine(FadeStart());
    }
    public void Stop(bool isDestory = false)
    {
        if (_audioSource.volume != 1 && isDestory)
        {
            Destroy(gameObject);
            return;
        }

        NS.Util.CoroutineRunner.Instance.Run(FadeStop(isDestory));
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
            timer += Time.deltaTime * (1 / fadeTime);
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
            timer -= Time.deltaTime * (1 / fadeTime);
        }
        if (isDestory)
        {
            Destroy(gameObject);
        }
        else
        {
            _audioSource.volume = 0;
        }
    }
}
