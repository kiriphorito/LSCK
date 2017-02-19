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
        private static int snippetCounter = 0, fileCounter = 0;
        public static void FindFiles(string key)
        {
            List<Tuple<string, string>> files = new List<Tuple<string, string>>();
            EnvDTE80.DTE2 dte2 = (EnvDTE80.DTE2)System.Runtime.InteropServices.Marshal.GetActiveObject("VisualStudio.DTE.14.0");
            string mainDir = System.IO.Path.GetDirectoryName(dte2.Solution.FullName);
            string[] extensions = { ".cs", ".c", ".java", ".py",".php",".html" };
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
            MessageBox.Show("" + snippetCounter + " new snippets were added from "+fileCounter+" files.");
            fileCounter = 0;
            snippetCounter = 0;
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
                    languageName = "csharp";
                    break;
                case ".java":
                    marker = "//";
                    languageName = "java";
                    break;
                case ".html":
                    marker = "//";
                    languageName = "html";
                    break;
                case ".php":
                    marker = "//";
                    languageName = "php";
                    break;
                default:
                    marker = "//";
                    languageName = "c";
                    break;
            }
            return new Tuple<string, string>(marker, languageName);
        }

        private static void FindSnippets(string file, string marker, string key,string languageName)
        {
            StringBuilder lines = new StringBuilder();
            StringBuilder code = new StringBuilder();
            FJController fjController = FJController.GetInstance;
            int keyCounter = 0;
            int totalCounter = 0;
            int startPos;
            string comment=null,sectionName=null;
            foreach (var line in File.ReadAllLines(file))
            {
                if (line.Contains(marker + key))
                {
                    startPos=line.IndexOf(marker + key);
                    keyCounter++;
                    totalCounter++;
                    if (keyCounter == 1)
                    {
                        sectionName = line.Substring(startPos+marker.Length + key.Length, line.Length - (startPos+marker.Length + key.Length));
                    }else if (keyCounter == 2)
                    {
                        comment = line.Substring(startPos+marker.Length + key.Length, line.Length - (startPos+marker.Length + key.Length));
                    }else
                    {
                        keyCounter = 0;
                        fjController.InsertSnippet(sectionName, languageName, comment, code.ToString());
                        snippetCounter++;
                    }
                }else
                {
                    lines.Append(line+"\n");
                    if (keyCounter == 2)
                    {
                        code.Append(line+"\n");
                    }
                }
            }
            if (totalCounter > 0) {
                fileCounter++;
                File.WriteAllText(file, lines.ToString());
            }

        }
    }
}
