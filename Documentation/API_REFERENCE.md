# API Reference

## Unity C# API

### PhysicsManager

Manages global physics settings for the simulator.

#### Properties
- `gravity: Vector3` - Current gravity vector (default: (0, -9.81, 0))
- `defaultFriction: float` - Default friction coefficient (0.6)
- `defaultBounciness: float` - Default bounciness (0.3)

#### Methods
- `SetGravity(Vector3 newGravity)` - Update gravity
- `GetGravity(): Vector3` - Get current gravity
- `CreatePhysicsMaterial(float friction, float bounciness): PhysicMaterial` - Create custom physics material
- `GetDefaultMaterial(): PhysicMaterial` - Get default material

### CollisionHandler

Handles collision detection and response.

#### Properties
- `isMovable: bool` - Whether object can be moved
- `isFixed: bool` - Whether object is fixed in place
- `pushForce: float` - Force applied when pushing objects
- `liftForce: float` - Force applied when lifting objects

#### Methods
- `SetFixed(bool fixed)` - Set object as fixed/movable
- `SetMovable(bool movable)` - Set movability
- `SetForces(float push, float lift)` - Configure push/lift forces

#### Events
- `OnCollisionDetected` - Triggered when collision occurs
- `OnTriggerDetected` - Triggered for trigger colliders

### LegoPiece

Base component for all Lego pieces.

#### Properties
- `pieceName: string` - Name of the piece
- `pieceType: LegoType` - Type (Block, Wheel, Motor, Sensor, Arm, Gear, Axle, Connector)
- `pieceColor: Color` - Visual color
- `dimensions: Vector3` - Size of piece
- `mass: float` - Mass in kg

#### Methods
- `ConfigurePiece(string name, LegoType type, Color color, Vector3 dims, float mass)` - Set all properties
- `SetPhysicsProperties(float friction, float bounciness)` - Set physics
- `SetColor(Color color)` - Change color
- `SetMass(float mass)` - Change mass
- `GetPieceType(): LegoType` - Get type
- `GetMass(): float` - Get mass

### LegoMotor

Motor component for rotational movements.

#### Properties
- `motorName: string` - Motor identifier
- `motorType: MotorType` - Type (Standard, Large, Medium, Servo)
- `maxSpeed: float` - Maximum speed in degrees/second
- `torque: float` - Motor torque
- `gearRatio: float` - Gear ratio multiplier

#### Methods
- `SetRotation(float degrees)` - Rotate to absolute position
- `RotateBy(float degrees)` - Rotate by relative amount
- `Run(RotationDirection direction, float speed)` - Continuous rotation
- `Stop()` - Stop motor
- `Reset()` - Reset to zero position
- `GetRotation(): float` - Get current rotation
- `ConfigureMotor(string name, MotorType type, float speed, float torque, float ratio)` - Configure all settings
- `GetMotorAxis(): Transform` - Get axis for attaching components
- `SetGearRatio(float ratio)` - Set gear ratio

#### Events
- `OnRotationChanged` - Triggered when rotation updates

### LegoArm

Arm component for complex movements.

#### Properties
- `armName: string` - Arm identifier
- `armType: ArmType` - Type (Simple, Claw, Gripper, Lifter)
- `length: float` - Arm length
- `maxAngle: float` - Maximum lift angle
- `minAngle: float` - Minimum lift angle
- `liftSpeed: float` - Lifting speed (degrees/second)
- `rotationSpeed: float` - Rotation speed (degrees/second)

#### Methods
- `Move(ArmMovement movement, float amount)` - Execute movement
- `SetAngles(float liftAngle, float rotationAngle)` - Set absolute angles
- `Reset()` - Return to default position
- `GetLiftAngle(): float` - Get lift angle
- `GetRotationAngle(): float` - Get rotation angle
- `ConfigureArm(string name, ArmType type, float length, float maxAngle, float minAngle)` - Configure settings

#### Enums
- `ArmMovement: {LiftUp, LiftDown, RotateLeft, RotateRight, RotateCircular}`

### RobotController

Main robot driving controller.

#### Properties
- `maxSpeed: float` - Maximum speed
- `acceleration: float` - Acceleration rate
- `deceleration: float` - Deceleration rate
- `turnSpeed: float` - Turning speed (degrees/second)

#### Methods
- `Drive(float power)` - Drive with power (-1 to 1)
- `Steer(float amount)` - Steer with amount (-1 left, 1 right)
- `DriveAndSteer(float drivePower, float steerAmount)` - Combined control
- `TankDrive(float leftPower, float rightPower)` - Independent wheel control
- `Stop()` - Stop robot
- `ConfigureDrive(float speed, float accel, float decel, float turn)` - Configure settings
- `SetWheels(Transform left, Transform right, float radius)` - Set wheel references
- `GetSpeed(): float` - Get current speed
- `GetTurnRate(): float` - Get turn rate

### PythonBridge

Communication bridge with Python.

