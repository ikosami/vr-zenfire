using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class PerformanceProfileInit : MonoBehaviour
{
    void Awake()
    {
        // Quality設定の最適化
        QualitySettings.vSyncCount = 0; // VSync無効化（Quest独自の同期を使用）
        QualitySettings.maxQueuedFrames = 2; // フレームキューイング制限
        QualitySettings.shadowDistance = 20f; // シャドウ描画距離制限
        QualitySettings.shadowResolution = ShadowResolution.Low;
        QualitySettings.shadowCascades = 2;
        
        // アンチエイリアシング設定
        QualitySettings.antiAliasing = 4; // MSAA 4x
        
        // テクスチャ品質設定
        QualitySettings.globalTextureMipmapLimit = 1; // テクスチャ解像度を半分に

        Application.targetFrameRate = 72;
        // OVRManager.display.displayFrequency = 72;
        OVRPlugin.systemDisplayFrequency = 72;

        // フォビエイテッドレンダリングの強度設定
        OVRManager.foveatedRenderingLevel = OVRManager.FoveatedRenderingLevel.High;
        OVRManager.useDynamicFoveatedRendering = true;
        // OVRManager.display.RecenterPose();
    }

    void Update()
    {
        // // パフォーマンスモニタリング
        // if (OVRManager.gpuUtilLevel > 0.8f)
        // {
        //     // GPU使用率が高い場合、レンダリング品質を下げる
        //     AdjustRenderQuality(false);
        // }
        // else if (OVRManager.gpuUtilLevel < 0.6f)
        // {
        //     // GPU使用率に余裕がある場合、レンダリング品質を上げる
        //     AdjustRenderQuality(true);
        // }
    }
    
    private void AdjustRenderQuality(bool increase)
    {
        float currentScale = XRSettings.eyeTextureResolutionScale;
        float newScale = increase ? currentScale + 0.1f : currentScale - 0.1f;
        newScale = Mathf.Clamp(newScale, 0.7f, 1.2f);
        XRSettings.eyeTextureResolutionScale = newScale;
    }
}
