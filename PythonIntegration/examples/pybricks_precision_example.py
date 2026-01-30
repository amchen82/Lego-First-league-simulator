"""
Example: Precise movement with Pybricks
Demonstrates DriveBase and precise positioning
"""

import time
import sys
import os

# Add parent directory to path
sys.path.append(os.path.dirname(os.path.dirname(os.path.abspath(__file__))))

from pybricks.pybricks_integration import EV3Brick


def main():
    """Main program"""
    print("Starting Pybricks precision movement example...")
    
    # Create and connect to brick
    brick = EV3Brick()
    
    if not brick.connect():
        print("Failed to connect to simulator")
        return
    
    print("Connected to simulator!")
    
    try:
        # Create drive base with wheel diameter and axle track
        robot = brick.create_drive_base(
            left_port="A",
            right_port="B",
            wheel_diameter=56,  # mm
            axle_track=114      # mm
        )
        
        print("Driving straight 300mm...")
        robot.straight(300)
        time.sleep(3)
        
        print("Turning 90 degrees right...")
        robot.turn(90)
        time.sleep(2)
        
        print("Driving straight 200mm...")
        robot.straight(200)
        time.sleep(2)
        
        print("Turning 90 degrees left...")
        robot.turn(-90)
        time.sleep(2)
        
        print("Driving back to start...")
        robot.straight(-300)
        time.sleep(3)
        
        print("Stopping...")
        robot.stop()
        
        print("Example complete!")
        
    except Exception as e:
        print(f"Error during execution: {e}")
    
    finally:
        # Always disconnect
        brick.disconnect()
        print("Disconnected from simulator")


if __name__ == "__main__":
    main()
