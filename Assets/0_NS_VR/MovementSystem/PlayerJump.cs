using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
public class PlayerJump : MonoBehaviour {

    [SerializeField] CharacterController _characterController;
    [SerializeField] InputActionReference _inputActionAsset;

        // 上に進む速度
    float moveVelocityY;

    // ジャンプするときの速度（重力での速度より大きくすると上にジャンプできる
    float jumpPower = 5;

    void Update() {
       return;
         // Jumpボタンを押されていない時、重力分の速度を加えます
        // Physics.gravity.yは、デフォルトで -9.8fなので+=ですが、引くことになります
        moveVelocityY += -9.8f * Time.deltaTime;


        // キャラクタが地上の場合
        if (_characterController.isGrounded)
        {
            // moveVelocityY = 0でいいのですが、接地判定が不安定になる現象があります
            // 接地判定を確実に知るため-0.5fを代入します
            // ちなみに0を代入すると、Debug.Logで表示されているのがtrue、falseとバタつくのがわかります
            moveVelocityY = -0.5f;

            if(_inputActionAsset.action.ReadValue<float>() > 0.5f) {
                moveVelocityY = jumpPower;
            }
        }

        // キャラクタの移動（今回Y軸だけでテスト）
        _characterController.Move(new Vector3(0, moveVelocityY, 0) * Time.deltaTime);
    }
}
