# Architecture Overview

## System Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                        Web Browser                          │
│  ┌───────────────────────────────────────────────────────┐  │
│  │              Unity WebGL Application                  │  │
│  │                                                       │  │
│  │  ┌─────────────────────────────────────────────────┐ │  │
│  │  │           3D Rendering & Physics              │ │  │
│  │  │  • PhysicsManager (Gravity, Friction)         │ │  │
│  │  │  • CollisionHandler (Collision Detection)     │ │  │
│  │  │  • Scene Rendering (3D Visualization)         │ │  │
│  │  └─────────────────────────────────────────────────┘ │  │
│  │                                                       │  │
│  │  ┌─────────────────────────────────────────────────┐ │  │
│  │  │         Robot Components                       │ │  │
│  │  │  • LegoPiece (Piece Properties)               │ │  │
│  │  │  • LegoMotor (Motor Control)                  │ │  │
│  │  │  • LegoArm (Arm Movements)                    │ │  │
│  │  │  • RobotController (Driving)                  │ │  │
│  │  └─────────────────────────────────────────────────┘ │  │
│  │                                                       │  │
│  │  ┌─────────────────────────────────────────────────┐ │  │
│  │  │         Scene Management                       │ │  │
│  │  │  • SimulatorSceneManager                      │ │  │
│  │  │  • Environment Setup                          │ │  │
│  │  │  • Piece Placement                            │ │  │
│  │  └─────────────────────────────────────────────────┘ │  │
│  │                                                       │  │
│  │  ┌─────────────────────────────────────────────────┐ │  │
│  │  │      Python Communication Bridge               │ │  │
│  │  │  • PythonBridge (TCP Server)                  │ │  │
│  │  │  • Message Handler                            │ │  │
│  │  │  • JSON Protocol                              │ │  │
│  │  └─────────────────────────────────────────────────┘ │  │
│  └───────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────┘
                            ▲
                            │ TCP Socket (Port 5555)
                            │ JSON Messages
                            ▼
