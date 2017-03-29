// <copyright file="LSCKWindowControl.xaml.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>

namespace LSCK
{
    using EnvDTE;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Shell.Interop;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Forms;

    /// <summary>
    /// Interaction logic for LSCKWindowControl.
    /// </summary>
    public partial class LSCKWindowControl : System.Windows.Controls.UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LSCKWindowControl"/> class.
        /// </summary>
        FJController fjController;
        EnvDTE80.DTE2 dte2;
        String solutionDir;
        String currentSection;

        public LSCKWindowControl()
        {
            dte2 = (EnvDTE80.DTE2)System.Runtime.InteropServices.Marshal.GetActiveObject("VisualStudio.DTE.14.0");
            solutionDir = Path.GetDirectoryName(dte2.Solution.FullName);
            this.InitializeComponent();
            fjController = FJController.GetInstance;
            updateUI(0);
            if (comboSectionsCode.HasItems)
            {
                comboSectionsCode.SelectedIndex = 0;
                comboSectionsFile.SelectedIndex = 0;
            }
                
        }

        /// <summary>
        /// Handles click on the button by displaying a message box.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event args.</param>
        [SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions", Justification = "Sample code")]
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Default event handler naming pattern")]

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            var selection = (TextSelection)dte2.ActiveDocument.Selection;
            string comment = textBox.Text;
            string code = selection.Text;
            string language = getConvertedLang();
            fjController.InsertSnippet(comboSectionsCode.SelectedValue.ToString(),language,comment,code);
        }

        private string getConvertedLang()
        {
            ListBoxItem selectedItem = comboLang.SelectedItem as ListBoxItem;
            string langName = langName = selectedItem.Content.ToString();
            switch (langName)
            {
                case "Java":
                    return "java";
                case "C#":
                    return "csharp";
                case "C":
                case "C++":
                    return "c_cpp";
                case "PHP":
                    return "php";
                case "Python":
                    return "python";
                case "Javascript":
                    return "javascript";
                default:
                    throw new InvalidInputException("Language Not Supported");
            }
        }

        public class InvalidInputException : Exception
        {
            public InvalidInputException(string message)
                : base(message) { }
        }



        private void createSection_Click(object sender, RoutedEventArgs e)
        {
            string sectionName = Prompt.ShowDialog("Section name:", "Create Section");
            fjController.InsertSection(sectionName);
            updateUI(0);
            
        }

        private void updateUI(int tab)
        {
            switch (tab) {
                case 0:
                    List<string> sectionNames = fjController.GetSectionNames();
                    comboSectionsCode.Items.Clear();
                    comboSectionsFile.Items.Clear();
                    foreach (string section in sectionNames)
                    {
                        comboSectionsFile.Items.Add(section);
                        comboSectionsCode.Items.Add(section);
                    }
                    projectTitle.Text = fjController.GetTitle();
                    comboTheme.SelectedIndex = fjController.GetAceThemeIndex();
                    comboTheme.SelectionChanged += comboTheme_SelectionChanged;
                    List<string> pageNames = fjController.GetPageTitles();
                    listPages.Items.Clear();
                    foreach (string pageName in pageNames)
                    {
                        listPages.Items.Add(pageName);
                    }
                    break;
                case 1:
                    break;
                case 2:
                    if (comboSectionsFile.SelectedIndex != -1)
                    {
                        string sectionName = comboSectionsFile.SelectedValue.ToString();
                        List<string> fileNames = fjController.GetFileNames(sectionName);
                        listFile.Items.Clear();
                        foreach (string fileName in fileNames)
                        {
                            listFile.Items.Add(fileName);
                        }
                    }
                    break;
                case 3:
                    
                    break;
                case 4:
                    
                    break;

            }
        }

