using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
public static class WeaponIconGenerator
{
    private const int IconResolution = 512;
    private const double PreviewTimeoutSeconds = 15.0;
    private class IconJob
    {
        public Object itemAsset;
        public GameObject model;
        public string itemName;
        public bool isGun;
        public bool isGrenade;
    }

    private static readonly Queue<IconJob> itemsToProcess = new Queue<IconJob>();
    private static IconJob currentJob;
    private static double currentItemStartTime;
    private static bool isGenerating;
    [MenuItem("Assets/Generate Weapon / Grenade Icons", false, 2000)] private static void GenerateSelectedIcons()
    {
        if (isGenerating)
        {
            Debug.LogWarning("Icon generator is already running.");
            return;
        }

        Object[] selectedObjects = Selection.objects;
        List<IconJob> jobs = new List<IconJob>();
        foreach (Object selectedObject in selectedObjects)
        {
            GunStats gun = selectedObject as GunStats;
            if (gun != null)
            {
                if (gun.gunModel == null)
                {
                    Debug.LogWarning(gun.itemName + " has no Gun Model assigned.");
                    continue;
                }

                IconJob gunJob = new IconJob();
                gunJob.itemAsset = gun;
                gunJob.model = gun.gunModel;
                gunJob.itemName = gun.itemName;
                gunJob.isGun = true;
                gunJob.isGrenade = false;
                jobs.Add(gunJob);
                continue;
            }

            GrenadeItemStats grenade = selectedObject as GrenadeItemStats;
            if (grenade != null)
            {
                GameObject grenadeModel = grenade.grenadeModel;
                if (grenadeModel == null && grenade.grenadePrefab != null)
                {
                    grenadeModel = grenade.grenadePrefab;
                }

                if (grenadeModel == null)
                {
                    Debug.LogWarning(grenade.itemName + " has no Grenade Model or Grenade Prefab assigned.");
                    continue;
                }

                IconJob grenadeJob = new IconJob();
                grenadeJob.itemAsset = grenade;
                grenadeJob.model = grenadeModel;
                grenadeJob.itemName = grenade.itemName;
                grenadeJob.isGun = false;
                grenadeJob.isGrenade = true;
                jobs.Add(grenadeJob);
            }
        }

        if (jobs.Count == 0)
        {
            Debug.LogWarning("Select one or more GunStats or GrenadeItemStats assets.");
            return;
        }

        itemsToProcess.Clear();
        foreach (IconJob job in jobs)
        {
            itemsToProcess.Enqueue(job);
        }

        AssetPreview.SetPreviewTextureCacheSize(Mathf.Max(64, jobs.Count * 4));
        currentJob = null;
        isGenerating = true;
        EditorApplication.update -= ProcessGeneration;
        EditorApplication.update += ProcessGeneration;
        Debug.Log("Icon Generator: Starting " + jobs.Count + " item(s).");
    }

    private static void ProcessGeneration()
    {
        if (currentJob == null)
        {
            if (itemsToProcess.Count == 0)
            {
                FinishGeneration();
                return;
            }

            currentJob = itemsToProcess.Dequeue();
            if (currentJob == null || currentJob.model == null)
            {
                currentJob = null;
                return;
            }

            currentItemStartTime = EditorApplication.timeSinceStartup;
            AssetPreview.GetAssetPreview(currentJob.model);
            return;
        }

        Texture2D preview = AssetPreview.GetAssetPreview(currentJob.model);
        double elapsed = EditorApplication.timeSinceStartup - currentItemStartTime;
        if (preview == null)
        {
            if (elapsed < PreviewTimeoutSeconds)
            {
                return;
            }

            Debug.LogError("Icon Generator: Unity could not create a preview for " + currentJob.itemName);
            currentJob = null;
            return;
        }

        SavePreviewAsIcon(currentJob, preview);
        currentJob = null;
    }

