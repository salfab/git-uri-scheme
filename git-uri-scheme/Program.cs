using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Web;
using Microsoft.Win32;

namespace git_uri_scheme
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(args.Length);
            foreach (var arg in args)
            {
                Console.WriteLine(arg);
            }

            if (args.Length > 0 && string.Equals(args[0], "-install", StringComparison.OrdinalIgnoreCase))
            {
                var fileName = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
                InstallScheme(fileName);
            }

            ParseGitCommand(args[0]);
            Console.WriteLine("Hello World!");
            Console.Read();

        }

        private static void ParseGitCommand(string commandlineArgument)
        {
            var uri = new Uri(commandlineArgument);
            var queryDictionary = System.Web.HttpUtility.ParseQueryString(uri.Query);
            var branch = uri.LocalPath;
            var localRepo = HttpUtility.UrlDecode(queryDictionary["localRepo"]);
        }

        private static void InstallScheme(string exefile)
        {
            RegistryKey key;
            key = Registry.ClassesRoot.CreateSubKey("git-uri");
            key.SetValue("", "URL: git-uri Protocol");
            key.SetValue("URL Protocol", "");

            key = key.CreateSubKey("shell");
            key = key.CreateSubKey("open");
            key = key.CreateSubKey("command");
            key.SetValue("", $"{exefile} \"%1\"");
        }
    }
}