┌─────────────────────────────────────────────────────────────┐
│                    Python Application                       │
│  ┌───────────────────────────────────────────────────────┐  │
│  │              UnityBridge (Communication)              │  │
│  │  • Socket Management                                  │  │
│  │  • Message Serialization                              │  │
│  │  • Event Handling                                     │  │
│  └───────────────────────────────────────────────────────┘  │
│                            ▲                                │
│                            │                                │
│       ┌────────────────────┼────────────────────┐           │
│       ▼                    ▼                    ▼           │
│  ┌─────────┐        ┌──────────┐        ┌──────────┐       │
│  │Mindstorms│       │Spike Prime│       │Pybricks  │       │
│  │   API    │       │    API    │       │   API    │       │
│  │          │       │           │       │          │       │
│  │ • Motors │       │ • Motors  │       │ • Motors │       │
│  │ • Sensors│       │ • Sensors │       │ • Sensors│       │
│  │ • Hub    │       │ • Hub     │       │ • Brick  │       │
│  └─────────┘        └──────────┘        └──────────┘       │
└─────────────────────────────────────────────────────────────┘
```

## Component Interaction Flow

### Startup Flow

1. **Unity Initialization**
   ```
   Unity Editor/WebGL Build
   └─> PhysicsManager.Awake()
       └─> Initialize physics settings
   └─> SimulatorSceneManager.Awake()
       └─> Create floor
       └─> Initialize scene
   └─> PythonBridge.Start()
       └─> Start TCP server on port 5555
       └─> Wait for Python connection
   ```

2. **Python Connection**
   ```
   Python Script
   └─> hub = MindstormsHub()
   └─> hub.connect()
       └─> UnityBridge.connect()
           └─> Establish TCP connection
           └─> Start receive thread
   ```

### Motor Control Flow

```
Python Script                Unity (C#)
     │                            │
     │  motor.run_to_position()   │
     │─────────────────────────>  │
     │                            │
     │  JSON: motor_command       │
     │─────────────────────────>  │
     │                            │
     │                     PythonBridge
     │                     receives message
     │                            │
     │                     Parse JSON
     │                            │
     │                     Queue on main thread
     │                            │
     │                     Find LegoMotor
     │                            │
     │                     motor.SetRotation()
     │                            │
     │                     Update motor state
     │                            │
     │                     Rotate motor axis
     │                            │
```

### Collision Detection Flow

```
Unity Physics Engine
     │
     ├─> Collision detected
     │   between two objects
     │
     ├─> CollisionHandler.OnCollisionEnter()
     │
     ├─> Check object properties
     │   • Is fixed?
     │   • Is movable?
     │
     ├─> Fixed object?
     │   └─> Block movement
     │       └─> Apply counter force
     │
     ├─> Both movable?
     │   └─> Check collision type
     │       ├─> Vertical? → Lift
     │       └─> Horizontal? → Push
     │
     └─> Apply forces
         └─> Update physics state
```

## Physics System

### Gravity and Friction

```
PhysicsManager
├─> Sets Unity Physics.gravity
├─> Creates PhysicMaterial objects
│   ├─> Dynamic friction
│   ├─> Static friction
│   └─> Bounciness
└─> Applied to all colliders
```

### Collision Handling

```
CollisionHandler on Object A    CollisionHandler on Object B
         │                                │
         ├─> isFixed = true               ├─> isMovable = true
         │                                │
         └─────────────┐    ┌─────────────┘
                       │    │
                  Collision Event
                       │
                       ├─> Object A blocks B
                       ├─> Apply counter force to B
                       └─> B cannot pass through A
```

## Motor System

### Motor Hierarchy

```
LegoMotor Component
├─> Motor Settings
│   ├─> maxSpeed (degrees/second)
│   ├─> torque
│   └─> gearRatio
├─> Motor Axis (Transform)
│   └─> Child objects rotate with axis
├─> Update Loop
│   ├─> Calculate rotation delta
│   ├─> Apply rotation
│   └─> Trigger events
└─> Commands
    ├─> SetRotation(absolute)
    ├─> RotateBy(relative)
    ├─> Run(continuous)
    └─> Stop()
```

### Arm System

```
LegoArm Component
├─> Arm Base (Transform)
│   └─> Rotation pivot
├─> Arm Segment (Visual)
│   └─> Visual representation
├─> Movement Types
│   ├─> LiftUp/LiftDown
│   │   └─> Rotate around X axis
│   ├─> RotateLeft/RotateRight
│   │   └─> Rotate around Y axis
│   └─> RotateCircular
│       └─> Continuous Y rotation
└─> Angle Limits
    ├─> maxAngle
    └─> minAngle
```

## Robot Driving System

### Differential Drive

```
RobotController
├─> Input
│   ├─> Drive power (-1 to 1)
│   └─> Steer amount (-1 to 1)
├─> Physics
│   ├─> Acceleration
│   ├─> Deceleration
│   └─> Turn rate
├─> Rigidbody Movement
│   ├─> MovePosition (forward/backward)
│   └─> MoveRotation (turning)
└─> Wheel Visuals
    ├─> Left wheel rotation
    └─> Right wheel rotation
```

### Tank Drive

```
Tank Drive Input
├─> Left motor power
└─> Right motor power
    │
    ├─> Calculate drive = (left + right) / 2
    ├─> Calculate steer = (right - left) / 2
    │
    └─> RobotController.Drive(drive, steer)
```

## Message Protocol

### JSON Message Structure

```
Message
├─> type: string
│   └─> "motor_command", "sensor_data", etc.
└─> data: object
    └─> Key-value pairs specific to type
```

### Message Flow

```
Python                  Network              Unity
  │                       │                    │
  │  Create message       │                    │
  │  ├─> Type            │                    │
  │  └─> Data            │                    │
  │                       │                    │
  │  Serialize to JSON    │                    │
  │  ├─> json.dumps()    │                    │
  │  └─> Add newline     │                    │
  │                       │                    │
  │  Send bytes          │                    │
  │──────────────────────>│                    │
  │                       │  Receive bytes     │
  │                       │───────────────────>│
  │                       │                    │
  │                       │  Parse JSON        │
  │                       │  ├─> JsonUtility   │
  │                       │  └─> Extract data  │
  │                       │                    │
  │                       │  Queue on main     │
  │                       │  thread            │
  │                       │                    │
  │                       │  Execute action    │
  │                       │                    │
```

## Scene Management

### Scene Initialization

```
SimulatorSceneManager
├─> Initialize()
│   ├─> Get PhysicsManager
│   ├─> Get/Create PythonBridge
│   └─> Create floor
├─> CreateRobot()
│   ├─> Instantiate robot
│   ├─> Add components
│   │   ├─> RobotController
│   │   ├─> LegoPiece
│   │   └─> CollisionHandler
│   └─> Configure wheels
└─> PlacePiece()
    ├─> Instantiate piece
    ├─> Add components
    └─> Track in list
```

## Data Flow Summary

```
User Python Code
      │
      ▼
Python SDK Wrapper (Mindstorms/Spike/Pybricks)
      │
      ▼
UnityBridge (Socket + JSON)
      │
      ▼
Network (TCP Socket)
      │
      ▼
Unity PythonBridge (Server)
      │
      ▼
Unity Components (Motors, Arms, Robot)
      │
      ▼
Unity Physics Engine
      │
      ▼
3D Visualization (WebGL)
```
