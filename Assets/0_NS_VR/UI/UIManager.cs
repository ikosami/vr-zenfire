using UnityEngine;
using TMPro;
using UnityEngine.Video;

/// <summary>
/// VR用UIマネージャー
/// チュートリアルとゲームクリアUIを管理
/// </summary>
public class UIManager : MonoBehaviour
{
    [Header("チュートリアルUI")]
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private TextMeshProUGUI tutorialText;
    [SerializeField] private VideoPlayer tutorialVideo;
    [SerializeField] private float tutorialDisplayDuration = 10f;

    [Header("ゲームクリアUI")]
    [SerializeField] private GameObject completePanel;
    [SerializeField] private TextMeshProUGUI completeText;
    [SerializeField] private ParticleSystem confettiParticles;
    [SerializeField] private TextMeshProUGUI emojiText;

    private void Start()
    {
        // 初期状態では非表示
        if (tutorialPanel != null) tutorialPanel.SetActive(false);
        if (completePanel != null) completePanel.SetActive(false);
    }

    /// <summary>
    /// チュートリアルUIを表示
    /// </summary>
    public void ShowTutorial()
    {
        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(true);
            if (tutorialText != null)
            {
                tutorialText.text = "How To Play\n\n" +
                    "1. コントローラーのスティックで移動\n" +
                    "2. トリガーを引いて腕を振ることでも移動可能\n" +
                    "3. Aボタンでジャンプ\n" +
                    "4. 範囲外に出ると画面が暗くなります";
            }
            if (tutorialVideo != null)
            {
                tutorialVideo.Play();
            }

            // 一定時間後に非表示
            Invoke(nameof(HideTutorial), tutorialDisplayDuration);
        }
    }

    /// <summary>
    /// チュートリアルUIを非表示
    /// </summary>
    public void HideTutorial()
    {
        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(false);
            if (tutorialVideo != null)
            {
                tutorialVideo.Stop();
            }
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
