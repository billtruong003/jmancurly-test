using Fusion;
using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;
using System.Linq;
using Fusion.Sockets;
using System.Collections;
using System.Text;
using System.Threading.Tasks;

namespace NetworkGame
{
    public class NetworkRunnerManager : MonoBehaviour, INetworkRunnerCallbacks
    {
        public static NetworkRunnerManager Instance { get; private set; }

        [Header("Player Prefab")]
        [SerializeField] private NetworkObject _playerPrefab;

        [Header("Settings")]
        [SerializeField] private int _maxPlayers = 4;
        [SerializeField] private int _gameplaySceneIndex = 1;
        [SerializeField] private int _menuSceneIndex = 0;
        [SerializeField] private float _spawnProtectionTime = 2.0f;

        [Header("UI References")]
        [SerializeField] private UIManager _uiManager;

        public NetworkRunner Runner => _runner;
        public PlayerRef? LocalPlayerRef => _runner?.LocalPlayer;
        private bool _isConnected => _runner != null && _runner.IsRunning;

        // Room keys
        private const string ROOM_ID_KEY = "RoomId";
        private const string ROOM_NAME_KEY = "RoomName";

        // Private variables
        private string _currentRoomID;
        private string _currentRoomName;
        private NetworkRunner _runner;
        private INetworkSceneManager _sceneManager;
        private Dictionary<PlayerRef, NetworkObject> _spawnedPlayers = new Dictionary<PlayerRef, NetworkObject>();
        private Dictionary<PlayerRef, float> _playerSpawnTimes = new Dictionary<PlayerRef, float>();
        private NetworkObject _localPlayerObject;

        // Network stats tracking
        private float _bytesIn, _bytesOut;
        private float _updateInterval = 1.0f;
        private float _lastStatsTime;
        private float _bytesSentPerSecond;
        private float _bytesReceivedPerSecond;
        private int _lastRtt = 0;
        private float _lastNetStatsCheck = 0f;
        private const float NET_STATS_PERIOD = 1.0f;

        // Kiểm tra thay đổi trong MasterClient status
        private bool _lastMasterClientStatus = false;

        private void Awake()
        {
            // Singleton pattern check
            if (Instance != null && Instance != this)
            {
                Debug.LogWarning($"Found duplicate NetworkRunnerManager! Destroying this instance.");
                Destroy(gameObject);
                return;
            }

            // Set up singleton instance
            Instance = this;
            DontDestroyOnLoad(gameObject);

            Debug.Log("NetworkRunner chưa được khởi tạo");

            // Cài đặt các components khác
            if (_runner != null)
            {
                Debug.Log($"NetworkRunner đã có sẵn. MasterClientID: {(_runner.IsSharedModeMasterClient ? "This client" : "Other client")}");
                _sceneManager = _runner.SceneManager;
            }
            else
            {
                Debug.Log("NetworkRunner chưa được khởi tạo");
                // Tạo NetworkSceneManager nếu chưa có
                if (_sceneManager == null)
                {
                    _sceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>();
                }
            }

            // Initialize random seed
            UnityEngine.Random.InitState(System.Environment.TickCount);

            // Check if player prefab is properly set up with animator controller
            CheckPlayerPrefabSetup();
        }

        private void CheckPlayerPrefabSetup()
        {
            if (_playerPrefab == null)
            {
                Debug.LogError("Player prefab is not assigned in NetworkRunnerManager!");
                return;
            }

            // Check if the player prefab has an animator with controller
            Animator prefabAnimator = _playerPrefab.GetComponentInChildren<Animator>();
            if (prefabAnimator == null)
            {
                Debug.LogError("Player prefab missing Animator component! Animations will not work.");
                return;
            }

            if (prefabAnimator.runtimeAnimatorController == null)
            {
                Debug.LogError("Player prefab Animator has no Controller assigned! Animations will not work.");

                // Try to locate the Synty sample prefab for reference
                GameObject syntyPrefab = Resources.Load<GameObject>("PF_Player");
                if (syntyPrefab == null)
                {
                    // Try alternative locations
                    syntyPrefab = Resources.Load<GameObject>("Assets/Synty/AnimationBaseLocomotion/Samples/Prefabs/PF_Player");
                }

                if (syntyPrefab != null)
                {
                    Animator syntyAnimator = syntyPrefab.GetComponentInChildren<Animator>();
                    if (syntyAnimator != null && syntyAnimator.runtimeAnimatorController != null)
                    {
                        Debug.LogWarning($"Consider using the animation controller from the Synty sample prefab: {syntyAnimator.runtimeAnimatorController.name}");
                    }
                }
            }
            else
            {
                Debug.Log($"Player prefab animator configured with controller: {prefabAnimator.runtimeAnimatorController.name}");
            }
        }

