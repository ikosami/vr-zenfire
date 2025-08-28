using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System;

/// <summary>
/// VR環境でのシーン遷移とフェード効果を管理するコンポーネント
/// シーン遷移時のVR酔い防止のためのフェード処理を実装
/// </summary>
public class VRTransitionManager : MonoBehaviour
{
    // シングルトンインスタンス
    private static VRTransitionManager _instance;
    public static VRTransitionManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<VRTransitionManager>();
                if (_instance == null)
                {
                    var go = new GameObject("VRTransitionManager");
                    _instance = go.AddComponent<VRTransitionManager>();
                    DontDestroyOnLoad(go);
                }
            }
            return _instance;
        }
    }

    [SerializeField] private Material fadeMaterial;
    [SerializeField] private float fadeDuration = 0.5f;
    private bool isFading = false;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Run(Action onOutEnded = null, Action onInEnded = null)
    {
        if (!isFading)
        {
            StartCoroutine(FadeRoutine(onOutEnded, onInEnded));
        }
    }

    private IEnumerator FadeRoutine(Action onOutEnded, Action onInEnded)
    {
        isFading = true;

        // フェードアウト
        yield return StartCoroutine(FadeOutRoutine());
        onOutEnded?.Invoke();

        // フェードイン
        yield return StartCoroutine(FadeInRoutine());
        onInEnded?.Invoke();

        isFading = false;
    }

    private IEnumerator FadeOutRoutine()
    {
        yield return StartCoroutine(FadeRoutine(0f, 1f));
    }

    private IEnumerator FadeInRoutine()
    {
        yield return StartCoroutine(FadeRoutine(1f, 0f));
    }

    private IEnumerator FadeRoutine(float startAlpha, float endAlpha)
    {
        float elapsedTime = 0f;
        Color color = fadeMaterial.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float currentAlpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / fadeDuration);
            color.a = currentAlpha;
            fadeMaterial.color = color;
            yield return null;
        }

        color.a = endAlpha;
        fadeMaterial.color = color;
    }

    public void LoadScene(string sceneName)
    {
        if (!isFading)
        {
            StartCoroutine(LoadSceneRoutine(sceneName));
        }
    }

    private IEnumerator LoadSceneRoutine(string sceneName)
    {
        isFading = true;

        // フェードアウト
        yield return StartCoroutine(FadeOutRoutine());

        // シーン読み込み
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // フェードイン
        yield return StartCoroutine(FadeInRoutine());

        isFading = false;
    }
}
