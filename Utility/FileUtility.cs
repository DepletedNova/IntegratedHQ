using KitchenLib.Utils;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace KitchenHQ.Utility
{
    public static class FileUtility
    {
        public static bool TryGetImage(string name, out Texture2D tex)
        {
            var path = Develop(SubfolderType.Image, name);
            if (Cache.ContainsValue(path))
            {
                tex = (Texture2D)Cache[path];
                return true;
            }
            
            tex = new(0, 0);
            if (File.Exists(path))
            {
                Tracker.UpdateFile(path);
                var bytes = File.ReadAllBytes(path);
                tex.LoadImage(bytes);
                Cache.Add(path, tex);
                return true;
            } else return false;
        }

        public static void WriteImage(string name, Texture2D tex) => WriteImage(name, tex.EncodeToPNG());

        public static void WriteImage(string name, byte[] bytes)
        {
            var path = Develop(SubfolderType.Image, name);
            if (!File.Exists(path))
            {
                Tracker.UpdateFile(path);
                File.WriteAllBytes(path, bytes);
            }
            else LogWarning("File already exists: " + path);
        }

        private static Dictionary<string, object> Cache = new();
        private static FileTracker Tracker;
        internal static void InitFiles()
        {
            // JSON file tracker
            var baseFolder = RetrieveFolder();
            var trackerLoc = Path.Combine(baseFolder, "Tracker.json");
            if (!File.Exists(trackerLoc))
            {
                // Create Tracker
                Tracker = new();
                File.WriteAllText(trackerLoc, JsonConvert.SerializeObject(Tracker));
            } else Tracker = JsonConvert.DeserializeObject<FileTracker>(File.ReadAllText(trackerLoc));
        }

        internal static string RetrieveFolder(string path = "")
        {
            path = "KitchenHQ/" + path;
            var folders = path.Split('/');
            var fullPath = Application.persistentDataPath;
            for (int i = 0; i < folders.Length; i++)
            {
                if (folders[i].IsNullOrEmpty())
                    continue;

                fullPath = Path.Combine(fullPath, folders[i]);
                if (!Directory.Exists(fullPath))
                {
                    Directory.CreateDirectory(fullPath);
                }
            }
            return fullPath;
        }

        private static string Develop(SubfolderType subfolder, string filename)
        {
            var folder = RetrieveFolder(subfolder switch
            {
                SubfolderType.Image => "Images",
                SubfolderType.Json => "Json",
                _ => ""
            });
            var filePath = Path.Combine(folder, subfolder switch
            {
                SubfolderType.Image => filename + ".png",
                SubfolderType.Json => filename + ".json",
                _ => filename
            });
            return filePath;
        }

        private enum SubfolderType
        {
            Image,
            Json
        }
    }

    internal class FileTracker
    {
        public void UpdateTrackerFile()
        {
            var path = Path.Combine(FileUtility.RetrieveFolder(), "Tracker.json");
            File.WriteAllText(path, JsonConvert.SerializeObject(this));
        }

        public void UpdateFile(string path)
        {
            if (!UnusedFiles.Contains(path))
            {
                if (!Files.Contains(path))
                    Files.Add(path);
                UpdateTrackerFile();
                return;
            }
            UnusedFiles.Remove(path);
            Files.Add(path);
            UpdateTrackerFile();
        }

        public IList<string> Files;
        public IList<string> UnusedFiles;

        [JsonConstructor]
        public FileTracker(string[] Files, string[] UnusedFiles)
        {
            this.Files = new List<string>();
            this.UnusedFiles = new List<string>(Files);

            // Destroy unused files
            foreach (var file in UnusedFiles)
            {
                File.Delete(file);
            }
        }

        public FileTracker()
        {
            Files = new List<string>();
            UnusedFiles = new List<string>();
        }
    }
}
