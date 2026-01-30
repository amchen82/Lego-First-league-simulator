"""
Example: Robot with arm using Spike Prime
Demonstrates motor control and arm movements
"""

import time
import sys
import os

# Add parent directory to path
sys.path.append(os.path.dirname(os.path.dirname(os.path.abspath(__file__))))

from spike_prime.spike_integration import SpikePrimeHub


def main():
    """Main program"""
    print("Starting Spike Prime robot with arm example...")
    
    # Create and connect to hub
    hub = SpikePrimeHub()
    
    if not hub.connect():
        print("Failed to connect to simulator")
        return
    
    print("Connected to simulator!")
    
    try:
        # Get drive motors
        motor_a = hub.motor("A")  # Left wheel
        motor_b = hub.motor("B")  # Right wheel
        motor_c = hub.motor("C")  # Arm motor
        
        print("Driving forward...")
        motor_a.run_for_rotations(2, speed=50)
        motor_b.run_for_rotations(2, speed=50)
        time.sleep(3)
        
        print("Lifting arm...")
        motor_c.run_to_position(90, speed=40)
        time.sleep(2)
        
        print("Driving with arm raised...")
        motor_a.run_for_rotations(1, speed=50)
        motor_b.run_for_rotations(1, speed=50)
        time.sleep(2)
        
        print("Lowering arm...")
        motor_c.run_to_position(0, speed=40)
        time.sleep(2)
        
        print("Stopping...")
        motor_a.stop()
        motor_b.stop()
        motor_c.stop()
        
        print("Example complete!")
        
    except Exception as e:
        print(f"Error during execution: {e}")
    
    finally:
        # Always disconnect
        hub.disconnect()
        print("Disconnected from simulator")


if __name__ == "__main__":
    main()
