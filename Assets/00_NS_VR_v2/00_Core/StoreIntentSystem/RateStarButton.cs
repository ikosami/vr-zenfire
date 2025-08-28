using UnityEngine;
using NS_VR.Core.Utils;

namespace NS_VR.StoreIntentSystem{
    public class RateStarButton : MonoBehaviour
    {
        [SerializeField] GameObject[] _stars;
        [SerializeField] string _appKey = "self";
        [SerializeField] GameObject _lockObj;
        [SerializeField] GameObject _unlockedObj;

        [SerializeField] int _star = 3;

        public System.Action OnRate;

        public int Star {
            get {
                return _star;
            }
            set {
                _star = value;
            }
        }
        private void Start()

        {
            for (int i = 0; i < _stars.Length; i++) {
                _stars[i].SetActive(i < _star);
            }
        }

        public void SetStar(int star) {
            _star = star;
            for (int i = 0; i < _stars.Length; i++) {
                _stars[i].SetActive(i < _star);
            }
            Debug.Log($"SetStar: {star}");
        }

        public void Rate() {
            Rating.Value = _star;
            if(Rating.Value >= 4) {
                var appInfo = References.Instance.OculusStoreAppData.GetAppInfo(_appKey);
                if (appInfo == null) {
                    Debug.LogError("アプリケーションIDが見つかりません。");
                    return;
                }
                GameEventSender.Instance.RateGame();
                if (appInfo.hasShortLink) {
                    OculusStoreOpener.OpenOculusStoreByURL(appInfo.shortLinkURL);
                } else {
                    OculusStoreOpener.OpenOculusStore(appInfo.appId);
                }
            }
            OnRate?.Invoke();
        }


        public void ChangeView(bool isUnlocked) {
            _lockObj.SetActive(!isUnlocked);
            _unlockedObj.SetActive(isUnlocked);
        }

    }
}