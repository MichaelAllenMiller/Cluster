using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace Cluster {
    class Program {
        static void Main(string[] args) {
            String rarPath = GetRarPath();
            if (String.IsNullOrEmpty(rarPath)) { Console.WriteLine("Unable to locate WinRar."); return; }

            String folder = args.Length > 0 ? args[0] : @"";
            String pattern = args.Length > 1 && !String.IsNullOrEmpty(args[1]) ? args[1] : @".*?\d\d\d\d-\d\d-\d\d ";
            Boolean deleteAfter = args.Length > 2 ? args[2] == "-d" : false;
            Console.WriteLine("Searching '{0}' for files matching pattern of '{1}'.", folder, pattern);
            if (deleteAfter) { Console.WriteLine("Files will be deleted after archiving."); }

            if (!String.IsNullOrEmpty(folder) && System.IO.Directory.Exists(folder)) {
                String[] directoryListing = System.IO.Directory.GetFiles(folder, "*", System.IO.SearchOption.TopDirectoryOnly);
                var matches = (from String f in directoryListing where Regex.IsMatch(f, pattern, RegexOptions.IgnoreCase) group f by Regex.Match(f, pattern, RegexOptions.IgnoreCase).Value into g select new { Group = g.Key.Trim(), Files = g.ToList() });
                foreach (var match in matches) {
                    if (!String.IsNullOrEmpty(match.Group)) {
                        System.Text.StringBuilder files = new System.Text.StringBuilder();
                        foreach (String file in match.Files) { files.Append("\"" + file + "\" "); }
                        ProcessStartInfo startInfo = new ProcessStartInfo(GetRarPath(), String.Format("a -m5 -ep1 {2} \"{0}.rar\" {1}", System.IO.Path.Combine(folder, match.Group), files.ToString(), deleteAfter ? "-df" : "")) { CreateNoWindow = true, WindowStyle = ProcessWindowStyle.Hidden, UseShellExecute = false, RedirectStandardOutput = true };
                        Console.WriteLine("{0} ({1} files)", match.Group, match.Files.Count);
                        using (Process process = Process.Start(startInfo)) {
                            Console.Write(process.StandardOutput.ReadToEnd());
                        }
                        Console.WriteLine();
                    } else {
                        Console.WriteLine("An empty group was found.");
                    }
                }
            } else {
                Console.WriteLine("You must specify a valid folder.");
                return;

            }
        }

        private static String GetRarPath() {
            if (System.IO.File.Exists(@"c:\Program Files\WinRAR\Rar.exe")) {
                return @"c:\Program Files\WinRAR\Rar.exe";
            } else if (System.IO.File.Exists(@"c:\Program Files (x86)\WinRAR\Rar.exe")) {
                return @"c:\Program Files (x86)\WinRAR\Rar.exe";
            }
            return "";
        }
    }
}

