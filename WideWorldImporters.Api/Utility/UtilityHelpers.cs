using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;

namespace WideWorldImporters.Api.Utility
{
    public static class UtilityHelpers
    {
        private static readonly Random _random = new Random();

        /// <summary>
        ///     Create a random alphanumeric string
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string RandomString(int length)
        {
            const string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(Chars, length)
                                        .Select(s => s[UtilityHelpers._random.Next(s.Length)]).ToArray());
        }

        /// <summary>
        ///     Create random unsigned integer for Entity Framework primark key
        /// </summary>
        /// <returns></returns>
        public static int RandomPK()
        {
            var r = new Random();
            return r.Next(0, int.MaxValue);
        }

        /// <summary>
        ///     Write a message to the output window
        /// </summary>
        /// <param name="message"></param>
        /// <param name="optionalTitle">optional title</param>
        public static void WriteDebugString(string message, string optionalTitle = "Debug Log:") { Debug.WriteLine($"{optionalTitle} {DateTime.Now} - {message}"); }

        /// <summary>
        ///     Write response contents to the output window
        /// </summary>
        /// <param name="response"></param>
        public static void WriteResponseContent(HttpResponseMessage response)
        {
            using (var content = response.Content)
            {
                string contentString = content.ReadAsStringAsync().Result;
                WriteDebugString(contentString, "Debug Log - Response Content:");
            }
        }

        /// <summary>
        ///     Path to current executing assembly - https://stackoverflow.com/questions/52797/how-do-i-get-the-path-of-the-assembly-the-code-is-in    
        /// </summary>
        public static string AssemblyDirectory
        {
            get
            {
                return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            }
        }
    }
}