using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager_ClawMovement : MonoBehaviour
{
    // これらはUIの画面上のボタンを使用してクレーンを動かすために使用されます
    private bool UI_ClawButtonUp = false;
    private bool UI_ClawButtonDown = false;
    private bool UI_ClawButtonLeft = false;
    private bool UI_ClawButtonRight = false;

    [SerializeField] Transform baseT;

    [SerializeField] List<HingeJoint> hingeJointList;

    [Space(5f)]

    [Header("Claw Settings")]

    // クレーンとロープを動かすために動かすオブジェクト
    public Transform clawHolder;
    public Transform Arm;
    public Vector3 initArmPos;

    // XおよびZの移動速度
    public float movementSpeed = 1.0f;

    // Yの降下および上昇速度
    public float dropSpeed = 1.0f;

    [Range(0, 10)]
    public int failRate = 0;

    [HideInInspector]
    // クレーンを降ろしたり上げたりしているときはfalseになります。
    public bool canMove = true;

    [HideInInspector]
    // これはY方向の下向きの移動を停止します。通常、このクラスの外部から呼び出されます。
    // この場合、WallSensor_ClawMachineからクレーンが壁に当たったため、移動を停止するように指示されます。
    public bool stopMovement = false;

    // クレーンが下に移動を停止する垂直限界
    public float LimitY = 2.074f;

    // 有効にすると、クレーンは自動的にホーム/初期位置に戻ります
    [Tooltip("有効にすると、クレーンは自動的にホーム/初期位置に戻ります")]
    public bool shouldReturnHomeAutomatically = false;

    // 位置変数
    [HideInInspector]
    public Vector3 initClawPosition;

    [HideInInspector]
    public Vector3 clawDropFromPosition;

    [SerializeField] MeshHighlighter _meshHighlighter;


    [Header("Claw Movement Boundary Limits")]

    // 境界とクレーンヘッドの中心の間にバッファを追加するために使用されます（クリップしないように）
    public float clawHeadSizeX = 0.30f;
    public float clawHeadSizeZ = 0.13f;

    // 移動境界
    private float boundaryX_Left;
    private float boundaryX_Right;
    private float boundaryZ_Back;
    private float boundaryZ_Forward;

    [Space(5f)]

    public Transform clawBoundaryX_Left;
    public Transform clawBoundaryX_Right;
    public Transform clawBoundaryZ_Back;
    public Transform clawBoundaryZ_Forward;

    [Header("Ovverhead Motor Settings")]

    // モーター
    public Transform topMainMotor;
    public Transform overHeadMotorRailSystem;

    [HideInInspector]
    public Vector3 initTopMainMotorPosition;

    [HideInInspector]
    public Vector3 initHomeOverHeadMotorRailSystemPosition;

    [HideInInspector]
    public bool isDroppingBall = false;
    bool isMove = false;


    [SerializeField] MeshRenderer _buttonMesh;
    Material buttonMaterial;
    UnityEngine.Color buttonColor;

    [Header("Misc Settings")]
    public PrizeCatcherDetector_ClawMachine prizeCatcherDetector;

    [SerializeField] AudioSource _audioSource;

    // 初期化
    void Start()
    {
        buttonMaterial = _buttonMesh.material;
        buttonColor = buttonMaterial.color;

        // 初期位置を設定します
        initClawPosition = clawHolder.transform.localPosition;
        initTopMainMotorPosition = topMainMotor.localPosition;
        initHomeOverHeadMotorRailSystemPosition = overHeadMotorRailSystem.localPosition;
        initArmPos = Arm.localPosition;

        // 境界を設定します
        boundaryX_Left = clawBoundaryX_Left.localPosition.x;
        boundaryX_Right = clawBoundaryX_Right.localPosition.x;
        boundaryZ_Back = clawBoundaryZ_Back.localPosition.z;
        boundaryZ_Forward = clawBoundaryZ_Forward.localPosition.z;

        boundaryX_Left += clawHeadSizeX;
        boundaryX_Right -= clawHeadSizeX;
        boundaryZ_Back -= clawHeadSizeZ;
        boundaryZ_Forward += clawHeadSizeZ;

    }

    void FixedUpdate()
    {
        // 移動が許可されている場合
        if (canMove)
        {
            if (UI_ClawButtonUp || UI_ClawButtonDown || UI_ClawButtonLeft || UI_ClawButtonRight)
            {
                if (!_audioSource.isPlaying)
                    _audioSource.Play();
            }
            else
            {
                if (_audioSource.isPlaying)
                    _audioSource.Stop();
            }

            bool isNowMove = false;
            // 以下は通常の入力です...
            if (UI_ClawButtonUp)
            {
                isMove = true;
                isNowMove = true;
                ClawMoveUp();
            }

            if (UI_ClawButtonDown)
            {
                isMove = true;
                isNowMove = true;
                ClawMoveDown();
            }

            if (UI_ClawButtonLeft)
            {
                isMove = true;
                isNowMove = true;
                ClawMoveLeft();
            }

            if (UI_ClawButtonRight)
            {
                isMove = true;
                isNowMove = true;
                ClawMoveRight();
            }

            if (isMove && !isNowMove)
            {
                _meshHighlighter.SetHighlighting(true);
            }
        }
    }


    private void dropClawButtonInput()
    {
        //// 賞品キャッチャーの上にいないことを確認します。ここではクレーンを降ろすのではなく、リリースする必要があります。
        //if (prizeCatcherDetector.isClawAbovePrizeCatcher)
        //{
        //    // 賞品キャッチャーの上にいるので、ここではクレーンを降ろさずに開くだけです。
        //    openClawButtonInput();
        //}
        //else
        //{
        if (!canMove) return;

        // クレーンを降ろします
        StartCoroutine(dropClaw());
        _meshHighlighter.SetHighlighting(false);
        buttonMaterial.color = UnityEngine.Color.Lerp(buttonColor, UnityEngine.Color.gray, 0.7f);
        // このアクションを実行している間は移動を無効にします
        canMove = false;
        //}
    }

    private void openClawButtonInput()
    {
        // ボールを落としていない場合 - これにより、複数回のドロップを試みるのを防ぎます
        if (!isDroppingBall)
        {
            // ボールを落とします
            StartCoroutine(DropBall());
        }
    }

    private void ClawMoveUp()
    {
        // + Z方向
        if (clawHolder.transform.localPosition.z < boundaryZ_Back)
        {
            var move = new Vector3(0f, 0f, movementSpeed * -1 * Time.deltaTime);

            // baseT の Y 軸回転を考慮して移動ベクトルを変換
            move = baseT.InverseTransformDirection(move);

            // クレーンを動かします
            clawHolder.Translate(move);

            // 上のモーターも動かします
            topMainMotor.Translate(move);

            // 上のモーターも動かします
            overHeadMotorRailSystem.Translate(move);
        }
    }

    private void ClawMoveDown()
    {
        if (clawHolder.transform.localPosition.z > boundaryZ_Forward)
        {
            var move = new Vector3(0f, 0f, movementSpeed * 1 * Time.deltaTime);

            // baseT の Y 軸回転を考慮して移動ベクトルを変換
            move = baseT.InverseTransformDirection(move);

            // クレーンを動かします
            clawHolder.Translate(move);

            // 上のモーターも動かします
            topMainMotor.Translate(move);

            // 上のモーターも動かします
            overHeadMotorRailSystem.Translate(move);
        }
    }

    private void ClawMoveLeft()
    {
        if (clawHolder.transform.localPosition.x > boundaryX_Left)
        {
            var move = new Vector3(movementSpeed * 1 * Time.deltaTime, 0f, 0f);

            // baseT の Y 軸回転を考慮して移動ベクトルを変換
            move = baseT.InverseTransformDirection(move);

            // クレーンを動かします
            clawHolder.Translate(move);

            // 上のモーターも動かします
            topMainMotor.Translate(move);
        }
    }

    private void ClawMoveRight()
    {
        // - X方向
        if (clawHolder.transform.localPosition.x < boundaryX_Right)
        {
            var move = new Vector3(movementSpeed * -1 * Time.deltaTime, 0f, 0f);

            // baseT の Y 軸回転を考慮して移動ベクトルを変換
            move = baseT.InverseTransformDirection(move);

            // クレーンを動かします
            clawHolder.Translate(move);

            // 上のモーターも動かします
            topMainMotor.Translate(move);
        }
    }

    /// <summary>
    /// 位置からクレーンを降ろすために使用されます。
    /// </summary>
    /// <returns></returns>
    IEnumerator dropClaw()
    {
        // 降下位置を保存します
        clawDropFromPosition = initClawPosition;

        // 開くアニメーションを再生します
        OpenClaw();

        float timer = 0;
        // 垂直Y限界より大きい間
        while (clawHolder.localPosition.y >= LimitY)
        {
            timer += Time.deltaTime;
            if (timer > 3)
            {
                //下がれなかった
                stopMovement = true;
                break;
            }

            // 何かが移動を停止させた場合、ブレークアウトします。これは現在、WallSensor_ClawMachineスクリプトです。
            if (stopMovement)
            {
                break;
            }

            // クレーンを降ろします
            clawHolder.Translate(0f, dropSpeed * -1 * Time.deltaTime, 0f);

            yield return null;
        }

        // 少し待ちます
        yield return new WaitForSeconds(1.0f);

        // 移動が停止された場合
        if (stopMovement)
        {
            // 降下位置に戻ります
            while (clawHolder.localPosition.y <= clawDropFromPosition.y)
            {
                // 移動します
                clawHolder.Translate(0f, dropSpeed * 1 * Time.deltaTime, 0f);

                yield return null;
            }
            var pos = clawHolder.localPosition;
            pos.y = clawDropFromPosition.y;
            clawHolder.localPosition = pos;


            yield return new WaitForSeconds(0.15f);

            // クレーンを閉じます
            CloseClaw();
        }
        else
        {
            // クレーンヘッドを閉じます
            CloseClaw();

            yield return new WaitForSeconds(1.0f);

            timer = 0;
            // まず上に戻ります
            while (clawHolder.localPosition.y <= clawDropFromPosition.y)
            {
                timer += Time.deltaTime;
                if (timer > 3)
                {
                    break;
                }
                clawHolder.Translate(0f, dropSpeed * 1 * Time.deltaTime, 0f);

                yield return null;
            }

            var pos = clawHolder.localPosition;
            pos.y = clawDropFromPosition.y;
            clawHolder.localPosition = pos;

            //自動で戻る場合
            if (shouldReturnHomeAutomatically)
            {
                yield return new WaitForSeconds(1.0f);

                // ホームに戻ります
                float startTime = Time.time;
                float journeyLength = Vector3.Distance(clawHolder.localPosition, initClawPosition);

                _audioSource.Play();
                while (Vector3.Distance(clawHolder.localPosition, initClawPosition) > 0.05f && startTime - Time.time < 2)
                {
                    // 移動距離 = 時間 * 速度。
                    float distCovered = (Time.time - startTime) * 0.025f;

                    // 旅の完了割合 = 現在の距離 / 総距離。
                    float fracJourney = distCovered / journeyLength;

                    // ホームに近づく位置を補間します
                    clawHolder.localPosition = Vector3.Lerp(clawHolder.localPosition, initClawPosition, fracJourney);
                    overHeadMotorRailSystem.localPosition = Vector3.Lerp(overHeadMotorRailSystem.localPosition, initHomeOverHeadMotorRailSystemPosition, fracJourney);
                    topMainMotor.localPosition = Vector3.Lerp(topMainMotor.localPosition, initTopMainMotorPosition, fracJourney);

                    yield return null;
                }
                _audioSource.Stop();

                // 正確な位置にリセットします
                clawHolder.localPosition = initClawPosition;
                //Arm.localPosition = initArmPos;


                // 開くアニメーションを再生します
                OpenClaw();

                yield return new WaitForSeconds(1.55f);

                CloseClaw();
            }
        }

        // 移動できます
        canMove = true;

        buttonMaterial.color = buttonColor;

        // 内壁との衝突で停止した場合、再び移動を許可します
        stopMovement = false;

        yield return null;
    }


    /// <summary>
    /// 通常、アニメーションを使用してクレーンを開きます。
    /// </summary>
    private void OpenClaw()
    {


        // 開くアニメーションを再生します
        foreach (HingeJoint hingeJoint in hingeJointList)
        {
            // モーターの角度を30度
            JointSpring spring = hingeJoint.spring;
            spring.spring = 1000 * ParamData.Instance.ArmPower;
            spring.targetPosition = 30;
            hingeJoint.spring = spring;
        }
    }

    /// <summary>
    /// アニメーションを使用してクレーンを閉じます。
    /// </summary>
    private void CloseClaw()
    {
        // 開くアニメーションを再生します
        foreach (HingeJoint hingeJoint in hingeJointList)
        {
            // スプリングの角度を0度
            JointSpring spring = hingeJoint.spring;
            spring.targetPosition = 0;
            hingeJoint.spring = spring;


        }
    }


    /// <summary>
    /// ボールを落とすために使用されます
    /// </summary>
    /// <returns></returns>
    IEnumerator DropBall()
    {
        // ボールを落としていることをフラグします
        isDroppingBall = true;

        OpenClaw();

        yield return new WaitForSeconds(0.55f);

        CloseClaw();

        // 再びボールを落とすことができることをフラグします
        isDroppingBall = false;
    }



    /// 公開UI関数 ///
    /// 画面上のボタンのみに使用されます
    public void UI_DropClawButton()
    {
        dropClawButtonInput();
    }

    public void UI_OpenClawButton()
    {
        openClawButtonInput();
    }

    // 左に移動するイベントタイプの場合（ポインターダウン/アップ）
    public void UI_MoveClawLeft()
    {
        UI_ClawButtonLeft = true;
    }
    public void UI_MoveClawLeft_Off()
    {
        UI_ClawButtonLeft = false;
    }
    public void UI_MoveClawLeft(bool flg)
    {
        UI_ClawButtonLeft = flg;
    }

    // 右に移動するイベントタイプの場合（ポインターダウン/アップ）
    public void UI_MoveClawRight()
    {
        UI_ClawButtonRight = true;
    }
    public void UI_MoveClawRight_Off()
    {
        UI_ClawButtonRight = false; ;
    }
    public void UI_MoveClawRight(bool flg)
    {
        UI_ClawButtonRight = flg;
    }

    // 上に移動するイベントタイプの場合（ポインターダウン/アップ）
    public void UI_MoveClawUp()
    {
        UI_ClawButtonUp = true;
    }
    public void UI_MoveClawUp_Off()
    {
        UI_ClawButtonUp = false;
    }
    public void UI_MoveClawUp(bool flg)
    {
        UI_ClawButtonUp = flg;
    }

    // 下に移動するイベントタイプの場合（ポインターダウン/アップ）
    public void UI_MoveClawDown()
    {
        UI_ClawButtonDown = true;
    }
    public void UI_MoveClawDown_Off()
    {
        UI_ClawButtonDown = false;
    }
    public void UI_MoveClawDown(bool flg)
    {
        UI_ClawButtonDown = flg;
    }
}


partial class ParamData
{
    public float ArmPower;
    public float ItemScaleClawMachine;
}