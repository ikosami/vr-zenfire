using UnityEngine;
using Oculus.Haptics;

/// <summary>
/// VRコントローラーの振動フィードバックを管理するコンポーネント
/// Oculusのハプティクスシステムを使用して、左右のコントローラーに個別に振動を提供
/// </summary>
public class VibrationController : MonoBehaviour
{
    // シングルトンインスタンス
    public static VibrationController Instance;
    // ハプティクス設定の参照
    public HapticReferencesScriptable _references;
    // ハプティクス再生用プレイヤー
    private HapticClipPlayer _player;

    private void Awake()
    {
        Instance = this;
        _player = new HapticClipPlayer();
    }

    public void Play(string key, ControllerSide controller)
    {
        _player.clip = _references.GetClip(key);
        if (controller == ControllerSide.Left)
        {
            _player.Play(Controller.Left);
        }
        else if (controller == ControllerSide.Right)
        {
            _player.Play(Controller.Right);
        }
        else
        {
            _player.Play(Controller.Both);
        }
    }
}

public enum ControllerSide
{
    Left,
    Right,
    Both
}
