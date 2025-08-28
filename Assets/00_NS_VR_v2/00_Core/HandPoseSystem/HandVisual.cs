using UnityEngine;

public class HandVisual : MonoBehaviour
	{
		[SerializeField]
		private HandTransform _handTransform;

		public HandTransform HandTransform => _handTransform;

		public Transform[] GetFingerBones(HandPartType handPartType)
		{
			switch (handPartType)
			{
				case HandPartType.Thumb:
					return _handTransform.Thumb;
				case HandPartType.Index:
					return _handTransform.Index;
				case HandPartType.Middle:
					return _handTransform.Middle;
				case HandPartType.Ring:
					return _handTransform.Ring;
				case HandPartType.Pinky:
					return _handTransform.Pinky;
				default:
					return null;
			}
		}
	}