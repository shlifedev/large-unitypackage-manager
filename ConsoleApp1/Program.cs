using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ConsoleApp1;
using Microsoft.VisualBasic.CompilerServices;

class Program
{
    private static FileManager initialized;
    static async Task Main(string[] args)
    {
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



        if (path != null && regexPattern != null)
        {
            var fm = new FileManager(@"C:\Users\shlif\Desktop\Telegram\Down\Test", ".*\\.unitypackage|zip|7z"); 
            await fm.Initialize(); 
            UpdateUserInput(fm);
        }
        else
        {
            var fm = new FileManager(@"C:\tdl", ".*\\.unitypackage|.zip|.7z"); 
            await fm.Initialize();
            foreach (var node in fm)
            {
                Console.WriteLine(node.FullPath);
                Console.WriteLine(node.Version);
                fm.FixFileName(node);
            }
            UpdateUserInput(fm);
        }
 
  

    }

    
    static void UpdateUserInput(FileManager fm)
    {
        while (true)
        {
            try
            {
                Console.WriteLine("입력 옵션을 선택해 주십시오:" +
                                  "\n- 모든 파일 출력 (list) " +
                                  "\n- 태그 검색 (tag), " +
                                  "\n- 파일 이름 검색 (name) " +
                                  "\n- .zip 파일 압축해제 (unzip) " +
                                  "\n- .zip 파일 압축해제후 원본삭제 (unzip-allow-delete)");
                var inputOption = Console.ReadLine();


                if (inputOption == "list")
                { 
                    fm.PrintFileSystemTree(fm.root, "ㄴ");
                }
                else if (inputOption == "unzip" || inputOption =="unzip-allow-delete")
                {
                    foreach (var node in fm)
                    {
                        if (node.NodeType == NodeType.File)
                        {
                            fm.ExtreactFileFromZipWhenPatternMatched(node, shouldDeleteZip: (inputOption != "unzip") );
                        }
                    }
                }
                else if (inputOption == "tag")
                {
                    Console.WriteLine("검색하려는 태그를 입력하세요:");
                    var allTags = fm.GetAllTags(fm.root);
                    Console.WriteLine("검색 가능한 모든 태그입니다.:");
                    foreach (var tagged in allTags) 
                    {
                        Console.Write(tagged+" | ");
                    }
                    
                    var tag = Console.ReadLine();
                    List<string> taggedFiles = new List<string>();

                    fm.FindFilesByTag(fm.root, tag, taggedFiles);
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
                    Console.WriteLine("검색하려는 파일명을 입력하세요:");
                    var fileName = Console.ReadLine();
                    List<string> matchingFiles = new List<string>();

                    fm.FindFilesByName(fm.root, fileName, matchingFiles);
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
                    Console.WriteLine("선택 사항이 잘못되었습니다.");
                }
            }
            catch(Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("작업중 오류 발생 => " + e.Message);
                Console.ResetColor();
                
            }
        }
    }
 
}