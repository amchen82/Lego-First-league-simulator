using UnityEngine;

namespace LegoSimulator
{
    /// <summary>
    /// Robot controller for driving simulation
    /// Handles acceleration, steering, and physics-based movement
    /// </summary>
    public class RobotController : MonoBehaviour
    {
        [Header("Drive Settings")]
        [SerializeField] private float maxSpeed = 10f;
        [SerializeField] private float acceleration = 5f;
        [SerializeField] private float deceleration = 8f;
        [SerializeField] private float turnSpeed = 90f;
        
        [Header("Wheel Configuration")]
        [SerializeField] private Transform leftWheel;
        [SerializeField] private Transform rightWheel;
        [SerializeField] private float wheelRadius = 0.5f;
        
        [Header("Current State")]
        [SerializeField] private float currentSpeed = 0f;
        [SerializeField] private float currentTurnRate = 0f;
        
        private Rigidbody rb;
        private bool isDriving = false;
        
        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = gameObject.AddComponent<Rigidbody>();
            }
            
            // Configure rigidbody for driving
            rb.mass = 2f;
            rb.drag = 0.5f;
            rb.angularDrag = 0.5f;
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        }
        
        private void FixedUpdate()
        {
            if (isDriving)
            {
                ApplyMovement();
                UpdateWheelVisuals();
            }
        }
        
        /// <summary>
        /// Apply movement based on current speed and turn rate
        /// </summary>
        private void ApplyMovement()
        {
            // Apply forward/backward movement
            Vector3 movement = transform.forward * currentSpeed * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + movement);
            
            // Apply turning
            float turn = currentTurnRate * Time.fixedDeltaTime;
            Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);
            rb.MoveRotation(rb.rotation * turnRotation);
        }
        
        /// <summary>
        /// Update wheel visual rotation
        /// </summary>
        private void UpdateWheelVisuals()
        {
            if (leftWheel != null && rightWheel != null)
            {
                float wheelRotation = (currentSpeed / wheelRadius) * Time.fixedDeltaTime * Mathf.Rad2Deg;
                
                leftWheel.Rotate(Vector3.right, wheelRotation, Space.Self);
                rightWheel.Rotate(Vector3.right, wheelRotation, Space.Self);
            }
        }
        
        /// <summary>
        /// Drive forward with specified power (-1 to 1)
        /// </summary>
        public void Drive(float power)
        {
            float targetSpeed = Mathf.Clamp(power, -1f, 1f) * maxSpeed;
            
            if (Mathf.Abs(targetSpeed) > Mathf.Abs(currentSpeed))
            {
                // Accelerating
                currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.deltaTime);
            }
            else
            {
                // Decelerating
                currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, deceleration * Time.deltaTime);
            }
            
            isDriving = true;
            Debug.Log($"Robot driving at speed: {currentSpeed}");
        }
        
        /// <summary>
        /// Steer with specified amount (-1 left, 1 right)
        /// </summary>
        public void Steer(float amount)
        {
            currentTurnRate = Mathf.Clamp(amount, -1f, 1f) * turnSpeed;
            Debug.Log($"Robot steering at rate: {currentTurnRate}");
        }
        
        /// <summary>
        /// Stop the robot
        /// </summary>
        public void Stop()
        {
            currentSpeed = 0f;
            currentTurnRate = 0f;
            isDriving = false;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            Debug.Log("Robot stopped");
        }
        
        /// <summary>
        /// Set drive and steer values simultaneously
        /// </summary>
        public void DriveAndSteer(float drivePower, float steerAmount)
        {
            Drive(drivePower);
            Steer(steerAmount);
        }
        
        /// <summary>
        /// Tank drive using left and right motor powers
        /// </summary>
        public void TankDrive(float leftPower, float rightPower)
        {
            leftPower = Mathf.Clamp(leftPower, -1f, 1f);
            rightPower = Mathf.Clamp(rightPower, -1f, 1f);
            
            // Calculate forward speed and turning from differential
            float drivePower = (leftPower + rightPower) / 2f;
            float steerAmount = (rightPower - leftPower) / 2f;
            
            Drive(drivePower);
            Steer(steerAmount);
            
            Debug.Log($"Tank drive - Left: {leftPower}, Right: {rightPower}");
        }
        
        /// <summary>
        /// Configure drive settings
        /// </summary>
        public void ConfigureDrive(float speed, float accel, float decel, float turn)
        {
            maxSpeed = speed;
            acceleration = accel;
            deceleration = decel;
            turnSpeed = turn;
        }
        
        /// <summary>
        /// Set wheel references
        /// </summary>
        public void SetWheels(Transform left, Transform right, float radius)
        {
            leftWheel = left;
            rightWheel = right;
            wheelRadius = radius;
        }
        
        /// <summary>
        /// Get current speed
        /// </summary>
        public float GetSpeed()
        {
            return currentSpeed;
        }
        
        /// <summary>
        /// Get current turn rate
        /// </summary>
        public float GetTurnRate()
        {
            return currentTurnRate;
        }
    }
}
