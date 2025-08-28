using UnityEngine;

public class MeshHighlighter : MonoBehaviour
{
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Color highlightColor = Color.red;
    [SerializeField] private float highlightSpeed = 2.0f;

    private Color originalColor;
    private bool isHighlighting = false;
    private float lerpTime = 0f;
    private bool increasing = true;
    private Material materialInstance;
    [SerializeField] bool isAwakePlay = false;
    bool isInit = false;
    private void Awake()
    {
        if (meshRenderer == null)
        {
            Debug.LogError("MeshRendererがアタッチされていません。", this);
            enabled = false;
            return;
        }

        Init();

        if (isAwakePlay)
        {
            SetHighlighting(true);
        }
    }

    public void Init()
    {
        if (isInit) return;
        isInit = true;

        // メッシュごとの色変更を可能にするためマテリアルをインスタンス化
        materialInstance = meshRenderer.material;
        originalColor = materialInstance.color;
    }

    private void Update()
    {
        if (!isHighlighting) return;

        // 色の補間を行う
        lerpTime += (increasing ? 1 : -1) * highlightSpeed * Time.deltaTime;
        lerpTime = Mathf.Clamp01(lerpTime);

        materialInstance.color = Color.Lerp(originalColor, highlightColor, lerpTime);

        if (lerpTime >= 1f || lerpTime <= 0f)
        {
            increasing = !increasing;
        }
    }

    public void SetHighlighting(bool value)
    {
        Init();
        isHighlighting = value;
        if (!value)
        {
            lerpTime = 0f;
            materialInstance.color = originalColor;
        }
    }

    private void OnDisable()
    {
        if (meshRenderer != null && materialInstance != null)
        {
            materialInstance.color = originalColor;
        }
    }

    private void OnDestroy()
    {
        if (meshRenderer != null && materialInstance != null)
        {
            materialInstance.color = originalColor;
        }
    }
}
