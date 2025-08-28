using UnityEngine;

public class AudioSourceCustom : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private string _seKey;
    [SerializeField] private bool _isLoop = false;
    [SerializeField] private bool _isPlayOnAwake = false;
    [SerializeField] private bool _isOneShot = false;

    private bool _isInited = false;

    private void Awake()
    {
        if (_isPlayOnAwake)
        {
            Init();
            _audioSource.Play();
        }
    }

    private void Init()
    {
        if (_isInited) return;
        _audioSource.clip = SoundManager.Instance.GetSound(_seKey);
        _audioSource.loop = _isLoop;
        _isInited = true;
    }

    public void SetClip(string name)
    {
        _seKey = name;
        _audioSource.clip = SoundManager.Instance.GetSound(_seKey);
    }

    public void SetTime(float time)
    {
        _audioSource.time = time;
    }

    public void Play()
    {
        Init();
        _audioSource.Play(); // Use Play instead of PlayOneShot to respect the set time
        if (_isOneShot)
        {
            Destroy(gameObject, _audioSource.clip.length - _audioSource.time); // Adjust destruction time
        }
    }

    public void Stop()
    {
        _audioSource.Stop();
    }
}
