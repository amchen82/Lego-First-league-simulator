# Lego First League Simulator

A comprehensive web-based 3D Lego simulator for kids to prepare for Lego competitions.

## Overview

Create and program virtual Lego robots in a realistic 3D environment with physics simulation. Build robot setups, configure piece properties, simulate driving with acceleration and steering, control motor rotations for gear-based movements, and experience realistic physics with gravity, friction, and collision handling.

## Features

- **3D Building Environment**: Build and configure Lego robot setups in 3D space
- **Physics Simulation**: Realistic gravity, friction, and collision detection
- **Motor Control**: Precise motor rotations for gear-based movements (arms lifting, rotating)
- **Robot Driving**: Acceleration, steering, and tank drive control
- **Python SDK Integration**: Support for Mindstorms EV3, Spike Prime, and Pybricks
- **WebGL Export**: Run in web browser, cross-platform compatible

## Quick Start

### Unity Project
1. Open `UnityProject` folder in Unity 2021.3.20f1 or later
2. Open the main scene and press Play
3. The simulator will start with a Python communication bridge on port 5555

### Python Programming
1. Install Python dependencies: `pip install -r PythonIntegration/requirements.txt`
2. Run example scripts from `PythonIntegration/examples/`
3. Control your virtual robot using Mindstorms, Spike Prime, or Pybricks APIs

## Documentation

See [Documentation/README.md](Documentation/README.md) for comprehensive documentation including:
- Detailed feature descriptions
- Installation instructions
- API reference
- Example code
- Building for WebGL
- Troubleshooting guide

## Example Usage

```python
from mindstorms.mindstorms_integration import MindstormsHub

# Connect to simulator
hub = MindstormsHub()
hub.connect()

# Control motors
left_motor = hub.get_motor("left_motor")
right_motor = hub.get_motor("right_motor")

# Drive forward
left_motor.run_for_rotations(2, speed=50)
right_motor.run_for_rotations(2, speed=50)

hub.disconnect()
```

## Project Structure

- `UnityProject/` - Unity project with C# scripts for 3D simulation
- `PythonIntegration/` - Python SDK wrappers for controlling robots
- `Documentation/` - Comprehensive documentation

## License

MIT License - see LICENSE file for details
