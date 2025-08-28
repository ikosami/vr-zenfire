using System.Collections;
using UnityEngine;

public class WaitStopParticle : MonoBehaviour
{
    public float stopTime = 3f; // パーティクルを停止するまでの時間
    public float destroyTime = 2f; // 停止後にオブジェクトを削除する時間

    void Start()
    {
        StartCoroutine(StopParticles());
    }

    IEnumerator StopParticles()
    {
        yield return new WaitForSeconds(stopTime);

        // 自身とすべての子オブジェクトの ParticleSystem を停止
        ParticleSystem[] particles = GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem ps in particles)
        {
            ps.Stop();
        }

        // 一定時間後に自身を削除
        yield return new WaitForSeconds(destroyTime);
        Destroy(gameObject);
    }
}
