using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunComplete : MonoBehaviour
{
    [SerializeField] GameObject _completeUI;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Run()
    {
        _completeUI.SetActive(true);
        References.Instance.GetSceneParticle("confetti").transform.SetPositionAndRotation(_completeUI.transform.position, _completeUI.transform.rotation);
        References.Instance.GetSceneParticle("confetti").Play();
        Invoke(nameof(Delay), 1f);
    }

    void Delay()
    {
        VRTransitionManager.Instance.Run(onOutEnded: () =>
        {
            _completeUI.SetActive(false);
        }, null);
    }
}
