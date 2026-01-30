# Setup Instructions

## Unity Setup

### Prerequisites
- Unity Hub installed
- Unity 2021.3.20f1 or later
- WebGL Build Support module

### Installation Steps

1. **Install Unity**
   - Download Unity Hub from https://unity.com/download
   - Install Unity 2021.3.20f1 or later through Unity Hub
   - During installation, make sure to include WebGL Build Support

2. **Open Project**
   - Launch Unity Hub
   - Click "Add" and select the `UnityProject` folder
   - Open the project by clicking on it in Unity Hub

3. **First Run**
   - Unity will import all assets (this may take a few minutes)
   - Once complete, the project is ready to use

### Creating a Scene

1. Create a new scene or use an existing one
2. Add the following components to your scene:
   - `PhysicsManager` - Add to an empty GameObject named "Managers"
   - `SimulatorSceneManager` - Add to the same GameObject
   - `PythonBridge` - Add to another GameObject named "PythonBridge"

3. The scene is now ready for simulation

## Python Setup

### Prerequisites
- Python 3.7 or later
- pip package manager

### Installation Steps

1. **Navigate to PythonIntegration directory**
   ```bash
   cd PythonIntegration
   ```

2. **Install dependencies**
   ```bash
   pip install -r requirements.txt
   ```

3. **Test installation**
   ```bash
   python examples/mindstorms_drive_example.py
   ```

### Virtual Environment (Recommended)

1. **Create virtual environment**
   ```bash
   python -m venv venv
   ```

2. **Activate virtual environment**
   - Windows: `venv\Scripts\activate`
   - Linux/Mac: `source venv/bin/activate`

3. **Install dependencies**
   ```bash
   pip install -r requirements.txt
   ```

## Running the Simulator

### Development Mode (Unity Editor)

1. Open Unity project
2. Open a scene with the simulator components
3. Press the Play button
4. Unity console will show "Python Bridge server started on port 5555"
5. Run your Python script in a separate terminal
6. The Python script will connect and control the robot in Unity

### Production Mode (WebGL Build)

1. **Build WebGL**
   - Open Unity
   - Go to File > Build Settings
   - Select WebGL platform
   - Click "Switch Platform" if not already selected
   - Click "Build" and choose output directory

2. **Host the Build**
   - Use a local web server (e.g., Python's http.server)
   - Navigate to build output directory
   - Run: `python -m http.server 8000`
   - Open browser to http://localhost:8000

3. **Connect Python**
   - Python scripts work the same way with WebGL build
   - Make sure to use correct IP and port in Python scripts

## Troubleshooting

### Unity Issues

**Problem**: Unity won't open project
- **Solution**: Ensure Unity version is 2021.3.20f1 or later

**Problem**: Missing references in scripts
- **Solution**: Let Unity re-import all assets (this happens automatically)

**Problem**: WebGL build fails
- **Solution**: Ensure WebGL Build Support is installed in Unity Hub

### Python Issues

**Problem**: Cannot connect to Unity
- **Solution**: 
  - Ensure Unity simulator is running
  - Check firewall settings for port 5555
  - Verify IP address (use "localhost" or "127.0.0.1")

**Problem**: Import errors
- **Solution**:
  - Ensure you're in the PythonIntegration directory
  - Check sys.path modifications in example scripts
  - Verify requirements.txt dependencies are installed

### Network Issues

**Problem**: Connection refused
- **Solution**:
  - Check if PythonBridge component is in the scene
  - Verify port 5555 is not in use by another application
  - Check firewall/antivirus settings

**Problem**: Connection timeout
- **Solution**:
  - Ensure Unity is running before starting Python script
  - Wait for "Python Bridge server started" message in Unity console
  - Try restarting both Unity and Python script

## Next Steps

After setup:
1. Review the example scripts in `PythonIntegration/examples/`
2. Read the full documentation in `Documentation/README.md`
3. Try modifying examples to learn the API
4. Build your own robot programs!
