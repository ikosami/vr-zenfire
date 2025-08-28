# Movement System

A comprehensive VR movement and locomotion system supporting multiple movement modes and boundary management.

## Components

### Core Components
- `MovementController.cs` - Main movement system supporting both stick-based and arm-swing locomotion
- `MovementBoundary.cs` - Play area restriction and boundary visualization
- `PlayerJump.cs` - VR-optimized jumping mechanics

## Features

### Movement Modes
1. Stick-Based Movement
   - Camera-relative direction
   - Configurable movement speed
   - Smooth acceleration and deceleration
   - Speed limiting

2. Arm-Swing Movement (Gorilla Tag Style)
   - Physics-based arm swing detection
   - Velocity-based movement
   - Configurable force multipliers
   - Trigger activation

3. Jumping System
   - Button-activated jumping
   - Physics-based vertical movement
   - Ground detection
   - Configurable jump height

### Boundary System
- Box and sphere boundary shapes
- Visual boundary indicators
- Screen fade on boundary violation
- Customizable boundary sizes
- Editor visualization tools

## Usage

1. Add MovementController to your VR rig
2. Configure movement parameters:
   - Base movement speed
   - Maximum velocity
   - Arm swing force
   - Jump power

3. Set up boundaries:
   - Choose boundary type (Box/Sphere)
   - Set boundary dimensions
   - Configure fade settings

## Implementation Details

### Movement Calculation
- Uses CharacterController for movement
- Implements friction and momentum
- Handles slope detection
- Prevents wall clipping

### Boundary Management
- Uses raycasting for boundary detection
- Implements smooth fade transitions
- Supports multiple boundary shapes
- Editor gizmo visualization
