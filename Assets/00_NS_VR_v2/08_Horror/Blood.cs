using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blood : MonoBehaviour
{
    public Vector3 startPosition;
    public Vector3 groundPosition;
    public float gravity = 9.8f;
    public Action OnHitGround;

    private float fallSpeed = 0f;
    private bool isFalling = true;

    public void SetPos(Vector3 startPos, Vector3 endPos, Action callBack)
    {
        transform.position = startPos;
        startPosition = startPos;
        groundPosition = endPos;
        isFalling = true;

        OnHitGround = callBack;
    }

    void Update()
    {
        if (!isFalling) return;

        fallSpeed += gravity * Time.deltaTime;
        transform.position += Vector3.down * fallSpeed * Time.deltaTime;

        if (transform.position.y <= groundPosition.y)
        {
            transform.position = groundPosition;
            isFalling = false;
            OnHitGround?.Invoke();
            Destroy(gameObject);
        }
    }
}
