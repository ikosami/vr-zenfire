using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DebuggableClass]
public partial class ParamData : MonoBehaviour
{
    public static ParamData Instance;

    public static System.Action<string> OnParamChange;

    public void Awake()
    {
        Instance = this;
    }

    [Header("一時的なチェック以外の変更はPrefabで行ってください")]
    public bool IsTest = false;

    [Header("ハーブ成長速度")]
    [DebugUI("ハーブ成長速度")]
    public float growthSpeedMulti = 1;
    [Header("ハーブ売却価格倍率")]
    [DebugUI("ハーブ売却価格倍率")]
    public float moneyMulti = 1;

    [Header("落ちているお金の金額")]
    [DebugUI("落ちているお金の金額")]
    public int DropMoney = 50;
    [Header("鉢植えレベルアップ時の時間短縮倍率")]
    [DebugUI("鉢植えレベルアップ時の時間短縮倍率")]
    public float growthSpeedLv = 0.9f;
    [Header("鉢植えレベルアップ時の金額倍率")]
    [DebugUI("鉢植えレベルアップ時の金額倍率")]
    public float levelUpCostLv = 1.2f;



    [Header("以下は、関係ないので触っても変化ありません")]
    public bool IsTest2 = false;

    [Header("-------------------------------------")]
    public bool IsTest3 = false;




    [Header("腕の振りの速さの閾値")]
    public float ArmSwingThreshold = 1.0f;
    [Header("手で動く時の左右の速度倍率")]
    public float HandMoveAddForce = 5.0f;
    [Header("ジャンプ処理に切り替える速度の閾値")]
    public float JumpThreshold = 2.5f;
    [Header("ジャンプ時の加算する力")]
    public float JumpAddForce = 10.0f;

    [Header("衝突判定の半径")]
    public float HandDetectionRadius = 0.4f;
    [Header("地面判定の距離")]
    public float GroundCheckDistance = 0.1f;

    [Header("重力")]
    public float Gravity = 9.81f;
    [Header("空気摩擦")]
    public float Friction = 5f;
    [Header("地面摩擦")]
    public float GroundFriction = 20f;


    [Header("MaxSpeed")]
    public float MaxSpeed = 20.0f;

    [Header("移動方法アクティベート")]
    [Header("コントローラーを振って移動可能")]
    public bool CanArmSwingMove = true;
    [Header("左スティックで移動可能")]
    public bool CanLeftStickMove = true;
    [Header("右スティックで回転可能")]
    public bool CanRightStickRotate = true;

    [Header("手のモデル")]
    public HandModelKind HandModelKind = HandModelKind.Human;
}

public enum HandModelKind
{
    Human,
    Cat,
    Gorilla
}
