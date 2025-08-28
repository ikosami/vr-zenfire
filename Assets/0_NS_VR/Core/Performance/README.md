# Performance Optimization Module

A comprehensive system for optimizing VR performance on Oculus Quest devices.

## Components

### Core Components
- `PerformanceProfileInit.cs` - Initializes and manages Quest-specific performance settings

## Performance Settings

### Display Settings
- Frame Rate: 72Hz (optimal for Quest)
- VSync: Disabled (using Quest's native synchronization)
- Frame Queue: Limited to 2 frames

### Quality Settings
1. Shadow Configuration
   - Shadow Distance: 20 units
   - Shadow Resolution: Low
   - Shadow Cascades: 2 levels

2. Anti-Aliasing
   - MSAA 4x enabled
   - Balanced quality/performance ratio

3. Texture Quality
   - Mipmap Limit: 1 (half resolution)
   - Optimized for mobile VR

### Foveated Rendering
- Level: High
- Dynamic Foveated Rendering: Enabled
- Optimizes rendering based on eye tracking

### Dynamic Resolution
- Base Scale: 1.0
- Dynamic Range: 0.7 to 1.2
- Adjusts based on GPU utilization
- Uses XRSettings.eyeTextureResolutionScale

## Usage

1. Add PerformanceProfileInit to your VR scene
2. Configure settings in inspector:
   - Frame rate target
   - Quality presets
   - Dynamic resolution bounds

## Implementation Details

### Initialization
```csharp
void Awake()
{
    // Frame rate configuration
    Application.targetFrameRate = 72;
    OVRPlugin.systemDisplayFrequency = 72;

    // Quality settings
    QualitySettings.vSyncCount = 0;
    QualitySettings.maxQueuedFrames = 2;
    QualitySettings.shadowDistance = 20f;
    QualitySettings.shadowResolution = ShadowResolution.Low;
    QualitySettings.shadowCascades = 2;
    QualitySettings.antiAliasing = 4;
    QualitySettings.globalTextureMipmapLimit = 1;

    // Foveated rendering
    OVRManager.foveatedRenderingLevel = OVRManager.FoveatedRenderingLevel.High;
    OVRManager.useDynamicFoveatedRendering = true;
}
```

### Dynamic Resolution Scaling
```csharp
private void AdjustRenderQuality(bool increase)
{
    float currentScale = XRSettings.eyeTextureResolutionScale;
    float newScale = increase ? currentScale + 0.1f : currentScale - 0.1f;
    newScale = Mathf.Clamp(newScale, 0.7f, 1.2f);
    XRSettings.eyeTextureResolutionScale = newScale;
}
```

## Performance Impact

### Memory
- Reduced texture memory usage
- Optimized shadow map allocation
- Efficient frame buffer management

### GPU
- Reduced pixel shader load
- Optimized fill rate
- Dynamic quality scaling

### CPU
- Minimal overhead
- Efficient frame pacing
- Reduced draw call impact
