using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using Synty.AnimationBaseLocomotion.Samples;

public class FusionGameplayManager : MonoBehaviour, INetworkRunnerCallbacks
{
    public NetworkRunner RunnerPrefab;
    public NetworkPrefabRef PlayerPrefabRef;

    private NetworkRunner _runner;

    async void Start()
    {
        _runner = Instantiate(RunnerPrefab);
        _runner.AddCallbacks(this);

        var args = GetCommandLineArgs();
        var mode = ParseCommandLineMode(args);

        if (mode == GameMode.Host)
        {
            await StartGame(GameMode.Host);
        }
        else if (mode == GameMode.Client)
        {
            await StartGame(GameMode.Client);
        }
        else
        {
            Debug.LogWarning("No game mode specified in command line arguments, starting as Host by default.");
            await StartGame(GameMode.Host);
        }
    }

    async Task StartGame(GameMode mode)
    {
        var scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);
        var sceneLoadMode = mode == GameMode.Host ? LoadSceneMode.Single : LoadSceneMode.Additive;

        var startGameArgs = new StartGameArgs()
        {
            GameMode = mode,
            SessionName = "TestSession",
            Scene = scene,
        };

        var result = await _runner.StartGame(startGameArgs);

        if (result.Ok)
        {
            Debug.Log($"Game started as {mode}");
        }
        else
        {
            Debug.LogError($"Failed to start game: {result.ShutdownReason}");
        }
    }

    public void OnConnectedToServer(NetworkRunner runner) { Debug.Log("Connected to server"); }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { Debug.LogError($"Connect failed: {reason}"); }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnDisconnectedFromServer(NetworkRunner runner) { Debug.Log("Disconnected from server"); }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        // *** Thu thập input trực tiếp trong OnInput() ***
        var data = new SamplePlayerAnimationController.NetworkInputData();

        Vector2 moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        data.MoveDirection = moveInput;

        if (Input.GetKey(KeyCode.Space))
        {
            data.Buttons.Set(EInputButtons.Jump, true);
        }

        input.Set(data); // Set input data vào NetworkInput - Fusion sẽ tự động xử lý
    }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (runner.IsServer)
        {
            NetworkObject networkPlayerObject = runner.Spawn(PlayerPrefabRef, Vector3.zero, Quaternion.identity, player);
            Debug.Log($"Player {player} Joined, spawned player object {networkPlayerObject.name}");
        }
        else
        {
            Debug.Log($"Player {player} Joined (Client)");
        }
    }
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (runner.IsServer)
        {
            if (runner.TryGetPlayerObject(player, out NetworkObject playerObject))
            {
                runner.Despawn(playerObject);
                Debug.Log($"Player {player} Left, despawned player object {playerObject.name}");
            }
        }
        else
        {
            Debug.Log($"Player {player} Left (Client)");
        }
    }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner, SceneRef scene, LoadSceneMode loadMode) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { Debug.Log($"Shutdown: {shutdownReason}"); }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }

    private static Dictionary<string, string> GetCommandLineArgs()
    {
        Dictionary<string, string> args = new Dictionary<string, string>();
        string[] commandLineArgs = Environment.GetCommandLineArgs();

        for (int i = 0; i < commandLineArgs.Length; ++i)
        {
            if (commandLineArgs[i].StartsWith("-"))
            {
                string key = commandLineArgs[i].Substring(1);
                string value = i + 1 < commandLineArgs.Length ? commandLineArgs[i + 1] : null;
                args[key] = value;
            }
        }
        return args;
    }

    private static GameMode ParseCommandLineMode(Dictionary<string, string> args)
    {
        if (args.TryGetValue("mode", out string modeStr))
        {
            if (Enum.TryParse(modeStr, true, out GameMode mode))
            {
                return mode;
            }
            else
            {
                Debug.LogError($"Invalid game mode specified in command line arguments: {modeStr}");
            }
        }
        return GameMode.AutoHostOrClient;
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
        throw new NotImplementedException();
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {
        throw new NotImplementedException();
    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {
        throw new NotImplementedException();
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
        throw new NotImplementedException();
    }

    public enum EInputButtons : byte // Important: Use byte for NetworkButtons
    {
        Jump = 0,
        Sprint = 1,
        Crouch = 2,
        Aim = 3,
        LockOn = 4,
    }
}