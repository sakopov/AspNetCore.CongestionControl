// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResourceLoader.cs">
//   Copyright (c) 2018-2021 Sergey Akopov
//
//   Permission is hereby granted, free of charge, to any person obtaining a copy
//   of this software and associated documentation files (the "Software"), to deal
//   in the Software without restriction, including without limitation the rights
//   to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//   copies of the Software, and to permit persons to whom the Software is
//   furnished to do so, subject to the following conditions:
//
//   The above copyright notice and this permission notice shall be included in
//   all copies or substantial portions of the Software.
//
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//   FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//   AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//   LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//   OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//   THE SOFTWARE.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AspNetCore.CongestionControl
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    /// <summary>
    /// The helper class to load assembly resources.
    /// </summary>
    internal static class ResourceLoader
    {
        /// <summary>
        /// Loads resource from the assembly.
        /// </summary>
        /// <param name="path">
        /// The path to the resource in the assembly.
        /// </param>
        /// <returns>
        /// The resource body.
        /// </returns>
        public static async Task<string> GetResourceAsync(string path)
        {
            var assembly = typeof(ResourceLoader).GetTypeInfo().Assembly;
            var resource = assembly.GetManifestResourceNames().SingleOrDefault(rsc => rsc.Contains(path));

            if (string.IsNullOrEmpty(resource))
            {
                throw new InvalidOperationException($"The resource \"{path}\" does not exist.");
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