    private static void SavePreviewAsIcon(IconJob job, Texture2D preview)
    {
        if (job == null || job.itemAsset == null || preview == null)
        {
            return;
        }

        RenderTexture renderTexture = null;
        Texture2D outputTexture = null;
        RenderTexture previousActive = RenderTexture.active;
        try
        {
            renderTexture = RenderTexture.GetTemporary(IconResolution, IconResolution, 0, RenderTextureFormat.ARGB32);
            renderTexture.filterMode = FilterMode.Bilinear;
            Graphics.Blit(preview, renderTexture);
            RenderTexture.active = renderTexture;
            outputTexture = new Texture2D(IconResolution, IconResolution, TextureFormat.RGBA32, false);
            outputTexture.ReadPixels(new Rect(0, 0, IconResolution, IconResolution), 0, 0, false);
            outputTexture.Apply(false, false);
            byte[] pngData = outputTexture.EncodeToPNG();
            if (pngData == null || pngData.Length == 0)
            {
                Debug.LogError("Icon Generator: Failed to encode " + job.itemName);
                return;
            }

            string itemAssetPath = AssetDatabase.GetAssetPath(job.itemAsset);
            string directory = Path.GetDirectoryName(itemAssetPath);
            if (string.IsNullOrEmpty(directory))
            {
                directory = "Assets";
            }

            directory = directory.Replace("\\", "/");
            string folderName;
            if (job.isGrenade)
            {
                folderName = "GrenadeIcons";
            }
            else
            {
                folderName = "WeaponIcons";
            }

            string iconFolder = directory + "/" + folderName;
            if (!AssetDatabase.IsValidFolder(iconFolder))
            {
                AssetDatabase.CreateFolder(directory, folderName);
            }

            string safeName = MakeSafeFileName(job.itemAsset.name);
            string iconPath = iconFolder + "/" + safeName + "_Icon.png";
            File.WriteAllBytes(Path.GetFullPath(iconPath), pngData);
            AssetDatabase.ImportAsset(iconPath, ImportAssetOptions.ForceUpdate);
            TextureImporter importer = AssetImporter.GetAtPath(iconPath) as TextureImporter;
            if (importer == null)
            {
                Debug.LogError("Icon Generator: Could not import " + iconPath);
                return;
            }

            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.alphaSource = TextureImporterAlphaSource.FromInput;
            importer.alphaIsTransparency = true;
            importer.mipmapEnabled = false;
            importer.wrapMode = TextureWrapMode.Clamp;
            importer.filterMode = FilterMode.Bilinear;
            importer.maxTextureSize = IconResolution;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.SaveAndReimport();
            Sprite generatedSprite = AssetDatabase.LoadAssetAtPath<Sprite>(iconPath);
            if (generatedSprite == null)
            {
                Debug.LogError("Icon Generator: Could not load generated Sprite for " + job.itemName);
                return;
            }

            if (job.isGun)
            {
                GunStats gun = job.itemAsset as GunStats;
                if (gun != null)
                {
                    Undo.RecordObject(gun, "Assign Weapon Icon");
                    gun.weaponIcon = generatedSprite;
                    EditorUtility.SetDirty(gun);
                    Debug.Log("Generated weapon icon: " + gun.itemName, gun);
                }
            }

            if (job.isGrenade)
            {
                GrenadeItemStats grenade = job.itemAsset as GrenadeItemStats;
                if (grenade != null)
                {
                    Undo.RecordObject(grenade, "Assign Grenade Icon");
                    grenade.grenadeIcon = generatedSprite;
                    EditorUtility.SetDirty(grenade);
                    Debug.Log("Generated grenade icon: " + grenade.itemName, grenade);
                }
            }

            AssetDatabase.SaveAssets();
        }

        catch (System.Exception exception)
        {
            Debug.LogError("Icon Generator failed for " + job.itemName + ":\n" + exception);
        }

        finally
        {
            RenderTexture.active = previousActive;
            if (renderTexture != null)
            {
                RenderTexture.ReleaseTemporary(renderTexture);
            }

            if (outputTexture != null)
            {
                Object.DestroyImmediate(outputTexture);
            }
        }
    }

    private static void FinishGeneration()
    {
        EditorApplication.update -= ProcessGeneration;
        isGenerating = false;
        currentJob = null;
        itemsToProcess.Clear();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Icon Generator: Finished.");
    }

    private static string MakeSafeFileName(string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
        {
            return "Item";
        }

        foreach (char invalidCharacter in Path.GetInvalidFileNameChars())
        {
            fileName = fileName.Replace(invalidCharacter, '_');
        }

        return fileName;
    }
}
