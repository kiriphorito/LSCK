// <copyright file="SitePreviewControl.xaml.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>

namespace LSCK
{
    using EnvDTE;
    using EnvDTE80;
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for SitePreviewControl.
    /// </summary>
    public partial class SitePreviewControl : UserControl
    {
        FJController fjController;

        /// <summary>
        /// Initializes a new instance of the <see cref="SitePreviewControl"/> class.
        /// </summary>
        public SitePreviewControl()
        {
            this.InitializeComponent();
            fjController = FJController.GetInstance;
            Refresh();

        }

        public void Refresh()
        {
            string homepage = fjController.GetPageTitles()[0];
            DTE2 dte = (DTE2)Marshal.GetActiveObject("VisualStudio.DTE");
            string solutionDir = Path.GetDirectoryName(dte.Solution.FullName);
            if (File.Exists(solutionDir + "/generatedWebsite/" + homepage + ".html"))
            {
                Browser.Navigate(new Uri(String.Format("file:///{0}/generatedWebsite/{1}.html", solutionDir, homepage)));
            }
        }

        /// <summary>
        /// Handles click on the button by displaying a message box.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event args.</param>
        [SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions", Justification = "Sample code")]
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Default event handler naming pattern")]
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                string.Format(System.Globalization.CultureInfo.CurrentUICulture, "Invoked '{0}'", this.ToString()),
                "SitePreview");
        }

        private void refreshButton_Click(object sender, RoutedEventArgs e)
        {
            Refresh();
        }
    }
}
