using UnityEngine;
using System;

namespace LegoSimulator
{
    /// <summary>
    /// Motor component for Lego robots
    /// Handles motor rotations for gear-based movements (arms lifting, rotating)
    /// </summary>
    public class LegoMotor : MonoBehaviour
    {
        [Header("Motor Settings")]
        [SerializeField] private string motorName = "Motor";
        [SerializeField] private MotorType motorType = MotorType.Standard;
        [SerializeField] private float maxSpeed = 360f; // degrees per second
        [SerializeField] private float torque = 10f;
        [SerializeField] private float gearRatio = 1.0f;
        
        [Header("Current State")]
        [SerializeField] private float currentRotation = 0f;
        [SerializeField] private float targetRotation = 0f;
        [SerializeField] private bool isRunning = false;
        
        public event Action<float> OnRotationChanged;
        
        private Transform motorAxis;
        private HingeJoint joint;
        
        public enum MotorType
        {
            Standard,
            Large,
            Medium,
            Servo
        }
        
        public enum RotationDirection
        {
            Clockwise,
            CounterClockwise
        }
        
        private void Awake()
        {
            InitializeMotor();
        }
        
        private void Update()
        {
            if (isRunning)
            {
                UpdateRotation();
            }
        }
        
        /// <summary>
        /// Initialize motor components
        /// </summary>
        private void InitializeMotor()
        {
            // Create motor axis if it doesn't exist
            Transform axisTransform = transform.Find("MotorAxis");
            if (axisTransform == null)
            {
                GameObject axisObj = new GameObject("MotorAxis");
                axisObj.transform.SetParent(transform);
                axisObj.transform.localPosition = Vector3.zero;
                motorAxis = axisObj.transform;
            }
            else
            {
                motorAxis = axisTransform;
            }
        }
        
        /// <summary>
        /// Update motor rotation
        /// </summary>
        private void UpdateRotation()
        {
            float rotationDelta = maxSpeed * Time.deltaTime;
            
            if (Mathf.Abs(targetRotation - currentRotation) > 0.1f)
            {
                float direction = Mathf.Sign(targetRotation - currentRotation);
                float actualRotation = Mathf.Min(rotationDelta, Mathf.Abs(targetRotation - currentRotation)) * direction;
                
                currentRotation += actualRotation;
                motorAxis.Rotate(Vector3.up, actualRotation * gearRatio, Space.Self);
                
                OnRotationChanged?.Invoke(currentRotation);
            }
            else
            {
                isRunning = false;
            }
        }
        
        /// <summary>
        /// Set motor rotation input (in degrees)
        /// </summary>
        public void SetRotation(float degrees)
        {
            targetRotation = degrees;
            isRunning = true;
            Debug.Log($"{motorName} rotating to {degrees} degrees");
        }
        
        /// <summary>
        /// Rotate motor by relative amount
        /// </summary>
        public void RotateBy(float degrees)
        {
            targetRotation = currentRotation + degrees;
            isRunning = true;
            Debug.Log($"{motorName} rotating by {degrees} degrees");
        }
        
        /// <summary>
        /// Run motor continuously in a direction
        /// </summary>
        public void Run(RotationDirection direction, float speed = -1f)
        {
            if (speed < 0)
            {
                speed = maxSpeed;
            }
            
            float targetSpeed = speed * (direction == RotationDirection.Clockwise ? 1f : -1f);
            targetRotation = currentRotation + targetSpeed * Time.deltaTime * 100f;
            isRunning = true;
        }
        
        /// <summary>
        /// Stop motor
        /// </summary>
        public void Stop()
        {
            isRunning = false;
            targetRotation = currentRotation;
            Debug.Log($"{motorName} stopped at {currentRotation} degrees");
        }
        
        /// <summary>
        /// Reset motor to zero position
        /// </summary>
        public void Reset()
        {
            currentRotation = 0f;
            targetRotation = 0f;
            isRunning = false;
            motorAxis.localRotation = Quaternion.identity;
            OnRotationChanged?.Invoke(0f);
        }
        
        /// <summary>
        /// Get current rotation
        /// </summary>
        public float GetRotation()
        {
            return currentRotation;
        }
        
        /// <summary>
        /// Set motor properties
        /// </summary>
        public void ConfigureMotor(string name, MotorType type, float speed, float motorTorque, float ratio)
        {
            motorName = name;
            motorType = type;
            maxSpeed = speed;
            torque = motorTorque;
            gearRatio = ratio;
        }
        
        /// <summary>
        /// Get motor axis transform for attaching gears/arms
        /// </summary>
        public Transform GetMotorAxis()
        {
            return motorAxis;
        }
        
        /// <summary>
        /// Set gear ratio
        /// </summary>
        public void SetGearRatio(float ratio)
        {
            gearRatio = ratio;
        }
    }
}
