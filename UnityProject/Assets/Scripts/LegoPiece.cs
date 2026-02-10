using UnityEngine;

namespace LegoSimulator
{
    /// <summary>
    /// Base class for all Lego pieces in the simulator
    /// Handles properties and configuration of individual pieces
    /// </summary>
    public class LegoPiece : MonoBehaviour
    {
        [Header("Piece Properties")]
        [SerializeField] private string pieceName = "Lego Piece";
        [SerializeField] private LegoType pieceType = LegoType.Block;
        [SerializeField] private Color pieceColor = Color.red;
        [SerializeField] private Vector3 dimensions = new Vector3(1, 1, 1);
        [SerializeField] private float mass = 1.0f;
        
        [Header("Physics Properties")]
        [SerializeField] private float friction = 0.6f;
        [SerializeField] private float bounciness = 0.3f;
        
        private Rigidbody rb;
        private Renderer pieceRenderer;
        private CollisionHandler collisionHandler;
        
        public enum LegoType
        {
            Block,
            Wheel,
            Motor,
            Sensor,
            Arm,
            Gear,
            Axle,
            Connector
        }
        
        private void Awake()
        {
            InitializeComponents();
        }
        
        private void Start()
        {
            ApplyProperties();
        }
        
        /// <summary>
        /// Initialize required components
        /// </summary>
        private void InitializeComponents()
        {
            rb = GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = gameObject.AddComponent<Rigidbody>();
            }
            
            pieceRenderer = GetComponent<Renderer>();
            if (pieceRenderer == null)
            {
                pieceRenderer = gameObject.AddComponent<MeshRenderer>();
            }
            
            collisionHandler = GetComponent<CollisionHandler>();
            if (collisionHandler == null)
            {
                collisionHandler = gameObject.AddComponent<CollisionHandler>();
            }
            
            // Ensure collider exists
            if (GetComponent<Collider>() == null)
            {
                gameObject.AddComponent<BoxCollider>();
            }
        }
        
        /// <summary>
        /// Apply piece properties to components
        /// </summary>
        private void ApplyProperties()
        {
            // Set mass
            if (rb != null)
            {
                rb.mass = mass;
            }
            
            // Set color
            if (pieceRenderer != null && pieceRenderer.material != null)
            {
                pieceRenderer.material.color = pieceColor;
            }
            
            // Apply physics material
            Collider collider = GetComponent<Collider>();
            if (collider != null)
            {
                PhysicMaterial material = PhysicsManager.Instance.CreatePhysicsMaterial(friction, bounciness);
                collider.material = material;
            }
            
            // Set dimensions
            transform.localScale = dimensions;
        }
        
        /// <summary>
        /// Configure piece properties
        /// </summary>
        public void ConfigurePiece(string name, LegoType type, Color color, Vector3 dims, float pieceMass)
        {
            pieceName = name;
            pieceType = type;
            pieceColor = color;
            dimensions = dims;
            mass = pieceMass;
            ApplyProperties();
        }
        
        /// <summary>
        /// Set physics properties
        /// </summary>
        public void SetPhysicsProperties(float newFriction, float newBounciness)
        {
            friction = newFriction;
            bounciness = newBounciness;
            
            Collider collider = GetComponent<Collider>();
            if (collider != null)
            {
                PhysicMaterial material = PhysicsManager.Instance.CreatePhysicsMaterial(friction, bounciness);
                collider.material = material;
            }
        }
        
        /// <summary>
        /// Get piece type
        /// </summary>
        public LegoType GetPieceType()
        {
            return pieceType;
        }
        
        /// <summary>
        /// Get piece name
        /// </summary>
        public string GetPieceName()
        {
            return pieceName;
        }
        
        /// <summary>
        /// Set piece color
        /// </summary>
        public void SetColor(Color color)
        {
            pieceColor = color;
            if (pieceRenderer != null && pieceRenderer.material != null)
            {
                pieceRenderer.material.color = pieceColor;
            }
        }
        
        /// <summary>
        /// Get current mass
        /// </summary>
        public float GetMass()
        {
            return mass;
        }
        
        /// <summary>
        /// Set mass
        /// </summary>
        public void SetMass(float newMass)
        {
            mass = newMass;
            if (rb != null)
            {
                rb.mass = mass;
            }
        }
    }
}
