using System.Collections;
using UnityEngine;

public class UIFadeViewChange : MonoBehaviour
{
    [SerializeField] CanvasGroup _canvasGroup;
    float fadeTime = 0.5f;

    Coroutine fadeCoroutine = null;
    float currentFadeTime = 0f; // 現在のフェード進行時間

    void Awake()
    {
        if (_canvasGroup == null)
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            if (_canvasGroup == null)
            {
                _canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
        }
    }

    public void SetActive(bool isActive)
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }
        fadeCoroutine = StartCoroutine(FadeCoroutine(isActive));
        _canvasGroup.blocksRaycasts = isActive;
    }
    public bool IsActive()
    {
        return _canvasGroup.blocksRaycasts;
    }

    private IEnumerator FadeCoroutine(bool isActive)
    {
        float startAlpha = _canvasGroup.alpha;
        float endAlpha = isActive ? 1f : 0f;

        // 現在のフェード進行時間をリセット
        currentFadeTime = 0f;

        while (currentFadeTime < fadeTime)
        {
            currentFadeTime += Time.deltaTime;
            float t = currentFadeTime / fadeTime;
            _canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, t);
            yield return null;
        }

        // 最終的な値を設定
        _canvasGroup.alpha = endAlpha;
        _canvasGroup.interactable = isActive;
        _canvasGroup.blocksRaycasts = isActive;

        fadeCoroutine = null;
    }
}
