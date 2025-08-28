import os
import json
import shutil
from pathlib import Path

# Create package directory
package_dir = Path("ns-vr-template")
package_dir.mkdir(exist_ok=True)

# Copy Assets/0_NS_VR directory
src_dir = Path("Assets/0_NS_VR")
dst_dir = package_dir / "Assets/0_NS_VR"
shutil.copytree(src_dir, dst_dir, dirs_exist_ok=True)

# Copy package.json
shutil.copy2("Assets/0_NS_VR/Package/package.json", package_dir)

# Create asmdef file
asmdef = {
    "name": "NewStory.VRTemplate",
    "references": [
        "Unity.XR.Oculus",
        "Unity.XR.Interaction.Toolkit"
    ],
    "includePlatforms": [],
    "excludePlatforms": [],
    "allowUnsafeCode": False,
    "overrideReferences": False,
    "precompiledReferences": [],
    "autoReferenced": True,
    "defineConstraints": [],
    "versionDefines": [],
    "noEngineReferences": False
}

with open(package_dir / "NewStory.VRTemplate.asmdef", "w") as f:
    json.dump(asmdef, f, indent=4)

# Create README.md
readme = """# NS VR Template

A comprehensive VR template for Oculus Quest development with modular systems including:
- Input Management
- Movement & Locomotion
- Physics Interactions
- Weapon Systems
- UI Framework
- Puzzle System
- Performance Optimization

## Installation

1. Add the package to your Unity project using the Package Manager
2. Import the sample scenes and prefabs
3. Configure XR settings for Oculus Quest

## Documentation

See the Documentation folder for detailed information about:
- Architecture
- Module Structure
- Usage Guidelines
"""

with open(package_dir / "README.md", "w") as f:
    f.write(readme)

# Create zip archive
shutil.make_archive("ns-vr-template", "zip", ".", "ns-vr-template")

# Cleanup temporary directory
shutil.rmtree(package_dir)

print("Package created successfully: ns-vr-template.zip")