        private void comboSectionsFile_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            e.Handled = true;
            updateUI(2);
        }

        private void browseButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog browseFileDialog = new OpenFileDialog();
            //openFileDialog1.Filter = "Text files|*.txt;*.docx;*.doc;*.pdf|PowerPoints|*.pptx;*.ppt";
            browseFileDialog.Title = "Select a File you wish to add";

            // Show the Dialog.
            if (browseFileDialog.ShowDialog() == DialogResult.OK)
            {
                string path = browseFileDialog.FileName;
                fjController.InsertSnippet(comboSectionsFile.SelectedValue.ToString(),"file","",path);
                updateUI(2);

            }
        }

        private void comboSectionsCode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            e.Handled = true;
        }

        private void projectTitle_LostFocus(object sender, RoutedEventArgs e)
        {
            fjController.SetTitle(projectTitle.Text);
        }

        private void createPage_Click(object sender, RoutedEventArgs e)
        {
            string pageName = Prompt.ShowDialog("Page name:", "Create Page");
            fjController.InsertPageTitle(pageName);
            updateUI(0);
        }

        private void deletePage_Click(object sender, RoutedEventArgs e)
        {
            string pageName = listPages.SelectedValue.ToString();
            fjController.DeletePage(pageName);
            updateUI(0);
        }

        private void comboTheme_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBoxItem selectedItem = comboTheme.SelectedItem as ListBoxItem;
            string themeName = selectedItem.Content.ToString();
            System.Windows.MessageBox.Show(themeName.ToLower().Replace(' ', '_'));
            fjController.SetAceTheme(themeName.ToLower().Replace(' ', '_'));
        }

        private void openStructure_Click(object sender, RoutedEventArgs e)
        {
            IVsUIShell vsUIShell = (IVsUIShell)Package.GetGlobalService(typeof(SVsUIShell));
            Guid guid = typeof(Structure).GUID;
            IVsWindowFrame windowFrame;
            int result = vsUIShell.FindToolWindow((uint)__VSFINDTOOLWIN.FTW_fFindFirst, ref guid, out windowFrame);   // Find MyToolWindow

            if (result != Microsoft.VisualStudio.VSConstants.S_OK)
                result = vsUIShell.FindToolWindow((uint)__VSFINDTOOLWIN.FTW_fForceCreate, ref guid, out windowFrame); // Crate MyToolWindow if not found

            if (result == Microsoft.VisualStudio.VSConstants.S_OK)                                                                           // Show MyToolWindow
                Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(windowFrame.Show());
        }

        private void uploadButton_Click(object sender, RoutedEventArgs e)
        {
            dte2 = (EnvDTE80.DTE2)System.Runtime.InteropServices.Marshal.GetActiveObject("VisualStudio.DTE.14.0");
            solutionDir = System.IO.Path.GetDirectoryName(dte2.Solution.FullName);
            WebsiteGenerator html = new WebsiteGenerator(fjController , false , solutionDir, solutionDir + @"/generatedWebsite");
            html.GenerateWebsite();
            IVsUIShell vsUIShell = (IVsUIShell)Package.GetGlobalService(typeof(SVsUIShell));
            Guid guid = typeof(SitePreview).GUID;
            IVsWindowFrame windowFrame;
            int result = vsUIShell.FindToolWindow((uint)__VSFINDTOOLWIN.FTW_fFindFirst, ref guid, out windowFrame);   // Find MyToolWindow

            if (result != Microsoft.VisualStudio.VSConstants.S_OK)
                result = vsUIShell.FindToolWindow((uint)__VSFINDTOOLWIN.FTW_fForceCreate, ref guid, out windowFrame); // Crate MyToolWindow if not found

            if (result == Microsoft.VisualStudio.VSConstants.S_OK)                                                                           // Show MyToolWindow
                Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(windowFrame.Show());
        }

        private void refreshButton_Click(object sender, RoutedEventArgs e)
        {
            WebsiteGenerator html = new WebsiteGenerator(fjController, false, solutionDir, solutionDir + @"/generatedWebsite");
            html.GenerateWebsite();
            System.Windows.MessageBox.Show("HTML Generated");
        }

        private void AutoSearch_Click(object sender, RoutedEventArgs e)
        {
            Extractor.FindFiles(keySequence.Text);
        }
    }
}
