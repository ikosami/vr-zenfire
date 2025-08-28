using System;
using UnityEngine;

public class UnlockObject : MonoBehaviour
{
    public GameObject HideObject;
    public GameObject ViewObject;

    internal void SetLock(bool v)
    {
        ViewObject.SetActive(!v);
        HideObject.SetActive(v);
    }
}
