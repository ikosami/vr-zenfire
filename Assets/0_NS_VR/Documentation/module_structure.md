# NS VR Template Module Structure

## General Modules

### Core/
- Input/ - VR input handling and initialization
- Debug/ - Debug visualization tools
- Performance/ - Performance optimization and profiling
- Physics/ - Physics interaction systems
- References/ - Asset and prefab references
- SceneManagement/ - Scene transitions and world reset
- Services/ - Shared service management

### Common/
- CoroutineRunner - Centralized coroutine management
- DictionaryUtils - Dictionary operation utilities
- ListExtensions - List operation extensions
- SingletonMonoBehavior - Base class for singletons

### Sound/
- Scripts/ - Sound management scripts
- AudioClips/ - Sound effect assets
- Mixers/ - Audio mixer configurations

### EventSystem/
- Event management and communication
- Type-safe event handling

### Haptics/
- Assets/ - Haptic pattern configurations
- Scripts/ - Haptic feedback controllers

### UI/
- Prefabs/ - Reusable UI components
- World-space UI management
- Tutorial and completion screens

## VR-Specific Modules

### HandPoseSystem/
- HandPoseManager - Pose state management
- HandCollisionHandler - Physics-based hand interactions
- HandPoseScriptable - Pose configuration assets
- Editor/ - Hand pose editor tools
- Poses/ - Predefined hand pose configurations

### MovementSystem/
- MovementController - Player locomotion
- MovementBoundary - Play area restrictions
- PlayerJump - Jump mechanics
- Arm-swing and stick-based movement

### Rendering/
- Shaders/ - VR-specific shaders
- Quality optimization
- Foveated rendering

### People/
- Character systems
- NPC behaviors
- Look-at controllers

## Additional Resources

### Template/
- Template scenes
- Example prefabs
- Getting started resources

### VRTemplateAssets/
- Materials/
- Models/
- Prefabs/
- Scripts/
- Sprites/

### XR Configuration
- XR/ - Platform configuration
- XRI/ - XR Interaction Toolkit setup
