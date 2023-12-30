using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ConsoleApp1;

class Program
{  
    static async Task Main(string[] args)
    {


        var fm = new FileManager(@"C:\Users\shlif\Desktop\Telegram\Down", ".*\\.unitypackage|zip|7z"); 
        await fm.Initialize();

  
        UserInput(fm);

    }

    static void UserInput(FileManager fm)
    {
        while (true)
        {
            Console.WriteLine("입력 옵션을 선택해 주십시오: 태그 검색 (1), 파일 이름 검색 (2)");
            var inputOption = Console.ReadLine();

            if (inputOption == "1")
            {
                Console.WriteLine("검색하려는 태그를 입력하세요:");
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
            else if (inputOption == "2")
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
    }
 
}