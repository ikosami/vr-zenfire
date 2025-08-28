using NS_VR.StoreIntentSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RatingActiveObjs : MonoBehaviour
{
    [SerializeField] GameObject[] _objs;
    // Start is called before the first frame update
    void Start()
    {
        foreach (var obj in _objs)
        {
            obj.SetActive(Rating.Value > 0);
        }
    }
}
