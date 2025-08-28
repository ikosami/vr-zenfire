// Core System Specification in Alloy

// 基本シグネチャ定義
// 浮動小数点数の定義
sig Float {
    value: Int,
    decimal: Int
} {
    // 小数部は0-99の範囲に制限
    decimal >= 0
    decimal < 100
}

abstract sig Controller {
    instance: lone Controller
}

sig Transform {}
sig GameObject {}
sig ParticleSystem {}
sig CwPaintDecal {}
sig InputActionReference {}
sig XRSimpleInteractable {}
sig CharacterController {}

// イベントインターフェース
abstract sig IEvent {}
sig DefaultEvent extends IEvent {
    eventName: String
}

// アクション定義
sig Action {}

// イベントシステム
sig Event {
    name: String,
    data: IEvent
}

sig EventManager extends Controller {
    events: Event -> Action
}

// バイブレーションシステム
sig VibrationPattern {
    name: String,
    amplitude: Float,
    duration: Float
}

enum ControllerSide {
    Left, Right
}

sig VibrationController extends Controller {
    patterns: String -> VibrationPattern,
    currentVibration: ControllerSide -> lone VibrationPattern
}

// 武器選択システム
sig WeaponSelectable {
    index: Int,
    inputActionLeft: InputActionReference,
    inputActionRight: InputActionReference,
    interactable: XRSimpleInteractable
}

sig WeaponSelector extends Controller {
    currentWeapon: Int,
    weapons: Int -> WeaponSelectable
}

// プレイヤー移動システム
sig PlayerJump {
    characterController: CharacterController,
    inputAction: InputActionReference,
    jumpPower: Int,
    moveVelocityY: Int
}

// リファレンス管理システム
sig References extends Controller {
    player: Transform,
    prefabs: String -> GameObject,
    decals: String -> CwPaintDecal,
    particles: String -> ParticleSystem
}

// シングルトン制約
fact Singletons {
    all c: Controller | one c.instance
}

// WeaponSelectable振る舞い
pred selectWeapon[ws: WeaponSelectable, s: WeaponSelector] {
    // 武器選択時の状態遷移
    s.currentWeapon' = ws.index
    some v: VibrationController | 
        v.currentVibration' = v.patterns["click"] -> ControllerSide
}

// PlayerJump振る舞い
pred jump[p: PlayerJump] {
    // ジャンプ時の状態遷移
    p.moveVelocityY' = p.jumpPower
}

pred applyGravity[p: PlayerJump] {
    // 重力適用時の状態遷移
    p.moveVelocityY' = p.moveVelocityY.add[-10]
}

// VibrationController振る舞い
pred playVibration[v: VibrationController, name: String, side: ControllerSide] {
    // バイブレーション実行時の状態遷移
    v.currentVibration' = v.patterns[name] -> side
}

// システム全体の制約
fact SystemConstraints {
    // WeaponSelectableのインデックスは一意
    all disj w1, w2: WeaponSelectable | w1.index != w2.index
    
    // バイブレーションパターン名は一意
    all disj v1, v2: VibrationPattern | v1.name != v2.name
}

// システム全体の振る舞い検証
assert WeaponSelectConsistency {
    // 武器選択の一貫性チェック
    all ws: WeaponSelectable, s: WeaponSelector |
        selectWeapon[ws, s] implies s.currentWeapon' = ws.index
}

// 実行例
run {
    some ws: WeaponSelectable, s: WeaponSelector |
        selectWeapon[ws, s]
} for 3

check WeaponSelectConsistency for 4
