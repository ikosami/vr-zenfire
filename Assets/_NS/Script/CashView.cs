using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Level.Cash
{
    public class CashView : MonoBehaviour
    {
        [SerializeField] private GameObject _goTrail;
        [SerializeField] private Transform _rotationNode;
        public Rigidbody Rigidbody;
        public Collider Collider;

        public Transform Rotation => _rotationNode;

        private IEnumerator Start()
        {
            float speed = 0.5f;
            float timer = 0;
            while (timer < speed)
            {
                timer += Time.deltaTime;
                transform.localScale = Vector3.one * timer * 1 / speed;
                yield return null;
            }
            //yield return new WaitForSeconds(10.5f);
            //var rigids = GetComponentsInChildren<Rigidbody>();
            //foreach (var rigid in rigids)
            //{
            //    rigid.isKinematic = true;
            //}
        }

        private void Update()
        {
            if (transform.position.y < -5)
            {
                var pos = transform.position;
                pos.y = 0.1f;
                transform.position = pos;
            }
        }

        public void ShowTrail()
        {
            _goTrail.SetActive(true);
        }

        public void HideTrail()
        {
            _goTrail.SetActive(false);
        }

        private void OnEnable()
        {
        }
    }
}