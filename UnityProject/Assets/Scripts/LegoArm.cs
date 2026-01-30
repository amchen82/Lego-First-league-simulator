using UnityEngine;

namespace LegoSimulator
{
    /// <summary>
    /// Arm component for Lego robots
    /// Supports lifting up/down, left/right, and circular movements
    /// </summary>
    public class LegoArm : MonoBehaviour
    {
        [Header("Arm Settings")]
        [SerializeField] private string armName = "Arm";
        [SerializeField] private ArmType armType = ArmType.Simple;
        [SerializeField] private float length = 5f;
        [SerializeField] private float maxAngle = 90f;
        [SerializeField] private float minAngle = -90f;
        
        [Header("Movement Settings")]
        [SerializeField] private float liftSpeed = 30f; // degrees per second
        [SerializeField] private float rotationSpeed = 45f; // degrees per second
        
        [Header("Current State")]
        [SerializeField] private float currentLiftAngle = 0f;
        [SerializeField] private float currentRotationAngle = 0f;
        
        private Transform armBase;
        private Transform armSegment;
        private LegoMotor attachedMotor;
        
        public enum ArmType
        {
            Simple,
            Claw,
            Gripper,
            Lifter
        }
        
        public enum ArmMovement
        {
            LiftUp,
            LiftDown,
            RotateLeft,
            RotateRight,
            RotateCircular
        }
        
        private void Awake()
        {
            InitializeArm();
        }
        
        /// <summary>
        /// Initialize arm components
        /// </summary>
        private void InitializeArm()
        {
            // Create arm base
            Transform baseTransform = transform.Find("ArmBase");
            if (baseTransform == null)
            {
                GameObject baseObj = new GameObject("ArmBase");
                baseObj.transform.SetParent(transform);
                baseObj.transform.localPosition = Vector3.zero;
                armBase = baseObj.transform;
            }
            else
            {
                armBase = baseTransform;
            }
            
            // Create arm segment
            Transform segmentTransform = armBase.Find("ArmSegment");
            if (segmentTransform == null)
            {
                GameObject segmentObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                segmentObj.name = "ArmSegment";
                segmentObj.transform.SetParent(armBase);
                segmentObj.transform.localPosition = new Vector3(0, length / 2, 0);
                segmentObj.transform.localScale = new Vector3(0.5f, length, 0.5f);
                armSegment = segmentObj.transform;
                
                // Add visual appearance
                Renderer renderer = segmentObj.GetComponent<Renderer>();
                if (renderer != null && renderer.material != null)
                {
                    renderer.material.color = Color.gray;
                }
            }
            else
            {
                armSegment = segmentTransform;
            }
            
            // Check for attached motor
            attachedMotor = GetComponent<LegoMotor>();
        }
        
        /// <summary>
        /// Move arm in specified direction
        /// </summary>
        public void Move(ArmMovement movement, float amount = 1f)
        {
            switch (movement)
            {
                case ArmMovement.LiftUp:
                    LiftUp(amount);
                    break;
                case ArmMovement.LiftDown:
                    LiftDown(amount);
                    break;
                case ArmMovement.RotateLeft:
                    RotateLeft(amount);
                    break;
                case ArmMovement.RotateRight:
                    RotateRight(amount);
                    break;
                case ArmMovement.RotateCircular:
                    RotateCircular(amount);
                    break;
            }
        }
        
        /// <summary>
        /// Lift arm up
        /// </summary>
        private void LiftUp(float amount)
        {
            float targetAngle = Mathf.Min(currentLiftAngle + (liftSpeed * amount * Time.deltaTime), maxAngle);
            float deltaAngle = targetAngle - currentLiftAngle;
            
            if (deltaAngle > 0)
            {
                armBase.Rotate(Vector3.right, deltaAngle, Space.Self);
                currentLiftAngle = targetAngle;
                Debug.Log($"{armName} lifting up to {currentLiftAngle} degrees");
            }
        }
        
        /// <summary>
        /// Lift arm down
        /// </summary>
        private void LiftDown(float amount)
        {
            float targetAngle = Mathf.Max(currentLiftAngle - (liftSpeed * amount * Time.deltaTime), minAngle);
            float deltaAngle = targetAngle - currentLiftAngle;
            
            if (deltaAngle < 0)
            {
                armBase.Rotate(Vector3.right, deltaAngle, Space.Self);
                currentLiftAngle = targetAngle;
                Debug.Log($"{armName} lifting down to {currentLiftAngle} degrees");
            }
        }
        
        /// <summary>
        /// Rotate arm left
        /// </summary>
        private void RotateLeft(float amount)
        {
            float deltaAngle = rotationSpeed * amount * Time.deltaTime;
            armBase.Rotate(Vector3.up, -deltaAngle, Space.Self);
            currentRotationAngle -= deltaAngle;
            Debug.Log($"{armName} rotating left to {currentRotationAngle} degrees");
        }
        
        /// <summary>
        /// Rotate arm right
        /// </summary>
        private void RotateRight(float amount)
        {
            float deltaAngle = rotationSpeed * amount * Time.deltaTime;
            armBase.Rotate(Vector3.up, deltaAngle, Space.Self);
            currentRotationAngle += deltaAngle;
            Debug.Log($"{armName} rotating right to {currentRotationAngle} degrees");
        }
        
        /// <summary>
        /// Continuous circular rotation
        /// </summary>
        private void RotateCircular(float amount)
        {
            float deltaAngle = rotationSpeed * amount * Time.deltaTime;
            armBase.Rotate(Vector3.up, deltaAngle, Space.Self);
            currentRotationAngle += deltaAngle;
            currentRotationAngle = currentRotationAngle % 360f;
        }
        
        /// <summary>
        /// Set arm to specific angles
        /// </summary>
        public void SetAngles(float liftAngle, float rotationAngle)
        {
            currentLiftAngle = Mathf.Clamp(liftAngle, minAngle, maxAngle);
            currentRotationAngle = rotationAngle;
            
            armBase.localRotation = Quaternion.Euler(currentLiftAngle, currentRotationAngle, 0);
        }
        
        /// <summary>
        /// Reset arm to default position
        /// </summary>
        public void Reset()
        {
            currentLiftAngle = 0f;
            currentRotationAngle = 0f;
            armBase.localRotation = Quaternion.identity;
        }
        
        /// <summary>
        /// Get current lift angle
        /// </summary>
        public float GetLiftAngle()
        {
            return currentLiftAngle;
        }
        
        /// <summary>
        /// Get current rotation angle
        /// </summary>
        public float GetRotationAngle()
        {
            return currentRotationAngle;
        }
        
        /// <summary>
        /// Configure arm properties
        /// </summary>
        public void ConfigureArm(string name, ArmType type, float armLength, float maxLiftAngle, float minLiftAngle)
        {
            armName = name;
            armType = type;
            length = armLength;
            maxAngle = maxLiftAngle;
            minAngle = minLiftAngle;
            
            // Update arm segment length
            if (armSegment != null)
            {
                armSegment.localPosition = new Vector3(0, length / 2, 0);
                armSegment.localScale = new Vector3(0.5f, length, 0.5f);
            }
        }
    }
}
