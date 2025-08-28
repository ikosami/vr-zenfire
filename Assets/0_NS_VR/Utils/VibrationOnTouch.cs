using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Haptics;

public class VibrationOnTouch : MonoBehaviour
{
    [SerializeField] ColliderEventListener _colliderEventListener;
    [SerializeField] LayerMask _layerMask;
    [SerializeField] ControllerSide _controller;
    [SerializeField] string _key;
    // Start is called before the first frame update
    void Start()
    {
        _colliderEventListener._OnTriggerEnter += TriggerEnter;
        _colliderEventListener._OnCollisionEnter += ColliderEnter;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void TriggerEnter(Collider other)
    {
        if ((_layerMask.value & (1 << other.gameObject.layer)) != 0)
        {

            VibrationController.instance.Play(_key, _controller);
        }
    }

    void ColliderEnter(Collision other)
    {
        if ((_layerMask.value & (1 << other.gameObject.layer)) != 0)
        {
            VibrationController.instance.Play(_key, _controller);
        }
    }
}

