using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.InputSystem;

/// <summary>
/// VR環境での武器選択を管理するコンポーネント
/// 左右のコントローラーからの入力を検知し、対応する武器を選択
/// </summary>
public class WeaponSelectable : MonoBehaviour
{
    // 武器のインデックス
    [SerializeField] int _index = 0;
    // 左右のコントローラー入力参照
    [SerializeField] InputActionReference _inputActionAssetLeft;
    [SerializeField] InputActionReference _inputActionAssetRight;

    [SerializeField] XRSimpleInteractable _interactable;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(_interactable.isHovered) {
            if(_inputActionAssetLeft.action.ReadValue<float>() > 0.5f) {
                VibrationController.Instance.Play("click", ControllerSide.Left);
                SelectWeapon();
            }
            if(_inputActionAssetRight.action.ReadValue<float>() > 0.5f) {
                VibrationController.Instance.Play("click", ControllerSide.Right);
                SelectWeapon();
            }
        }
    }

    public void SelectWeapon() {
        WeaponSelector.Instance.SelectWeapon(_index.ToString());
    }
}
