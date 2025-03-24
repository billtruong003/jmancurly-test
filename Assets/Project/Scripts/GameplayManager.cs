using UnityEngine;
using System.Collections.Generic;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace NetworkGame
{
    public class GameplayManager : MonoBehaviour
    {
        public static GameplayManager Instance { get; private set; }

        [Header("Spawn Settings")]
        [SerializeField] private Transform[] _spawnPoints;

        [Header("Debug")]
        [SerializeField] private bool _showSpawnGizmos = true;
        [SerializeField] private float _gizmoSize = 1f;
        [SerializeField] private Color _gizmoColor = Color.green;

        [Header("References")]
        [SerializeField] private UIManager _uiManager;

        [Header("Auto Spawn Points Generation")]
        [SerializeField] private int _numberOfSpawnPoints = 8;
        [SerializeField] private float _spawnAreaSize = 5f;
        [SerializeField] private float _minDistanceBetweenPoints = 1.5f;
        [SerializeField] private GameObject _spawnPointPrefab;
        [SerializeField] private Transform _spawnPointsParent;

        private void Awake()
        {
            // Singleton pattern
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        public UIManager GetUIManager()
        {
            return _uiManager;
        }

        public void SetUIManager(UIManager uiManager)
        {
            _uiManager = uiManager;
            Debug.Log("GameplayManager received UIManager reference");
        }

        public Vector3 GetRandomSpawnPosition()
        {
            if (_spawnPoints == null || _spawnPoints.Length == 0)
            {
                Debug.LogWarning("No spawn points defined!");
                return Vector3.zero;
            }

            return _spawnPoints[Random.Range(0, _spawnPoints.Length)].position;
        }

        public Vector3 GetSpawnPosition(int index)
        {
            if (_spawnPoints == null || _spawnPoints.Length == 0)
            {
                Debug.LogWarning("No spawn points configured! Returning Vector3.zero");
                return Vector3.zero;
            }

            if (index < 0 || index >= _spawnPoints.Length)
            {
                Debug.LogWarning($"Spawn point index {index} is out of range! Returning random spawn point.");
                return GetRandomSpawnPosition();
            }

            return _spawnPoints[index].position;
        }

        public int GetSpawnPointCount()
        {
            return _spawnPoints != null ? _spawnPoints.Length : 0;
        }

#if ODIN_INSPECTOR
        [Button("Generate Random Spawn Points")]
#endif
        public void GenerateRandomSpawnPoints()
        {
            // Clear existing spawn points if any
            ClearExistingSpawnPoints();

            // Create parent if needed
            if (_spawnPointsParent == null)
            {
                GameObject parent = new GameObject("SpawnPoints");
                _spawnPointsParent = parent.transform;
                _spawnPointsParent.SetParent(transform);
                _spawnPointsParent.localPosition = Vector3.zero;
            }

            // Create prefab if needed
            if (_spawnPointPrefab == null)
            {
                GameObject prefab = new GameObject("SpawnPoint");
                prefab.AddComponent<SpawnPointVisualizer>();
                _spawnPointPrefab = prefab;
            }

            // Generate points
            List<Vector3> generatedPositions = new List<Vector3>();
            int maxAttempts = _numberOfSpawnPoints * 10; // Avoid infinite loop
            int attempts = 0;

            while (generatedPositions.Count < _numberOfSpawnPoints && attempts < maxAttempts)
            {
                attempts++;

                // Generate random position within area
                float halfSize = _spawnAreaSize / 2f;
                Vector3 randomPosition = new Vector3(
                    Random.Range(-halfSize, halfSize),
                    0, // Y is always 0 for ground level
                    Random.Range(-halfSize, halfSize)
                );

                // Add transform's position to make it relative to the manager
                randomPosition += transform.position;

                // Check if point is too close to existing points
                bool isTooClose = false;
                foreach (Vector3 existingPosition in generatedPositions)
                {
                    if (Vector3.Distance(randomPosition, existingPosition) < _minDistanceBetweenPoints)
                    {
                        isTooClose = true;
                        break;
                    }
                }

                // If valid position, create spawn point
                if (!isTooClose)
                {
                    generatedPositions.Add(randomPosition);
                    CreateSpawnPoint(randomPosition, generatedPositions.Count);
                }
            }

            // Update the spawn points array
            RefreshSpawnPointsArray();

            Debug.Log($"Generated {generatedPositions.Count} spawn points. Attempts: {attempts}");
        }

        private void CreateSpawnPoint(Vector3 position, int index)
        {
            GameObject spawnPoint;
            if (_spawnPointPrefab != null)
            {
                spawnPoint = Instantiate(_spawnPointPrefab, position, Quaternion.identity, _spawnPointsParent);
            }
            else
            {
                spawnPoint = new GameObject();
                spawnPoint.transform.position = position;
                spawnPoint.transform.SetParent(_spawnPointsParent);
            }

            spawnPoint.name = $"SpawnPoint_{index}";
        }

        private void ClearExistingSpawnPoints()
        {
            if (_spawnPointsParent != null)
            {
                // Remove all children
                while (_spawnPointsParent.childCount > 0)
                {
                    DestroyImmediate(_spawnPointsParent.GetChild(0).gameObject);
                }
            }
        }

        private void RefreshSpawnPointsArray()
        {
            if (_spawnPointsParent != null)
            {
                _spawnPoints = new Transform[_spawnPointsParent.childCount];
                for (int i = 0; i < _spawnPointsParent.childCount; i++)
                {
                    _spawnPoints[i] = _spawnPointsParent.GetChild(i);
                }
            }
        }

        // Draw gizmos for spawn points in the editor
        private void OnDrawGizmos()
        {
            if (!_showSpawnGizmos || _spawnPoints == null)
                return;

            Gizmos.color = _gizmoColor;
            foreach (var spawnPoint in _spawnPoints)
            {
                if (spawnPoint != null)
                {
                    Gizmos.DrawSphere(spawnPoint.position, _gizmoSize);
                    Gizmos.DrawWireCube(spawnPoint.position, Vector3.one * _gizmoSize * 1.5f);
                }
            }
        }
    }

    // Helper class to visualize spawn points in scene
    public class SpawnPointVisualizer : MonoBehaviour
    {
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(transform.position, 0.3f);
            Gizmos.DrawLine(transform.position, transform.position + Vector3.up * 2f);
        }
    }
}