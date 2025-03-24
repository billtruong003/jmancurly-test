using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace NetworkGame
{
    public class UIManager : MonoBehaviour
    {
        #region UI Panels
        [Header("Panels")]
        [SerializeField] private GameObject _mainMenuPanel;
        [SerializeField] private GameObject _hostPanel;
        [SerializeField] private GameObject _joinPanel;
        [SerializeField] private GameObject _gamePanel;
        [SerializeField] private GameObject _settingsPanel;
        #endregion

        #region Main Menu Elements
        [Header("Main Menu")]
        [SerializeField] private Button _hostOptionButton;
        [SerializeField] private Button _joinOptionButton;
        #endregion

        #region Host Panel Elements
        [Header("Host Panel")]
        [SerializeField] private TMP_InputField _hostRoomNameInput;
        [SerializeField] private Button _startHostButton;
        [SerializeField] private Button _hostBackButton;
        [SerializeField] private TMP_Text _generatedRoomIdText; // To display the generated room ID after hosting
        [SerializeField] private Button _copyRoomIdButton; // New button to copy room ID
        #endregion

        #region Join Panel Elements
        [Header("Join Panel")]
        [SerializeField] private TMP_InputField _joinRoomIdInput; // Field for room ID
        [SerializeField] private Button _joinGameButton;
        [SerializeField] private Button _joinBackButton;
        #endregion

        #region Game Panel Elements
        [Header("Game Panel")]
        [SerializeField] private Button _settingsButton; // Nút 3 chấm hoặc cài đặt
        [SerializeField] private Button _copyGameRoomIdButton; // New button to copy room ID during gameplay
        [SerializeField] private TMP_Text _playerCountText; // Hiển thị số lượng người chơi
        #endregion

        #region Settings Panel Elements
        [Header("Settings Panel")]
        [SerializeField] private Button _disconnectButton;
        [SerializeField] private Button _closeSettingsButton;
        [SerializeField] private TMP_Text _settingsRoomNameText;
        [SerializeField] private TMP_Text _settingsRoomIdText; // New field for room ID
        [SerializeField] private Button _copySettingsRoomIdButton; // New button to copy room ID from settings

        [Header("Game Stats")]
        [SerializeField] private TMP_Text _roomNameText;
        [SerializeField] private TMP_Text _roomIdText; // New field for room ID
        [SerializeField] private TMP_Text _pingText;
        [SerializeField] private TMP_Text _fpsText;
        [SerializeField] private TMP_Text _networkStatsText;
        #endregion

        [Header("Shared Elements")]
        [SerializeField] private TMP_Text _statusText;

        [Header("References")]
        [SerializeField] private NetworkRunnerManager _networkManager;

        // For FPS calculation
        private float _deltaTime = 0;
        private float _updateInterval = 0.5f;
        private float _nextUpdateTime = 0;

        private void Awake()
        {
            // Try to register with GameplayManager asynchronously
            StartCoroutine(RegisterWithGameplayManager());

            // Find NetworkManager if not set
            if (_networkManager == null)
            {
                _networkManager = FindObjectOfType<NetworkRunnerManager>();
                if (_networkManager == null)
                {
                    Debug.LogError("NetworkRunnerManager not found!");
                }
            }
        }

        private IEnumerator RegisterWithGameplayManager()
        {
            // Wait at least one frame
            yield return null;

            float timeout = 3f;
            float elapsed = 0f;

            // Check if we're in a scene with GameplayManager
            while (GameplayManager.Instance == null && elapsed < timeout)
            {
                elapsed += Time.deltaTime;
                yield return null;
            }

            // If GameplayManager exists, register with it
            if (GameplayManager.Instance != null)
            {
                Debug.Log("UIManager registering with GameplayManager");
                GameplayManager.Instance.SetUIManager(this);
            }
        }

        private void Start()
        {
            // Setup button listeners
            if (_hostOptionButton != null) _hostOptionButton.onClick.AddListener(ShowHostPanel);
            if (_joinOptionButton != null) _joinOptionButton.onClick.AddListener(ShowJoinPanel);

            if (_hostBackButton != null) _hostBackButton.onClick.AddListener(ShowMainMenu);
            if (_joinBackButton != null) _joinBackButton.onClick.AddListener(ShowMainMenu);

            if (_startHostButton != null) _startHostButton.onClick.AddListener(OnHostButtonClicked);
            if (_joinGameButton != null) _joinGameButton.onClick.AddListener(OnJoinButtonClicked);
            if (_disconnectButton != null) _disconnectButton.onClick.AddListener(OnDisconnectButtonClicked);

            // Settings panel buttons
            if (_settingsButton != null) _settingsButton.onClick.AddListener(ShowSettingsPanel);
            if (_closeSettingsButton != null) _closeSettingsButton.onClick.AddListener(HideSettingsPanel);

            // Copy Room ID buttons
            if (_copyRoomIdButton != null) _copyRoomIdButton.onClick.AddListener(() => CopyRoomIdToClipboard());
            if (_copyGameRoomIdButton != null) _copyGameRoomIdButton.onClick.AddListener(() => CopyRoomIdToClipboard());
            if (_copySettingsRoomIdButton != null) _copySettingsRoomIdButton.onClick.AddListener(() => CopyRoomIdToClipboard());

            // Make sure settings panel is hidden initially
            if (_settingsPanel != null) _settingsPanel.SetActive(false);

            // Determine which panel to show initially based on available panels
            DetermineInitialPanel();
        }

        private void Update()
        {
            // Handle ESC key to toggle settings panel
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ToggleSettingsPanel();
            }

            if (_gamePanel != null && _gamePanel.activeSelf)
            {
                // Calculate FPS
                _deltaTime += (Time.unscaledDeltaTime - _deltaTime) * 0.1f;

                // Update stats periodically to avoid performance impact
                if (Time.time > _nextUpdateTime)
                {
                    UpdateGameplayStats();
                    _nextUpdateTime = Time.time + _updateInterval;
                }
            }
        }

        private void UpdateGameplayStats()
        {
            if (_networkManager == null) return;

            // Update Room Name
            if (_roomNameText != null)
            {
                string roomName = _networkManager.GetCurrentRoomName();
                _roomNameText.text = string.IsNullOrEmpty(roomName) ? "Not Connected" : roomName;
            }

            // Update Room ID
            if (_roomIdText != null)
            {
                string roomId = _networkManager.GetCurrentRoomID();
                _roomIdText.text = string.IsNullOrEmpty(roomId) ? "" : $"ID: {roomId}";
            }

            // Update Settings Room Name
            if (_settingsRoomNameText != null)
            {
                string roomName = _networkManager.GetCurrentRoomName();
                _settingsRoomNameText.text = string.IsNullOrEmpty(roomName) ? "Not Connected" : roomName;
            }

            // Update Settings Room ID
            if (_settingsRoomIdText != null)
            {
                string roomId = _networkManager.GetCurrentRoomID();
                _settingsRoomIdText.text = string.IsNullOrEmpty(roomId) ? "" : $"ID: {roomId}";
            }

            // Update Player Count
            if (_playerCountText != null)
            {
                int playerCount = _networkManager.GetPlayerCount();
                int maxPlayers = _networkManager.GetMaxPlayers();
                _playerCountText.text = $"Players: {playerCount}/{maxPlayers}";
            }

            // Update Ping
            if (_pingText != null)
            {
                int ping = _networkManager.GetPing();
                _pingText.text = $"Ping: {ping} ms";
            }

            // Update FPS
            if (_fpsText != null)
            {
                float fps = 1.0f / _deltaTime;
                _fpsText.text = $"FPS: {Mathf.Round(fps)}";
            }

            // Update Network Stats
            if (_networkStatsText != null && _networkManager.IsConnected())
            {
                string stats = _networkManager.GetNetworkStats();
                _networkStatsText.text = stats;
            }
        }

        private void ToggleSettingsPanel()
        {
            if (_settingsPanel != null)
            {
                bool isActive = _settingsPanel.activeSelf;
                _settingsPanel.SetActive(!isActive);

                // Update stats immediately when opening settings
                if (!isActive)
                {
                    UpdateGameplayStats();
                }
            }
        }

        private void ShowSettingsPanel()
        {
            if (_settingsPanel != null)
            {
                _settingsPanel.SetActive(true);
                UpdateGameplayStats();
            }
        }

        private void HideSettingsPanel()
        {
            if (_settingsPanel != null)
            {
                _settingsPanel.SetActive(false);
            }
        }

        private void DetermineInitialPanel()
        {
            // Show main menu initially if in MenuScene
            if (_mainMenuPanel != null)
            {
                ShowMainMenu();
            }
            // Show game panel initially if in GameplayScene
            else if (_gamePanel != null)
            {
                ShowGamePanel();
            }
        }

        #region Panel Management
        public void ShowMainMenu()
        {
            if (_mainMenuPanel != null) _mainMenuPanel.SetActive(true);
            if (_hostPanel != null) _hostPanel.SetActive(false);
            if (_joinPanel != null) _joinPanel.SetActive(false);
            if (_gamePanel != null) _gamePanel.SetActive(false);
            if (_settingsPanel != null) _settingsPanel.SetActive(false);

            // Clear status text
            SetStatusText("");
        }

        public void ShowHostPanel()
        {
            if (_mainMenuPanel != null) _mainMenuPanel.SetActive(false);
            if (_hostPanel != null) _hostPanel.SetActive(true);
            if (_joinPanel != null) _joinPanel.SetActive(false);
            if (_gamePanel != null) _gamePanel.SetActive(false);
            if (_settingsPanel != null) _settingsPanel.SetActive(false);

            // Clear input field
            if (_hostRoomNameInput != null) _hostRoomNameInput.text = "";
            if (_generatedRoomIdText != null) _generatedRoomIdText.text = "";
            SetStatusText("");
        }

        public void ShowJoinPanel()
        {
            if (_mainMenuPanel != null) _mainMenuPanel.SetActive(false);
            if (_hostPanel != null) _hostPanel.SetActive(false);
            if (_joinPanel != null) _joinPanel.SetActive(true);
            if (_gamePanel != null) _gamePanel.SetActive(false);
            if (_settingsPanel != null) _settingsPanel.SetActive(false);

            // Clear input field
            if (_joinRoomIdInput != null) _joinRoomIdInput.text = "";

            // Auto focus on room ID field
            if (_joinRoomIdInput != null) _joinRoomIdInput.Select();

            SetStatusText("");
        }

        public void ShowGamePanel()
        {
            if (_mainMenuPanel != null) _mainMenuPanel.SetActive(false);
            if (_hostPanel != null) _hostPanel.SetActive(false);
            if (_joinPanel != null) _joinPanel.SetActive(false);
            if (_gamePanel != null) _gamePanel.SetActive(true);
            if (_settingsPanel != null) _settingsPanel.SetActive(false);

            // Update stats immediately
            UpdateGameplayStats();
        }
        #endregion

        #region Button Handlers
        private void OnHostButtonClicked()
        {
            if (_hostRoomNameInput == null || string.IsNullOrEmpty(_hostRoomNameInput.text))
            {
                SetStatusText("Please enter a room name");
                return;
            }

            if (_networkManager != null)
            {
                _networkManager.StartHost(_hostRoomNameInput.text);
                SetStatusText("Starting host...");
            }
        }

        private void OnJoinButtonClicked()
        {
            // Chỉ sử dụng Room ID
            if (_joinRoomIdInput == null || string.IsNullOrEmpty(_joinRoomIdInput.text))
            {
                SetStatusText("Please enter a room ID");
                return;
            }

            string roomId = _joinRoomIdInput.text.ToUpper(); // Chuyển thành chữ hoa

            if (_networkManager != null)
            {
                _networkManager.JoinGame(roomId);
                SetStatusText("Searching for room...");
            }
        }

        private void OnDisconnectButtonClicked()
        {
            if (_networkManager != null)
            {
                HideSettingsPanel();
                _networkManager.Disconnect();
                SetStatusText("Disconnected from game");
                // Let the NetworkRunnerManager handle the scene change
            }
        }

        private void CopyRoomIdToClipboard()
        {
            if (_networkManager != null)
            {
                string roomId = _networkManager.GetCurrentRoomID();
                if (!string.IsNullOrEmpty(roomId))
                {
                    GUIUtility.systemCopyBuffer = roomId;
                    StartCoroutine(ShowCopyNotification());
                }
            }
        }

        private IEnumerator ShowCopyNotification()
        {
            string originalText = _statusText.text;
            SetStatusText("Room ID copied to clipboard!");
            yield return new WaitForSeconds(1.5f);
            SetStatusText(originalText);
        }
        #endregion

        public void SetStatusText(string message)
        {
            if (_statusText != null)
            {
                _statusText.text = message;
            }
        }

        // Called from NetworkRunnerManager to display room ID after hosting
        public void SetRoomIdText(string roomId)
        {
            if (_generatedRoomIdText != null)
            {
                _generatedRoomIdText.text = $"Room ID: {roomId}\nShare this with friends to join!";
            }

            // Enable copy button if we have a room ID
            if (_copyRoomIdButton != null)
            {
                _copyRoomIdButton.gameObject.SetActive(!string.IsNullOrEmpty(roomId));
            }
        }

        /// <summary>
        /// Cập nhật số lượng người chơi hiện tại và tối đa trong giao diện
        /// </summary>
        public void UpdatePlayerCount(int currentPlayers, int maxPlayers)
        {
            if (_playerCountText != null)
            {
                _playerCountText.text = $"Players: {currentPlayers}/{maxPlayers}";
            }
        }

        public void SetRoomNameText(string roomName)
        {
            if (_roomNameText != null)
            {
                _roomNameText.text = roomName;
            }
        }
    }
}