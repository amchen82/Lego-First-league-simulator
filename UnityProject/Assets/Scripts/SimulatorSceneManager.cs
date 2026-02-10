using UnityEngine;
using System.Collections.Generic;

namespace LegoSimulator
{
    /// <summary>
    /// Scene manager for the Lego simulator
    /// Handles environment setup, piece management, and simulation control
    /// </summary>
    public class SimulatorSceneManager : MonoBehaviour
    {
        [Header("Scene Objects")]
        [SerializeField] private GameObject floorPrefab;
        [SerializeField] private GameObject robotPrefab;
        [SerializeField] private List<GameObject> legoPiecePrefabs = new List<GameObject>();
        
        [Header("Scene Settings")]
        [SerializeField] private Vector3 sceneSize = new Vector3(10, 0, 10);
        [SerializeField] private Color floorColor = Color.white;
        
        [Header("Simulation")]
        [SerializeField] private bool simulationRunning = false;
        [SerializeField] private float timeScale = 1.0f;
        
        private GameObject floor;
        private GameObject robot;
        private List<GameObject> placedPieces = new List<GameObject>();
        private PhysicsManager physicsManager;
        private PythonBridge pythonBridge;
        
        private static SimulatorSceneManager instance;
        
        public static SimulatorSceneManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<SimulatorSceneManager>();
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
            
            InitializeScene();
        }
        
        /// <summary>
        /// Initialize the scene
        /// </summary>
        private void InitializeScene()
        {
            // Get or create physics manager
            physicsManager = PhysicsManager.Instance;
            
            // Get or create Python bridge
            pythonBridge = FindObjectOfType<PythonBridge>();
            if (pythonBridge == null)
            {
                GameObject bridgeObj = new GameObject("PythonBridge");
                pythonBridge = bridgeObj.AddComponent<PythonBridge>();
            }
            
            // Create floor if not exists
            if (floor == null)
            {
                CreateFloor();
            }
            
            Debug.Log("Simulator scene initialized");
        }
        
        /// <summary>
        /// Create the floor/ground
        /// </summary>
        private void CreateFloor()
        {
            if (floorPrefab != null)
            {
                floor = Instantiate(floorPrefab, Vector3.zero, Quaternion.identity);
            }
            else
            {
                floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
                floor.name = "Floor";
                floor.transform.position = Vector3.zero;
                floor.transform.localScale = sceneSize / 10f; // Plane is 10x10 by default
                
                // Apply floor color
                Renderer renderer = floor.GetComponent<Renderer>();
                if (renderer != null && renderer.material != null)
                {
                    renderer.material.color = floorColor;
                }
                
                // Add physics material
                Collider collider = floor.GetComponent<Collider>();
                if (collider != null)
                {
                    collider.material = physicsManager.GetDefaultMaterial();
                }
                
                // Add collision handler
                CollisionHandler handler = floor.AddComponent<CollisionHandler>();
                handler.SetFixed(true);
            }
        }
        
        /// <summary>
        /// Create and place a robot in the scene
        /// </summary>
        public GameObject CreateRobot(Vector3 position)
        {
            if (robotPrefab != null)
            {
                robot = Instantiate(robotPrefab, position, Quaternion.identity);
            }
            else
            {
                // Create simple robot
                robot = new GameObject("Robot");
                robot.transform.position = position;
                
                // Add body
                GameObject body = GameObject.CreatePrimitive(PrimitiveType.Cube);
                body.name = "Body";
                body.transform.SetParent(robot.transform);
                body.transform.localPosition = Vector3.zero;
                body.transform.localScale = new Vector3(2, 1, 3);
                
                // Add wheels
                GameObject leftWheel = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                leftWheel.name = "LeftWheel";
                leftWheel.transform.SetParent(robot.transform);
                leftWheel.transform.localPosition = new Vector3(-1.2f, -0.5f, 0.5f);
                leftWheel.transform.localRotation = Quaternion.Euler(0, 0, 90);
                leftWheel.transform.localScale = new Vector3(1, 0.2f, 1);
                
                GameObject rightWheel = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                rightWheel.name = "RightWheel";
                rightWheel.transform.SetParent(robot.transform);
                rightWheel.transform.localPosition = new Vector3(1.2f, -0.5f, 0.5f);
                rightWheel.transform.localRotation = Quaternion.Euler(0, 0, 90);
                rightWheel.transform.localScale = new Vector3(1, 0.2f, 1);
                
                // Add components
                robot.AddComponent<RobotController>();
                robot.AddComponent<LegoPiece>();
                robot.AddComponent<CollisionHandler>();
                
                // Set colors
                Renderer bodyRenderer = body.GetComponent<Renderer>();
                if (bodyRenderer != null && bodyRenderer.material != null)
                {
                    bodyRenderer.material.color = Color.blue;
                }
            }
            
            Debug.Log($"Robot created at {position}");
            return robot;
        }
        
        /// <summary>
        /// Place a Lego piece in the scene
        /// </summary>
        public GameObject PlacePiece(string pieceType, Vector3 position, Quaternion rotation)
        {
            GameObject piece = null;
            
            // Try to find prefab
            GameObject prefab = legoPiecePrefabs.Find(p => p.name == pieceType);
            
            if (prefab != null)
            {
                piece = Instantiate(prefab, position, rotation);
            }
            else
            {
                // Create simple piece
                piece = GameObject.CreatePrimitive(PrimitiveType.Cube);
                piece.name = pieceType;
                piece.transform.position = position;
                piece.transform.rotation = rotation;
                
                // Add components
                piece.AddComponent<LegoPiece>();
                piece.AddComponent<CollisionHandler>();
                
                Renderer renderer = piece.GetComponent<Renderer>();
                if (renderer != null && renderer.material != null)
                {
                    renderer.material.color = Random.ColorHSV();
                }
            }
            
            placedPieces.Add(piece);
            Debug.Log($"Placed {pieceType} at {position}");
            
            return piece;
        }
        
        /// <summary>
        /// Start simulation
        /// </summary>
        public void StartSimulation()
        {
            simulationRunning = true;
            Time.timeScale = timeScale;
            Debug.Log("Simulation started");
        }
        
        /// <summary>
        /// Pause simulation
        /// </summary>
        public void PauseSimulation()
        {
            simulationRunning = false;
            Time.timeScale = 0;
            Debug.Log("Simulation paused");
        }
        
        /// <summary>
        /// Reset simulation
        /// </summary>
        public void ResetSimulation()
        {
            // Remove all placed pieces
            foreach (GameObject piece in placedPieces)
            {
                Destroy(piece);
            }
            placedPieces.Clear();
            
            // Reset robot if exists
            if (robot != null)
            {
                Destroy(robot);
                robot = null;
            }
            
            simulationRunning = false;
            Time.timeScale = 1;
            
            Debug.Log("Simulation reset");
        }
        
        /// <summary>
        /// Set simulation time scale
        /// </summary>
        public void SetTimeScale(float scale)
        {
            timeScale = Mathf.Clamp(scale, 0.1f, 10f);
            if (simulationRunning)
            {
                Time.timeScale = timeScale;
            }
        }
        
        /// <summary>
        /// Get current robot
        /// </summary>
        public GameObject GetRobot()
        {
            return robot;
        }
        
        /// <summary>
        /// Check if simulation is running
        /// </summary>
        public bool IsSimulationRunning()
        {
            return simulationRunning;
        }
    }
}
