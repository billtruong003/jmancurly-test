using UnityEngine;
using UnityEditor;

namespace NetworkGame
{
    [CustomEditor(typeof(GameplayManager))]
    public class GameplayManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            GameplayManager manager = (GameplayManager)target;

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Spawn Point Generation", EditorStyles.boldLabel);

            // Add button for generating spawn points
            if (GUILayout.Button("Generate Random Spawn Points", GUILayout.Height(30)))
            {
                manager.GenerateRandomSpawnPoints();
                EditorUtility.SetDirty(manager);
            }

            // Add button for clearing spawn points
            if (GUILayout.Button("Clear All Spawn Points", GUILayout.Height(30)))
            {
                // We can call the private method using reflection if needed
                // System.Reflection.MethodInfo method = typeof(GameplayManager).GetMethod("ClearExistingSpawnPoints", 
                //     System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                // method.Invoke(manager, null);

                // Or just rely on GenerateRandomSpawnPoints to clear first
                SerializedProperty parent = serializedObject.FindProperty("_spawnPointsParent");
                if (parent.objectReferenceValue != null)
                {
                    Transform parentTransform = (Transform)parent.objectReferenceValue;
                    int childCount = parentTransform.childCount;

                    // Delete all children from last to first
                    for (int i = childCount - 1; i >= 0; i--)
                    {
                        DestroyImmediate(parentTransform.GetChild(i).gameObject);
                    }

                    // Clear the spawn points array
                    SerializedProperty spawnPoints = serializedObject.FindProperty("_spawnPoints");
                    spawnPoints.arraySize = 0;

                    serializedObject.ApplyModifiedProperties();
                    EditorUtility.SetDirty(manager);
                }
            }

            // Show spawn point statistics
            int spawnPointCount = manager.GetSpawnPointCount();
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField($"Current Spawn Points: {spawnPointCount}", EditorStyles.boldLabel);

            // Preview settings
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Preview Settings", EditorStyles.boldLabel);

            SerializedProperty areaSize = serializedObject.FindProperty("_spawnAreaSize");
            SerializedProperty minDistance = serializedObject.FindProperty("_minDistanceBetweenPoints");

            EditorGUILayout.PropertyField(areaSize, new GUIContent("Spawn Area Size"));
            EditorGUILayout.PropertyField(minDistance, new GUIContent("Min Distance Between Points"));

            // Apply changes
            serializedObject.ApplyModifiedProperties();

            // Show preview info
            float density = (areaSize.floatValue * areaSize.floatValue) / (minDistance.floatValue * minDistance.floatValue * Mathf.PI);
            int theoreticalMax = Mathf.FloorToInt(density);

            EditorGUILayout.HelpBox(
                $"With current settings:\n" +
                $"- Area: {areaSize.floatValue}x{areaSize.floatValue} = {areaSize.floatValue * areaSize.floatValue}m²\n" +
                $"- Min Distance: {minDistance.floatValue}m\n" +
                $"- Theoretical Max Points: ~{theoreticalMax} points\n\n" +
                $"Adjust these values if you need more or fewer spawn points.",
                MessageType.Info
            );
        }

        private void OnSceneGUI()
        {
            GameplayManager manager = (GameplayManager)target;
            Transform managerTransform = manager.transform;

            // Get spawn area size
            SerializedProperty areaSize = serializedObject.FindProperty("_spawnAreaSize");
            float size = areaSize.floatValue;

            // Draw spawn area
            Handles.color = new Color(0.2f, 0.2f, 0.8f, 0.2f);
            Vector3 center = managerTransform.position;
            Vector3 cubeSize = new Vector3(size, 0.1f, size);

            // Draw wireframe cube for the spawn area
            Handles.DrawWireCube(center, cubeSize);

            // Label the area
            Handles.Label(center + Vector3.up * 2, "Spawn Area");
        }
    }
}