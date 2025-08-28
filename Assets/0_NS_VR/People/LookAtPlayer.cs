using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using ToonPeople;

[RequireComponent(typeof(Animator))]
public class LookAtPlayer : MonoBehaviour
{
    Transform _player => References.Instance.Player;
    [SerializeField] Playanimation _animation;
    [SerializeField] float _lookAtWeight = 1f;
    [SerializeField] float _bodyWeight = 0f;
    [SerializeField] float _headWeight = 0f;
    [SerializeField] float _eyesWeight = 0.3f;
    [SerializeField] float _clampWeight = 0.5f;
    [SerializeField] Vector3 _offset = new Vector3(0, -0.3f, 0);

    Animator animator;

    // Start is called before the first frame update
    void Awake()
    {
        this.animator = GetComponent<Animator> ();
        // _defaultRotation = transform.localRotation;
    }


	private void OnAnimatorIK (int layerIndex)
	{

		this.animator.SetLookAtWeight (_lookAtWeight, _bodyWeight, _headWeight, _eyesWeight, _clampWeight);
        var pos = _player.position + _offset;

		//どこを見るか（今回はカメラの位置）
		this.animator.SetLookAtPosition (pos);
	}
    // Update is called once per frame
    void Update()
    {
        
    }

}
