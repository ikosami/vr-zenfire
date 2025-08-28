using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class VRSceneTransitionManager : MonoBehaviour
{
    public static bool IsTransitioning { get; private set; } = false;
    
    [SerializeField] private Material fadeMaterial;
    [SerializeField] private float fadeDuration = 1.0f;
    [SerializeField] private Color fadeColor = Color.black;
    
    private void Awake()
    {
         // フェード用マテリアルが設定されていない場合は作成
        if (fadeMaterial == null)
        {
            fadeMaterial = new Material(Shader.Find("Hidden/FadeShader"));
        }
        if(IsTransitioning) {
            StartCoroutine(Fade(1, 0));
            IsTransitioning = false;
        }
    }
    
    public void LoadScene(string sceneName)
    {
        IsTransitioning = true;
        StartCoroutine(LoadSceneCoroutine(sceneName));
    }
    
    private IEnumerator LoadSceneCoroutine(string sceneName)
    {
        // フェードアウト
        yield return StartCoroutine(Fade(0, 1));
        
        // シーンロード
        SceneManager.LoadScene(sceneName);
    }
    
    private IEnumerator Fade(float startAlpha, float targetAlpha)
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
    }
}