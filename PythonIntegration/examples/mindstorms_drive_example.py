"""
Example: Simple robot driving with Mindstorms
Demonstrates basic forward movement and turning
"""

import time
import sys
import os

# Add parent directory to path
sys.path.append(os.path.dirname(os.path.dirname(os.path.abspath(__file__))))

from mindstorms.mindstorms_integration import MindstormsHub


def main():
    """Main program"""
    print("Starting Mindstorms robot driving example...")
    
    # Create and connect to hub
    hub = MindstormsHub()
    
    if not hub.connect():
        print("Failed to connect to simulator")
        return
    
    print("Connected to simulator!")
    
    try:
        # Get motors
        left_motor = hub.get_motor("left_motor")
        right_motor = hub.get_motor("right_motor")
        
        print("Driving forward...")
        # Drive forward 2 rotations
        left_motor.run_for_rotations(2, speed=50)
        right_motor.run_for_rotations(2, speed=50)
        time.sleep(3)
        
        print("Turning right...")
        # Turn right
        left_motor.run_for_rotations(1, speed=50)
        right_motor.run_for_rotations(-1, speed=50)
        time.sleep(2)
        
        print("Driving forward again...")
        # Drive forward again
        left_motor.run_for_rotations(2, speed=50)
        right_motor.run_for_rotations(2, speed=50)
        time.sleep(3)
        
        print("Stopping...")
        left_motor.stop()
        right_motor.stop()
        
        print("Example complete!")
        
    except Exception as e:
        print(f"Error during execution: {e}")
    
    finally:
        # Always disconnect
        hub.disconnect()
        print("Disconnected from simulator")


if __name__ == "__main__":
    main()
