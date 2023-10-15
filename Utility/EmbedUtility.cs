using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace KitchenHQ.Utility
{
    public static class EmbedUtility
    {
        public static byte[] ReadEmbeddedBytes(string name) => ReadEmbedded(name, stream =>
        {
            using (MemoryStream memoryStream = new())
            {
                stream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        });

        public static string ReadEmbeddedTextFile(string name) => ReadEmbedded(name, stream =>
        {
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        });

        public static Texture2D ReadEmbeddedTextureFile(string name) => ReadEmbedded(name, stream =>
        {
            Texture2D tex = new(0, 0);
            using (MemoryStream memoryStream = new())
            {
                var bitmap = new Bitmap(stream);
                bitmap.Save(memoryStream, ImageFormat.Png);
                var bytes = memoryStream.ToArray();
                tex.LoadImage(bytes);
            }
            return tex;
        });

        private static Dictionary<string, object> CachedObjects = new();
        public static T ReadEmbedded<T>(string name, Func<Stream, T> func) where T : class
        {
            if (CachedObjects.ContainsKey(name))
                return CachedObjects[name] as T;
            var type = typeof(Main);
            var loc = $"{type.Namespace}.Embedded.{name}";
            T result = default;
            using (var stream = type.Assembly.GetManifestResourceStream(loc))
            {
                try
                {
                    result = func.Invoke(stream);
                    CachedObjects.Add(name, result);
                } catch
                {
                    LogError($"Could not reach file: {loc}");
                }
            }
            return result;
        }

        internal static void PrintEmbedResourceNames()
        {
            LogDebug("[EMBED] Printing resources...");
            var assembly = Assembly.GetExecutingAssembly();
            var names = assembly.GetManifestResourceNames();
            for (int i = 0; i < names.Length; i++)
            {
                var name = names[i];
                if (name == null || name == default)
                    continue;
                LogDebug(name);
            }
            LogDebug("[EMBED] All resources have been printed");
        }
    }
}
