using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PaintIn3D;

public class DecalPaintObj : MonoBehaviour
{
    public CwPaintDecal Decal;
    // Start is called before the first frame update
    void Start()
    {
        Decal.HandleHitPoint(false, 0, 100, 0, transform.position, transform.rotation);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}