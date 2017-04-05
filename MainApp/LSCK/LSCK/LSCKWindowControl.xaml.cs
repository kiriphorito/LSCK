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
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Forms;
    using System.Windows.Threading;
    using static LSCK.Bridge;

    /// <summary>
    /// Interaction logic for LSCKWindowControl.
    /// </summary>
    public partial class LSCKWindowControl : System.Windows.Controls.UserControl
    {
        WebsiteGenerator wg;
        bool upload = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="LSCKWindowControl"/> class.
        /// </summary>

        public LSCKWindowControl()
        {
            System.Threading.Thread solutionChangeThread = new System.Threading.Thread(checkDTEChange);
            SetDTE2();
            solutionChangeThread.Start();
            this.InitializeComponent();
            updateUI(0);
            if (comboSectionsFile.HasItems)
                comboSectionsFile.SelectedIndex = 0;
            if (comboSectionsCode.HasItems)
                comboSectionsCode.SelectedIndex = 0;
        }

        private void resetControl() {
            if (state == 1)
            {
                this.IsEnabled = true;
                state = 0;
                updateUI(0);
                if (comboSectionsFile.HasItems)
                    comboSectionsFile.SelectedIndex = 0;
                if (comboSectionsCode.HasItems)
                    comboSectionsCode.SelectedIndex = 0;
            }
            else if (state == 2)
            {
                state = 0;
                this.IsEnabled = false;
            }
        }

        public void checkDTEChange()
        {
            while (true)
            {
                System.Threading.Thread.Sleep(500);
                //this.Dispatcher.Invoke(new Action(() => CheckDir()));
                if (state != 0)
                {
                    this.Dispatcher.Invoke(new Action(() => resetControl()));
                }
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
                    return "c";
                case "C++":
                    return "c_cpp";
                case "PHP":
                    return "php";
                case "Python":
                    return "python";
                case "Javascript":
                    return "javascript";
                case "HTML":
                    return "html";
                case "Less":
                    return "less";
                case "SQL":
                    return "sql";
                case "Markdown":
                    return "markdown";
                case "Typescript":
                    return "typescript";
                case "Go":
                    return "golang";
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
                    if (fjController != null)
                    {
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
                        List<string> cssSettings = fjController.GetCSSSettings();
                        List<System.Windows.Media.Color> colors = new List<System.Windows.Media.Color>();
                        for (int x = 0; x < 10; x++)
                        {
                            if (cssSettings[x][0] == '#')
                            {
                                cssSettings[x] = "#FF" + cssSettings[x].Substring(1);
                            }
                            colors.Add((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(cssSettings[x]));
                        }
                        ClrPcker_Background.SelectedColor = colors[0];
                        ClrPcker_TitleColor.SelectedColor = colors[1];
                        ClrPcker_TitleSelect.SelectedColor = colors[2];
                        ClrPcker_OtherColor.SelectedColor = colors[3];
                        ClrPcker_OtherSelect.SelectedColor = colors[4];
                        ClrPcker_SelBackground.SelectedColor = colors[5];
                        ClrPcker_Border.SelectedColor = colors[6];
                        ClrPcker_PageBg.SelectedColor = colors[7];
                        ClrPcker_SectionTitle.SelectedColor = colors[8];
                        ClrPcker_Comment.SelectedColor = colors[9];
                        fontSectionTitle.Text = cssSettings[10];
                        fontComment.Text = cssSettings[11];
                        sizeSectionTitle.Text = cssSettings[12];
                        sizeComment.Text = cssSettings[13];
                        ipAddress.Text = fjController.GetDeployIP();
                        username.Text = fjController.GetDeployUsername();
                    }
                    else
                    {
                        this.IsEnabled = false;
                    }
                    break;
                case 1:
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
            }
        }

        private void comboSectionsFile_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            e.Handled = true;
            updateUI(1);
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
                updateUI(1);

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

        private void previewButton_Click(object sender, RoutedEventArgs e)
        {
            fjController.SetCSSSettings(getCustomStrings());
            System.Threading.Thread webGen = new System.Threading.Thread(generateWebsite);
            uploadButton.Visibility = Visibility.Collapsed;
            loadText.Visibility = Visibility.Visible;
            previewButton.Visibility = Visibility.Collapsed;
            checkCDN.Visibility = Visibility.Collapsed;
            loadText.Text = "Creating website ...";
            webGen.Start();

            //genText.Visibility = Visibility.Hidden;

        }

        private bool getCDNCheck()
        {
            return (bool)checkCDN.IsChecked;
        }

        private List<string> getCustomStrings()
        {
            List<string> custStrings = new List<string>();
            custStrings.Add(ClrPcker_Background.SelectedColorText);
            custStrings.Add(ClrPcker_TitleColor.SelectedColorText);
            custStrings.Add(ClrPcker_TitleSelect.SelectedColorText);
            custStrings.Add(ClrPcker_OtherColor.SelectedColorText);
            custStrings.Add(ClrPcker_OtherSelect.SelectedColorText);
            custStrings.Add(ClrPcker_SelBackground.SelectedColorText);
            custStrings.Add(ClrPcker_Border.SelectedColorText);
            custStrings.Add(ClrPcker_PageBg.SelectedColorText);
            custStrings.Add(ClrPcker_SectionTitle.SelectedColorText);
            custStrings.Add(ClrPcker_Comment.SelectedColorText);
            for (int i = 0; i < custStrings.Count; i++)
            {
                if (custStrings[i][0] == '#')
                {
                    custStrings[i] = "#" + custStrings[i].Substring(3);
                }
            }
            custStrings.Add(fontSectionTitle.Text);
            custStrings.Add(fontComment.Text);
            custStrings.Add(sizeSectionTitle.Text);
            custStrings.Add(sizeComment.Text);
            return custStrings;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void generateWebsite()
        {
            bool cdn = this.Dispatcher.Invoke(getCDNCheck, DispatcherPriority.Normal);
            wg = new WebsiteGenerator(fjController,cdn, solutionDir, solutionDir + @"/generatedWebsite");
            wg.GenerateWebsite();
            IVsUIShell vsUIShell = (IVsUIShell)Package.GetGlobalService(typeof(SVsUIShell));
            Guid guid = typeof(SitePreview).GUID;
            IVsWindowFrame windowFrame;
            int result = vsUIShell.FindToolWindow((uint)__VSFINDTOOLWIN.FTW_fFindFirst, ref guid, out windowFrame);   // Find MyToolWindow

            if (result != Microsoft.VisualStudio.VSConstants.S_OK)
                result = vsUIShell.FindToolWindow((uint)__VSFINDTOOLWIN.FTW_fForceCreate, ref guid, out windowFrame); // Crate MyToolWindow if not found

            if (result == Microsoft.VisualStudio.VSConstants.S_OK)                                                                           // Show MyToolWindow
                Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(windowFrame.Show());
            refresh = false;
            this.Dispatcher.Invoke(resetWebView,DispatcherPriority.Normal);
        }

        private void resetWebView()
        {
            if (!upload)
            {
                changeLoadText();
                previewButton.Visibility = Visibility.Visible;
                uploadButton.Visibility = Visibility.Visible;
                loadText.Visibility = Visibility.Collapsed;
                checkCDN.Visibility = Visibility.Visible;
            }else
            {
                changeLoadText();
                upload = false;
            }
        }

        private void refreshButton_Click(object sender, RoutedEventArgs e)
        {
            WebsiteGenerator html = new WebsiteGenerator(fjController, false, solutionDir, solutionDir + @"/generatedWebsite");
            html.GenerateWebsite();
            System.Windows.MessageBox.Show("HTML Generated");
        }

        private void autoSearch_Click(object sender, RoutedEventArgs e)
        {
            Extractor.FindFiles(keySequence.Text);
        }

        private void connectType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (connectType.SelectedIndex == 0)
            {
                if (browsePEM != null)
                {
                    browsePEM.Visibility = Visibility.Visible;
                    browsePEM.Focusable = false;
                    detailType.Text = "PEM Directory:";
                }
            }
            else
            {
                browsePEM.Visibility = Visibility.Collapsed;
                browsePEM.Focusable = true;
                detailType.Text = "Password:";
            }
        }

        private void browsePEM_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog browseFileDialog = new OpenFileDialog();
            browseFileDialog.Filter = "PEM file|*.pem;*.docx";
            browseFileDialog.Title = "Select PEM directory";

            // Show the Dialog.
            if (browseFileDialog.ShowDialog() == DialogResult.OK)
            {
                string path = browseFileDialog.FileName;
                details.Text = path;

            }
        }

        private void addType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (addType.SelectedIndex == 0)
            {
                if (autoAdd != null && manualAdd != null)
                {
                    autoAdd.Visibility = Visibility.Collapsed;
                    manualAdd.Visibility = Visibility.Visible;
                }
            }
            else
            {
                autoAdd.Visibility = Visibility.Visible;
                manualAdd.Visibility = Visibility.Collapsed;
            }
        }

        private void ClrPcker_Background_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<System.Windows.Media.Color?> e)
        {
            
        }

        private void custType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (custType.SelectedIndex == 0)
            {
                if (customGeneral != null && customNav != null)
                {
                    customNav.Visibility = Visibility.Collapsed;
                    customGeneral.Visibility = Visibility.Visible;
                }
            }
            else
            {
                customNav.Visibility = Visibility.Visible;
                customGeneral.Visibility = Visibility.Collapsed;
            }
        }

        private void fontTitle_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var cvt = new FontConverter();
            string text = (fontSectionTitle.SelectedItem as ComboBoxItem).Content.ToString();
            if (text != null && prevTit != null)
            {
                prevTit.FontFamily = new System.Windows.Media.FontFamily(text);
            }
        }

        private void fontText_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var cvt = new FontConverter();
            string text = (fontComment.SelectedItem as ComboBoxItem).Content.ToString();
            if (text != null && prevComm != null)
            {
                prevComm.FontFamily = new System.Windows.Media.FontFamily(text);
            }
        }

        private void setExistingCustomCSS()
        {

        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void sshGeneration()
        {
            generateWebsite();
            SSH ssh = new SSH(solutionDir + "/generatedWebsite");
            List<string> sshDetails= this.Dispatcher.Invoke(getSSHDetails, DispatcherPriority.Normal);
            ssh.UploadWebsite(sshDetails[0], sshDetails[1],sshDetails[2], sshDetails[3]);
            this.Dispatcher.Invoke(resetWebView, DispatcherPriority.Normal);

        }
       
        private void changeLoadText()
        {
            if (upload)
            {
                loadText.Text = "Uploading website ...";
            }else
            {
                loadText.Text = "Creating website ...";
            }
        }
        private List<string> getSSHDetails()
        {
            string conType = (connectType.SelectedItem as ComboBoxItem).Content.ToString();
            List<string> sshDetails = new List<string>();
            sshDetails.Add(ipAddress.Text);
            sshDetails.Add(username.Text);
            sshDetails.Add(conType);
            sshDetails.Add(details.Text);
            return sshDetails;
        }
        private void uploadButton_Click(object sender, RoutedEventArgs e)
        {
            fjController.SetCSSSettings(getCustomStrings());
            fjController.SetDeploy(getSSHDetails()[0], getSSHDetails()[1]);
            System.Threading.Thread sshGen = new System.Threading.Thread(sshGeneration);
            previewButton.Visibility = Visibility.Collapsed;
            uploadButton.Visibility = Visibility.Collapsed;
            loadText.Visibility = Visibility.Visible;
            checkCDN.Visibility = Visibility.Collapsed;
            upload = true;
            sshGen.Start();
        }

        private void saveCSS_Click(object sender, RoutedEventArgs e)
        {
            fjController.SetCSSSettings(getCustomStrings());
        }
    }

}
