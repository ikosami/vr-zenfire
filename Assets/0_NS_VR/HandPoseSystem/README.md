# Hand Pose System

A comprehensive system for managing VR hand poses, animations, and interactions.

## Components

### Core Components
- `HandPoseManager.cs` - Manages hand pose states and animations
- `HandPoseScriptable.cs` - ScriptableObject for configuring hand poses
- `HandCollisionHandler.cs` - Handles physical interactions using FixedJoint
- `ControllerHandPose.cs` - Maps controller inputs to hand poses

### Editor Tools
- `HandPoseManagerEditor.cs` - Custom editor for hand pose configuration

### Asset Types
- Hand Pose Assets (in `/Poses/`) - Predefined hand pose configurations
  - Neutral poses
  - Grab poses
  - Gesture poses (Guu, Paa, etc.)

## Features

- Per-finger animation control
- Physics-based collision handling
- Smooth pose transitions
- Mirror pose generation
- Controller input mapping
- Editor tools for pose configuration

## Usage

1. Add HandPoseManager to your VR rig
2. Configure poses using HandPoseScriptable assets
3. Set up collision handling with HandCollisionHandler
4. Map controller inputs using ControllerHandPose

## Implementation Details

### Hand Pose Animation
- Uses per-finger rigging for precise control
- Implements smooth transitions between poses
- Supports both immediate and interpolated pose changes

### Physics Integration
- Uses FixedJoint for stable hand-object interactions
- Prevents hand penetration through solid objects
- Configurable joint parameters for different interaction types

### Input Mapping
- Supports various controller inputs (buttons, triggers)
- Configurable pose mapping per input
- Left/right hand independence
