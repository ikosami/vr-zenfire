using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ToonPeople;

public class Emotable : MonoBehaviour
{
    public class EmotableList : MonoBehaviour
    {
        public List<Emotable> Values = new List<Emotable>();
    }

    public enum EmotableType {
        GrabAndNear,
    }

    public EmotableType Type;

    public static EmotableList Emotables;

    public int EmoteLayer;
    public bool HasVibration = false;

    void Awake() {
        if(Emotables == null) {
            Emotables = new EmotableList();
        }
    }

    public void OnGrab() {
        Emotables.Values.Add(this);
    }

    public void OnRelease() {
        Emotables.Values.Remove(this);
    }
}