        private void Start()
        {
            // Register callbacks
            if (_runner != null)
                _runner.AddCallbacks(this);

            // Register for scene change events
            SceneManager.sceneLoaded += OnSceneLoaded;

            // Connect with the UIManager in the starting scene
            StartCoroutine(InitialConnectToUIManager());

            // Initialize stats tracking
            _lastStatsTime = Time.time;
        }

        private void Update()
        {
            // Update network stats
            if (_runner != null && _runner.IsRunning)
            {
                // Update network stats every second
                if (Time.time > _lastStatsTime + _updateInterval)
                {
                    UpdateNetworkStats();
                    _lastStatsTime = Time.time;
                }

                // Kiểm tra thay đổi trạng thái MasterClient
                if (_runner.IsSharedModeMasterClient != _lastMasterClientStatus)
                {
                    _lastMasterClientStatus = _runner.IsSharedModeMasterClient;
                    Debug.Log($"Trạng thái MasterClient thay đổi. Tôi là MasterClient: {_lastMasterClientStatus}");

                    // Nếu tôi vừa trở thành MasterClient
                    if (_lastMasterClientStatus)
                    {
                        Debug.Log("Tôi trở thành MasterClient, kiểm tra người chơi chưa được spawn...");
                        foreach (var player in _runner.ActivePlayers)
                        {
                            Debug.Log($"Kiểm tra người chơi: {player}");
                            // Bạn có thể thêm logic để spawn lại người chơi ở đây nếu cần
                        }
                    }
                }
            }
        }

        // Generate a random 6-character room code
        private string GenerateRoomID()
        {
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789"; // Omitting easily confused characters
            var random = new System.Random();
            var result = new StringBuilder(6);

            for (int i = 0; i < 6; i++)
            {
                result.Append(chars[random.Next(chars.Length)]);
            }

            return result.ToString();
        }

        private void UpdateNetworkStats()
        {
            if (_runner != null && _runner.IsRunning)
            {
                // In Fusion, direct access to detailed statistics might depend on version
                // Using a simplified approach for basic metrics

                // For RTT (ping), some versions of Fusion expose it directly
                try
                {
                    // Try to get ping from the runner if available in this version
                    // This is a simplified approach since exact API may vary
                    float rtt = (float)_runner.GetPlayerRtt(PlayerRef.None);
                    _lastRtt = (int)Mathf.Round(rtt);
                }
                catch (Exception)
                {
                    // If not available, use a dummy value or previous value
                    Debug.LogWarning("Could not retrieve RTT from NetworkRunner");
                }

                // For bandwidth tracking, a simplified approach: track frame traffic
                // This is a simplified approach since exact API may vary
                float totalTraffic = EstimateTraffic();
                float duration = Time.time - _lastStatsTime;
                if (duration > 0)
                {
                    // Simple estimate - in a real implementation you would get these from Fusion
                    _bytesSentPerSecond = totalTraffic * 0.4f / duration; // Assuming 40% is outgoing
                    _bytesReceivedPerSecond = totalTraffic * 0.6f / duration; // Assuming 60% is incoming

                    _bytesIn += _bytesReceivedPerSecond * duration;
                    _bytesOut += _bytesSentPerSecond * duration;
                }
            }
        }

