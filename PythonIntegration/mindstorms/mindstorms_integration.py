"""
Mindstorms SDK Integration
Wraps Lego Mindstorms EV3 SDK for Unity simulator
"""

from typing import Optional
from unity_bridge import UnityBridge, MessageType


class MindstormsMotor:
    """Motor wrapper for Mindstorms in Unity simulator"""
    
    def __init__(self, name: str, bridge: UnityBridge):
        self.name = name
        self.bridge = bridge
        self.position = 0
        
    def run_to_position(self, position: int, speed: int = 100):
        """Run motor to absolute position"""
        self.position = position
        self.bridge.send_motor_command(self.name, position, speed / 100.0)
        
    def run_for_degrees(self, degrees: int, speed: int = 100):
        """Run motor for relative degrees"""
        self.position += degrees
        self.bridge.send_motor_command(self.name, self.position, speed / 100.0)
        
    def run_for_rotations(self, rotations: float, speed: int = 100):
        """Run motor for rotations"""
        degrees = rotations * 360
        self.run_for_degrees(int(degrees), speed)
        
    def start(self, speed: int = 100):
        """Start motor at speed"""
        # For continuous running, send large rotation value
        self.bridge.send_motor_command(self.name, 360 * 100, speed / 100.0)
        
    def stop(self):
        """Stop motor"""
        self.bridge.send_motor_command(self.name, self.position, 0)
        
    def reset(self):
        """Reset motor position"""
        self.position = 0


class MindstormsSensor:
    """Sensor wrapper for Mindstorms in Unity simulator"""
    
    def __init__(self, name: str, sensor_type: str, bridge: UnityBridge):
        self.name = name
        self.sensor_type = sensor_type
        self.bridge = bridge
        self.value = 0
        
        # Register handler for sensor data
        bridge.register_handler(f"sensor_{name}", self._on_sensor_data)
        
    def _on_sensor_data(self, data):
        """Handle sensor data from Unity"""
        self.value = data.get("value", 0)
        
    def read(self) -> float:
        """Read sensor value"""
        return self.value


class MindstormsHub:
    """Main hub for Mindstorms integration"""
    
    def __init__(self, host: str = "localhost", port: int = 5555):
        self.bridge = UnityBridge(host, port)
        self.motors = {}
        self.sensors = {}
        
    def connect(self) -> bool:
        """Connect to Unity simulator"""
        return self.bridge.connect()
        
    def disconnect(self):
        """Disconnect from Unity simulator"""
        self.bridge.disconnect()
        
    def get_motor(self, name: str) -> MindstormsMotor:
        """Get or create motor by name"""
        if name not in self.motors:
            self.motors[name] = MindstormsMotor(name, self.bridge)
        return self.motors[name]
        
    def get_sensor(self, name: str, sensor_type: str = "generic") -> MindstormsSensor:
        """Get or create sensor by name"""
        if name not in self.sensors:
            self.sensors[name] = MindstormsSensor(name, sensor_type, self.bridge)
        return self.sensors[name]


# Example usage
if __name__ == "__main__":
    # Create hub and connect
    hub = MindstormsHub()
    
    if hub.connect():
        # Get motors
        left_motor = hub.get_motor("left_motor")
        right_motor = hub.get_motor("right_motor")
        arm_motor = hub.get_motor("arm_motor")
        
        # Run motors
        left_motor.run_for_rotations(2, speed=50)
        right_motor.run_for_rotations(2, speed=50)
        
        # Move arm
        arm_motor.run_to_position(90, speed=30)
        
        # Get sensor
        distance_sensor = hub.get_sensor("distance", "ultrasonic")
        print(f"Distance: {distance_sensor.read()}")
        
        # Disconnect
        hub.disconnect()
