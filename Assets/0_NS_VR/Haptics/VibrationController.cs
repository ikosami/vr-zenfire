using System.Collections;
using UnityEngine;
using Oculus.Haptics;

public class VibrationController : MonoBehaviour
{
    public static VibrationController instance;
    public HapticReferencesScriptable _references;
    private HapticClipPlayer _player;

    public enum ControllerSide {
        Left,
        Right,
        Both
    }

    private void Awake()
    {
        instance = this;
        _player = new HapticClipPlayer();
    }

    public void Play(string key, ControllerSide controller) {
        _player.clip = _references.GetClip(key);
        if(controller == ControllerSide.Left) {
            _player.Play(Controller.Left);
        } else if(controller == ControllerSide.Right) {
            _player.Play(Controller.Right);
        } else {
            _player.Play(Controller.Both);
        }
        // StartCoroutine(Vibrate(frequency, amplitude, duration, controller));
    }

    // private IEnumerator Vibrate(float frequency, float amplitude, float duration, OVRInput.Controller controller)
    // {
    //     OVRInput.SetControllerVibration(frequency, amplitude, controller);
    //     yield return new WaitForSeconds(duration);
    //     OVRInput.SetControllerVibration(0, 0, controller); // バイブレーションを停止
    // }

}