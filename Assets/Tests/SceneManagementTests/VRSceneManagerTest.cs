using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using NUnit.Framework;
using System.Collections;

public class VRSceneManagerTest
{
    private GameObject managerObject;
    private VRSceneManager manager;
    private Material originalFadeMaterial;
    private bool transitionCompleted;

    [SetUp]
    public void Setup()
    {
        // Create manager
        managerObject = new GameObject("VRSceneManager");
        manager = managerObject.AddComponent<VRSceneManager>();
        
        // Create test fade material
        var shader = Shader.Find("Hidden/FadeShader");
        if (shader != null)
        {
            originalFadeMaterial = new Material(shader);
        }
        
        transitionCompleted = false;
    }

    [TearDown]
    public void Teardown()
    {
        Object.DestroyImmediate(managerObject);
        if (originalFadeMaterial != null)
        {
            Object.DestroyImmediate(originalFadeMaterial);
        }
    }

    [UnityTest]
    public IEnumerator TestSceneLoading()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        manager.LoadScene(currentScene, () => transitionCompleted = true);
        
        // Wait for fade out
        yield return new WaitForSeconds(0.5f);
        
        // Wait for fade in
        yield return new WaitForSeconds(0.5f);
        
        // Wait for completion callback
        yield return new WaitUntil(() => transitionCompleted);
        
        Assert.IsTrue(transitionCompleted, "Scene transition should complete");
    }

    [UnityTest]
    public IEnumerator TestSceneReset()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        manager.ResetScene(() => transitionCompleted = true);
        
        // Wait for reset delay
        yield return new WaitForSeconds(0.3f);
        
        // Wait for fade transitions
        yield return new WaitForSeconds(1.0f);
        
        Assert.IsTrue(transitionCompleted, "Scene reset should complete");
        Assert.AreEqual(currentScene, SceneManager.GetActiveScene().name, 
            "Scene should reset to the same scene");
    }

    [UnityTest]
    public IEnumerator TestFadeEffect()
    {
        // Get initial fade color
        Color initialColor = manager.fadeMaterial.GetColor("_Color");
        
        manager.LoadScene(SceneManager.GetActiveScene().name);
        
        // Wait for fade out
        yield return new WaitForSeconds(0.5f);
        
        Color midFadeColor = manager.fadeMaterial.GetColor("_Color");
        Assert.AreNotEqual(initialColor.a, midFadeColor.a, 
            "Fade material alpha should change during transition");
    }

    [UnityTest]
    public IEnumerator TestMultipleTransitions()
    {
        int transitionCount = 0;
        string currentScene = SceneManager.GetActiveScene().name;
        
        // Attempt multiple rapid transitions
        manager.LoadScene(currentScene, () => transitionCount++);
        manager.LoadScene(currentScene, () => transitionCount++);
        
        // Wait for all transitions
        yield return new WaitForSeconds(2.0f);
        
        Assert.AreEqual(2, transitionCount, 
            "Multiple scene transitions should complete independently");
    }

    [UnityTest]
    public IEnumerator TestTransitionCancellation()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        bool firstTransitionCompleted = false;
        bool secondTransitionCompleted = false;
        
        // Start first transition
        manager.LoadScene(currentScene, () => firstTransitionCompleted = true);
        
        // Wait briefly then start second transition
        yield return new WaitForSeconds(0.1f);
        manager.LoadScene(currentScene, () => secondTransitionCompleted = true);
        
        // Wait for transitions to complete
        yield return new WaitForSeconds(2.0f);
        
        Assert.IsFalse(firstTransitionCompleted, 
            "First transition should be cancelled by second transition");
        Assert.IsTrue(secondTransitionCompleted, 
            "Second transition should complete");
    }
}
