using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSCK
{
    public static class Bridge
    {
        public static EnvDTE80.DTE2 dte2;
        public static bool refresh = true;
        public static string solutionDir;
        public static string fileDir;
        public static FJController fjController;

        public static void SetDTE2()
        {
            dte2 = (EnvDTE80.DTE2)System.Runtime.InteropServices.Marshal.GetActiveObject("VisualStudio.DTE.14.0");
            solutionDir = Path.GetDirectoryName(dte2.Solution.FullName);
            fileDir = solutionDir + @"\LSCK Data";
            fjController =  FJController.GetInstance;
        }
        public static void CheckDir()
        {
            string newSolutionDir = Path.GetDirectoryName(dte2.Solution.FullName);
            if (solutionDir != newSolutionDir)
            {
                System.Windows.MessageBox.Show(newSolutionDir);
                solutionDir = newSolutionDir;
                fileDir = solutionDir + @"\LSCK Data";
                fjController = FJController.GetInstance;
            }
        }


    }
}
