using System.Collections.Generic;
using UnityEngine;

	public abstract class HandColliderBase : MonoBehaviour
		{
			public bool IsInited { get; protected set; }
			public System.Action OnInited;
			public abstract List<ColliderEventListener> ColliderEventListeners {get;}
			public virtual bool IsCollisionEnabled { get; }

			public abstract void CollisionEnable();

			public abstract void CollisionDisable();
		}