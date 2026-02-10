# Lego First League Simulator

A comprehensive web-based 3D Lego simulator for kids to prepare for Lego competitions. Build robot setups in a 3D environment, configure piece properties, simulate driving with acceleration and steering, control motor rotations for gear-based movements, and experience realistic physics with gravity, friction, and collision handling.

## Features

### 3D Building Environment
- Build and configure Lego robot setups in 3D space
- Place and arrange Lego pieces with customizable properties
- Visual representation of robots, motors, arms, and sensors

### Physics Simulation
- Realistic gravity and friction physics
- Collision detection and response
  - Fixed objects block movement
  - Movable objects can be pushed or lifted
- Configurable physics materials for different surfaces

### Motor and Movement Control
- Motor rotation input for gear-based movements
- Support for arms lifting up/down, left/right, and circular rotation
- Differential drive robot controller with acceleration and steering
- Tank drive mode for independent wheel control

### Python SDK Integration
The simulator integrates with three major Lego programming platforms:

1. **Mindstorms EV3**
   - Motor control (position, rotation, continuous running)
   - Sensor support (ultrasonic, color, touch)
   - Hub communication

2. **Spike Prime**
   - Port-based motor control (A-F)
   - Distance and color sensors
   - Motor pairing for drive bases

3. **Pybricks**
   - Advanced motor control with precise positioning
   - DriveBase for differential drive robots
   - Comprehensive sensor suite

### WebGL Export
- Unity project configured for WebGL export
- Run simulator in web browser
- Cross-platform compatibility

## Project Structure

```
Lego-First-league-simulator/
├── UnityProject/                    # Unity project files
│   ├── Assets/
│   │   ├── Scripts/                 # Unity C# scripts
│   │   │   ├── PhysicsManager.cs    # Physics system
│   │   │   ├── CollisionHandler.cs  # Collision detection
│   │   │   ├── LegoPiece.cs        # Lego piece component
│   │   │   ├── LegoMotor.cs        # Motor control
│   │   │   ├── LegoArm.cs          # Arm movements
│   │   │   ├── RobotController.cs  # Robot driving
│   │   │   ├── PythonBridge.cs     # Python communication
│   │   │   └── SimulatorSceneManager.cs # Scene management
│   │   ├── Prefabs/                # Reusable game objects
│   │   └── Scenes/                 # Unity scenes
│   └── ProjectSettings/            # Unity project settings
├── PythonIntegration/              # Python SDK wrappers
│   ├── unity_bridge.py             # Unity communication bridge
│   ├── mindstorms/
│   │   └── mindstorms_integration.py
│   ├── spike_prime/
│   │   └── spike_integration.py
│   └── pybricks/
│       └── pybricks_integration.py
├── Documentation/                   # Documentation files
└── README.md
```

## Getting Started

### Prerequisites

#### Unity Setup
- Unity 2021.3.20f1 or later
- WebGL Build Support module

#### Python Setup
- Python 3.7 or later
- Required packages (see requirements.txt)

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/amchen82/Lego-First-league-simulator.git
   cd Lego-First-league-simulator
   ```

2. **Open Unity Project**
   - Open Unity Hub
   - Add the `UnityProject` folder as a project
   - Open the project in Unity

3. **Install Python dependencies**
   ```bash
   cd PythonIntegration
   pip install -r requirements.txt
   ```

### Running the Simulator

#### Option 1: Unity Editor (Development)

1. Open the project in Unity
2. Open the main scene from `Assets/Scenes`
3. Press Play in Unity Editor
4. Run your Python script to connect and control the robot

#### Option 2: WebGL Build (Production)

1. In Unity, go to `File > Build Settings`
2. Select `WebGL` platform
3. Click `Build` and choose output folder
4. Host the build folder on a web server
5. Access via web browser

### Example Usage

#### Mindstorms Example

```python
from mindstorms.mindstorms_integration import MindstormsHub

# Create and connect hub
hub = MindstormsHub()
hub.connect()

# Get motors
left_motor = hub.get_motor("left_motor")
right_motor = hub.get_motor("right_motor")
arm_motor = hub.get_motor("arm_motor")

# Drive forward
left_motor.run_for_rotations(2, speed=50)
right_motor.run_for_rotations(2, speed=50)

