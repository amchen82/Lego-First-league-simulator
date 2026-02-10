"""
Pybricks Integration
Wraps Pybricks API for Unity simulator
"""

from typing import Optional, Tuple
import sys
import os

# Add parent directory to path
sys.path.append(os.path.dirname(os.path.dirname(os.path.abspath(__file__))))

from unity_bridge import UnityBridge, MessageType


class Motor:
    """Pybricks Motor wrapper for Unity simulator"""
    
    def __init__(self, port: str, bridge: UnityBridge):
        self.port = port
        self.bridge = bridge
        self.angle = 0
        
    def run(self, speed: int):
        """Run motor at constant speed"""
        # Convert speed to normalized value
        normalized_speed = speed / 1000.0  # Assuming max speed ~1000 deg/s
        self.bridge.send_motor_command(f"pybricks_motor_{self.port}", self.angle + 36000, normalized_speed)
        
    def run_time(self, speed: int, time: int):
        """Run motor for specific time in milliseconds"""
        degrees = (speed * time) / 1000.0
        self.angle += degrees
        normalized_speed = speed / 1000.0
        self.bridge.send_motor_command(f"pybricks_motor_{self.port}", self.angle, normalized_speed)
        
    def run_angle(self, speed: int, rotation_angle: int):
        """Run motor for specific angle"""
        self.angle += rotation_angle
        normalized_speed = speed / 1000.0
        self.bridge.send_motor_command(f"pybricks_motor_{self.port}", self.angle, normalized_speed)
        
    def run_target(self, speed: int, target_angle: int):
        """Run motor to target angle"""
        self.angle = target_angle
        normalized_speed = speed / 1000.0
        self.bridge.send_motor_command(f"pybricks_motor_{self.port}", self.angle, normalized_speed)
        
    def stop(self):
        """Stop motor"""
        self.bridge.send_motor_command(f"pybricks_motor_{self.port}", self.angle, 0)
        
    def reset_angle(self, angle: int = 0):
        """Reset motor angle"""
        self.angle = angle


class DriveBase:
    """Pybricks DriveBase for differential drive robots"""
    
    def __init__(self, left_motor: Motor, right_motor: Motor, wheel_diameter: float, axle_track: float, bridge: UnityBridge):
        self.left_motor = left_motor
        self.right_motor = right_motor
        self.wheel_diameter = wheel_diameter
        self.axle_track = axle_track
        self.bridge = bridge
        
    def straight(self, distance: float):
        """Drive straight for distance in mm"""
        # Calculate wheel rotations needed
        wheel_circumference = 3.14159 * self.wheel_diameter
        rotations = distance / wheel_circumference
        degrees = rotations * 360
        
        self.left_motor.run_angle(500, int(degrees))
        self.right_motor.run_angle(500, int(degrees))
        
    def turn(self, angle: float):
        """Turn in place by angle in degrees"""
        # Calculate wheel movement for turn
        arc_length = (angle / 360.0) * 3.14159 * self.axle_track
        wheel_circumference = 3.14159 * self.wheel_diameter
        rotations = arc_length / wheel_circumference
        degrees = rotations * 360
        
        self.left_motor.run_angle(500, int(degrees))
        self.right_motor.run_angle(500, int(-degrees))
        
    def drive(self, drive_speed: float, turn_rate: float):
        """Drive with speed and turn rate"""
        # Calculate left and right motor speeds
        left_speed = drive_speed + turn_rate
        right_speed = drive_speed - turn_rate
        
        self.left_motor.run(int(left_speed))
        self.right_motor.run(int(right_speed))
        
    def stop(self):
        """Stop driving"""
        self.left_motor.stop()
        self.right_motor.stop()


class UltrasonicSensor:
    """Pybricks Ultrasonic Sensor wrapper"""
    
    def __init__(self, port: str, bridge: UnityBridge):
        self.port = port
        self.bridge = bridge
        self.distance = 0
        
        bridge.register_handler(f"pybricks_ultrasonic_{port}", self._on_sensor_data)
        
    def _on_sensor_data(self, data):
        """Handle sensor data from Unity"""
        self.distance = data.get("distance", 0)
        
    def distance(self) -> int:
        """Get distance in mm"""
        return int(self.distance * 10)  # Convert cm to mm


class ColorSensor:
    """Pybricks Color Sensor wrapper"""
    
    def __init__(self, port: str, bridge: UnityBridge):
        self.port = port
        self.bridge = bridge
        self.color_value = "none"
        self.reflection = 0
        
        bridge.register_handler(f"pybricks_color_{port}", self._on_sensor_data)
        
    def _on_sensor_data(self, data):
        """Handle sensor data from Unity"""
        self.color_value = data.get("color", "none")
        self.reflection = data.get("reflection", 0)
        
    def color(self) -> str:
        """Get detected color"""
        return self.color_value
        
    def reflection(self) -> int:
        """Get reflection value (0-100)"""
        return int(self.reflection)


class EV3Brick:
    """Pybricks EV3 Brick for Unity simulator"""
    
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
        
    def motor(self, port: str) -> Motor:
        """Get motor on specified port"""
        if port not in self.motors:
            self.motors[port] = Motor(port, self.bridge)
        return self.motors[port]
        
    def ultrasonic_sensor(self, port: str) -> UltrasonicSensor:
        """Get ultrasonic sensor"""
        if port not in self.sensors:
            self.sensors[port] = UltrasonicSensor(port, self.bridge)
        return self.sensors[port]
        
    def color_sensor(self, port: str) -> ColorSensor:
        """Get color sensor"""
        if port not in self.sensors:
            self.sensors[port] = ColorSensor(port, self.bridge)
        return self.sensors[port]
        
    def create_drive_base(self, left_port: str, right_port: str, wheel_diameter: float, axle_track: float) -> DriveBase:
        """Create drive base with two motors"""
        left_motor = self.motor(left_port)
        right_motor = self.motor(right_port)
        return DriveBase(left_motor, right_motor, wheel_diameter, axle_track, self.bridge)


# Example usage
if __name__ == "__main__":
    # Create brick and connect
    brick = EV3Brick()
    
    if brick.connect():
        # Create motors
        left_motor = brick.motor("A")
        right_motor = brick.motor("B")
        
        # Create drive base
        robot = brick.create_drive_base("A", "B", wheel_diameter=56, axle_track=114)
        
        # Drive straight
        robot.straight(200)  # 200mm forward
        
        # Turn
        robot.turn(90)  # 90 degrees
        
        # Drive with speed and turn
        robot.drive(200, 50)
        
        # Get sensors
        ultrasonic = brick.ultrasonic_sensor("S1")
        color = brick.color_sensor("S2")
        
        print(f"Distance: {ultrasonic.distance()} mm")
        print(f"Color: {color.color()}")
        
        # Stop and disconnect
        robot.stop()
        brick.disconnect()
