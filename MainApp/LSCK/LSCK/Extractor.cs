
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LSCK
{
    public static class Extractor
    {
        public static void FindFiles(string key)
        {
            List<Tuple<string, string>> files = new List<Tuple<string, string>>();
            EnvDTE80.DTE2 dte2 = (EnvDTE80.DTE2)System.Runtime.InteropServices.Marshal.GetActiveObject("VisualStudio.DTE.14.0");
            string mainDir = System.IO.Path.GetDirectoryName(dte2.Solution.FullName);
            string[] extensions = { ".cs", ".c", ".java", ".py" };
            string[] foundFiles = Directory.GetFiles(mainDir, "*.*", System.IO.SearchOption.AllDirectories);
            foreach (string foundFile in foundFiles)
            {
                if (extensions.Any(x => foundFile.EndsWith(x, StringComparison.Ordinal)))
                {
                    string extension = Path.GetExtension(foundFile);
                    Tuple<string,string> langProperties = getLanguageProperties(extension);
                    FindSnippets(foundFile,langProperties.Item1,key,langProperties.Item2);
                }
            }
        }

        private static Tuple<string,string> getLanguageProperties(string extension)
        {
            string marker, languageName;
            switch (extension)
            {
                case ".py":
                    marker = "**";
                    languageName = "python";
                    break;
                case ".cs":
                    marker = "//";
                    languageName = "C#";
                    break;
                case ".java":
                    marker = "//";
                    languageName = "java";
                    break;
                default:
                    marker = "//";
                    languageName = "C";
                    break;
            }
            return new Tuple<string, string>(marker, languageName);
        }
        private static void FindSnippets(string file, string marker, string key,string languageName)
        {
            StringBuilder code = new StringBuilder();
            FJController fjController = FJController.GetInstance;
            int keyCounter = 0;
            string comment=null,sectionName=null;
            foreach (var line in File.ReadAllLines(file))
            {
                if (line.Contains(marker + key))
                {
                    keyCounter++;
                    if (keyCounter == 1)
                    {
                        sectionName = line.Substring(marker.Length + key.Length, line.Length - (marker.Length + key.Length));
                        System.Windows.MessageBox.Show(sectionName);
                    }else if (keyCounter == 2)
                    {
                        comment = line.Substring(marker.Length + key.Length, line.Length - (marker.Length + key.Length));
                        System.Windows.MessageBox.Show(comment);
                    }else
                    {
                        keyCounter = 0;
                        fjController.InsertSnippet(sectionName, languageName, comment, code.ToString());
                    }
                }else
                {
                    code.Append(line);
                }
            }

        }
    }
}
