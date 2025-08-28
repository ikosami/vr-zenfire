using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using TMPro;
using UnityEngine.Video;

public class UIManagerTest
{
    private GameObject _managerObject;
    private UIManager _uiManager;
    private GameObject _tutorialPanel;
    private GameObject _completePanel;
    private VideoPlayer _tutorialVideo;
    private ParticleSystem _confettiParticles;
    private TextMeshProUGUI _completeText;
    private TextMeshProUGUI _emojiText;

    [SetUp]
    public void Setup()
    {
        // Create UI hierarchy
        _managerObject = new GameObject("UIManager");
        _uiManager = _managerObject.AddComponent<UIManager>();
        
        // Create tutorial panel
        _tutorialPanel = new GameObject("TutorialPanel");
        _tutorialVideo = _tutorialPanel.AddComponent<VideoPlayer>();
        
        // Create complete panel
        _completePanel = new GameObject("CompletePanel");
        var completeTextObj = new GameObject("CompleteText");
        var emojiTextObj = new GameObject("EmojiText");
        _completeText = completeTextObj.AddComponent<TextMeshProUGUI>();
        _emojiText = emojiTextObj.AddComponent<TextMeshProUGUI>();
        _confettiParticles = _completePanel.AddComponent<ParticleSystem>();
        
        // Setup hierarchy
        completeTextObj.transform.parent = _completePanel.transform;
        emojiTextObj.transform.parent = _completePanel.transform;
        
        // Setup references
        _uiManager.tutorialPanel = _tutorialPanel;
        _uiManager.tutorialVideo = _tutorialVideo;
        _uiManager.completePanel = _completePanel;
        _uiManager.completeText = _completeText;
        _uiManager.emojiText = _emojiText;
        _uiManager.confettiParticles = _confettiParticles;
    }

    [TearDown]
    public void Teardown()
    {
        Object.DestroyImmediate(_managerObject);
        Object.DestroyImmediate(_tutorialPanel);
        Object.DestroyImmediate(_completePanel);
    }

    [UnityTest]
    public IEnumerator TestTutorialDisplay()
    {
        _uiManager.ShowTutorial();
        
        yield return null;
        
        Assert.IsTrue(_tutorialPanel.activeSelf, "Tutorial panel should be active");
        Assert.IsTrue(_tutorialVideo.isPlaying, "Tutorial video should be playing");
    }

    [UnityTest]
    public IEnumerator TestTutorialHide()
    {
        _uiManager.ShowTutorial();
        yield return null;
        
        _uiManager.HideTutorial();
        yield return null;
        
        Assert.IsFalse(_tutorialPanel.activeSelf, "Tutorial panel should be inactive");
        Assert.IsFalse(_tutorialVideo.isPlaying, "Tutorial video should not be playing");
    }

    [UnityTest]
    public IEnumerator TestGameCompleteDisplay()
    {
        _uiManager.ShowGameComplete();
        
        yield return null;
        
        Assert.IsTrue(_completePanel.activeSelf, "Complete panel should be active");
        Assert.AreEqual("Game Complete!", _completeText.text, "Complete text should be set");
        Assert.AreEqual("🎉🎮✨", _emojiText.text, "Emoji text should be set");
        Assert.IsTrue(_confettiParticles.isPlaying, "Confetti particles should be playing");
    }

    [UnityTest]
    public IEnumerator TestGameCompleteHide()
    {
        _uiManager.ShowGameComplete();
        yield return null;
        
        _uiManager.HideGameComplete();
        yield return null;
        
        Assert.IsFalse(_completePanel.activeSelf, "Complete panel should be inactive");
        Assert.IsFalse(_confettiParticles.isPlaying, "Confetti particles should not be playing");
    }

    [UnityTest]
    public IEnumerator TestTutorialAutoHide()
    {
        _uiManager.tutorialDisplayDuration = 0.1f;
        _uiManager.ShowTutorial();
        
        yield return new WaitForSeconds(0.2f);
        
        Assert.IsFalse(_tutorialPanel.activeSelf, "Tutorial panel should auto-hide after duration");
        Assert.IsFalse(_tutorialVideo.isPlaying, "Tutorial video should stop after duration");
    }
}
