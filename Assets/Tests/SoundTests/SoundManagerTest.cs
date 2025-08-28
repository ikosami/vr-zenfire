using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;

public class SoundManagerTest
{
    private GameObject _managerObject;
    private SoundManager _soundManager;
    private AudioSource _audioSource;
    private BGMAudio _bgmAudio;

    [SetUp]
    public void Setup()
    {
        // Create manager object with required components
        _managerObject = new GameObject("SoundManager");
        _audioSource = _managerObject.AddComponent<AudioSource>();
        _bgmAudio = _managerObject.AddComponent<BGMAudio>();
        _soundManager = _managerObject.AddComponent<SoundManager>();
        
        // Setup test audio clips
        var testClip = AudioClip.Create("TestSound", 44100, 1, 44100, false);
        var testBGM = AudioClip.Create("TestBGM", 44100, 1, 44100, false);
        
        // Create test sound sets
        var soundSet = new SoundManager.SoundSet
        {
            Name = "test_sound",
            Sound = testClip,
            PitchCustom = true,
            VolumeCustom = true,
            Pitch = 1.5f,
            Volume = 0.8f
        };
        
        var bgmSet = new SoundManager.SoundSet
        {
            Name = "bgm",
            Sound = testBGM,
            PitchCustom = false,
            VolumeCustom = false
        };
        
        // Assign to manager
        _soundManager._sounds = new[] { soundSet, bgmSet };
        _soundManager._audioSource = _audioSource;
        _soundManager._bgmAudio = _bgmAudio;
    }

    [TearDown]
    public void Teardown()
    {
        Object.DestroyImmediate(_managerObject);
    }

    [UnityTest]
    public IEnumerator TestSoundPlayback()
    {
        bool played = _soundManager.Play("test_sound");
        Assert.IsTrue(played, "Sound should play successfully");
        Assert.IsTrue(_audioSource.isPlaying, "AudioSource should be playing");
        
        yield return null;
    }

    [UnityTest]
    public IEnumerator TestCustomPitchAndVolume()
    {
        _soundManager.Play("test_sound");
        
        yield return null;
        
        Assert.AreEqual(1.5f, _audioSource.pitch, "Custom pitch should be applied");
        Assert.AreEqual(0.8f, _audioSource.volume, "Custom volume should be applied");
    }

    [UnityTest]
    public IEnumerator TestBGMPlayback()
    {
        _soundManager.PlayBGM();
        
        yield return null;
        
        Assert.IsTrue(_bgmAudio.IsPlaying(), "BGM should be playing");
    }

    [UnityTest]
    public IEnumerator TestBGMStop()
    {
        _soundManager.PlayBGM();
        yield return null;
        
        _soundManager.StopBGM();
        yield return null;
        
        Assert.IsFalse(_bgmAudio.IsPlaying(), "BGM should stop playing");
    }

    [UnityTest]
    public IEnumerator TestChangeBGM()
    {
        _soundManager.ChangeBGM("bgm");
        
        yield return null;
        
        Assert.IsTrue(_bgmAudio.IsPlaying(), "New BGM should be playing");
    }

    [UnityTest]
    public IEnumerator TestDurationPlayback()
    {
        _soundManager.Play("test_sound", 0.1f);
        
        yield return new WaitForSeconds(0.2f);
        
        Assert.IsFalse(_audioSource.isPlaying, "Sound should stop after duration");
    }

    [UnityTest]
    public IEnumerator TestInvalidSoundName()
    {
        bool played = _soundManager.Play("invalid_sound");
        
        yield return null;
        
        Assert.IsFalse(played, "Invalid sound should not play");
        Assert.IsFalse(_audioSource.isPlaying, "AudioSource should not be playing");
    }

    [UnityTest]
    public IEnumerator TestGetSound()
    {
        AudioClip clip = _soundManager.GetSound("test_sound");
        
        yield return null;
        
        Assert.IsNotNull(clip, "Should retrieve valid audio clip");
        Assert.AreEqual("TestSound", clip.name, "Should retrieve correct audio clip");
    }
}
