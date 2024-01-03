using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ConsoleApp1;
using System.Text.Json;
using Microsoft.VisualBasic.CompilerServices;


/*tdl -f C:\tdl\export.json dl -d C:\tdl\output --template "{{ .FileName }}" -l 8 --pool 0 -t 32*/
class Program
{
    private static FileManager initialized;

    static async Task Main(string[] args)
    {
#if RELEASE
        string path = null;
        string regexPattern = null;
        bool isReady = false;
        for (int i = 0; i < args.Length; ++i) 
        {
            switch (args[i]) 
            {
                case "-p":
                    if (i + 1 < args.Length) path = args[++i];
                    break;
                
                case "-r":
                    if (i + 1 < args.Length) regexPattern = args[++i];
                    break; 
            }
        }

        Console.WriteLine(path);
        Console.WriteLine(regexPattern);

        var fm = new FileManager(path, (string.IsNullOrEmpty(regexPattern) ? ".*\\.unitypackage|.zip|.7z" : regexPattern)); 
        await fm.Initialize(); assetstore.unity.com
        UpdateUserInput(fm);
#endif

#if DEBUG

        var fm = new FileManager(@"H:\Tera\UnSync\TDL\tdl", ".*\\.unitypackage|.zip|.7z");
        await fm.Initialize(); 
        UpdateUserInput(fm);
#endif
    }


    static void UpdateUserInput(FileManager fm)
    {
        while (true)
        {
            try
            {
                Console.WriteLine("Input Command :" +
                                  "\n- Show All Searched File List (list) " +
                                  "\n- Search by Tag (tag), " +
                                  "\n- Search by Name (name) " +
                                  "\n- Uncompress .zip File  (unzip) " +
                                  "\n- Uncompress .zip File And Delete Original .zip File (unzip-allow-delete)");
                var inputOption = Console.ReadLine();
 
                if (inputOption == "list")
                {
                    fm.PrintFileSystemTree(fm.Root, "ㄴ");
                }
                else if (inputOption == "unzip" || inputOption == "unzip-allow-delete")
                {
                    var nodeList = fm.Where(node => node.NodeType == NodeType.File).ToList();
                    nodeList.Sort((x, y) => new FileInfo(x.FullPath).Length.CompareTo(new FileInfo(y.FullPath).Length));
                    foreach (var node in nodeList)
                    {
                        if (node.NodeType == NodeType.File)
                        {
                            Console.WriteLine("Extract =>" + node.FullPath);
                            fm.ExtreactFileFromZipWhenPatternMatched(node, shouldDeleteZip: (inputOption != "unzip"));
                        }
                    }
                }
                else if (inputOption == "tag")
                {
                    Console.WriteLine("Input Search Tag:");
                    var allTags = fm.GetAllTags(fm.Root);
                    Console.WriteLine("These are all the searchable tags:");
                    foreach (var tagged in allTags)
                    {
                        Console.Write(tagged + " | ");
                    }

                    var tag = Console.ReadLine();
                    List<string> taggedFiles = new List<string>();

                    fm.FindFilesByTag(fm.Root, tag, taggedFiles);
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine("Found files:");
                    Console.ResetColor();
                    foreach (var file in taggedFiles)
                    {
                        Console.WriteLine(file);
                    }
                }
                else if (inputOption == "name")
                {
                    Console.WriteLine("Input Search FileName:");
                    var fileName = Console.ReadLine();
                    List<string> matchingFiles = new List<string>();

                    fm.FindFilesByName(fm.Root, fileName, matchingFiles);
                    Console.ForegroundColor = ConsoleColor.Blue;


                    Console.WriteLine("Found files:");
                    Console.ResetColor();
                    foreach (var file in matchingFiles)
                    {
                        Console.WriteLine(file);
                    }
                }
                else
                {
                    Console.WriteLine("Invalid Command.");
                }
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error => " + e.Message);
                Console.WriteLine("Error => " + e.StackTrace);
                Console.ResetColor();
            }
        }
    }
}