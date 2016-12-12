//------------------------------------------------------------------------------
// <copyright file="ToolWindow1Control.xaml.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Final
{
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Controls;
    using System.IO;
    using EnvDTE;
    using System.Diagnostics;
    using System;
    using System.Collections.Generic;
    using System.Windows.Forms;
    using Microsoft.VisualBasic;

    /// <summary>
    /// Interaction logic for ToolWindow1Control.
    /// </summary>
    public partial class ToolWindow1Control : System.Windows.Controls.UserControl
    {
        string tempsectionID;
        string sectionID;
        string solutionDir;
        string ending = null, comstart = null;
        List<string> sections = new List<string>();
        EnvDTE80.DTE2 dte2;
        /// <summary>
        /// Initializes a new instance of the <see cref="ToolWindow1Control"/> class.
        /// </summary>
        public ToolWindow1Control()
        {
            dte2 = (EnvDTE80.DTE2)System.Runtime.InteropServices.Marshal.GetActiveObject("VisualStudio.DTE.14.0");
            solutionDir = System.IO.Path.GetDirectoryName(dte2.Solution.FullName);
            if (!Directory.Exists(solutionDir + "\\Saved Code"))
            {
                Directory.CreateDirectory(solutionDir + "\\Saved Code");
            }
            solutionDir = solutionDir + "\\Saved Code";
            this.InitializeComponent();
            marker.Text = Properties.Settings.Default.marker;
            Properties.Settings.Default.Save();
            YourListBox.ItemsSource = new List<String> { "One", "Two", "Three" };
        }
        private void ChangeSections()
        {
            string result;
            int i = 0;
            string[] lines = File.ReadAllLines(solutionDir + "\\" + ending);
            foreach (var line in File.ReadAllLines(solutionDir + "\\" + ending))
            {
                if (line.Contains("///" + tempsectionID))
                {
                    result = "///"+sectionID+line.Substring(3+tempsectionID.Length, line.Length - (3+tempsectionID.Length));
                    lines[i] = result;
                }
                else
                {

                }
                i++;
            }
            File.WriteAllLines(solutionDir + "\\" + ending, lines);
            Properties.Settings.Default.marker = sectionID;
            Properties.Settings.Default.Save();
        }
        private void FindSections()
        {
            sections.Clear();
            if (combo2 != null)
            {
                combo2.Items.Clear();
            }
            string result = null;
            foreach (var line in File.ReadAllLines(solutionDir + "\\" + ending))
            {
                if (line.Contains("///"+sectionID))
                {
                    result = line.Substring(3+sectionID.Length, line.Length -(3 + sectionID.Length));
                    sections.Add(result);
                }
                else
                {

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
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            if (combo2.Items.Count != 0)
            {
                sectionID = marker.Text;
                var selection = (TextSelection)dte2.ActiveDocument.Selection;
                string comment = comstart + textBox.Text;
                string text = comment + Environment.NewLine + selection.Text + Environment.NewLine;
                Debug.WriteLine(text);
                int i = 0, t = 0;
                string[] lines;
                string section;
                lines = File.ReadAllLines(solutionDir + "\\" + ending);
                foreach (var line in lines)
                {
                    if (combo2.SelectedIndex == combo2.Items.Count - 1)
                    {
                        t = 1;
                        section = combo2.Items[combo2.SelectedIndex].ToString();
                    }
                    else
                    {
                        t = 2;
                        section = combo2.Items[combo2.SelectedIndex + 1].ToString();
                    }
                    if (line == "///" + sectionID + section)
                    {
                        if (i != 0)
                        {
                            if (t == 1)
                            {
                                lines[lines.Length - 1] = Environment.NewLine+text;
                            }
                            else
                            {
                                lines[i - 1] = Environment.NewLine+text;
                            }
                        }
                        else
                        {
                            lines[i + 1] = Environment.NewLine+text;
                        }
                        break;
                    }
                    i++;
                }
                File.WriteAllLines(solutionDir + "\\" + ending, lines);
                System.Windows.MessageBox.Show("Added code");
            }else
            {
                System.Windows.MessageBox.Show("Pick or add a section");
            }
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            sectionID = marker.Text;
            string name = Microsoft.VisualBasic.Interaction.InputBox("Name of section:","New Section","Name",0,0);
            string text = "///"+sectionID + name+Environment.NewLine+Environment.NewLine;
            File.AppendAllText(solutionDir + "\\" + ending, text);
            FindSections();
            foreach (string section in sections)
            {
                combo2.Items.Add(section);
            }
            combo2.SelectedIndex = combo2.Items.Count - 1;
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tabs.SelectedIndex == 0)
            {
                
            }
            else if (tabs.SelectedIndex == 1){
            }
        }

        private void marker_TextChanged(object sender, TextChangedEventArgs e)
        {
            tempsectionID = sectionID;
            sectionID = marker.Text;
            ChangeSections();
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            // Displays an OpenFileDialog so the user can select a Cursor.
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "Cursor Files|*.cur|Text files|*.txt;*.docx;*.doc;*.pdf|PowerPoints|*.pptx" ;
            openFileDialog1.Title = "Select a Cursor File";

            // Show the Dialog.
            // If the user clicked OK in the dialog and
            // a .CUR file was selected, open it.
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                // Assign the cursor in the Stream to the Form's Cursor property.

                this.Cursor = new System.Windows.Input.Cursor(openFileDialog1.OpenFile());
            }
        }

        private void combo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (combo2!= null)
            {
                combo2.Items.Clear();
            }
            if (combo.SelectedIndex == 0)
            {
                ending = "C#.cs";
                comstart = "//";
            }
            else if (combo.SelectedIndex == 1)
            {
                ending = "Java.java";
                comstart = "//";
            }
            else
            {

            }
            
            if (!File.Exists(solutionDir + "\\" + ending))
            {
                File.AppendAllText(solutionDir + "\\" + ending, "");
            }
            FindSections();
            foreach (string section in sections)
            {
                combo2.Items.Add(section);
            }
            if (sections.Count!=0) {
                combo2.SelectedIndex = 0;
            }
        }
    }

}