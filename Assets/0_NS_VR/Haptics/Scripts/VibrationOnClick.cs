using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Haptics;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class VibrationOnClick : MonoBehaviour
{
    [SerializeField] Button _button;
    // Start is called before the first frame update
    void Start()
    {
        _button.onClick.AddListener(OnClick);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnClick()
    {
        VibrationController.Instance.Play("click", ControllerSide.Both);
    }
}

