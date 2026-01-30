using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Generic;

namespace LegoSimulator
{
    /// <summary>
    /// Python communication bridge for Unity
    /// Receives commands from Python SDKs and executes them in Unity
    /// </summary>
    public class PythonBridge : MonoBehaviour
    {
        [Header("Connection Settings")]
        [SerializeField] private int port = 5555;
        [SerializeField] private bool autoStart = true;
        
        private TcpListener server;
        private TcpClient client;
        private Thread listenerThread;
        private bool isRunning = false;
        
        private Queue<Action> mainThreadActions = new Queue<Action>();
        private object queueLock = new object();
        
        public event Action<string, Dictionary<string, object>> OnMessageReceived;
        
        private void Start()
        {
            if (autoStart)
            {
                StartServer();
            }
        }
        
        private void Update()
        {
            // Execute queued actions on main thread
            lock (queueLock)
            {
                while (mainThreadActions.Count > 0)
                {
                    Action action = mainThreadActions.Dequeue();
                    action?.Invoke();
                }
            }
        }
        
        private void OnDestroy()
        {
            StopServer();
        }
        
        /// <summary>
        /// Start the server to listen for Python connections
        /// </summary>
        public void StartServer()
        {
            if (isRunning)
            {
                Debug.LogWarning("Server already running");
                return;
            }
            
            try
            {
                server = new TcpListener(IPAddress.Any, port);
                server.Start();
                isRunning = true;
                
                listenerThread = new Thread(ListenForConnections);
                listenerThread.IsBackground = true;
                listenerThread.Start();
                
                Debug.Log($"Python Bridge server started on port {port}");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to start server: {e.Message}");
            }
        }
        
        /// <summary>
        /// Stop the server
        /// </summary>
        public void StopServer()
        {
            isRunning = false;
            
            if (client != null)
            {
                client.Close();
            }
            
            if (server != null)
            {
                server.Stop();
            }
            
            Debug.Log("Python Bridge server stopped");
        }
        
        /// <summary>
        /// Listen for incoming connections
        /// </summary>
        private void ListenForConnections()
        {
            while (isRunning)
            {
                try
                {
                    Debug.Log("Waiting for Python connection...");
                    client = server.AcceptTcpClient();
                    Debug.Log("Python client connected");
                    
                    HandleClient(client);
                }
                catch (Exception e)
                {
                    if (isRunning)
                    {
                        Debug.LogError($"Connection error: {e.Message}");
                    }
                }
            }
        }
        
        /// <summary>
        /// Handle connected client
        /// </summary>
        private void HandleClient(TcpClient tcpClient)
        {
            NetworkStream stream = tcpClient.GetStream();
            byte[] buffer = new byte[4096];
            StringBuilder messageBuffer = new StringBuilder();
            
            while (isRunning && tcpClient.Connected)
            {
                try
                {
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    if (bytesRead == 0) break;
                    
                    string data = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    messageBuffer.Append(data);
                    
                    // Process complete messages (newline-delimited)
                    string bufferStr = messageBuffer.ToString();
                    int newlineIndex;
                    
                    while ((newlineIndex = bufferStr.IndexOf('\n')) >= 0)
                    {
                        string message = bufferStr.Substring(0, newlineIndex);
                        bufferStr = bufferStr.Substring(newlineIndex + 1);
                        
                        ProcessMessage(message);
                    }
                    
                    messageBuffer.Clear();
                    messageBuffer.Append(bufferStr);
                }
                catch (Exception e)
                {
                    Debug.LogError($"Client handling error: {e.Message}");
                    break;
                }
            }
            
            tcpClient.Close();
            Debug.Log("Python client disconnected");
        }
        
        /// <summary>
        /// Process received message from Python
        /// </summary>
        private void ProcessMessage(string messageJson)
        {
            try
            {
                var message = JsonUtility.FromJson<PythonMessage>(messageJson);
                
                // Queue action for main thread
                lock (queueLock)
                {
                    mainThreadActions.Enqueue(() => HandleMessage(message));
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to process message: {e.Message}");
            }
        }
        
        /// <summary>
        /// Handle parsed message on main thread
        /// </summary>
        private void HandleMessage(PythonMessage message)
        {
            switch (message.type)
            {
                case "motor_command":
                    HandleMotorCommand(message.data);
                    break;
                    
                case "robot_state":
                    HandleRobotState(message.data);
                    break;
                    
                case "configuration":
                    HandleConfiguration(message.data);
                    break;
                    
                default:
                    Debug.LogWarning($"Unknown message type: {message.type}");
                    break;
            }
            
            // Trigger event
            OnMessageReceived?.Invoke(message.type, message.data);
        }
        
        /// <summary>
        /// Handle motor command from Python
        /// </summary>
        private void HandleMotorCommand(Dictionary<string, object> data)
        {
            if (!data.ContainsKey("motor_name") || !data.ContainsKey("rotation"))
            {
                Debug.LogWarning("Invalid motor command data");
                return;
            }
            
            string motorName = data["motor_name"].ToString();
            float rotation = Convert.ToSingle(data["rotation"]);
            float speed = data.ContainsKey("speed") ? Convert.ToSingle(data["speed"]) : 1.0f;
            
            // Find motor in scene
            LegoMotor motor = FindMotorByName(motorName);
            if (motor != null)
            {
                motor.SetRotation(rotation);
                Debug.Log($"Motor {motorName} commanded to rotate to {rotation} degrees");
            }
            else
            {
                Debug.LogWarning($"Motor {motorName} not found in scene");
            }
        }
        
        /// <summary>
        /// Handle robot state update from Python
        /// </summary>
        private void HandleRobotState(Dictionary<string, object> data)
        {
            // Robot state updates from Python
            Debug.Log("Robot state updated from Python");
        }
        
        /// <summary>
        /// Handle configuration from Python
        /// </summary>
        private void HandleConfiguration(Dictionary<string, object> data)
        {
            // Configuration updates
            Debug.Log("Configuration updated from Python");
        }
        
        /// <summary>
        /// Find motor by name in scene
        /// </summary>
        private LegoMotor FindMotorByName(string motorName)
        {
            LegoMotor[] motors = FindObjectsOfType<LegoMotor>();
            foreach (var motor in motors)
            {
                if (motor.gameObject.name == motorName || motor.name == motorName)
                {
                    return motor;
                }
            }
            return null;
        }
        
        /// <summary>
        /// Send message to Python
        /// </summary>
        public void SendToPython(string messageType, Dictionary<string, object> data)
        {
            if (client == null || !client.Connected)
            {
                Debug.LogWarning("No Python client connected");
                return;
            }
            
            try
            {
                PythonMessage message = new PythonMessage
                {
                    type = messageType,
                    data = data
                };
                
                string json = JsonUtility.ToJson(message) + "\n";
                byte[] bytes = Encoding.UTF8.GetBytes(json);
                
                NetworkStream stream = client.GetStream();
                stream.Write(bytes, 0, bytes.Length);
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to send message to Python: {e.Message}");
            }
        }
    }
    
    [Serializable]
    public class PythonMessage
    {
        public string type;
        public Dictionary<string, object> data;
    }
}
