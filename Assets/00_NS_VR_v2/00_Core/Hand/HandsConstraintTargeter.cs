using UnityEngine;
using UnityEngine.Animations;

public class HandsConstraintTargeter : HandsTargeterBase
    {
        [SerializeField]
        [Header("Visual Hands")]
        private Transform _handLeft;

        [SerializeField]
        private Transform _handRight;

        [SerializeField]
        [Header("Physical Hands")]
        private Transform _handGoalDefaultLeft;

        [SerializeField]
        private Transform _handGoalDefaultRight;

        [SerializeField]
        [Header("VR Controller References")]
        private Transform _controllerLeft;

        [SerializeField]
        private Transform _controllerRight;

        [SerializeField]
        [Header("Movement Settings")]
        private float _followSpeed = 30f;

        [SerializeField]
        private float _maxDistance = 0.3f;

        private ParentConstraint _constraintLeft;
        private ParentConstraint _constraintRight;
        private Rigidbody _rbLeft;
        private Rigidbody _rbRight;

        public override Transform HandGoalDefaultLeft => _handGoalDefaultLeft;
        public override Transform HandGoalDefaultRight => _handGoalDefaultRight;

        private void Awake()
        {
            // コンストレイントの初期化
            _constraintLeft = CreateParentConstraint(_handLeft.gameObject, _handGoalDefaultLeft);
            _constraintRight = CreateParentConstraint(_handRight.gameObject, _handGoalDefaultRight);

            // Rigidbodyの取得と設定
            _rbLeft = _handGoalDefaultLeft.GetComponent<Rigidbody>();
            _rbRight = _handGoalDefaultRight.GetComponent<Rigidbody>();

            ConfigureRigidbody(_rbLeft);
            ConfigureRigidbody(_rbRight);
        }

        private void ConfigureRigidbody(Rigidbody rb)
        {
            if (rb != null)
            {
                rb.isKinematic = true;
                rb.interpolation = RigidbodyInterpolation.Interpolate;
                rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
                rb.constraints = RigidbodyConstraints.FreezeRotation;
            }
        }

        private void Update()
        {
            UpdateHandPosition(_rbLeft, _controllerLeft);
            UpdateHandPosition(_rbRight, _controllerRight);
        }

        private void UpdateHandPosition(Rigidbody handRb, Transform controller)
        {
            if (handRb == null || controller == null) return;

            Vector3 targetPosition = controller.position;
            Vector3 currentPosition = handRb.position;
            
            // コントローラーと物理的な手の距離を制限
            Vector3 direction = targetPosition - currentPosition;
            if (direction.magnitude > _maxDistance)
            {
                targetPosition = currentPosition + direction.normalized * _maxDistance;
            }

            // 物理的な手を目標位置に移動
            Vector3 newPosition = Vector3.Lerp(
                currentPosition,
                targetPosition,
                _followSpeed * Time.deltaTime
            );

            handRb.MovePosition(newPosition);
        }

        private ParentConstraint CreateParentConstraint(GameObject obj, Transform target)
        {
            var constraint = obj.AddComponent<ParentConstraint>();
            
            var source = new ConstraintSource
            {
                sourceTransform = target,
                weight = 1
            };
            
            constraint.AddSource(source);
            constraint.constraintActive = true;
            
            return constraint;
        }

        public override void TargetHandLeft(Transform target)
        {
            if (_constraintLeft != null && target != null)
            {
                _constraintLeft.SetSource(0, new ConstraintSource
                {
                    sourceTransform = target,
                    weight = 1
                });
            }
        }

        public override void TargetHandRight(Transform target)
        {
            if (_constraintRight != null && target != null)
            {
                _constraintRight.SetSource(0, new ConstraintSource
                {
                    sourceTransform = target,
                    weight = 1
                });
            }
        }

        public override void UntargetHandLeft()
        {
            if (_constraintLeft != null)
            {
                _constraintLeft.SetSource(0, new ConstraintSource
                {
                    sourceTransform = _handGoalDefaultLeft,
                    weight = 1
                });
            }
        }

        public override void UntargetHandRight()
        {
            if (_constraintRight != null)
            {
                _constraintRight.SetSource(0, new ConstraintSource
                {
                    sourceTransform = _handGoalDefaultRight,
                    weight = 1
                });
            }
        }
    }