using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.InputSystem;

/// <summary>
/// VR環境での武器選択を管理するコンポーネント
/// 左右のコントローラーからの入力を検知し、対応する武器を選択
/// </summary>
public class Selectable : MonoBehaviour
{
    // 武器のインデックス
    [SerializeField] int _index = 0;

    [SerializeField] XRSimpleInteractable _interactable;


    public System.Action<int> OnSelect;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(_interactable.isHovered) {
            if(VRInputManager.Instance.IsTriggerPressed(ControllerSide.Left, true)) {
                VibrationController.Instance.Play("click", ControllerSide.Left);
                Select();
            }
            if(VRInputManager.Instance.IsTriggerPressed(ControllerSide.Right, true)) {
                VibrationController.Instance.Play("click", ControllerSide.Right);
                Select();
            }
        }
    }

    public void Select() {
        OnSelect?.Invoke(_index);
    }
}
