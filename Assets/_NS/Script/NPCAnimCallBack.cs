using System;
using UnityEngine;

public class NPCAnimCallBack : MonoBehaviour
{
    public Action OnShoot;
    public void OnFootstep()
    {

    }
    public void Shoot()
    {
        OnShoot?.Invoke();
    }
}