        private float EstimateTraffic()
        {
            // This is a placeholder - in a real implementation, you would get this from Fusion
            // The actual implementation depends on the Fusion version and available APIs
            // Using a simple simulation for demonstration purposes
            int playerCount = GetPlayerCount();
            float baseTraffic = 2048; // base traffic in bytes per second
            return baseTraffic * (1 + playerCount * 0.5f); // Traffic increases with player count
        }

        private void OnApplicationQuit()
        {
            // Đảm bảo NetworkRunner được shutdown đúng cách khi thoát ứng dụng
            if (_runner != null && _runner.IsRunning)
            {
                Debug.Log("Application is quitting - shutting down NetworkRunner");
                _runner.Shutdown();
            }
        }

        private void OnDestroy()
        {
            // Đảm bảo NetworkRunner được shutdown đúng cách khi đối tượng bị hủy
            if (_runner != null && _runner.IsRunning)
            {
                Debug.Log("NetworkRunnerManager is being destroyed - shutting down NetworkRunner");
                _runner.Shutdown();
            }

            // Xóa singleton reference nếu đây là instance hiện tại
            if (Instance == this)
            {
                Instance = null;
            }

            // Unregister from scene change events
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // When scene is loaded, try to connect to UIManager
            // Use coroutine to wait for GameplayManager to be initialized if needed
            StartCoroutine(ConnectToUIManagerCoroutine(scene));
        }

        private IEnumerator ConnectToUIManagerCoroutine(Scene scene)
        {
            // Wait for at least one frame for scene objects to initialize
            yield return null;

            // If we're in gameplay scene, wait for GameplayManager to be available
            if (scene.buildIndex == _gameplaySceneIndex)
            {
                float timeout = 5f; // Maximum wait time in seconds
                float elapsed = 0f;

                // Wait until GameplayManager is available or timeout expires
                while (GameplayManager.Instance == null && elapsed < timeout)
                {
                    elapsed += Time.deltaTime;
                    yield return null;
                }

                if (GameplayManager.Instance == null)
                {
                    Debug.LogError("Timeout waiting for GameplayManager.Instance!");
                }
            }

            // Now try to connect to UIManager
            ConnectToUIManager();

            // Update UI state based on current scene
            if (_uiManager != null)
            {
                if (scene.buildIndex == _menuSceneIndex)
                {
                    _uiManager.ShowMainMenu();
                }
                else if (scene.buildIndex == _gameplaySceneIndex)
                {
                    _uiManager.ShowGamePanel();
                }
            }
        }

        private IEnumerator InitialConnectToUIManager()
        {
            // Wait for at least one frame for all objects to initialize
            yield return null;

            // Get the current scene
            Scene currentScene = SceneManager.GetActiveScene();

            // Use the same coroutine we use for scene transitions
            yield return StartCoroutine(ConnectToUIManagerCoroutine(currentScene));
        }

        private void ConnectToUIManager()
        {
            // First check if we're in the gameplay scene with GameplayManager
            if (GameplayManager.Instance != null)
            {
                _uiManager = GameplayManager.Instance.GetUIManager();
                if (_uiManager == null)
                {
                    Debug.LogWarning("UIManager reference not set in GameplayManager!");
                }
            }
            else
            {
                // We're in a scene without GameplayManager, directly find UIManager
                // This will only happen in MenuScene where there's only a single UIManager
                _uiManager = GameObject.FindObjectOfType<UIManager>();

                if (_uiManager == null)
                {
                    Debug.LogError("UIManager not found in current scene!");
                }
            }
        }

        #region Network Stats Methods
        public string GetCurrentRoomName()
        {
            return _currentRoomName;
        }

        public string GetCurrentRoomID()
        {
            return _currentRoomID;
        }

        public string GetFullRoomInfo()
        {
            if (string.IsNullOrEmpty(_currentRoomID))
                return _currentRoomName;
            else
                return $"{_currentRoomName} (ID: {_currentRoomID})";
        }

        public int GetPlayerCount()
        {
            if (_runner != null && _runner.IsRunning)
            {
                return _runner.ActivePlayers.Count();
            }
            return 0;
        }

        public int GetMaxPlayers()
        {
            return _maxPlayers;
        }

        public int GetPing()
        {
            return _lastRtt;
        }

