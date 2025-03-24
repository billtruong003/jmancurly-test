using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using CharacterCustomization;

public class CharacterPartDataGenerator : EditorWindow
{
    private string meshFolder = "Assets/ExtractedMeshes";
    private string scriptableObjectFolder = "Assets/ScriptableObjects/CharacterParts";
    private List<string> processedFiles = new List<string>();

    [MenuItem("Tools/Character Customization/Generate Part Data")]
    public static void ShowWindow()
    {
        GetWindow<CharacterPartDataGenerator>("Part Data Generator");
    }

    private void OnGUI()
    {
        GUILayout.Label("Character Part Data Generator", EditorStyles.boldLabel);

        meshFolder = EditorGUILayout.TextField("Mesh Folder", meshFolder);
        scriptableObjectFolder = EditorGUILayout.TextField("ScriptableObject Folder", scriptableObjectFolder);

        if (GUILayout.Button("Generate Part Data"))
        {
            GeneratePartData();
        }

        if (processedFiles.Count > 0)
        {
            EditorGUILayout.LabelField("Processed Files:", EditorStyles.boldLabel);
            foreach (var file in processedFiles)
            {
                EditorGUILayout.LabelField(file);
            }
        }
    }

    private void GeneratePartData()
    {
        processedFiles.Clear();

        // Tạo thư mục ScriptableObject nếu chưa tồn tại
        if (!Directory.Exists(scriptableObjectFolder))
        {
            Directory.CreateDirectory(scriptableObjectFolder);
        }

        // Xử lý từng thư mục con
        ProcessDirectory("ModularCharacter_StaticParts");
        ProcessDirectory("Weapons");
        ProcessDirectory("FixedScale");

        AssetDatabase.Refresh();
    }

    private void ProcessDirectory(string subFolder)
    {
        string fullPath = Path.Combine(meshFolder, subFolder);
        if (!Directory.Exists(fullPath))
            return;

        string[] files = Directory.GetFiles(fullPath, "*.fbx", SearchOption.TopDirectoryOnly);
        foreach (string file in files)
        {
            string fileName = Path.GetFileNameWithoutExtension(file);
            CreatePartData(fileName, subFolder);
            processedFiles.Add(fileName);
        }
    }

    private void CreatePartData(string fileName, string subFolder)
    {
        // Tạo ScriptableObject mới
        CharacterPartData partData = ScriptableObject.CreateInstance<CharacterPartData>();

        // Phân tích tên file để xác định loại part
        string[] parts = fileName.Split('_');
        if (parts.Length >= 2)
        {
            partData.partID = fileName;
            partData.partName = fileName;

            // Xác định loại part từ tên file
            if (fileName.Contains("Head"))
                partData.partType = CharacterPartType.Head;
            else if (fileName.Contains("Torso"))
                partData.partType = CharacterPartType.Torso;
            // ... thêm các loại khác

            // Xác định giới tính
            partData.genderType = fileName.Contains("Male") ? GenderType.Male : GenderType.Female;

            // Load prefab
            string prefabPath = Path.Combine(meshFolder, subFolder, fileName + ".fbx");
            partData.partPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

            // Tạo material với shader tùy chỉnh
            Material material = new Material(Shader.Find("Shader Graphs/POLYGON_CustomCharacters"));

            // Convert Texture2D to Sprite for partIcon
            Texture2D previewTexture = AssetPreview.GetAssetPreview(partData.partPrefab);
            if (previewTexture != null)
            {
                partData.partIcon = Sprite.Create(
                    previewTexture,
                    new Rect(0, 0, previewTexture.width, previewTexture.height),
                    new Vector2(0.5f, 0.5f)
                );
            }

            // Lưu ScriptableObject
            string assetPath = Path.Combine(scriptableObjectFolder, fileName + "_Data.asset");
            AssetDatabase.CreateAsset(partData, assetPath);
            AssetDatabase.SaveAssets();
        }
    }
}