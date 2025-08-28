using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System;

public class VRTransitionManager : MonoBehaviour
{
    public static VRTransitionManager Instance { get; private set; }
    [SerializeField] private Material fadeMaterial;
    public float fadeDuration = 1.0f;
    public Color fadeColor = Color.black;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // フェード用マテリアルが設定されていない場合は作成
        //if (fadeMaterial == null)
        //{
        //    fadeMaterial = new Material(Shader.Find("Hidden/FadeShader"));
        //}
        fadeMaterial.SetColor("_Color", new Color(fadeColor.r, fadeColor.g, fadeColor.b, 0));
    }

    public void Run(Action onOutEnded, Action onInEnded)
    {
        StartCoroutine(Fade(0, 1, () => { onOutEnded(); StartCoroutine(Fade(1, 0, onInEnded)); }));
    }
    public void RunEnd(Action onOutEnded)
    {
        StartCoroutine(Fade(0, 1, () => { onOutEnded(); }));
    }

    private IEnumerator Fade(float startAlpha, float targetAlpha, Action onEnded)
    {
        float elapsedTime = 0;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float currentAlpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeDuration);
            fadeMaterial.SetColor("_Color", new Color(fadeColor.r, fadeColor.g, fadeColor.b, currentAlpha));
            yield return null;
        }

        fadeMaterial.SetColor("_Color", new Color(fadeColor.r, fadeColor.g, fadeColor.b, targetAlpha));

        yield return null;
        onEnded?.Invoke();
    }
}