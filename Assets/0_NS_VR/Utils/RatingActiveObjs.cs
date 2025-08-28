using NS_VR.StoreIntentSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RatingActiveObjs : MonoBehaviour
{
    [SerializeField] GameObject[] _views;
    [SerializeField] GameObject[] _hides;
    // Start is called before the first frame update
    void Start()
    {
        var isRating = Rating.Value > 0;
        foreach (var obj in _views)
        {
            obj.SetActive(isRating);
        }
        foreach (var obj in _hides)
        {
            obj.SetActive(!isRating);
        }
    }
}
