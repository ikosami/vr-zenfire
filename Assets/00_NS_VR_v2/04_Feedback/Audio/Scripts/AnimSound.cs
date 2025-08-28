using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimSound : MonoBehaviour
{
    //アニメーションから呼び出すためのメソッド
    public void PlaySound(string soundName)
    {
        SoundManager.Instance.Play(soundName);
    }
}
