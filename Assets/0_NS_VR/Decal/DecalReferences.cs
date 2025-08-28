using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PaintIn3D;

public class DecalReferences : MonoBehaviour
{
    public List<DecalReference> Decals;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

[System.Serializable]
public class DecalReference : IKeyValue<string, CwPaintDecal> {
}
