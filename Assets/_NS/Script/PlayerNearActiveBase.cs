using System;
using UnityEngine;

public class PlayerNearActiveBase : MonoBehaviour
{
    public Action<bool> OnChange;
    public bool isActive = false;
    protected virtual void Update()
    {
        var player = PlayerController.Instance.Camera.transform;
        Vector3 playerPos = player.position;
        Vector3 navigatorPos = transform.position;
        float dx = playerPos.x - navigatorPos.x;
        float dz = playerPos.z - navigatorPos.z;

        bool isActive = (dx * dx + dz * dz < 25f); // 5m以内
        if (isActive != this.isActive)
        {
            this.isActive = isActive;
            OnChange?.Invoke(isActive);
        }
    }

}