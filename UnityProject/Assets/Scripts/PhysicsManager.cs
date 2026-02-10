using UnityEngine;

namespace LegoSimulator
{
    /// <summary>
    /// Manages physics settings for the Lego simulator including gravity and friction
    /// </summary>
    public class PhysicsManager : MonoBehaviour
    {
        [Header("Physics Settings")]
        [SerializeField] private Vector3 gravity = new Vector3(0, -9.81f, 0);
        [SerializeField] private float defaultFriction = 0.6f;
        [SerializeField] private float defaultBounciness = 0.3f;
        
        private static PhysicsManager instance;
        
        public static PhysicsManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<PhysicsManager>();
                    if (instance == null)
                    {
                        GameObject go = new GameObject("PhysicsManager");
                        instance = go.AddComponent<PhysicsManager>();
                    }
                }
                return instance;
            }
        }
        
        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
            DontDestroyOnLoad(gameObject);
            InitializePhysics();
        }
        
        private void InitializePhysics()
        {
            Physics.gravity = gravity;
            Debug.Log($"Physics initialized with gravity: {gravity}");
        }
        
        /// <summary>
        /// Update gravity settings
        /// </summary>
        public void SetGravity(Vector3 newGravity)
        {
            gravity = newGravity;
            Physics.gravity = gravity;
            Debug.Log($"Gravity updated to: {gravity}");
        }
        
        /// <summary>
        /// Get current gravity
        /// </summary>
        public Vector3 GetGravity()
        {
            return gravity;
        }
        
        /// <summary>
        /// Create a physics material with specified friction and bounciness
        /// </summary>
        public PhysicMaterial CreatePhysicsMaterial(float friction, float bounciness)
        {
            PhysicMaterial material = new PhysicMaterial();
            material.dynamicFriction = friction;
            material.staticFriction = friction;
            material.bounciness = bounciness;
            material.frictionCombine = PhysicMaterialCombine.Average;
            material.bounceCombine = PhysicMaterialCombine.Average;
            return material;
        }
        
        /// <summary>
        /// Get default physics material
        /// </summary>
        public PhysicMaterial GetDefaultMaterial()
        {
            return CreatePhysicsMaterial(defaultFriction, defaultBounciness);
        }
    }
}
