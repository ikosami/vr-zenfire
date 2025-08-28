using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandardLookAtEventHandler : MonoBehaviour
{
    [SerializeField] LookAtObjectFromAreaEventHandler _lookAtObjectFromAreaEventHandler;
    [SerializeField] TriggerTime _triggerTime = TriggerTime.Every;
    [SerializeField] GameObject[] _showOnEnter;
    [SerializeField] GameObject[] _hideOnEnter;
    [SerializeField] GameObject[] _showOnLookAt;
    [SerializeField] GameObject[] _hideOnLookAt;

    bool _isAlreadyLooked = false;

    void Start()
    {
        _lookAtObjectFromAreaEventHandler.OnEnterArea.AddListener(_OnEnterArea);
        _lookAtObjectFromAreaEventHandler.OnStartLookAt.AddListener(_OnStartLookAt);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void _OnEnterArea()
    {
        if(_isAlreadyLooked) return;
        foreach(var obj in _showOnEnter) {
            obj.SetActive(true);
        }
        foreach(var obj in _hideOnEnter) {
            obj.SetActive(false);
        }
    }

    void _OnExitArea() {
        if(_isAlreadyLooked) return;
        foreach(var obj in _showOnLookAt) {
            obj.SetActive(false);
        }
        foreach(var obj in _hideOnLookAt) {
            obj.SetActive(false);
        }
    }

    void _OnStartLookAt()
    {
        if(_isAlreadyLooked) return;
        _isAlreadyLooked = true;
        foreach(var obj in _showOnLookAt) {
            obj.SetActive(true);
        }
        foreach(var obj in _hideOnLookAt) {
            obj.SetActive(false);
        }
    }
}

public enum TriggerTime {
    Every,
    DeactivateOnExit,
    DeactivateOnEndLookAt,
}
