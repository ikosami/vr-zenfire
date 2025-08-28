# NS VR Template - Proposed Modules

## Core Modules (Non-VR Specific)

### Event System
- **Location**: `Assets/0_NS_VR/EventSystem/EventManager.cs`
- **Description**: Type-safe event handling system using string-based event names and generic event data.
- **Key Features**:
  - Event subscription/unsubscription
  - Event triggering with custom data
  - Automatic event cleanup
  - DontDestroyOnLoad support

### Sound System
- **Location**: `Assets/0_NS_VR/Sound/SoundManager.cs`
- **Description**: Centralized audio management with support for BGM and SFX.
- **Key Features**:
  - Sound pooling
  - 3D audio support
  - Custom pitch and volume control
  - BGM crossfading

### Scene Management
- **Location**: `Assets/0_NS_VR/Utils/VRTransitionManager.cs`
- **Description**: Scene transition handling with fade effects.
- **Key Features**:
  - Smooth fade transitions
  - Callback support
  - Custom fade colors and durations

### Performance Optimization
- **Location**: `Assets/0_NS_VR/Core/Performance/PerformanceProfileInit.cs`
- **Description**: Quest-specific performance optimization settings.
- **Key Features**:
  - Frame rate management
  - Quality settings optimization
  - Dynamic resolution scaling
  - Foveated rendering configuration

### Coroutine Management
- **Location**: `Assets/0_NS_VR/Utils/CoroutineRunner.cs`
- **Description**: Centralized coroutine management utility.
- **Key Features**:
  - Delayed execution
  - Frame-based waiting
  - Coroutine cancellation
  - Singleton pattern implementation

## VR-Specific Modules

### Input Management
- **Location**: `Assets/0_NS_VR/Core/Input/VRInputManager.cs`
- **Description**: Unified VR input handling system.
- **Key Features**:
  - Controller tracking
  - Button/trigger state management
  - Analog input support
  - Hand-specific input mapping

### Hand Pose System
- **Location**: `Assets/0_NS_VR/Hand/HandPoseManager.cs`
- **Description**: Hand pose and animation management.
- **Key Features**:
  - Per-finger animation
  - Pose mirroring
  - Smooth transitions
  - Event-based pose changes

### Movement System
- **Location**: `Assets/0_NS_VR/Core/Movement/MovementController.cs`
- **Description**: VR movement and locomotion system.
- **Key Features**:
  - Stick-based movement
  - Arm-swing locomotion
  - Boundary management
  - Physics-based movement

### Haptic Feedback
- **Location**: `Assets/0_NS_VR/Utils/HapticReferencesScriptable.cs`
- **Description**: Haptic feedback management system.
- **Key Features**:
  - Customizable haptic patterns
  - Controller-specific feedback
  - Intensity control
  - Pattern presets

### World-Space UI
- **Location**: `Assets/0_NS_VR/UI/UIManager.cs`
- **Description**: VR-specific UI management system.
- **Key Features**:
  - World-space canvas handling
  - Tutorial system
  - Game completion UI
  - Video playback support

### Interaction System
- **Location**: `Assets/0_NS_VR/Grabbable.cs`
- **Description**: Object interaction and grabbing system.
- **Key Features**:
  - Hand pose integration
  - Physics-based grabbing
  - Mirror support
  - Event triggers

## Testing Strategy

Each module will require specific testing approaches:

1. **Event System Tests**:
   - Event registration/unregistration
   - Event triggering
   - Event data passing
   - Cleanup verification

2. **Sound System Tests**:
   - Audio playback
   - 3D positioning
   - Volume/pitch modification
   - BGM state management

3. **Input Tests**:
   - Button state verification
   - Analog input ranges
   - Controller tracking
   - Input mapping

4. **Hand Pose Tests**:
   - Pose transitions
   - Mirror functionality
   - Event handling
   - Animation timing

5. **Movement Tests**:
   - Locomotion methods
   - Collision handling
   - Boundary checks
   - Physics integration

6. **UI Tests**:
   - Canvas positioning
   - Video playback
   - Event handling
   - State management

Test scenes will be created for modules requiring runtime verification, particularly for VR-specific features that need visual confirmation.
