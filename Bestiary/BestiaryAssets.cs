using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Bestiary
{
    public static class BestiaryAssets
    {
        private static readonly HashSet<string> LoadedImages =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        private static readonly HashSet<string> FailedImages =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        private static readonly Dictionary<string, string> DiscoveredIcons =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        private static readonly Dictionary<string, string> DiscoveredImages =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        private static bool _assetFoldersScanned;

        public static bool TryGetCustomElement(
            string relativePathWithoutExtension,
            out string elementName)
        {
            elementName = null;

            if (string.IsNullOrWhiteSpace(relativePathWithoutExtension))
                return false;

            if (TryLoadElement(
                relativePathWithoutExtension,
                out elementName))
            {
                return true;
            }

            EnsureAssetFolderIndex();

            string normalized =
                relativePathWithoutExtension.Replace('\\', '/');

            int slash = normalized.LastIndexOf('/');

            string id =
                slash >= 0
                    ? normalized.Substring(slash + 1)
                    : normalized;

            if (string.IsNullOrWhiteSpace(id))
                return false;

            bool lookingForImage =
                normalized.IndexOf(
                    "images",
                    StringComparison.OrdinalIgnoreCase) >= 0;

            bool lookingForIcon =
                normalized.IndexOf(
                    "icons",
                    StringComparison.OrdinalIgnoreCase) >= 0;

            if (lookingForImage &&
                DiscoveredImages.TryGetValue(
                    id,
                    out string discoveredImage))
            {
                return TryLoadElement(
                    discoveredImage,
                    out elementName);
            }

            if (lookingForIcon &&
                DiscoveredIcons.TryGetValue(
                    id,
                    out string discoveredIcon))
            {
                return TryLoadElement(
                    discoveredIcon,
                    out elementName);
            }

            return false;
        }

        private static bool TryLoadElement(
            string relativePathWithoutExtension,
            out string elementName)
        {
            elementName = null;

            if (string.IsNullOrWhiteSpace(relativePathWithoutExtension))
                return false;

            string pngPath =
                AssetManager.ResolveFilePath(
                    relativePathWithoutExtension + ".png");

            if (!File.Exists(pngPath))
                return false;

            if (FailedImages.Contains(relativePathWithoutExtension))
                return false;

            try
            {
                if (!LoadedImages.Contains(relativePathWithoutExtension))
                {
                    if (!Futile.atlasManager.DoesContainAtlas(
                        relativePathWithoutExtension))
                    {
                        Futile.atlasManager.LoadImage(
                            relativePathWithoutExtension);
                    }

                    LoadedImages.Add(relativePathWithoutExtension);
                }

                if (Futile.atlasManager
                    ._allElementsByName
                    .ContainsKey(relativePathWithoutExtension))
                {
                    elementName = relativePathWithoutExtension;
                    return true;
                }

                Plugin.logger.LogWarning(
                    "[Bestiary] PNG loaded but atlas element was not found: " +
                    relativePathWithoutExtension);

                FailedImages.Add(relativePathWithoutExtension);

                return false;
            }
            catch (Exception ex)
            {
                Plugin.logger.LogWarning(
                    "[Bestiary] Failed to load PNG '" +
                    relativePathWithoutExtension +
                    ".png': " +
                    ex.Message);

                FailedImages.Add(relativePathWithoutExtension);

                return false;
            }
        }

        private static void EnsureAssetFolderIndex()
        {
            if (_assetFoldersScanned)
                return;

            _assetFoldersScanned = true;

            DiscoveredIcons.Clear();
            DiscoveredImages.Clear();

            string modRoot = GetBestiaryModRoot();

            if (string.IsNullOrEmpty(modRoot) ||
                !Directory.Exists(modRoot))
            {
                Plugin.logger.LogWarning(
                    "[Bestiary] Could not find Bestiary mod root " +
                    "for custom asset scanning.");

                return;
            }

            try
            {
                string[] directories =
                    Directory.GetDirectories(
                        modRoot,
                        "*",
                        SearchOption.AllDirectories);

                Array.Sort(
                    directories,
                    StringComparer.OrdinalIgnoreCase);

                foreach (string directory in directories)
                {
                    string directoryName =
                        Path.GetFileName(directory);

                    if (string.IsNullOrEmpty(directoryName))
                        continue;

                    bool isIconFolder =
                        directoryName.IndexOf(
                            "icons",
                            StringComparison.OrdinalIgnoreCase) >= 0;

                    bool isImageFolder =
                        directoryName.IndexOf(
                            "images",
                            StringComparison.OrdinalIgnoreCase) >= 0;

                    if (!isIconFolder && !isImageFolder)
                        continue;

                    string[] files =
                        Directory.GetFiles(
                            directory,
                            "*.png",
                            SearchOption.TopDirectoryOnly);

                    Array.Sort(
                        files,
                        StringComparer.OrdinalIgnoreCase);

                    foreach (string file in files)
                    {
                        string id =
                            Path.GetFileNameWithoutExtension(file);

                        if (string.IsNullOrEmpty(id))
                            continue;

                        string relativePath =
                            MakeRelativeAssetPath(
                                modRoot,
                                file);

                        if (relativePath.EndsWith(
                            ".png",
                            StringComparison.OrdinalIgnoreCase))
                        {
                            relativePath =
                                relativePath.Substring(
                                    0,
                                    relativePath.Length - 4);
                        }

                        if (isIconFolder)
                        {
                            if (!DiscoveredIcons.ContainsKey(id))
                            {
                                DiscoveredIcons.Add(
                                    id,
                                    relativePath);

                                Plugin.logger.LogInfo(
                                    $"[Bestiary] Custom icon: " +
                                    $"{id} -> {relativePath}");
                            }
                            else
                            {
                                Plugin.logger.LogWarning(
                                    $"[Bestiary] Duplicate custom icon " +
                                    $"for '{id}'. Keeping: " +
                                    $"{DiscoveredIcons[id]}");
                            }
                        }

                        if (isImageFolder)
                        {
                            if (!DiscoveredImages.ContainsKey(id))
                            {
                                DiscoveredImages.Add(
                                    id,
                                    relativePath);

                                Plugin.logger.LogInfo(
                                    $"[Bestiary] Custom image: " +
                                    $"{id} -> {relativePath}");
                            }
                            else
                            {
                                Plugin.logger.LogWarning(
                                    $"[Bestiary] Duplicate custom image " +
                                    $"for '{id}'. Keeping: " +
                                    $"{DiscoveredImages[id]}");
                            }
                        }
                    }
                }

                Plugin.logger.LogInfo(
                    $"[Bestiary] Custom asset scan finished. " +
                    $"Icons: {DiscoveredIcons.Count}, " +
                    $"images: {DiscoveredImages.Count}");
            }
            catch (Exception ex)
            {
                Plugin.logger.LogWarning(
                    "[Bestiary] Custom asset folder scan failed: " +
                    ex.Message);
            }
        }

        private static string GetBestiaryModRoot()
        {
            try
            {
                string assemblyPath =
                    Assembly.GetExecutingAssembly().Location;

                string pluginFolder =
                    Path.GetDirectoryName(assemblyPath);

                if (string.IsNullOrEmpty(pluginFolder))
                    return null;

                DirectoryInfo pluginDirectory =
                    new DirectoryInfo(pluginFolder);

                if (pluginDirectory.Name.Equals(
                    "plugins",
                    StringComparison.OrdinalIgnoreCase) &&
                    pluginDirectory.Parent != null)
                {
                    return pluginDirectory.Parent.FullName;
                }

                return pluginDirectory.FullName;
            }
            catch
            {
                return null;
            }
        }

        private static string MakeRelativeAssetPath(
            string root,
            string fullPath)
        {
            string normalizedRoot =
                Path.GetFullPath(root)
                    .TrimEnd(
                        Path.DirectorySeparatorChar,
                        Path.AltDirectorySeparatorChar)
                + Path.DirectorySeparatorChar;

            string normalizedFile =
                Path.GetFullPath(fullPath);

            if (!normalizedFile.StartsWith(
                normalizedRoot,
                StringComparison.OrdinalIgnoreCase))
            {
                return normalizedFile.Replace('\\', '/');
            }

            string relative =
                normalizedFile.Substring(
                    normalizedRoot.Length);

            return relative.Replace('\\', '/');
        }

        public static void RefreshAssetFolders()
        {
            _assetFoldersScanned = false;

            DiscoveredIcons.Clear();
            DiscoveredImages.Clear();
        }
    }
}