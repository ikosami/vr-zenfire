using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using NS_VR.Core.Utils;

namespace NS_VR.StoreIntentSystem
{
    [RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable))]
    public class OpenStoreButton : MonoBehaviour
    {
        private UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable _interactable;
        private MeshRenderer _meshRenderer;
        private bool _isHovered = false;
        private ControllerSide _hoveringController = ControllerSide.Left;

        [Header("見た目の設定")]
        [SerializeField]
        private Color _normalColor = Color.white;
        [SerializeField]
        private Color _hoverColor = Color.cyan;
        [SerializeField]
        private float _triggerThreshold = 0.5f;

        [Header("ストアアプリの設定")]
        [SerializeField]
        private string _appKey;
        [SerializeField]
        private SpriteRenderer _thumbnailRenderer;

        public System.Action OnOpenStore;

        private void Awake()
        {
            _interactable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>();
            _meshRenderer = GetComponent<MeshRenderer>();

            // ホバーイベントの設定
            _interactable.hoverEntered.AddListener(OnHoverEnter);
            _interactable.hoverExited.AddListener(OnHoverExit);

            // サムネイル画像の設定
            SetupThumbnail();
        }

        private void SetupThumbnail()
        {
            if (!string.IsNullOrEmpty(_appKey) && _thumbnailRenderer != null)
            {
                var appInfo = References.Instance.OculusStoreAppData.GetAppInfo(_appKey);
                if (appInfo != null)
                {
                    _thumbnailRenderer.sprite = appInfo.thumbnail;
                }
            }
        }

        private void Update()
        {
            if (_isHovered)
            {
                // ホバーしているコントローラーのトリガーのみチェック
                bool triggerPressed = VRInputManager.Instance.IsTriggerPressed(_hoveringController, true, _triggerThreshold);

                if (triggerPressed)
                {
                    var appInfo = References.Instance.OculusStoreAppData.GetAppInfo(_appKey);
                    if (appInfo == null) {
                        Debug.LogError("アプリケーションIDが見つかりません。");
                        return;
                    }
                    if (appInfo.hasShortLink) {
                        OculusStoreOpener.OpenOculusStoreByURL(appInfo.shortLinkURL);
                    } else {
                        OculusStoreOpener.OpenOculusStore(appInfo.appId);
                    }
                    OnOpenStore?.Invoke();
                }
            }
        }

        public void SetEmmissionColor(Color color)
        {
            if (_meshRenderer != null && _meshRenderer.material != null)
            {
                _meshRenderer.material.SetColor("_EmissionColor", color);
            }
        }

        public void SetColorNormal(Color color)
        {
            if (_meshRenderer != null && _meshRenderer.material != null)
            {
                _meshRenderer.material.color = color;
            }
        }

        public void SetColorHover(Color color)
        {
            if (_meshRenderer != null && _meshRenderer.material != null)
            {
                _meshRenderer.material.color = color;
            }
        }

        private void OnHoverEnter(HoverEnterEventArgs args)
        {
            _isHovered = true;
            
            // ホバーしているコントローラーを判定
            if (args.interactorObject is UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInteractor interactor)
            {
                // コントローラーの名前で左右を判定
                _hoveringController = interactor.name.ToLower().Contains("left") ? 
                    ControllerSide.Left : ControllerSide.Right;
            }

            // ホバー時の色変更
            SetColorHover(_hoverColor);
        }

        private void OnHoverExit(HoverExitEventArgs args)
        {
            _isHovered = false;
            // ホバー解除時の色を戻す
            SetColorNormal(_normalColor);
        }

        private void OnDestroy()
        {
            // イベントの解除
            if (_interactable != null)
            {
                _interactable.hoverEntered.RemoveListener(OnHoverEnter);
                _interactable.hoverExited.RemoveListener(OnHoverExit);
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            // エディタでの値変更時にサムネイルを更新
            // SetupThumbnail();
        }
#endif
    }
} 