        public bool IsConnected()
        {
            return _runner != null && _runner.IsRunning;
        }

        public string GetNetworkStats()
        {
            if (_runner != null && _runner.IsRunning)
            {
                StringBuilder sb = new StringBuilder();

                sb.AppendLine($"Room: {_currentRoomName}");
                sb.AppendLine($"Room ID: {_currentRoomID}");
                sb.AppendLine($"RTT: {_lastRtt} ms");
                sb.AppendLine($"Upload: {FormatBytesPerSecond(_bytesSentPerSecond)}");
                sb.AppendLine($"Download: {FormatBytesPerSecond(_bytesReceivedPerSecond)}");
                sb.AppendLine($"Total Sent: {FormatBytes(_bytesOut)}");
                sb.AppendLine($"Total Received: {FormatBytes(_bytesIn)}");

                return sb.ToString();
            }
            return "Not connected";
        }

        private string FormatBytes(float bytes)
        {
            string[] suffixes = { "B", "KB", "MB", "GB" };
            int suffixIndex = 0;
            while (bytes >= 1024 && suffixIndex < suffixes.Length - 1)
            {
                bytes /= 1024;
                suffixIndex++;
            }
            return $"{bytes:F1} {suffixes[suffixIndex]}";
        }

        private string FormatBytesPerSecond(float bytesPerSecond)
        {
            return $"{FormatBytes(bytesPerSecond)}/s";
        }
        #endregion

