using UnityEngine;
using TMPro;
using UnityEngine.Video;

/// <summary>
/// チュートリアルUIを管理するコンポーネント
/// ワールド空間でのUI表示と動画再生を制御
/// </summary>
public class TutorialUIManager : MonoBehaviour
{
    [Header("UI要素")]
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private TextMeshProUGUI tutorialText;
    [SerializeField] private VideoPlayer tutorialVideo;
    [SerializeField] private float displayDuration = 10f;

    private void Start()
    {
        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(false);
        }
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

            Invoke(nameof(HideTutorial), displayDuration);
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
}
