using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class CollisionSound : MonoBehaviour
{
    // 衝突時に再生する音声クリップ
    public AudioClip collisionClip;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // オブジェクトとの衝突を検出して音を再生
    void OnCollisionEnter(Collision collision)
    {
        if (collisionClip != null)
        {
            audioSource.PlayOneShot(collisionClip);
        }
    }
} 