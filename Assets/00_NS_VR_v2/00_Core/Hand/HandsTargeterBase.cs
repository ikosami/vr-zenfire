using UnityEngine;

public abstract class HandsTargeterBase : MonoBehaviour
	{
		public virtual Transform HandGoalDefaultLeft { get; }

		public virtual Transform HandGoalDefaultRight { get; }

		public Transform HandGoalCurrentLeft { get; protected set; }

		public Transform HandGoalCurrentRight { get; protected set; }

		public abstract void TargetHandLeft(Transform target);

		public abstract void TargetHandRight(Transform target);

		public abstract void UntargetHandLeft();

		public abstract void UntargetHandRight();
	}