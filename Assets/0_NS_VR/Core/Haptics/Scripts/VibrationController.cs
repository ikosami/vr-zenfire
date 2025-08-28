using UnityEngine;
using Oculus.Haptics;

public class VibrationController : MonoBehaviour
{
    public static VibrationController instance;
    public HapticReferencesScriptable _references;
    private HapticClipPlayer _player;

    private void Awake()
    {
        instance = this;
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