        #region Public Methods for UI
        public async Task<bool> StartHost(string roomName)
        {
            if (string.IsNullOrEmpty(roomName))
            {
                ShowStatus("Please enter a room name");
                return false;
            }

            Debug.Log("===== STARTING HOST IN SHARED MODE =====");
            Debug.Log($"Room Name: {roomName}");

            try
            {
                // Cleanup any existing runner
                await CleanupRunner();

                // Create a new runner
                _runner = CreateRunner();

                // Add callbacks and scene manager
                _runner.AddCallbacks(this);
                _sceneManager = _runner.gameObject.AddComponent<NetworkSceneManagerDefault>();

                // Generate a unique room ID
                _currentRoomID = GenerateRoomID();
                _currentRoomName = roomName;
                Debug.Log($"Generated new Room ID: {_currentRoomID}");

                // Set up custom room properties
                var customProps = new Dictionary<string, SessionProperty>();
                customProps[ROOM_ID_KEY] = _currentRoomID;
                customProps[ROOM_NAME_KEY] = _currentRoomName;
                customProps["MaxPlayers"] = _maxPlayers;

                // Set up scene info
                var sceneInfo = new NetworkSceneInfo();
                int buildIndex = _gameplaySceneIndex;
                string scenePath = SceneUtility.GetScenePathByBuildIndex(buildIndex);
                sceneInfo.AddSceneRef(_sceneManager.GetSceneRef(scenePath), LoadSceneMode.Single);

                Debug.Log("Setting up StartGameArgs with Shared mode and custom properties");
                var startGameArgs = new StartGameArgs()
                {
                    GameMode = GameMode.Shared,
                    SessionName = _currentRoomID,
                    SessionProperties = customProps,
                    SceneManager = _sceneManager,
                    Scene = sceneInfo,
                    PlayerCount = _maxPlayers
                };

                // Start the game
                Debug.Log($"Calling StartGame with mode: {startGameArgs.GameMode}, SessionName: {startGameArgs.SessionName}");
                var result = await _runner.StartGame(startGameArgs);

                if (result.Ok)
                {
                    Debug.Log("===== HOST STARTED SUCCESSFULLY =====");
                    Debug.Log($"Local PlayerRef: {_runner.LocalPlayer}, PlayerId: {_runner.LocalPlayer.PlayerId}");
                    Debug.Log($"IsSharedModeMasterClient: {_runner.IsSharedModeMasterClient}");
                    Debug.Log($"Runner Object ID: {_runner.GetInstanceID()}, IsRunning: {_runner.IsRunning}");

                    LogActivePlayers();

                    // Update UI
                    if (_uiManager != null)
                    {
                        _uiManager.SetRoomIdText(_currentRoomID);
                        _uiManager.UpdatePlayerCount(_runner.ActivePlayers.Count(), _maxPlayers);
                        _uiManager.SetRoomNameText(_currentRoomName);
                    }

                    return true;
                }
                else
                {
                    Debug.LogError($"Failed to start host: {result.ShutdownReason}");
                    ShowUIMessage($"Failed to create room: {result.ShutdownReason}");
                    return false;
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Exception during StartHost: {e.Message}\n{e.StackTrace}");
                ShowUIMessage($"Error starting host: {e.Message}");
                return false;
            }
        }

        public async Task<bool> JoinGame(string roomID)
        {
            if (string.IsNullOrEmpty(roomID))
            {
                Debug.LogError("Cannot join game: Room ID is empty");
                ShowUIMessage("Invalid Room ID!");
                return false;
            }

            // Standardize room ID format
            roomID = roomID.Trim().ToUpper();

            Debug.Log("===== JOINING GAME IN SHARED MODE =====");
            Debug.Log($"Room ID to join: {roomID}");

            try
            {
                // Cleanup any existing runner
                await CleanupRunner();

                // Create a new runner
                _runner = CreateRunner();

                // Add callbacks and scene manager
                _runner.AddCallbacks(this);
                _sceneManager = _runner.gameObject.AddComponent<NetworkSceneManagerDefault>();

                // Setup join game arguments
                Debug.Log("Setting up StartGameArgs for joining with Shared mode");
                var joinGameArgs = new StartGameArgs()
                {
                    GameMode = GameMode.Shared,
                    SessionName = roomID, // Use the provided room ID to find the session
                    SceneManager = _sceneManager,
                    PlayerCount = _maxPlayers
                };

                // Join the game
                Debug.Log($"Calling StartGame to join room: {joinGameArgs.SessionName}, Mode: {joinGameArgs.GameMode}");
                var result = await _runner.StartGame(joinGameArgs);

                if (result.Ok)
                {
                    _currentRoomID = roomID;
                    Debug.Log("===== JOINED GAME SUCCESSFULLY =====");
                    Debug.Log($"Local PlayerRef: {_runner.LocalPlayer}, PlayerId: {_runner.LocalPlayer.PlayerId}");
                    Debug.Log($"IsSharedModeMasterClient: {_runner.IsSharedModeMasterClient}");
                    Debug.Log($"Runner Object ID: {_runner.GetInstanceID()}, IsRunning: {_runner.IsRunning}");

                    LogSessionProperties();
                    LogActivePlayers();

                    // Set room name from properties if available
                    if (_runner.SessionInfo.Properties.TryGetValue(ROOM_NAME_KEY, out SessionProperty roomNameProp))
                    {
                        _currentRoomName = roomNameProp.ToString();
                        if (_currentRoomName.Contains("SessionProperty:"))
                        {
                            int startIndex = _currentRoomName.IndexOf(":") + 1;
                            int endIndex = _currentRoomName.IndexOf(",");
                            if (endIndex > startIndex)
                            {
                                _currentRoomName = _currentRoomName.Substring(startIndex, endIndex - startIndex).Trim();
                            }
                        }
                        Debug.Log($"Got room name from properties: {_currentRoomName}");
                    }
                    else
                    {
                        _currentRoomName = $"Room {roomID}";
                        Debug.Log($"Using default room name: {_currentRoomName}");
                    }

                    // Update UI
                    if (_uiManager != null)
                    {
                        _uiManager.SetRoomIdText(_currentRoomID);
                        _uiManager.UpdatePlayerCount(_runner.ActivePlayers.Count(), _maxPlayers);
                        _uiManager.SetRoomNameText(_currentRoomName);
                    }

                    return true;
                }
                else
                {
                    Debug.LogError($"Failed to join room: {result.ShutdownReason}");
                    ShowUIMessage($"Failed to join room: {result.ShutdownReason}");
                    return false;
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Exception during JoinGame: {e.Message}\n{e.StackTrace}");
                ShowUIMessage($"Error joining room: {e.Message}");
                return false;
            }
        }

        private async Task CleanupRunner()
        {
            if (_runner != null)
            {
                if (_runner.IsRunning)
                {
                    Debug.Log($"Shutting down existing runner. Active players: {(_runner.ActivePlayers?.Count() ?? 0)}");
                    await _runner.Shutdown();
                    Debug.Log("Previous runner shut down successfully");
                }

                Destroy(_runner.gameObject);
                _runner = null;
                await Task.Delay(200); // Short delay to ensure cleanup
            }
        }

        private NetworkRunner CreateRunner()
        {
            Debug.Log("Creating a new NetworkRunner GameObject");
            var go = new GameObject("NetworkRunner");
            DontDestroyOnLoad(go);
            return go.AddComponent<NetworkRunner>();
        }

        public async Task Disconnect()
        {
            if (_runner != null && _runner.IsRunning)
            {
                Debug.Log("Disconnecting from game...");

                try
                {
                    await _runner.Shutdown();
                    Debug.Log("Disconnected successfully");

                    Destroy(_runner.gameObject);
                    _runner = null;
                    _playerSpawnTimes.Clear();

                    // Load main menu scene
                    SceneManager.LoadScene(0);
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error during disconnect: {e.Message}");
                }
            }
        }
        #endregion

        private void ShowStatus(string message)
        {
            if (_uiManager != null)
            {
                _uiManager.SetStatusText(message);
            }
            Debug.Log($"Network Status: {message}");
        }

        // Helper method to show messages in the UI
        private void ShowUIMessage(string message)
        {
            if (_uiManager == null)
            {
                ConnectToUIManager();
            }

            if (_uiManager != null)
            {
                _uiManager.SetStatusText(message);
            }
            else
            {
                Debug.LogWarning($"UI Message: {message} (UIManager not found)");
            }
        }

        // INetworkRunnerCallbacks implementation
        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            Debug.Log($"Player {player} joined. IsLocal: {player == runner.LocalPlayer}, LocalPlayerRef: {runner.LocalPlayer}, IsMaster: {runner.IsSharedModeMasterClient}");

            // Trong Shared Mode, mỗi người chơi sẽ spawn nhân vật của chính mình
            if (player == runner.LocalPlayer)
            {
                // Spawn player's NetworkObject with offset based on PlayerRef to avoid overlapping
                Debug.Log($"Spawning player object for local player {player}");

                // Check if player prefab has proper animations setup
                Animator prefabAnimator = _playerPrefab.GetComponentInChildren<Animator>();
                if (prefabAnimator != null)
                {
                    Debug.Log($"Player prefab has Animator: {prefabAnimator.name}, Has Controller: {prefabAnimator.runtimeAnimatorController != null}");
                    if (prefabAnimator.runtimeAnimatorController != null)
                    {
                        Debug.Log($"Prefab Animation Controller: {prefabAnimator.runtimeAnimatorController.name}");
                    }
                    else
                    {
                        Debug.LogError("Player prefab animator has no controller assigned! This will cause animation issues.");
                    }
                }
                else
                {
                    Debug.LogError("Player prefab has no Animator component! This will cause animation issues.");
                }

                // Tính vị trí spawn dựa trên PlayerRef để tránh nhân vật chồng lên nhau
                Vector3 spawnPosition = new Vector3(player.PlayerId * 2.0f, 1f, 0f);

                var playerObject = runner.Spawn(_playerPrefab, spawnPosition, Quaternion.identity, player);

                if (playerObject != null)
                {
                    Debug.Log($"Player object spawned successfully with ID: {playerObject.Id}");
                    _spawnedPlayers[player] = playerObject;
                    _playerSpawnTimes[player] = Time.time;
                    _localPlayerObject = playerObject;

                    // Đảm bảo quyền InputAuthority và StateAuthority trong Shared Mode
                    NetworkObject networkObject = playerObject.GetComponent<NetworkObject>();
                    if (networkObject != null)
                    {
                        Debug.Log($"Checking input authority for {player} on object {playerObject.Id}");
                        // Trong Shared Mode, Fusion 2 tự động gán Input Authority khi spawn
                        // Không cần gọi RequestInputAuthority
                    }
                }
                else
                {
                    Debug.LogError("Failed to spawn player object!");
                }
            }
            else
            {
                Debug.Log($"Remote player {player} joined, they will spawn their own player object");
            }

            // Update player count
            if (_uiManager != null)
            {
                _uiManager.UpdatePlayerCount(runner.ActivePlayers.Count(), _maxPlayers);
            }
        }

        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
            Debug.Log($"Player {player} left");

            // Despawn player if they have a spawned object
            if (_playerSpawnTimes.TryGetValue(player, out float spawnTime))
            {
                if (Time.time - spawnTime < _spawnProtectionTime)
                {
                    Debug.Log($"Player {player} is in spawn protection. Not despawning.");
                    return;
                }
            }

            _playerSpawnTimes.Remove(player);
            ShowStatus($"Player {player} left the game");
        }

        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
        {
            Debug.Log($"Game shutdown: {shutdownReason}");

            _playerSpawnTimes.Clear();

            // Return to main menu
            if (SceneManager.GetActiveScene().buildIndex != _menuSceneIndex)
            {
                SceneManager.LoadScene(_menuSceneIndex);
            }
        }

        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
        {
            // Accept all connection requests
            request.Accept();
        }

        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
        {
            // Handle simulation messages
        }

        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
        {
            // Log thông tin phòng để debug
            Debug.Log($"Session list updated: {sessionList.Count} sessions found");
            foreach (var session in sessionList)
            {
                Debug.Log($"Available session: '{session.Name}', Players: {session.PlayerCount}/{session.MaxPlayers}, IsOpen: {session.IsOpen}, IsVisible: {session.IsVisible}");
            }
        }

        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
        {
            // Handle custom authentication
        }

        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
        {
            // Handle host migration
        }

        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
        {
            // Handle reliable data
        }

