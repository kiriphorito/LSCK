using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                    string marker;
                    switch (extension)
                    {
                        case ".py":
                            marker = "**";
                            break;
                        default:
                            marker = "//";
                            break;
                    }
                    FindSnippets(foundFile,marker,key);
                }
            }
        }
        //??Section
        //??Comment
        public static void FindSnippets(string file,string comment,string key)
        {
            FJController fjController = FJController.GetInstance;
            int keyCounter = 0;
            string marker;
            foreach (var line in File.ReadAllLines(file))
            {
                if (line.Contains(comment + key))
                {
                    keyCounter++;
                    if (keyCounter == 2)
                    {
                        marker = line.Substring(comment.Length + key.Length - 1, line.Length - (comment.Length + key.Length));
                        System.Windows.MessageBox.Show(marker);
                        keyCounter = 0;
                    }
                }
            }

        }
    }
}
