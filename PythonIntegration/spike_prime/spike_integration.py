"""
Spike Prime SDK Integration
Wraps Lego Spike Prime SDK for Unity simulator
"""

from typing import Optional, Tuple
import sys
import os

# Add parent directory to path
sys.path.append(os.path.dirname(os.path.dirname(os.path.abspath(__file__))))

from unity_bridge import UnityBridge, MessageType


class SpikePrimeMotor:
    """Motor wrapper for Spike Prime in Unity simulator"""
    
    def __init__(self, port: str, bridge: UnityBridge):
        self.port = port
        self.bridge = bridge
        self.position = 0
        
    def run_to_position(self, degrees: int, speed: int = 100):
        """Run motor to absolute position"""
        self.position = degrees
        self.bridge.send_motor_command(f"spike_motor_{self.port}", degrees, speed / 100.0)
        
    def run_for_degrees(self, degrees: int, speed: int = 100):
        """Run motor for relative degrees"""
        self.position += degrees
        self.bridge.send_motor_command(f"spike_motor_{self.port}", self.position, speed / 100.0)
        
    def run_for_rotations(self, rotations: float, speed: int = 100):
        """Run motor for rotations"""
        degrees = rotations * 360
        self.run_for_degrees(int(degrees), speed)
        
    def start(self, speed: int = 100):
        """Start motor at speed continuously"""
        self.bridge.send_motor_command(f"spike_motor_{self.port}", self.position + 36000, speed / 100.0)
        
    def stop(self):
        """Stop motor"""
        self.bridge.send_motor_command(f"spike_motor_{self.port}", self.position, 0)
        
    def set_degrees_counted(self, degrees: int):
        """Reset motor encoder to specific value"""
        self.position = degrees
        
    def get_position(self) -> int:
        """Get current motor position"""
        return self.position


class SpikePrimeSensor:
    """Sensor wrapper for Spike Prime in Unity simulator"""
    
    def __init__(self, port: str, sensor_type: str, bridge: UnityBridge):
        self.port = port
        self.sensor_type = sensor_type
        self.bridge = bridge
        self.value = 0
        self.color = "none"
        
        # Register handler
        bridge.register_handler(f"spike_sensor_{port}", self._on_sensor_data)
        
    def _on_sensor_data(self, data):
        """Handle sensor data from Unity"""
        self.value = data.get("value", 0)
        self.color = data.get("color", "none")
        
    def get_distance_cm(self) -> Optional[int]:
        """Get distance in centimeters (for distance sensor)"""
        if self.sensor_type == "distance":
            return int(self.value)
        return None
        
    def get_color(self) -> str:
        """Get detected color (for color sensor)"""
        if self.sensor_type == "color":
            return self.color
        return "none"
        
    def get_ambient_light(self) -> int:
        """Get ambient light level"""
        return int(self.value)


class SpikePrimeHub:
    """Main hub for Spike Prime integration"""
    
    def __init__(self, host: str = "localhost", port: int = 5555):
        self.bridge = UnityBridge(host, port)
        self.motors = {}
        self.sensors = {}
        self.connected = False
        
    def connect(self) -> bool:
        """Connect to Unity simulator"""
        self.connected = self.bridge.connect()
        return self.connected
        
    def disconnect(self):
        """Disconnect from Unity simulator"""
        self.bridge.disconnect()
        self.connected = False
        
    def motor(self, port: str) -> SpikePrimeMotor:
        """Get motor on specified port (A-F)"""
        if port not in self.motors:
            self.motors[port] = SpikePrimeMotor(port, self.bridge)
        return self.motors[port]
        
    def distance_sensor(self, port: str) -> SpikePrimeSensor:
        """Get distance sensor on specified port"""
        if port not in self.sensors:
            self.sensors[port] = SpikePrimeSensor(port, "distance", self.bridge)
        return self.sensors[port]
        
    def color_sensor(self, port: str) -> SpikePrimeSensor:
        """Get color sensor on specified port"""
        if port not in self.sensors:
            self.sensors[port] = SpikePrimeSensor(port, "color", self.bridge)
        return self.sensors[port]
        
    def motor_pair(self, port1: str, port2: str) -> Tuple[SpikePrimeMotor, SpikePrimeMotor]:
        """Get pair of motors for tank drive"""
        return (self.motor(port1), self.motor(port2))


# Example usage
if __name__ == "__main__":
    # Create hub and connect
    hub = SpikePrimeHub()
    
    if hub.connect():
        # Get motors
        motor_a = hub.motor("A")
        motor_b = hub.motor("B")
        
        # Run motors
        motor_a.run_for_rotations(2, speed=50)
        motor_b.run_for_rotations(2, speed=50)
        
        # Get motor pair for tank drive
        left, right = hub.motor_pair("A", "B")
        left.start(50)
        right.start(50)
        
        # Get sensors
        distance = hub.distance_sensor("C")
        color = hub.color_sensor("D")
        
        print(f"Distance: {distance.get_distance_cm()} cm")
        print(f"Color: {color.get_color()}")
        
        # Disconnect
        hub.disconnect()
