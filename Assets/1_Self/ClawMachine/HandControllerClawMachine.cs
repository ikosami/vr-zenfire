using Ev;
using UnityEngine;

public class HandControllerClawMachine : MonoBehaviour
{
    [SerializeField] ControllerSide controllerSide;

    [SerializeField] Transform parent;

    //つかんでいるスティック
    Joystick gripStick;

    HandState handState = HandState.Move;
    enum HandState
    {
        Move,
        Grab,
    }

    // Start is called before the first frame update
    void Awake()
    {
        //あとで親の座標を追従するため親を取得
        parent = transform.parent;
        //
        transform.SetParent(null);
    }

    // Update is called once per frame
    void Update()
    {
        switch (handState)
        {
            case HandState.Move:
                Move();
                break;
            case HandState.Grab:
                Grab();
                break;
        }
    }


    private void Move()
    {
        //親の座標と角度を追従
        transform.position = parent.position;
        transform.rotation = parent.rotation;

        //スティック
        if (VRInputManager.Instance.IsTriggerPressed(controllerSide, false))
        {
            var cols = Physics.OverlapSphere(transform.position, 0.10f);
            //Debug.LogError(cols.Length);

            foreach (var obj in cols)
            {
                if (obj.tag == "joyStick")
                {
                    gripStick = obj.GetComponent<Joystick>();
                    //スティックをつかむ
                    ChangeState(HandState.Grab);
                    gripStick.Grab(controllerSide);
                    break;
                }
            }
        }
    }

    private void Grab()
    {
        if (gripStick == null)
        {
            ChangeState(HandState.Move);
            return;
        }
        //スティックから一定距離離れたらステートをMoveに変更
        if (Vector3.Distance(parent.transform.position, gripStick.gripInitPos) > 0.75f)
        {
            ChangeState(HandState.Move);
            return;


        }// スティックの初期位置と現在の手の位置の差分を取得
        Vector3 move = parent.position - gripStick.gripInitPos;
        gripStick.MoveRotate(move);


        //親の座標と角度を追従
        transform.position = gripStick.gripPos;
        transform.rotation = gripStick.gripRotation;


        //スティックを離したらステートをMoveに変更
        if (!VRInputManager.Instance.IsTriggerPressed(controllerSide, false))
        {
            ChangeState(HandState.Move);
            return;
        }
    }

    void ChangeState(HandState state)
    {
        if (state == HandState.Move)
        {
            gripStick.ResetRotate();

            EventManager.TriggerEvent(Ev.Events.Name.HAND_POSE_RESET, new Ev.Events.HandPoseReset() { ControllerSide = controllerSide, HandPartType = HandPartType.All });
            EventManager.TriggerEvent(Ev.Events.Name.HAND_POSE_UNLOCK, new Ev.Events.HandPoseUnlock() { ControllerSide = controllerSide, HandPartType = HandPartType.All });
        }
        handState = state;
    }
}
