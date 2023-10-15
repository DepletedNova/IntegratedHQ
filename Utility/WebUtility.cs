using KitchenLib.Utils;
using Steamworks.Ugc;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using UnityEngine;

namespace KitchenHQ.Utility
{
    public static class WebUtility
    {
        internal const int MaxCallWait = 250; // ms

        public static async Task<List<Item>> GetItemsFromQuery(Query query, int pageLimit = 999)
        {
            if (!PrefManager.Get<bool>("AllowAPI"))
            {
                LogDebug("[WEB] [STEAM] Skipping Item retrieval");
                return new();
            }

            LogDebug("[WEB] [STEAM] Getting Items from Query...");
            try
            {
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
            } catch
            {
                LogDebug($"[WEB] [STEAM] Failed to collect items from Query");
                return new();
            }
        }

        public static async Task<int> GetItemCountFromQuery(Query query)
        {
            if (!PrefManager.Get<bool>("AllowAPI"))
            {
                LogDebug("[WEB] [STEAM] Skipping Item count retrieval");
                return 0;
            }

            query.WithTotalOnly(true);

            LogDebug("[WEB] [STEAM] Getting count from Query...");
            try
            {
                ResultPage? result = await query.GetPageAsync(1);
                if (!result.HasValue)
                {
                    LogDebug("[WEB] [STEAM] No Items found in Query");
                    return 0;
                }
                LogDebug($"[WEB] [STEAM] Found a total of {result.Value.TotalCount} Items from Query");
                return result.Value.TotalCount;
            } catch
            {
                LogDebug($"[WEB] [STEAM] Failed to get count from Query");
                return 0;
            }
        }

        public static Texture2D GetItemIcon(Item item)
        {
            var embedded = EmbedUtility.ReadEmbeddedTextureFile("SteamDefault.png");
            if (item.PreviewImageUrl.IsNullOrEmpty())
                return embedded;

            var icon = GetIcon(item.Id.Value.ToString(), item.PreviewImageUrl);
            return icon == null ? embedded : icon;
        }

        public static Texture2D GetIcon(string tag, string url)
        {
            Texture2D tex = new(0, 0);
            if (!FileUtility.TryGetImage(tag, out tex))
            {
                LogDebug($"[WEB] [IMAGE] Retrieving icon from URL: \"{url}\"");
                using WebClient client = new();
                using Stream stream = Task.Run(() => client.OpenReadTaskAsync(url)).GetAwaiter().GetResult();

                if (stream == default || stream == null)
                {
                    LogError($"(EC:1) Failed to retrieve image from URL: \"{url}\"");
                    return null;
                }

                using Bitmap bitmap = new(stream);
                if (bitmap != null)
                {
                    using var memoryStream = new MemoryStream();

                    bitmap.Save(memoryStream, ImageFormat.Png);
                    var bytes = memoryStream.ToArray();
                    tex.LoadImage(bytes);
                    FileUtility.WriteImage(tag, tex);

                    LogDebug($"[WEB] [IMAGE] Retrieved and cached icon from URL: \"{url}\"");
                }
                else
                {

                    LogError($"(EC:2) Failed to retrieve image from URL: \"{url}\"");
                    return null;
                }
            }
            return tex;
        }

        public static T AwaitTask<T>(Task<T> task, int additionalWait = 0)
        {
            if (task.Wait(MaxCallWait + additionalWait))
            {
                return task.Result;
            }
            else
            {
                LogWarning($"Elapsed task exceeded time limit ({MaxCallWait + additionalWait}ms). Reverting to default.");
                return default;
            }
        }
    }
}
