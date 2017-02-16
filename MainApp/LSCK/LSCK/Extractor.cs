using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSCK
{
    /*
    public static class Extractor
    {
        public static List<Tuple<string,string>> FindFiles(string key)
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
                    string comment;
                    switch (extension)
                    {
                        case ".py":
                            comment = "**";
                            break;
                        default:
                            comment = "//";
                            break;
                    }
                    /*files.Add(new Tuple<string, string>());
                    autoFind(s, comment+key);
                }
            }
        }

    }*/
}
