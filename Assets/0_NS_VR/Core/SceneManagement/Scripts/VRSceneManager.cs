using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System;

/// <summary>
/// Manages scene transitions and fading effects in VR
/// Combines functionality from VRTransitionManager and WorldResetService
/// </summary>
public class VRSceneManager : MonoBehaviour
{
    private static VRSceneManager _instance;
    public static VRSceneManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<VRSceneManager>();
                if (_instance == null)
                {
                    var go = new GameObject("VRSceneManager");
                    _instance = go.AddComponent<VRSceneManager>();
                }
            }
            return _instance;
        }
    }

    [Header("Transition Settings")]
    [SerializeField] private Material fadeMaterial;
    [SerializeField] private float fadeDuration = 1.0f;
    [SerializeField] private Color fadeColor = Color.black;
    [SerializeField] private float resetDelay = 0.2f;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeFadeMaterial();
    }

    private void InitializeFadeMaterial()
    {
        if (fadeMaterial == null)
        {
            fadeMaterial = new Material(Shader.Find("Hidden/FadeShader"));
        }
        fadeMaterial.SetColor("_Color", new Color(fadeColor.r, fadeColor.g, fadeColor.b, 0));
    }

    /// <summary>
    /// Load a new scene with fade transition
    /// </summary>
    public void LoadScene(string sceneName, Action onComplete = null)
    {
        StartCoroutine(LoadSceneCoroutine(sceneName, onComplete));
    }

    /// <summary>
    /// Reset current scene with fade transition
    /// </summary>
    public void ResetScene(Action onComplete = null)
    {
        StartCoroutine(ResetSceneCoroutine(onComplete));
    }

    private IEnumerator LoadSceneCoroutine(string sceneName, Action onComplete)
    {
        // Fade out
        yield return StartCoroutine(FadeCoroutine(0, 1));

        // Load scene
        var operation = SceneManager.LoadSceneAsync(sceneName);
        while (!operation.isDone)
        {
            yield return null;
        }

        // Fade in
        yield return StartCoroutine(FadeCoroutine(1, 0));

        onComplete?.Invoke();
    }

    private IEnumerator ResetSceneCoroutine(Action onComplete)
    {
        yield return new WaitForSeconds(resetDelay);
        yield return StartCoroutine(LoadSceneCoroutine(SceneManager.GetActiveScene().name, onComplete));
    }

    private IEnumerator FadeCoroutine(float startAlpha, float targetAlpha)
    {
        float elapsedTime = 0;
        Color currentColor = fadeMaterial.GetColor("_Color");

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float currentAlpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeDuration);
            fadeMaterial.SetColor("_Color", new Color(fadeColor.r, fadeColor.g, fadeColor.b, currentAlpha));
            yield return null;
        }

        fadeMaterial.SetColor("_Color", new Color(fadeColor.r, fadeColor.g, fadeColor.b, targetAlpha));
    }

    /// <summary>
    /// Add fade post-processing effect
    /// </summary>
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, fadeMaterial);
    }
}
