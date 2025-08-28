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
