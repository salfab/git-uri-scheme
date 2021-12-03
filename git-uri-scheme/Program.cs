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

            ParseGitCommand(new Uri(args[0]));
        }

        private static void ParseGitCommand(Uri uri)
        {
            switch (uri.Authority)
            {
                case "checkout":
                    PerformCheckout(uri);
                    break;
            }
        }

        private static void PerformCheckout(Uri uri)
        {
            var queryDictionary = System.Web.HttpUtility.ParseQueryString(uri.Query);
            var branch = uri.LocalPath.TrimStart('/');
            var localRepo = HttpUtility.UrlDecode(queryDictionary["localRepo"]);
            var processStartInfo = new ProcessStartInfo("git.exe", $"checkout {branch}");
            processStartInfo.WorkingDirectory = localRepo;
            processStartInfo.RedirectStandardOutput = true;
            var process = System.Diagnostics.Process.Start(processStartInfo);
            while (!process.StandardOutput.EndOfStream)
            {
                string line = process.StandardOutput.ReadLine();
                Console.WriteLine(line);
                // do something with line
            }
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
