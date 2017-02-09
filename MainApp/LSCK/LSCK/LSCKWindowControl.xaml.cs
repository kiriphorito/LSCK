//------------------------------------------------------------------------------
// <copyright file="LSCKWindowControl.xaml.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace LSCK
{
    using EnvDTE;
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for LSCKWindowControl.
    /// </summary>
    public partial class LSCKWindowControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LSCKWindowControl"/> class.
        /// </summary>
        FJController fjController;
        EnvDTE80.DTE2 dte2;
        String solutionDir;
        public LSCKWindowControl()
        {
            dte2 = (EnvDTE80.DTE2)System.Runtime.InteropServices.Marshal.GetActiveObject("VisualStudio.DTE.14.0");
            solutionDir = System.IO.Path.GetDirectoryName(dte2.Solution.FullName);
            this.InitializeComponent();
            fjController = new FJController(Environment.CurrentDirectory);
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
            string text = selection.Text;
        }
    }
}