#### Properties
- `port: int` - Server port (default: 5555)
- `autoStart: bool` - Start server automatically

#### Methods
- `StartServer()` - Start TCP server
- `StopServer()` - Stop server
- `SendToPython(string messageType, Dictionary<string, object> data)` - Send message to Python

#### Events
- `OnMessageReceived` - Triggered when message received

### SimulatorSceneManager

Scene management and simulation control.

#### Properties
- `sceneSize: Vector3` - Scene dimensions
- `floorColor: Color` - Floor color
- `simulationRunning: bool` - Simulation state
- `timeScale: float` - Time scale multiplier

#### Methods
- `CreateRobot(Vector3 position): GameObject` - Create robot
- `PlacePiece(string pieceType, Vector3 position, Quaternion rotation): GameObject` - Place Lego piece
- `StartSimulation()` - Start simulation
- `PauseSimulation()` - Pause simulation
- `ResetSimulation()` - Reset scene
- `SetTimeScale(float scale)` - Adjust time scale
- `GetRobot(): GameObject` - Get current robot
- `IsSimulationRunning(): bool` - Check if running

## Python API

### UnityBridge

Low-level communication bridge.

#### Methods
- `__init__(host="localhost", port=5555)` - Initialize
- `connect(): bool` - Connect to Unity
- `disconnect()` - Disconnect
- `send_message(MessageType type, dict data): bool` - Send message
- `register_handler(str type, Callable handler)` - Register message handler
- `send_motor_command(str name, float rotation, float speed)` - Send motor command
- `send_robot_state(tuple position, tuple rotation, tuple velocity)` - Send state

### Mindstorms API

#### MindstormsHub
- `__init__(host="localhost", port=5555)`
- `connect(): bool`
- `disconnect()`
- `get_motor(str name): MindstormsMotor`
- `get_sensor(str name, str type): MindstormsSensor`

#### MindstormsMotor
- `run_to_position(int position, int speed=100)`
- `run_for_degrees(int degrees, int speed=100)`
- `run_for_rotations(float rotations, int speed=100)`
- `start(int speed=100)`
- `stop()`
- `reset()`

#### MindstormsSensor
- `read(): float` - Read sensor value

### Spike Prime API

#### SpikePrimeHub
- `__init__(host="localhost", port=5555)`
- `connect(): bool`
- `disconnect()`
- `motor(str port): SpikePrimeMotor` - Get motor on port A-F
- `distance_sensor(str port): SpikePrimeSensor`
- `color_sensor(str port): SpikePrimeSensor`
- `motor_pair(str port1, str port2): tuple` - Get motor pair

#### SpikePrimeMotor
- `run_to_position(int degrees, int speed=100)`
- `run_for_degrees(int degrees, int speed=100)`
- `run_for_rotations(float rotations, int speed=100)`
- `start(int speed=100)`
- `stop()`
- `set_degrees_counted(int degrees)`
- `get_position(): int`

#### SpikePrimeSensor
- `get_distance_cm(): int` - Distance in cm
- `get_color(): str` - Detected color
- `get_ambient_light(): int` - Light level

### Pybricks API

#### EV3Brick
- `__init__(host="localhost", port=5555)`
- `connect(): bool`
- `disconnect()`
- `motor(str port): Motor`
- `ultrasonic_sensor(str port): UltrasonicSensor`
- `color_sensor(str port): ColorSensor`
- `create_drive_base(str left_port, str right_port, float wheel_diameter, float axle_track): DriveBase`

#### Motor
- `run(int speed)` - Continuous run
- `run_time(int speed, int time)` - Run for milliseconds
- `run_angle(int speed, int angle)` - Run for angle
- `run_target(int speed, int target)` - Run to target
- `stop()`
- `reset_angle(int angle=0)`

#### DriveBase
- `straight(float distance)` - Drive straight in mm
- `turn(float angle)` - Turn in degrees
- `drive(float speed, float turn_rate)` - Drive with turn
- `stop()`

#### UltrasonicSensor
- `distance(): int` - Distance in mm

#### ColorSensor
- `color(): str` - Detected color
- `reflection(): int` - Reflection value (0-100)

## Message Protocol

### Message Format
```json
{
  "type": "message_type",
  "data": {
    "key": "value"
  }
}
```

### Message Types

#### motor_command
```json
{
  "type": "motor_command",
  "data": {
    "motor_name": "left_motor",
    "rotation": 180.0,
    "speed": 0.5
  }
}
```

#### sensor_data
```json
{
  "type": "sensor_data",
  "data": {
    "sensor_name": "distance",
    "value": 42.5
  }
}
```

#### robot_state
```json
{
  "type": "robot_state",
  "data": {
    "position": {"x": 0, "y": 0, "z": 0},
    "rotation": {"x": 0, "y": 90, "z": 0},
    "velocity": {"x": 1.5, "y": 0, "z": 0}
  }
}
```

#### configuration
```json
{
  "type": "configuration",
  "data": {
    "setting": "value"
  }
}
```
