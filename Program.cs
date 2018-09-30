using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using NUnitMerger.Versions;

namespace NUnitMerger
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            //How to:
            //NunitMerge.exe "filepath1" "filepath2" "filepath3" "outputPath"
            //Value
            //[0 = Nunit 3.10 (Default value)]
            //[1 = NUnit 2.5] 

            int nunitVersion = 0;
            int numOfFiles = 0;
            string output = "AllResults.xml";
            
            List<string> filesMergedNames = new List<string>();
            List<string> files = new List<string>();

            for (int i = 0; i < args.Length - 1; i++)
            {
                if (File.Exists(args[i]))
                {
                    files.Add(args[i]);
                    filesMergedNames.Add(args[i].Split('\\').Last());
                    numOfFiles++;
                }else
                {
                    output = args[args.Length - 1];
                }
            }
            

            
            if (nunitVersion == 0)
            {
                NUnit3.MergeFiles(files, output);
            }
            else if (nunitVersion == 1)
            {
                NUnit25.MergeFiles(files, output);
            }
            else
            {
                Console.WriteLine("Please select only NUnit2/NUnit3 files!");
            }


            StringBuilder sb = new StringBuilder();
            foreach (var file in filesMergedNames)
            {
                sb.Append(" " + file);
            }

            Console.WriteLine($"Merged {numOfFiles} files:", sb);
        }
    }
}