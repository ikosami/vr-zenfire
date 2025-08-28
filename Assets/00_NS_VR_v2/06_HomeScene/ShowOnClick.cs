using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowOnClick : MonoBehaviour
{
    [SerializeField] bool _skip = true;
    [SerializeField] GameObject[] _objectsToShow;

    [SerializeField] Button _button;
    // Start is called before the first frame update
    void Start()
    {
        foreach (var obj in _objectsToShow)
        {
            obj.SetActive(_skip);
        }

        if(!_skip)
        {
            _button.onClick.AddListener(ShowObjects);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ShowObjects()
    {
        foreach (var obj in _objectsToShow)
        {
            obj.SetActive(true);
        }
    }
}
