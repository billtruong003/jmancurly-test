using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class CharacterMeshExtractor : EditorWindow
{
    private string sourceFolder = "Assets/PolygonFantasyHeroCharacters/Models";
    private string targetFolder = "Assets/ExtractedMeshes";
    private List<string> processedFiles = new List<string>();

    [MenuItem("Tools/Character Customization/Extract Meshes")]
    public static void ShowWindow()
    {
        GetWindow<CharacterMeshExtractor>("Mesh Extractor");
    }

    private void OnGUI()
    {
        GUILayout.Label("Character Mesh Extractor", EditorStyles.boldLabel);

        sourceFolder = EditorGUILayout.TextField("Source Folder", sourceFolder);
        targetFolder = EditorGUILayout.TextField("Target Folder", targetFolder);

        if (GUILayout.Button("Extract Meshes"))
        {
            ExtractMeshes();
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

    private void ExtractMeshes()
    {
        processedFiles.Clear();

        // Tạo thư mục đích nếu chưa tồn tại
        if (!Directory.Exists(targetFolder))
        {
            Directory.CreateDirectory(targetFolder);
        }

        // Xử lý từng thư mục con
        ProcessDirectory("ModularCharacter_StaticParts");
        ProcessDirectory("Weapons");
        ProcessDirectory("FixedScale");

        AssetDatabase.Refresh();
    }

    private void ProcessDirectory(string subFolder)
    {
        string fullPath = Path.Combine(sourceFolder, subFolder);
        if (!Directory.Exists(fullPath))
            return;

        string[] files = Directory.GetFiles(fullPath, "*.fbx", SearchOption.TopDirectoryOnly);
        foreach (string file in files)
        {
            string fileName = Path.GetFileNameWithoutExtension(file);
            ExtractMeshFromFBX(file, subFolder, fileName);
            processedFiles.Add(fileName);
        }
    }

    private void ExtractMeshFromFBX(string sourcePath, string subFolder, string fileName)
    {
        // Load FBX file
        GameObject fbxObject = AssetDatabase.LoadAssetAtPath<GameObject>(sourcePath);
        if (fbxObject == null)
        {
            Debug.LogError($"Failed to load FBX: {sourcePath}");
            return;
        }

        // Tạo thư mục con nếu cần
        string targetSubFolder = Path.Combine(targetFolder, subFolder);
        if (!Directory.Exists(targetSubFolder))
        {
            Directory.CreateDirectory(targetSubFolder);
        }

        // Extract mesh từ FBX
        MeshFilter[] meshFilters = fbxObject.GetComponentsInChildren<MeshFilter>();
        SkinnedMeshRenderer[] skinnedMeshRenderers = fbxObject.GetComponentsInChildren<SkinnedMeshRenderer>();

        foreach (var meshFilter in meshFilters)
        {
            if (meshFilter.sharedMesh != null)
            {
                SaveMesh(meshFilter.sharedMesh, targetSubFolder, $"{fileName}_{meshFilter.name}");
            }
        }

        foreach (var skinnedMeshRenderer in skinnedMeshRenderers)
        {
            if (skinnedMeshRenderer.sharedMesh != null)
            {
                SaveMesh(skinnedMeshRenderer.sharedMesh, targetSubFolder, $"{fileName}_{skinnedMeshRenderer.name}");
            }
        }
    }

    private void SaveMesh(Mesh mesh, string folder, string meshName)
    {
        // Tạo mesh mới để tránh reference đến mesh gốc
        Mesh newMesh = new Mesh();
        newMesh.vertices = mesh.vertices;
        newMesh.triangles = mesh.triangles;
        newMesh.uv = mesh.uv;
        newMesh.normals = mesh.normals;
        newMesh.tangents = mesh.tangents;
        newMesh.boneWeights = mesh.boneWeights;
        newMesh.bindposes = mesh.bindposes;
        newMesh.subMeshCount = mesh.subMeshCount;

        // Copy submeshes
        for (int i = 0; i < mesh.subMeshCount; i++)
        {
            newMesh.SetSubMesh(i, mesh.GetSubMesh(i));
        }

        // Lưu mesh
        string assetPath = Path.Combine(folder, $"{meshName}.asset");
        AssetDatabase.CreateAsset(newMesh, assetPath);
        AssetDatabase.SaveAssets();
    }
}