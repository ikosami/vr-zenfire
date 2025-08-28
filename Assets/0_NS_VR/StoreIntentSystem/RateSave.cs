using UnityEngine;

namespace NS_VR.StoreIntentSystem{
    public class RateSave : MonoBehaviour
    {
        public OpenStoreButton _openStoreButton;

        [SerializeField] Color _ratedColorNormal;
        [SerializeField] Color _ratedColorHover;

        [ColorUsage(true, true)]
        [SerializeField] Color _ratedEmmissionColor;

        public bool IsRated {
            get {
                return PlayerPrefs.GetInt("IsRated", 0) == 1;
            }
            set {
                PlayerPrefs.SetInt("IsRated", value ? 1 : 0);
            }
        }

        private void Start()
        {
            _openStoreButton.OnOpenStore += OnOpenStore;
            OnOpenStore();
        }

        private void OnOpenStore()
        {
            if (IsRated) {
                return;
            }
            _openStoreButton.SetColorNormal(_ratedColorNormal);
            _openStoreButton.SetColorHover(_ratedColorHover);
            _openStoreButton.SetEmmissionColor(_ratedEmmissionColor);

        }
    }
}