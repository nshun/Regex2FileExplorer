using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FolderTools
{
    class Program
    {
        static Dictionary<string, string> namesTable = new Dictionary<string, string>();

        static void Main(string[] args)
        {
            var filePath = @"C:\Users\shun_nishimura\Desktop\生物参考書_ファイル名指示 - 図版ファイル名.csv";
            var sourceDir = @"C:\Users\shun_nishimura\Documents\GitHub\n-content-manuscripts-2018-spring\subjects\BI";


            var reader = new StreamReader(filePath, Encoding.GetEncoding("Shift_JIS"));
            while (reader.Peek() >= 0)
            {
                string[] cols = reader.ReadLine().Split(',');
                namesTable.Add(cols[0], cols[1]);
            }
            reader.Close();
            // RegexMove(sourceDir, sourceDir, @"(BI_G1[0-9]{2})\\.*\.png", @".\$1.\");


            Console.ReadLine();
            var fromFiles = GetFileNameDir(sourceDir);
            foreach (var file in fromFiles)
            {
                if (Regex.IsMatch(file[0], @"(BI_G1[0-9]{2})\\(BI_G[0-9]{3}\.html)"))
                {
                    var toFile = Regex.Replace(file[1], @"(BI_G1[0-9]{2})\\(BI_G[0-9]{3}\.html)", @".\$1.\$2");
                    var sourcePath = new Uri(new Uri(sourceDir + @"\"), file[1]).LocalPath;
                    var destPath = new Uri(new Uri(sourceDir + @"\"), toFile).LocalPath;
                    ReplaceHTML(sourcePath);
                }
            }
        }

        private static List<string[]> GetFileNameDir(string path)
        {
            var filesNameDir = new List<string[]>();
            try
            {
                var files = Directory.GetFiles(path, "*", System.IO.SearchOption.AllDirectories);
                for (int i = 0; i < files.Count(); i++)
                {
                    var name = files[i].Replace(path + "\\", "");
                    filesNameDir.Add(new string[] { name, files[i] });
                }
            }
            catch
            {
            }
            return filesNameDir;
        }

        private static void RegexMove(string sourceDir, string destDir, string sourceReg, string destReg)
        {
            var fromFiles = GetFileNameDir(sourceDir);
            foreach (var file in fromFiles)
            {
                if (Regex.IsMatch(file[0], sourceReg))
                {
                    if (!namesTable.ContainsKey(Path.GetFileName(file[1])))
                    {
                        Console.WriteLine("Error at " + file[1]);
                        continue;
                    }

                    var newName = namesTable[Path.GetFileName(file[1])];
                    var toFile = Regex.Replace(file[1], sourceReg, destReg) + newName;
                    var sourcePath = new Uri(new Uri(sourceDir + @"\"), file[1]).LocalPath;
                    var destPath = new Uri(new Uri(destDir + @"\"), toFile).LocalPath;
                    Move(sourcePath, destPath);
                }
            }
        }

        private static void Move(string sourcePath, string destPath)
        {
            var parentDir = Path.GetDirectoryName(destPath);
            if (!Directory.Exists(parentDir))
                Directory.CreateDirectory(parentDir);
            File.Move(sourcePath, destPath);
        }

        private static void ReplaceHTML(string sourcePath)
        {
            StringBuilder strread = new StringBuilder();
            string[] strarray = File.ReadAllLines(sourcePath, Encoding.UTF8);
            for (int i = 0; i < strarray.GetLength(0); i++)
            {
                var written = false;
                foreach (var dic in namesTable)
                {
                    if (strarray[i].Contains(dic.Key))
                    {
                        strread.AppendLine(strarray[i].Replace(dic.Key, dic.Value));
                        written = true;
                        break;
                    }
                }
                if (!written) strread.AppendLine(strarray[i]);

            }
            File.WriteAllText(sourcePath, strread.ToString());
        }
    }
}
