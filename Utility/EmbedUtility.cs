using System.IO;
using System.Reflection;

namespace KitchenHQ.Utility
{
    public static class EmbedUtility
    {
        public static string ReadEmbeddedFile(string filename)
        {
            var fullname = "KitchenHQ." + filename;

            string result = "";
            using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(fullname);
            using (StreamReader reader = new StreamReader(stream))
            {
                try
                {
                    result = reader.ReadToEnd();
                } catch
                {
                    LogError($"Could not reach nor read file: {fullname}");
                }
            }
            return result;
        }
    }
}
