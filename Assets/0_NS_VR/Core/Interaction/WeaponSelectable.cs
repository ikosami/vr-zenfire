using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.InputSystem;

public class WeaponSelectable : MonoBehaviour
{
    [SerializeField] int _index = 0;
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
                VibrationController.instance.Play("click", ControllerSide.Left);
                SelectWeapon();
            }
            if(_inputActionAssetRight.action.ReadValue<float>() > 0.5f) {
                VibrationController.instance.Play("click", ControllerSide.Right);
                SelectWeapon();
            }
        }
    }

    public void SelectWeapon() {
        WeaponSelector.Instance.SelectWeapon(_index.ToString());
    }
}
