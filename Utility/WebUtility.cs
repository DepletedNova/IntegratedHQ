using Steamworks.Ugc;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using UnityEngine;

namespace KitchenHQ.Utility
{
    internal static class WebUtility
    {
        public static async Task<List<Item>> GetItemsFromQueryAsync(Query query, int pageLimit = 999)
        {
            LogDebug("[WEB] [STEAM] Getting Items from Query...");
            List<Item> items = new();
            int page = 1;
            int total = 0;
            ResultPage value;
            do
            {
                ResultPage? result = await query.GetPageAsync(page);
                if (!result.HasValue)
                    break;

                value = result.Value;
                items.AddRange(value.Entries.Where(i => i.IsPublic));
                total += value.ResultCount;
                page++;
            } while (value.ResultCount != 0 && total < value.TotalCount && page < pageLimit);
            LogDebug($"[WEB] [STEAM] Collected {total} items from {page} pages");
            return items;
        }

        public static List<Item> GetItemsFromQuery(Query query, int pageLimit = 999) => Task.Run(() => GetItemsFromQueryAsync(query, pageLimit)).GetAwaiter().GetResult();

        public static async Task<int> GetItemCountFromQuery(Query query, int pageLimit = 999)
        {
            LogDebug("[WEB] [STEAM] Getting count from Query...");
            ResultPage? result = await query.GetPageAsync(1);
            if (!result.HasValue)
            {
                LogDebug("[WEB] [STEAM] No Items found in Query");
                return 0;
            }
            LogDebug($"[WEB] [STEAM] Found a total of {result.Value.TotalCount} Items from Query");
            return result.Value.TotalCount;
        }

        public static Texture2D GetItemIcon(Item item) => GetIcon(ImageType.Icon, item.Id.Value.ToString(), item.PreviewImageUrl);

        public static Texture2D GetIcon(ImageType type, string tag, string url)
        {
            string folder;
            switch(type)
            {
                case ImageType.Icon: folder = DevelopPath("Icons"); break;
                default: folder = DevelopPath("Images"); break;
            }

            var iconPath = Path.Combine(folder, tag + ".png");
            Texture2D tex = new Texture2D(0, 0);
            if (File.Exists(iconPath))
            {
                var imageBytes = File.ReadAllBytes(iconPath);
                tex.LoadImage(imageBytes);
                LogDebug($"[WEB] [IMAGE] Retrieved cached icon from file: \"{tag}.png\"");
            }
            else
            {
                LogDebug($"[WEB] [IMAGE] Retrieving icon from URL: \"{url}\"");
                using WebClient client = new();
                try
                {
                    using Stream stream = client.OpenRead(url);
                    using Bitmap bitmap = new(stream);
                    if (bitmap != null)
                    {
                        using var memoryStream = new MemoryStream();
                        
                        bitmap.Save(memoryStream, ImageFormat.Png);
                        var bytes = memoryStream.ToArray();
                        tex.LoadImage(bytes);

                        File.WriteAllBytes(iconPath, bytes);
                        LogDebug($"[WEB] [IMAGE] Retrieved and cached icon from URL: \"{url}\"");
                    }
                    else
                    {
                        LogDebug($"[WEB] [IMAGE] Failed to retrieve image from URL: \"{url}\"");
                        return null;
                    }
                }
                catch
                {
                    LogDebug($"[WEB] [IMAGE] Failed to retrieve image from URL: \"{url}\"");
                    return null;
                }
            }
            return tex;
        }

        public enum ImageType
        {
            Image,
            Icon,
        }

        private static string DevelopPath(string path)
        {
            var baseFolder = Path.Combine(Application.persistentDataPath, "IntegratedHQ");
            if (!Directory.Exists(baseFolder))
                Directory.CreateDirectory(baseFolder);

            var fullPath = Path.Combine(baseFolder, path);
            if (!Directory.Exists(fullPath))
                Directory.CreateDirectory(fullPath);

            return fullPath;
        }
    }
}
