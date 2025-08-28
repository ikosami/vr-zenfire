using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : NS.Util.SingletonMonoBehaviour<SoundManager>
{
    [System.Serializable]
    public class SoundSet
    {
        public string Name;
        public AudioClip Sound;
        public bool PitchCustom = false;
        public bool VolumeCustom = false;
        public float Pitch = 1f;
        public float Volume = 1f;
    }

    [SerializeField] string _bgmName = "bgm";

    [SerializeField] BGMAudio _bgmAudio;

    [SerializeField] SoundSet[] _sounds;
    [SerializeField] SoundSetData[] _soundSetDatas;
    [SerializeField] AudioSource _audioSource;

    List<SoundSet> soundSets = new List<SoundSet>();
    public List<SoundSet> SoundSets
    {
        get
        {
            if (soundSets.Count == 0)
            {
                soundSets.AddRange(_sounds);
                foreach (var soundSetData in _soundSetDatas)
                {
                    soundSets.AddRange(soundSetData.Data);
                }
            }
            return soundSets;
        }
    }


    float _defaultVolume = 1f;
    float _defaultPitch = 1f;
    
    int _index = -1;

    float _coolTime = 0;
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        _bgmAudio.SetAudioClip(SoundSets.Find(x => x.Name == _bgmName).Sound);
        PlayBGM();
        _defaultVolume = _audioSource.volume;
        _audioSource.outputAudioMixerGroup.audioMixer.GetFloat("Pitch", out _defaultPitch);
    }

    void Update() {
        if(_coolTime > 0) {
            _coolTime -= Time.deltaTime;
        }
    }


    public bool Play(string name)
    {
        if(_coolTime > 0) {
            return false;
        }
        _coolTime = 0.1f;
        bool isMissing = true;
        for (int i = 0; i < SoundSets.Count; i++)
        {
            if (SoundSets[i].Name == name)
            {
                _audioSource.outputAudioMixerGroup.audioMixer.SetFloat("Pitch", SoundSets[i].PitchCustom ? SoundSets[i].Pitch : _defaultPitch);
                _audioSource.volume = SoundSets[i].VolumeCustom ? SoundSets[i].Volume : _defaultVolume;
                _audioSource.PlayOneShot(SoundSets[i].Sound);
                isMissing = false;
                break;
            }
        }
        if (isMissing)
        {
            Debug.LogError($"Sound Missing {name}");
        }
        return !isMissing;
    }

    public void Play(string name, float duration) {
        if(Play(name)) {
            StartCoroutine(StopAfter(name, duration));
        }
    }

    IEnumerator StopAfter(string name, float duration) {
        yield return new WaitForSeconds(duration);
        _audioSource.Stop();
    }

    public void StopBGM()
    {
        _bgmAudio.Stop();
    }

    public void PlayBGM() {
        // _index++;
        // _index %= 3;
        // var namePrefix = "bgm_battle_";
        //  for (int i = 0; i < SoundSets.Count; i++)
        // {
        //     if (SoundSets[i].Name == namePrefix + _index)
        //     {
        //         _bgmAudio.ChangeBGM(SoundSets[i].Sound);
        //         break;
        //     }
        // }
        _bgmAudio.Play();
    }

    public void ChangeBGM(string name)
    {
        bool isMissing = true;
        for (int i = 0; i < SoundSets.Count; i++)
        {
            if (SoundSets[i].Name == name)
            {
                _bgmAudio.ChangeBGM(SoundSets[i].Sound);
                isMissing = false;
                break;
            }
        }
        if (isMissing)
        {
            _bgmAudio.Stop();
            Debug.LogError($"Sound Missing {name}");
        }
    }

    public AudioClip GetSound(string name)
    {
        return SoundSets.Find(x => x.Name == name).Sound;
    }
}
