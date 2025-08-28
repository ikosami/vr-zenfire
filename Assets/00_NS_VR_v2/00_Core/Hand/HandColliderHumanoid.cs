using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HandColliderHumanoid : HandColliderBase
{
	[SerializeField]
	private HandVisual _handVisual;

	[SerializeField]
	private float _capsuleRadius;

	[SerializeField]
	private PhysicsMaterial _physicMaterial;

	private Transform[] _index;

	private Transform[] _middle;

	private Transform[] _pinky;

	private Transform[] _ring;

	private Transform[] _thumb;

	private Dictionary<HandPartType, List<Collider>> _fingerColliders;

	private bool _isCollisionEnabled = true;

	[SerializeField]
	private bool _isLeftHand;

	public override bool IsCollisionEnabled => _isCollisionEnabled;

	List<ColliderEventListener> _colliderEventListeners = new List<ColliderEventListener>();

	public override List<ColliderEventListener> ColliderEventListeners => _colliderEventListeners;

	private void Awake()
	{
		_fingerColliders = new Dictionary<HandPartType, List<Collider>>();
		
		// 指のTransform配列を初期化
		_thumb = _handVisual.GetFingerBones(HandPartType.Thumb);
		_index = _handVisual.GetFingerBones(HandPartType.Index);
		_middle = _handVisual.GetFingerBones(HandPartType.Middle);
		_ring = _handVisual.GetFingerBones(HandPartType.Ring);
		_pinky = _handVisual.GetFingerBones(HandPartType.Pinky);
		
		// 各指のコライダーチェーンを作成
		CreateColliderChain(_thumb, HandPartType.Thumb);
		CreateColliderChain(_index, HandPartType.Index);
		CreateColliderChain(_middle, HandPartType.Middle);
		CreateColliderChain(_ring, HandPartType.Ring);
		CreateColliderChain(_pinky, HandPartType.Pinky);
		
		// 追加のコライダーは別途管理
		// if (_extraColliders != null)
		// {
		// 	_fingerColliders["Extra"] = new List<Collider>(_extraColliders);
		// }
		
		// 初期状態ではコリジョンを無効化
		// CollisionDisable();

		IsInited = true;
		OnInited?.Invoke();

		CollisionEnable();
	}

	private void Update()
	{
		if (!_isCollisionEnabled) return;
		
		// 各指のコライダーチェーンを更新
		UpdateColliderChain(_thumb, HandPartType.Thumb);
		UpdateColliderChain(_index, HandPartType.Index);
		UpdateColliderChain(_middle, HandPartType.Middle);
		UpdateColliderChain(_ring, HandPartType.Ring);
		UpdateColliderChain(_pinky, HandPartType.Pinky);
	}

	private Transform[] CreateColliderChain(Transform[] transforms, HandPartType fingerType)
	{
		if (transforms == null || transforms.Length < 2) return transforms;

		var colliders = new List<Collider>();
		_fingerColliders[fingerType] = colliders;

		for (int i = 0; i < transforms.Length - 1; i++)
		{
			var capsule = new GameObject($"Collider_{transforms[i].name}").AddComponent<CapsuleCollider>();
			capsule.gameObject.layer = LayerMask.NameToLayer("HandCollider");
			capsule.enabled = true;
			capsule.transform.SetParent(transforms[i]);
			capsule.transform.localPosition = Vector3.zero;
			capsule.radius = _capsuleRadius;
			capsule.material = _physicMaterial;
			capsule.direction = 2; // Z軸方向
			colliders.Add(capsule);
			_colliderEventListeners.Add(capsule.AddComponent<ColliderEventListener>());
		}

		return transforms;
	}

	private void UpdateColliderChain(Transform[] transforms, HandPartType fingerType)
	{
		if (transforms == null || !_fingerColliders.ContainsKey(fingerType)) return;
		
		var colliders = _fingerColliders[fingerType];
		for (int i = 0; i < transforms.Length - 1; i++)
		{
			if (i >= colliders.Count) continue;
			
			var capsule = colliders[i] as CapsuleCollider;
			if (capsule == null) continue;

			// カプセルの位置とサイズを更新
			Vector3 start = transforms[i].position;
			Vector3 end = transforms[i + 1].position;
			float length = Vector3.Distance(start, end);
			
			capsule.height = length / capsule.transform.lossyScale.z;
			capsule.center = new Vector3(0, 0, length / 2);
			
			// カプセルの向きを更新
			capsule.transform.rotation = Quaternion.LookRotation(end - start);
		}
	}

	public override void CollisionEnable()
	{
		_isCollisionEnabled = true;
		foreach (var colliderList in _fingerColliders.Values)
		{
			foreach (var collider in colliderList)
			{
				collider.enabled = true;
			}
		}
	}

	public override void CollisionDisable()
	{
		_isCollisionEnabled = false;
		foreach (var colliderList in _fingerColliders.Values)
		{
			foreach (var collider in colliderList)
			{
				collider.enabled = false;
			}
		}
	}
}
