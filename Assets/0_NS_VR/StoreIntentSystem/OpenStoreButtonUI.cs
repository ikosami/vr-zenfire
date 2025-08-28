using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using NS_VR.Core.Utils;
using UnityEngine.UI;

namespace NS_VR.StoreIntentSystem
{
    public class OpenStoreButtonUI : MonoBehaviour
    {

        [Header("ストアアプリの設定")]
        [SerializeField]
        private string _appKey;
        [SerializeField]
        private Image _thumbnail;

        public System.Action OnOpenStore;

        private void Start()
        {
            // サムネイル画像の設定
            SetupThumbnail();
        }

        private void SetupThumbnail()
        {
            if (!string.IsNullOrEmpty(_appKey) && _thumbnail != null)
            {
                var appInfo = References.Instance.OculusStoreAppData.GetAppInfo(_appKey);
                if (appInfo != null)
                {

                    _thumbnail.sprite = appInfo.thumbnail;
                }
            }
        }

        private void Update()
        {
        }

        public void OpenStore() {
            var appInfo = References.Instance.OculusStoreAppData.GetAppInfo(_appKey);
            if (appInfo == null) {
                Debug.LogError("アプリケーションIDが見つかりません。");
                return;
            }
            OculusStoreOpener.OpenOculusStore(appInfo.appId);
            OnOpenStore?.Invoke();
        }
    }
} 
