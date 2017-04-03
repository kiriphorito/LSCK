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
            string[] extensions = { ".cs", ".c", ".java", ".py",".php",".html",".cpp",".md",".markdown",".ts",".less",".sql",".js",".go" };
            string[] foundFiles = Directory.GetFiles(Bridge.solutionDir, "*.*", System.IO.SearchOption.AllDirectories);
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
            marker = "//";
            switch (extension)
            {
                case ".py":
                    marker = "#";
                    languageName = "python";
                    break;
                case ".cs":
                    languageName = "csharp";
                    break;
                case ".java":
                    languageName = "java";
                    break;
                case ".html":
                    languageName = "html";
                    break;
                case ".php":
                    languageName = "php";
                    break;
                case ".cpp":
                    languageName = "c_cpp";
                    break;
                case ".less":
                    languageName = "less";
                    break;
                case ".sql":
                    marker = "--";
                    languageName = "sql";
                    break;
                case ".md":
                case ".markdown":
                    languageName = "markdown";
                    break;
                case ".ts":
                    languageName = "typescript";
                    break;
                case ".js":
                    languageName = "javascript";
                    break;
                case ".go":
                    languageName = "golang";
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
            string comment = null,sectionName = null;
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
                        if (fjController.SectionExists(sectionName))
                        {
                            fjController.InsertSnippet(sectionName, languageName, comment, code.ToString());
                        }else
                        {
                            fjController.InsertSection(sectionName);
                            fjController.InsertSnippet(sectionName, languageName, comment, code.ToString());

                        }
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
