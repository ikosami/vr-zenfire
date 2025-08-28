using Ev;
using UnityEngine;

public class Joystick : MonoBehaviour
{
    [SerializeField] Manager_ClawMovement clawMovement;
    [SerializeField] GameObject gripPosObj;
    [HideInInspector] public Vector3 gripInitPos;
    internal Quaternion initRotation;
    [SerializeField] Transform baseT;
    [SerializeField] bool isRight = false;

    private void Awake()
    {
        initRotation = transform.rotation;
        gripInitPos = gripPos;
    }

    string _poseName = "JoyStick";

    [SerializeField] Vector3 offsetPosition;
    [SerializeField] Vector3 offsetRatate;

    public Vector3 gripPos
    {
        get { return gripPosObj.transform.position + gripPosObj.transform.rotation * offsetPosition; }
    }
    public Quaternion gripRotation
    {
        get
        {
            Quaternion gripRot = gripPosObj.transform.rotation;
            Quaternion correctionRot;
            //Debug.LogError(controllerSide);
            //if (controllerSide == ControllerSide.Right && isRight)
            //{
            correctionRot = Quaternion.Euler(offsetRatate.x, offsetRatate.y, offsetRatate.z); // x30, y180 の補正回転
            //}
            //else
            //{
            //    correctionRot = Quaternion.Euler(offsetRatate.x + 180, offsetRatate.y + 180, offsetRatate.z); // x30, y180 の補正回転
            //}
            return gripRot * correctionRot; // ジョイスティックの回転に補正回転を適用
        }
    }
    ControllerSide controllerSide;
    public void Grab(ControllerSide controllerSide)
    {
        this.controllerSide = controllerSide;
        EventManager.TriggerEvent(Ev.Events.Name.HAND_POSE_CHANGE, new Ev.Events.HandPoseChange() { PoseName = _poseName, ControllerSide = controllerSide, HandPartType = HandPartType.All });
        EventManager.TriggerEvent(Ev.Events.Name.HAND_POSE_LOCK, new Ev.Events.HandPoseLock() { ControllerSide = controllerSide, HandPartType = HandPartType.All });

    }

    public void MoveRotate(Vector3 move)
    {
        // baseT の Y 軸回転を考慮して移動ベクトルを変換
        Vector3 localMove = baseT.InverseTransformDirection(move);

        // X軸（前後）と Z軸（左右）の移動を利用して回転を計算
        float xAngle = localMove.z * 300f; // 前後の移動で前後に傾く
        float zAngle = -localMove.x * 300f; // 左右の移動で左右に傾く

        clawMovement.UI_MoveClawRight(zAngle < -15);
        clawMovement.UI_MoveClawLeft(zAngle > 15);
        clawMovement.UI_MoveClawDown(xAngle < -15);
        clawMovement.UI_MoveClawUp(xAngle > 15);

        // 傾きを制限（30度以上傾かないようにする）
        xAngle = Mathf.Clamp(xAngle, -30f, 30f);
        zAngle = Mathf.Clamp(zAngle, -30f, 30f);

        // スティックの初期角度をベースに回転を適用
        transform.rotation = initRotation * Quaternion.Euler(xAngle, 0, zAngle);
    }

    public void ResetRotate()
    {
        transform.rotation = initRotation;
        clawMovement.UI_MoveClawRight(false);
        clawMovement.UI_MoveClawLeft(false);
        clawMovement.UI_MoveClawDown(false);
        clawMovement.UI_MoveClawUp(false);
    }
}
