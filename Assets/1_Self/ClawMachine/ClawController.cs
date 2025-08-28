using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClawController : MonoBehaviour
{
    [SerializeField] VRButton _armOpenButton;
    [SerializeField] VRButton _armDownButton;

    [SerializeField] Manager_ClawMovement clawManager;
    // Start is called before the first frame update
    void Start()
    {
        _armOpenButton.OnPressed += clawManager.UI_OpenClawButton;
        _armDownButton.OnPressed += clawManager.UI_DropClawButton;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