        public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
        {
            // Handle reliable data progress
        }

        public void OnSceneLoadDone(NetworkRunner runner)
        {
            // Scene loaded by the NetworkSceneManager
            // The normal SceneManager.sceneLoaded will handle UI setup
            ShowStatus("Scene loaded successfully");
        }

        public void OnSceneLoadStart(NetworkRunner runner)
        {
            ShowStatus("Loading scene...");
        }

        public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
        {
            // Handle object leaving Area of Interest
        }

        public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
        {
            // Handle object entering Area of Interest
        }

        public void OnConnectedToServer(NetworkRunner runner)
        {
            ShowStatus("Connected to server");
        }

        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
        {
            ShowStatus($"Connection failed: {reason}");
            // Return to menu scene
            SceneManager.LoadScene(_menuSceneIndex);
        }

        public void OnInput(NetworkRunner runner, NetworkInput input)
        {
            // Handle network input
        }

        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
        {
            // Handle missing input
        }

        public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
        {
            ShowStatus($"Disconnected from server: {reason}");

            // Return to menu scene
            if (SceneManager.GetActiveScene().buildIndex != _menuSceneIndex)
            {
                SceneManager.LoadScene(_menuSceneIndex);
            }
        }

        // Helper methods
        public bool IsPlayerInSpawnProtection(PlayerRef player)
        {
            if (_playerSpawnTimes.TryGetValue(player, out float spawnTime))
            {
                return Time.time - spawnTime < _spawnProtectionTime;
            }
            return false;
        }

        private void LogActivePlayers()
        {
            if (_runner == null || !_runner.IsRunning) return;

            Debug.Log($"Active Players: {_runner.ActivePlayers.Count()}");
            int i = 0;
            foreach (var p in _runner.ActivePlayers)
            {
                Debug.Log($"  Player[{i}]: {p}, PlayerId: {p.PlayerId}, IsLocal: {p == _runner.LocalPlayer}");
                i++;
            }
        }

        private void LogSessionProperties()
        {
            if (_runner == null || _runner.SessionInfo == null) return;

            Debug.Log("Session Properties:");
            foreach (var prop in _runner.SessionInfo.Properties)
            {
                Debug.Log($"  {prop.Key}: {prop.Value}");
            }
        }
    }
}