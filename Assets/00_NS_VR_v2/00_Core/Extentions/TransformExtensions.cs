using UnityEngine;

public static class TransformExtensions
{
    // この拡張メソッドは、与えられたワールド空間のQuaternion（回転値）を、
    // 呼び出し元のTransformのローカル空間の回転へ変換します。
    public static Quaternion InverseTransformRotation(this Transform transform, Quaternion worldRotation)
    {
        return Quaternion.Inverse(transform.rotation) * worldRotation;
    }
}
