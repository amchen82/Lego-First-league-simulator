using UnityEngine;
using System;

namespace LegoSimulator
{
    /// <summary>
    /// Handles collision detection and response for Lego pieces
    /// Supports blocking fixed objects and lifting/pushing movable ones
    /// </summary>
    public class CollisionHandler : MonoBehaviour
    {
        [Header("Collision Settings")]
        [SerializeField] private bool isMovable = true;
        [SerializeField] private bool isFixed = false;
        [SerializeField] private float pushForce = 5f;
        [SerializeField] private float liftForce = 3f;
        
        public event Action<Collision> OnCollisionDetected;
        public event Action<Collider> OnTriggerDetected;
        
        private Rigidbody rb;
        private LegoPiece legoPiece;
        
        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            legoPiece = GetComponent<LegoPiece>();
            
            if (isFixed && rb != null)
            {
                rb.isKinematic = true;
                rb.constraints = RigidbodyConstraints.FreezeAll;
            }
        }
        
        /// <summary>
        /// Called when collision occurs
        /// </summary>
        private void OnCollisionEnter(Collision collision)
        {
            OnCollisionDetected?.Invoke(collision);
            HandleCollision(collision);
        }
        
        /// <summary>
        /// Handle collision based on object properties
        /// </summary>
        private void HandleCollision(Collision collision)
        {
            CollisionHandler otherHandler = collision.gameObject.GetComponent<CollisionHandler>();
            
            if (otherHandler == null)
            {
                return;
            }
            
            // If this object is fixed, block the other object
            if (isFixed)
            {
                BlockObject(collision);
                return;
            }
            
            // If the other object is fixed, this object is blocked
            if (otherHandler.isFixed)
            {
                // This object is blocked, no further action
                return;
            }
            
            // Both objects are movable - handle push/lift
            if (isMovable && otherHandler.isMovable)
            {
                HandleMovableCollision(collision, otherHandler);
            }
        }
        
        /// <summary>
        /// Block an object from moving through this fixed object
        /// </summary>
        private void BlockObject(Collision collision)
        {
            Rigidbody otherRb = collision.rigidbody;
            if (otherRb != null && !otherRb.isKinematic)
            {
                // Apply counter force to stop penetration
                Vector3 normal = collision.contacts[0].normal;
                otherRb.velocity = Vector3.Reflect(otherRb.velocity * 0.5f, normal);
            }
        }
        
        /// <summary>
        /// Handle collision between two movable objects
        /// </summary>
        private void HandleMovableCollision(Collision collision, CollisionHandler otherHandler)
        {
            if (rb == null || otherHandler.rb == null)
            {
                return;
            }
            
            Vector3 collisionNormal = collision.contacts[0].normal;
            Vector3 relativeVelocity = rb.velocity - otherHandler.rb.velocity;
            
            // Determine if this is a push or lift scenario
            float verticalComponent = Vector3.Dot(collisionNormal, Vector3.up);
            
            if (Mathf.Abs(verticalComponent) > 0.7f)
            {
                // Vertical collision - lift
                ApplyLift(collision, otherHandler);
            }
            else
            {
                // Horizontal collision - push
                ApplyPush(collision, otherHandler);
            }
        }
        
        /// <summary>
        /// Apply lift force to the other object
        /// </summary>
        private void ApplyLift(Collision collision, CollisionHandler otherHandler)
        {
            if (otherHandler.rb != null && !otherHandler.isFixed)
            {
                Vector3 liftDirection = Vector3.up;
                otherHandler.rb.AddForce(liftDirection * liftForce, ForceMode.Impulse);
                Debug.Log($"{gameObject.name} lifting {otherHandler.gameObject.name}");
            }
        }
        
        /// <summary>
        /// Apply push force to the other object
        /// </summary>
        private void ApplyPush(Collision collision, CollisionHandler otherHandler)
        {
            if (otherHandler.rb != null && !otherHandler.isFixed)
            {
                Vector3 pushDirection = collision.contacts[0].normal;
                pushDirection.y = 0; // Keep push horizontal
                pushDirection.Normalize();
                
                otherHandler.rb.AddForce(pushDirection * pushForce, ForceMode.Impulse);
                Debug.Log($"{gameObject.name} pushing {otherHandler.gameObject.name}");
            }
        }
        
        /// <summary>
        /// Trigger detection for sensors
        /// </summary>
        private void OnTriggerEnter(Collider other)
        {
            OnTriggerDetected?.Invoke(other);
        }
        
        /// <summary>
        /// Set whether this object is fixed
        /// </summary>
        public void SetFixed(bool fixed)
        {
            isFixed = fixed;
            if (rb != null)
            {
                rb.isKinematic = fixed;
                if (fixed)
                {
                    rb.constraints = RigidbodyConstraints.FreezeAll;
                }
                else
                {
                    rb.constraints = RigidbodyConstraints.None;
                }
            }
        }
        
        /// <summary>
        /// Set whether this object is movable
        /// </summary>
        public void SetMovable(bool movable)
        {
            isMovable = movable;
        }
        
        /// <summary>
        /// Set push and lift forces
        /// </summary>
        public void SetForces(float push, float lift)
        {
            pushForce = push;
            liftForce = lift;
        }
    }
}
