"""
Python integration bridge for Unity Lego Simulator
Provides communication layer between Python SDKs and Unity
"""

import json
import socket
import threading
from typing import Dict, Any, Callable
from enum import Enum


class MessageType(Enum):
    """Message types for Unity communication"""
    MOTOR_COMMAND = "motor_command"
    SENSOR_DATA = "sensor_data"
    ROBOT_STATE = "robot_state"
    CONFIGURATION = "configuration"


class UnityBridge:
    """
    Bridge for communicating with Unity from Python
    Handles socket communication and message serialization
    """
    
    def __init__(self, host: str = "localhost", port: int = 5555):
        self.host = host
        self.port = port
        self.socket = None
        self.connected = False
        self.message_handlers: Dict[str, Callable] = {}
        self.receive_thread = None
        
    def connect(self) -> bool:
        """Establish connection to Unity"""
        try:
            self.socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
            self.socket.connect((self.host, self.port))
            self.connected = True
            
            # Start receive thread
            self.receive_thread = threading.Thread(target=self._receive_loop, daemon=True)
            self.receive_thread.start()
            
            print(f"Connected to Unity at {self.host}:{self.port}")
            return True
        except Exception as e:
            print(f"Failed to connect to Unity: {e}")
            return False
    
    def disconnect(self):
        """Close connection to Unity"""
        self.connected = False
        if self.socket:
            self.socket.close()
        print("Disconnected from Unity")
    
    def send_message(self, message_type: MessageType, data: Dict[str, Any]) -> bool:
        """Send message to Unity"""
        if not self.connected:
            print("Not connected to Unity")
            return False
        
        try:
            message = {
                "type": message_type.value,
                "data": data
            }
            
            json_data = json.dumps(message)
            self.socket.sendall(json_data.encode('utf-8') + b'\n')
            return True
        except Exception as e:
            print(f"Failed to send message: {e}")
            return False
    
    def register_handler(self, message_type: str, handler: Callable):
        """Register handler for specific message type"""
        self.message_handlers[message_type] = handler
    
    def _receive_loop(self):
        """Background thread for receiving messages from Unity"""
        buffer = ""
        
        while self.connected:
            try:
                data = self.socket.recv(4096).decode('utf-8')
                if not data:
                    break
                
                buffer += data
                
                # Process complete messages (newline-delimited)
                while '\n' in buffer:
                    line, buffer = buffer.split('\n', 1)
                    self._handle_message(line)
                    
            except Exception as e:
                if self.connected:
                    print(f"Receive error: {e}")
                break
    
    def _handle_message(self, message_str: str):
        """Handle received message from Unity"""
        try:
            message = json.loads(message_str)
            message_type = message.get("type")
            data = message.get("data", {})
            
            # Call registered handler if exists
            if message_type in self.message_handlers:
                self.message_handlers[message_type](data)
            else:
                print(f"No handler for message type: {message_type}")
                
        except Exception as e:
            print(f"Error handling message: {e}")
    
    def send_motor_command(self, motor_name: str, rotation: float, speed: float = 1.0):
        """Send motor command to Unity"""
        data = {
            "motor_name": motor_name,
            "rotation": rotation,
            "speed": speed
        }
        return self.send_message(MessageType.MOTOR_COMMAND, data)
    
    def send_robot_state(self, position: tuple, rotation: tuple, velocity: tuple):
        """Send robot state update to Unity"""
        data = {
            "position": {"x": position[0], "y": position[1], "z": position[2]},
            "rotation": {"x": rotation[0], "y": rotation[1], "z": rotation[2]},
            "velocity": {"x": velocity[0], "y": velocity[1], "z": velocity[2]}
        }
        return self.send_message(MessageType.ROBOT_STATE, data)
