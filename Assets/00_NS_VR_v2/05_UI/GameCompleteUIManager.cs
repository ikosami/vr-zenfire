using UnityEngine;
using TMPro;

/// <summary>
/// ゲームクリアUIを管理するコンポーネント
/// ワールド空間でのUI表示とパーティクル効果を制御
/// </summary>
public class GameCompleteUIManager : MonoBehaviour
{
    [Header("UI要素")]
    [SerializeField] private GameObject completePanel;
    [SerializeField] private TextMeshProUGUI completeText;
    [SerializeField] private TextMeshProUGUI emojiText;
    [SerializeField] private ParticleSystem confettiParticles;

    private void Start()
    {
        if (completePanel != null)
        {
            completePanel.SetActive(false);
        }
    }

    /// <summary>
    /// ゲームクリアUIを表示
    /// </summary>
    public void ShowGameComplete()
    {
        if (completePanel != null)
        {
            completePanel.SetActive(true);
            
            if (completeText != null)
            {
                completeText.text = "Game Complete!";
            }

            if (emojiText != null)
            {
                emojiText.text = "🎉🎮✨";
            }

            if (confettiParticles != null)
            {
                confettiParticles.Play();
            }
        }
    }

    /// <summary>
    /// ゲームクリアUIを非表示
    /// </summary>
    public void HideGameComplete()
    {
        if (completePanel != null)
        {
            completePanel.SetActive(false);
            if (confettiParticles != null)
            {
                confettiParticles.Stop();
            }
        }
    }
}
