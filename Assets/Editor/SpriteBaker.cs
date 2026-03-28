using UnityEngine;
using UnityEditor;
using System.IO;

public class SpriteBaker : MonoBehaviour
{
    [MenuItem("Tools/Bake Selected to PNG")]
    public static void BakeToPNG()
    {
        GameObject selectedObj = Selection.activeGameObject;
        if (selectedObj == null)
        {
            EditorUtility.DisplayDialog("Error", "Please click on a GameObject in the hierarchy first!", "OK");
            return;
        }

        // 1. Create a temporary clone at exactly 0,0,0 so we know where it is
        GameObject clone = Instantiate(selectedObj);
        clone.transform.position = Vector3.zero;

        SpriteRenderer[] renderers = clone.GetComponentsInChildren<SpriteRenderer>();
        if (renderers.Length == 0)
        {
            DestroyImmediate(clone);
            EditorUtility.DisplayDialog("Error", "No SpriteRenderers found in this object.", "OK");
            return;
        }

        Bounds bounds = renderers[0].bounds;
        foreach (SpriteRenderer sr in renderers)
        {
            bounds.Encapsulate(sr.bounds);
        }

        float ppu = renderers[0].sprite != null ? renderers[0].sprite.pixelsPerUnit : 100f;
        int width = Mathf.RoundToInt(bounds.size.x * ppu);
        int height = Mathf.RoundToInt(bounds.size.y * ppu);

        if (width <= 0 || height <= 0)
        {
            DestroyImmediate(clone);
            EditorUtility.DisplayDialog("Error", "Calculated size is too small.", "OK");
            return;
        }

        // 2. Setup the Camera
        GameObject camObj = new GameObject("TempBakerCamera");
        Camera cam = camObj.AddComponent<Camera>();
        cam.cameraType = CameraType.Preview;
        cam.orthographic = true;
        cam.orthographicSize = bounds.size.y / 2f;
        
        // FIX: Force the camera aspect ratio to match the sprite dimensions perfectly!
        cam.aspect = (float)width / height; 
        
        cam.transform.position = new Vector3(bounds.center.x, bounds.center.y, -10f); 
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0, 0, 0, 0); // Transparent

        // 3. Render and save
        RenderTexture rt = new RenderTexture(width, height, 24);
        cam.targetTexture = rt;
        cam.Render();

        RenderTexture.active = rt;
        Texture2D finalTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        finalTexture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        finalTexture.Apply();

        // 4. Clean up everything we spawned
        RenderTexture.active = null;
        cam.targetTexture = null;
        DestroyImmediate(camObj);
        DestroyImmediate(rt);
        DestroyImmediate(clone); // Destroy the clone

        // 5. Save the file
        byte[] bytes = finalTexture.EncodeToPNG();
        string path = EditorUtility.SaveFilePanelInProject("Save Baked Sprite", selectedObj.name, "png", "Save the assembled sprite");
        
        if (!string.IsNullOrEmpty(path))
        {
            File.WriteAllBytes(path, bytes);
            AssetDatabase.Refresh(); 
            
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spritePixelsPerUnit = ppu;
                importer.filterMode = FilterMode.Point; 
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                importer.SaveAndReimport();
            }
            Debug.Log("<b>Success!</b> Sprite assembled and saved to: " + path);
        }
    }
}