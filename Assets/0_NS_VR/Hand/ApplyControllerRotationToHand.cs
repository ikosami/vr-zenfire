using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyControllerRotationToHand : MonoBehaviour
{
    [SerializeField] Transform _target;
    [SerializeField] Transform _armStartPoint;

    [SerializeField] Transform _wrist;
    [SerializeField] bool _isInvertedYRotation = false;

    [SerializeField] Transform _parent;
    [SerializeField] Transform _controllerWrist;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // _armStartPointから_targetのベクトルを向くように回転
        var armStartPointToTarget = _target.position - _armStartPoint.position;
        transform.rotation = Quaternion.LookRotation(armStartPointToTarget);

        // Z軸は全体
        var localEuler = transform.localRotation.eulerAngles;
        localEuler.z = _parent.localRotation.eulerAngles.z;
        transform.localRotation = Quaternion.Euler(localEuler);

        // Y軸は_wristのX回転
        // localEuler = _wrist.localRotation.eulerAngles;
        // float targetYRotation = _target.localRotation.eulerAngles.y;
        // // 0-360度の値を-180から180度の範囲に変換
        // if (targetYRotation > 180f)
        //     targetYRotation -= 360f;
        // // // -90から90度の範囲に制限
        // targetYRotation = Mathf.Clamp(targetYRotation, -90f, 90f);
        // localEuler.x = _isInvertedYRotation ? targetYRotation : -targetYRotation;
        // _wrist.localRotation = Quaternion.Euler(localEuler);

        // _

        // _parentにおける_controllerWristのRotationを計算
        // var localRotation = _controllerWrist.rotation * _parent.localRotation;
        _wrist.rotation = _controllerWrist.rotation;


        // // _parentにおける_fulcrumから_childのベクトルを計算し、_wristのlocalに適用
        // var parentToChild = _child.position - _parent.position;
        // var parentToFulcrum = _fulcrum.position - _parent.position;
        // var parentToChildInParent = parentToChild - parentToFulcrum;
        // // _wristがparentToChildInParentの方向を向くように回転
        // localEuler = _wrist.localRotation.eulerAngles;
        // var targetYRotation = Quaternion.LookRotation(parentToChildInParent).eulerAngles.x;
        // // 0-360度の値を-180から180度の範囲に変換
        // // if (targetYRotation > 180f)
        // //     targetYRotation -= 360f;
        // // // // -90から90度の範囲に制限
        // // targetYRotation = Mathf.Clamp(targetYRotation, -90f, 90f);
        // localEuler.x = _isInvertedYRotation ? targetYRotation : -targetYRotation;
        // _wrist.localRotation = Quaternion.Euler(localEuler);


        // // Y軸が_wristの-X軸になるように回転
        // localEuler = _wrist.localRotation.eulerAngles;
        // localEuler.x = -Mathf.Atan2(parentToChildInParent.y, parentToChildInParent.z) * Mathf.Rad2Deg;
        // _wrist.localRotation = Quaternion.Euler(localEuler);
    }
}
