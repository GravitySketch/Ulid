﻿using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class PackageExporter
{
    [MenuItem("Tools/Export Unitypackage")]
    public static void Export()
    {
        var version = Environment.GetEnvironmentVariable("UNITY_PACKAGE_VERSION");

        // configure
        var root = "Scripts/Ulid";
        var fileName = string.IsNullOrEmpty(version) ? "Ulid.Unity.unitypackage" : $"Ulid.Unity.{version}.unitypackage";
        var exportPath = "./" + fileName;

        var path = Path.Combine(Application.dataPath, root);
        var assets = Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories)
            .Where(x => Path.GetExtension(x) == ".cs" || Path.GetExtension(x) == ".meta" || Path.GetExtension(x) == ".asmdef")
            .Where(x => Path.GetFileNameWithoutExtension(x) != "_InternalVisibleTo")
            .Select(x => "Assets" + x.Replace(Application.dataPath, "").Replace(@"\", "/"))
            .ToArray();

        var netStandardsAsset = Directory.EnumerateFiles(Path.Combine(Application.dataPath, "Plugins/"), "*", SearchOption.AllDirectories)
            .Select(x => "Assets" + x.Replace(Application.dataPath, "").Replace(@"\", "/"))
            .ToArray();

        assets = assets.Concat(netStandardsAsset).ToArray();

        UnityEngine.Debug.Log("Export below files" + Environment.NewLine + string.Join(Environment.NewLine, assets));

        var dir = new FileInfo(exportPath).Directory;
        if (!dir.Exists) dir.Create();
        AssetDatabase.ExportPackage(
            assets,
            exportPath,
            ExportPackageOptions.Default);

        UnityEngine.Debug.Log("Export complete: " + Path.GetFullPath(exportPath));
    }
}