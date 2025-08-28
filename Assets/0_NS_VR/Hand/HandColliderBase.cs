using UnityEngine;

	public abstract class HandColliderBase : MonoBehaviour
		{
			public virtual bool IsCollisionEnabled { get; }

			public abstract void CollisionEnable();

			public abstract void CollisionDisable();
		}