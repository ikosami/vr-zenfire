using UnityEngine;

public class FollowVRHeadset : MonoBehaviour
{
    public Transform vrCamera;  // VRカメラ（HMD）のTransform

    void FixedUpdate()
    {
        if (vrCamera != null)
        {
            // カメラの位置に追従（Y軸方向のオフセットあり）
            transform.position = new Vector3(vrCamera.position.x, transform.position.y, vrCamera.position.z);

            // 回転はリセット（ワールド座標系でゼロ）
            transform.rotation = Quaternion.Euler(0, vrCamera.eulerAngles.y, 0);
        }
    }
}
