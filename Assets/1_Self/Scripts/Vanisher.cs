using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vanisher : MonoBehaviour
{
    [SerializeField] ColliderEventListener _colliderEventListener;

    [SerializeField] LayerMask _layerMask;
    // Start is called before the first frame update
    void Start()
    {
        _colliderEventListener._OnTriggerEnter += _OnTriggerEnter;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void _OnTriggerEnter(Collider col) {
        if(_layerMask == (_layerMask | (1 << col.gameObject.layer))) {
            col.gameObject.SetActive(false);
        }
    }
}
