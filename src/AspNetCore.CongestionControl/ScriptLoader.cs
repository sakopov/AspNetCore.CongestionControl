using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace AspNetCore.CongestionControl
{
    /// <summary>
    /// This class implements a helper method to load Lua scripts.
    /// </summary>
    public static class ScriptLoader
    {
        /// <summary>
        /// Loads Lua script from assembly resources.
        /// </summary>
        /// <returns>
        /// The Lua script body.
        /// </returns>
        public static async Task<string> GetScriptAsync(string scriptName)
        {
            var assembly = typeof(ScriptLoader).GetTypeInfo().Assembly;
            var resource = assembly.GetManifestResourceNames().SingleOrDefault(script => script.Contains(scriptName));

            if (string.IsNullOrEmpty(resource))
            {
                throw new InvalidOperationException($"The script \"{scriptName}\" does not exist.");
            }

            using (var stream = assembly.GetManifestResourceStream(resource))
            {
                using (var streamReader = new StreamReader(stream))
                {
                    return await streamReader.ReadToEndAsync();
                }
            }
        }
    }
}