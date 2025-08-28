using System;
using UnityEngine;

[Serializable]
	public struct HandTransform
	{
		public Transform[] Index;

		public Transform[] Middle;

		public Transform[] Ring;

		public Transform[] Pinky;

		public Transform[] Thumb;
	}