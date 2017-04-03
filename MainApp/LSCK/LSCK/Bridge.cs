using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
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
        public static int state = 0;

        public static void SetDTE2()
        {
            try
            {
                dte2 = (EnvDTE80.DTE2)System.Runtime.InteropServices.Marshal.GetActiveObject("VisualStudio.DTE.14.0");
                solutionDir = Path.GetDirectoryName(dte2.Solution.FullName);
                fileDir = solutionDir + @"\LSCK Data";
                fjController = FJController.GetInstance;
            }
            catch (Exception ex){

            }
        }
        public static void CheckDir()
        {
            if (dte2 != null && solutionDir != null)
            {
                try
                {
                    string newSolutionDir = Path.GetDirectoryName(dte2.Solution.FullName);
                    if (solutionDir != newSolutionDir)
                    {
                        System.Windows.MessageBox.Show(newSolutionDir);
                        solutionDir = newSolutionDir;
                        fileDir = solutionDir + @"\LSCK Data";
                        fjController.Recreate();
                        state = 1;
                    }
                }
                catch
                {
                    solutionDir = null;
                    state = 2;
                }
            }else
            {
                try
                {
                    dte2 = (EnvDTE80.DTE2)System.Runtime.InteropServices.Marshal.GetActiveObject("VisualStudio.DTE.14.0");
                    solutionDir = Path.GetDirectoryName(dte2.Solution.FullName);
                    fileDir = solutionDir + @"\LSCK Data";
                    if (fjController == null)
                    {
                        fjController = FJController.GetInstance;
                        state = 1;
                    }
                    else
                    {
                        fjController.Recreate();
                        state = 1;
                    }
                }
                catch
                {

                }
            }

        }


    }
}
