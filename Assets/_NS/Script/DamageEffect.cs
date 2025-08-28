using System.Collections;
using UnityEngine;

public class DamageEffect : MonoBehaviour
{
    [SerializeField] private MeshRenderer[] meshRenderers;
    [SerializeField] private float fadeDuration = 0.2f;

    private Material[] fadeMaterials;
    Coroutine corutine;
    private void Awake()
    {
        // マテリアルのコピー
        fadeMaterials = new Material[meshRenderers.Length];
        for (int i = 0; i < meshRenderers.Length; i++)
        {
            fadeMaterials[i] = meshRenderers[i].material;
        }
        SetAlpha(0f);
    }


    public void PlayEffect()
    {
        // すでにコルーチンが実行中なら停止
        if (corutine != null)
        {
            StopCoroutine(corutine);
            for (int i = 0; i < meshRenderers.Length; i++)
            {
                meshRenderers[i].material = fadeMaterials[i];
            }
        }
        corutine = StartCoroutine(FadeEffect());
    }

    private IEnumerator FadeEffect()
    {
        float timer = 0f;

        // 最初にマテリアルを適用
        for (int i = 0; i < meshRenderers.Length; i++)
        {
            meshRenderers[i].material = fadeMaterials[i];
        }

        // 出現フェード（透明 → 不透明）
        while (timer < fadeDuration)
        {
            float alpha = timer / fadeDuration;
            SetAlpha(alpha);
            timer += Time.deltaTime;
            yield return null;
        }

        SetAlpha(1f);

        // 少し表示
        yield return new WaitForSeconds(fadeDuration);

        // 消えるフェード（不透明 → 透明）
        timer = 0f;
        while (timer < fadeDuration)
        {
            float alpha = 1f - (timer / fadeDuration);
            SetAlpha(alpha);
            timer += Time.deltaTime;
            yield return null;
        }

        SetAlpha(0f);
    }

    private void SetAlpha(float alpha)
    {
        foreach (var mat in fadeMaterials)
        {
            Color c = mat.color;
            c.a = alpha;
            mat.color = c;
        }
    }
}
