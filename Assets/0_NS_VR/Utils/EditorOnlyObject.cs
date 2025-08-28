using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorOnlyObject : MonoBehaviour
{
    void Awake()
    {
#if !UNITY_EDITOR
        // エディタ以外の場合、オブジェクトを非表示にする
        gameObject.SetActive(false);
#endif
    }
}