# Lift arm
arm_motor.run_to_position(90, speed=30)

# Disconnect
hub.disconnect()
```

#### Spike Prime Example

```python
from spike_prime.spike_integration import SpikePrimeHub

# Create and connect hub
hub = SpikePrimeHub()
hub.connect()

# Get motors
motor_a = hub.motor("A")
motor_b = hub.motor("B")

# Drive forward
motor_a.run_for_rotations(2, speed=50)
motor_b.run_for_rotations(2, speed=50)

# Get motor pair for tank drive
left, right = hub.motor_pair("A", "B")
left.start(50)
right.start(50)

# Disconnect
hub.disconnect()
```

#### Pybricks Example

```python
from pybricks.pybricks_integration import EV3Brick

# Create and connect brick
brick = EV3Brick()
brick.connect()

# Create drive base
robot = brick.create_drive_base("A", "B", wheel_diameter=56, axle_track=114)

# Drive straight 200mm
robot.straight(200)

# Turn 90 degrees
robot.turn(90)

# Stop and disconnect
robot.stop()
brick.disconnect()
```

## Unity Components

### PhysicsManager
Manages global physics settings including gravity and friction. Provides physics material creation for different surface types.

### CollisionHandler
Handles collision detection and response:
- **Fixed objects**: Block other objects from passing through
- **Movable objects**: Can be pushed horizontally or lifted vertically
- Configurable push and lift forces

### LegoPiece
Base component for all Lego pieces:
- Configurable properties (name, type, color, dimensions, mass)
- Physics properties (friction, bounciness)
- Support for different piece types (blocks, wheels, motors, sensors, arms)

### LegoMotor
Motor component for rotational movements:
- Set absolute rotation position
- Rotate by relative amount
- Continuous running at specified speed
- Gear ratio support for compound mechanisms

### LegoArm
Arm component for complex movements:
- Lift up/down with angle limits
- Rotate left/right
- Circular rotation
- Configurable arm types (simple, claw, gripper, lifter)

### RobotController
Main robot driving controller:
- Acceleration and deceleration
- Steering control
- Tank drive mode
- Wheel visual rotation

### PythonBridge
Communication bridge between Unity and Python:
- TCP socket server (default port 5555)
- JSON message protocol
- Motor command handling
- Bidirectional communication

### SimulatorSceneManager
Scene management and simulation control:
- Environment setup
- Robot and piece placement
- Simulation start/pause/reset
- Time scale control

## Python Integration

### UnityBridge
Low-level communication with Unity:
- Socket connection management
- Message serialization/deserialization
- Event-driven message handling
- Threaded receive loop

### SDK Wrappers
Each SDK wrapper (Mindstorms, Spike Prime, Pybricks) provides:
- Motor control classes
- Sensor classes
- Hub/Brick main class
- Example usage patterns

## Building for WebGL

1. In Unity: `File > Build Settings`
2. Select `WebGL` platform
3. Click `Switch Platform`
4. Configure `Player Settings`:
   - Set company name and product name
   - Configure WebGL template
   - Set memory size (512MB recommended)
5. Click `Build` or `Build And Run`

## Configuration

### Unity Settings
- Edit physics settings in `PhysicsManager` component
- Adjust collision forces in `CollisionHandler` components
- Configure motor speeds and gear ratios in `LegoMotor` components

### Python Settings
- Default connection: `localhost:5555`
- Change in hub/brick initialization: `Hub(host="127.0.0.1", port=5555)`

## Troubleshooting

### Connection Issues
- Ensure Unity simulator is running before connecting Python
- Check firewall settings for port 5555
- Verify IP address and port in Python scripts

### Performance Issues
- Reduce physics update rate in Unity Project Settings
- Simplify Lego piece meshes
- Limit number of simultaneous collisions

### Build Issues
- Ensure WebGL Build Support is installed in Unity Hub
- Check Unity version compatibility (2021.3.20f1 recommended)
- Clear build cache if issues persist

## Contributing

Contributions are welcome! Please feel free to submit issues, feature requests, or pull requests.

## License

This project is open source and available under the MIT License.

## Acknowledgments

- Lego Mindstorms EV3
- Lego Spike Prime
- Pybricks
- Unity Technologies
